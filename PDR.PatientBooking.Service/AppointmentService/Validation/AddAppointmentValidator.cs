using System;
using System.Linq;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.AppointmentService.Request;

namespace PDR.PatientBooking.Service.AppointmentService.Validation
{
    public class AddAppointmentValidator : IAddAppointmentValidator
    {
        private readonly PatientBookingContext _context;

        public AddAppointmentValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public AppointmentValidationResult ValidateRequest(AddAppointmentRequest request)
        {
            var result = new AppointmentValidationResult(true);

            if (!IsAppointmentTimeValid(request, result))
                return result;

            if (!IsAppointmentSlotAvailable(request, result))
                return result;

            return result;
        }

        private bool IsAppointmentSlotAvailable(AddAppointmentRequest newAppointment, AppointmentValidationResult result)
        {
            result.AppointmentPatient = _context.Patient.FirstOrDefault(x => x.Id == newAppointment.PatientId);

            if (result.AppointmentPatient == null)
            {
                result.PassedValidation = false;
                result.Errors.Add($"Patient {newAppointment.PatientId} does not exist");

                return false;
            }

            result.AppointmentDoctor = _context.Doctor.FirstOrDefault(x => x.Id == newAppointment.DoctorId);

            if (result.AppointmentDoctor == null)
            {
                result.PassedValidation = false;
                result.Errors.Add($"Doctor {newAppointment.DoctorId} does not exist");

                return false;
            }

            if (result.AppointmentDoctor.Orders.Any(a => a.StartTime <= newAppointment.EndTime && a.EndTime >= newAppointment.StartTime && !a.IsCancelled))
            {
                result.PassedValidation = false;
                result.Errors.Add("Appointment slot is taken");

                return false;
            }

            return true;
        }

        public bool IsAppointmentTimeValid(AddAppointmentRequest request, AppointmentValidationResult result)
        {
            if (request.StartTime < DateTime.UtcNow)
            {
                result.PassedValidation = false;
                result.Errors.Add("Appointmnet cannot be in the past");

                return false;
            }

            return true;
        }
    }
}
