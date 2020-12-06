using System;
using PDR.PatientBooking.Service.AppointmentService.Request;

namespace PDR.PatientBooking.Service.AppointmentService.Validation
{
    public interface IAddAppointmentValidator
    {
        AppointmentValidationResult ValidateRequest(AddAppointmentRequest request);
    }
}
