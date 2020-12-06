using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.AppointmentService;
using PDR.PatientBooking.Service.AppointmentService.Request;
using PDR.PatientBooking.Service.AppointmentService.Validation;

namespace PDR.PatientBooking.Service.Tests.AppointmentService
{
    [TestFixture]
    public class AppointmentServiceTest
    {
        private MockRepository _mockRepository;
        private IFixture _fixture;
        private PatientBookingContext _context;
        private Mock<IAddAppointmentValidator> _validator;
        private IAppointmentService _sut;

        [SetUp]
        public void Startup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _fixture = new Fixture();

            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _validator = _mockRepository.Create<IAddAppointmentValidator>();

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddAppointmentRequest>())).Returns(new AppointmentValidationResult(true)
            {
                AppointmentDoctor = new Doctor { },
                AppointmentPatient = new Patient { Clinic = new Clinic { SurgeryType = SurgeryType.SystemOne } }
            });

            _sut = new Service.AppointmentService.AppointmentService(_context, _validator.Object);
        }

        [Test]
        public void AddBooking_ValidatesRequest()
        {
            var request = _fixture.Create<AddAppointmentRequest>();

            _sut.AddNewAppointment(request);

            _validator.Verify(x => x.ValidateRequest(request), Times.Once);
        }

        [Test]
        public void AddBooking_AddsBookingToContextWithGeneratedId()
        {
            var bookingId = new Guid();

            var request = _fixture.Create<AddAppointmentRequest>();

            var validatedResult = new AppointmentValidationResult(true)
            {
                AppointmentDoctor = new Doctor { Id = request.DoctorId },
                AppointmentPatient = new Patient { Id = request.PatientId, Clinic = new Clinic { SurgeryType = SurgeryType.SystemOne } }
            };

            _validator.Setup(x => x.ValidateRequest(It.IsAny<AddAppointmentRequest>())).Returns(validatedResult);

            var expected = new Order
            {
                Id = bookingId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                Patient = new Patient(),
                Doctor = new Doctor(),
                SurgeryType = (int)SurgeryType.SystemOne
            };

            _sut.AddNewAppointment(request);

            _context.Order.Should().ContainEquivalentOf(expected, options => options.Excluding(order => order.Patient).Excluding(order => order.Doctor).Excluding(order => order.Id).Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1000)).WhenTypeIs<DateTime>());
        }

    }
}
