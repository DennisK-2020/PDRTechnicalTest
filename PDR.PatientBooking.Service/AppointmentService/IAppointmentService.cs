using System;
using PDR.PatientBooking.Service.AppointmentService.Request;

namespace PDR.PatientBooking.Service.AppointmentService
{
    public interface IAppointmentService
    {
        public Guid AddNewAppointment(AddAppointmentRequest newAppointment);
    }
}
