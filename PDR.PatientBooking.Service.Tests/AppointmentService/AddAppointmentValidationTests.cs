using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.AppointmentService.Request;
using PDR.PatientBooking.Service.AppointmentService.Validation;

namespace PDR.PatientBooking.Service.Tests.AppointmentService
{
    [TestFixture]
    public class AddAppointmentValidationTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddAppointmentValidator _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            _sut = new AddAppointmentValidator(_context);
        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            var request = GetValidRequest();

            var res = _sut.ValidateRequest(request);

            res.PassedValidation.Should().BeTrue();
        }

        [Test]
        public void ValidateRequest_WhenAppointmentExits_ReturnsFailedValidationResult()
        {
            var order = _fixture.Create<Order>();
            order.StartTime = DateTime.UtcNow.AddDays(1);
            order.EndTime = order.StartTime.AddHours(1);

            var doctor = _fixture.Create<Doctor>();
            var patient = _fixture.Create<Patient>();
            doctor.Orders.Add(order);

            _context.Patient.Add(patient);
            _context.Doctor.Add(doctor);

            _context.SaveChanges();

            var request = _fixture.Build<AddAppointmentRequest>()
                      .With(x => x.PatientId, patient.Id)
                      .With(x => x.DoctorId, doctor.Id)
                      .With(x => x.StartTime, order.StartTime)
                      .With(x => x.EndTime, order.EndTime)
                      .Create();


            var res = _sut.ValidateRequest(request);

            res.PassedValidation.Should().BeFalse();
            res.Errors.Should().Contain("Appointment slot is taken");
        }

        private AddAppointmentRequest GetValidRequest()
        {

            var patient = _fixture.Create<Patient>();
            var doctor = _fixture.Create<Doctor>();

            _context.Patient.Add(patient);
            _context.Doctor.Add(doctor);

            _context.SaveChanges();

            var request = _fixture.Build<AddAppointmentRequest>()
                .With(x => x.PatientId, patient.Id)
                .With(x => x.DoctorId, doctor.Id)
                .With(x => x.StartTime, DateTime.UtcNow.AddYears(1))
                .Create();

            return request;
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
