using System;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.AppointmentService.Request;

namespace PDR.PatientBooking.Service.AppointmentService
{
    public interface IAppointmentService
    {
        public Guid AddNewAppointment(AddAppointmentRequest newAppointment);

        void CancelAppointment(long parientId, Guid bookingId);

        Order GetPatientNextAppointment(long patientId);
    }
}
