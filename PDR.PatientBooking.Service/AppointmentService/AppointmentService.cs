using System;
using System.Collections.Generic;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.AppointmentService.Request;
using PDR.PatientBooking.Service.AppointmentService.Validation;

namespace PDR.PatientBooking.Service.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddAppointmentValidator _addBookingValidator;

        public AppointmentService(PatientBookingContext context, IAddAppointmentValidator addBookingValidator)
        {
            _context = context;
            _addBookingValidator = addBookingValidator;
        }

        public Guid AddNewAppointment(AddAppointmentRequest newAppointment)
        {
            var bookingId = new Guid();

            var validatedRquest = _addBookingValidator.ValidateRequest(newAppointment);

            if (!validatedRquest.PassedValidation)
                throw new ArgumentException(validatedRquest.Errors.First());

            var myBooking = new Order
            {
                Id = bookingId,
                StartTime = newAppointment.StartTime,
                EndTime = newAppointment.EndTime,
                PatientId = validatedRquest.AppointmentPatient.Id,
                DoctorId = validatedRquest.AppointmentPatient.Id,
                Patient = validatedRquest.AppointmentPatient,
                Doctor = validatedRquest.AppointmentDoctor,
                SurgeryType = (int)validatedRquest.AppointmentPatient.Clinic.SurgeryType
            };

            _context.Order.AddRange(new List<Order> { myBooking });
            _context.SaveChanges();

            return myBooking.Id;
        }

        public void CancelAppointment(long parientId, Guid bookingId)
        {
            var result = _context.Order.FirstOrDefault(a => a.PatientId == parientId && a.Id.Equals(bookingId) && !a.IsCancelled);

            if (result == null)
                throw new ArgumentException("Appointment does not exist or already cancelled");

            result.IsCancelled = true;

            _context.Order.Update(result);
            _context.SaveChanges();
        }

        public Order GetPatientNextAppointment(long patientId)
        {
            var appointment = _context.Order.Where(a => a.PatientId == patientId && a.StartTime > DateTime.UtcNow && !a.IsCancelled).OrderBy(a => a.StartTime).FirstOrDefault();

            return appointment;
        }
    }
}
