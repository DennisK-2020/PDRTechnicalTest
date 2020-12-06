using System;
namespace PDR.PatientBooking.Service.AppointmentService.Request
{
    public class AddAppointmentRequest
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
    }
}
