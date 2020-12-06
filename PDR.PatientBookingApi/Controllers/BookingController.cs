using Microsoft.AspNetCore.Mvc;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.AppointmentService;
using PDR.PatientBooking.Service.AppointmentService.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public BookingController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("patient/{identificationNumber}/next")]
        public IActionResult GetPatientNextAppointnemtn(long identificationNumber)
        {
            var result = _appointmentService.GetPatientNextAppointment(identificationNumber);

            if (result == null)
                return StatusCode(502);

            return Ok(new MyOrderResult { Id = result.Id, DoctorId = result.DoctorId, StartTime = result.StartTime, EndTime = result.EndTime, PatientId = result.PatientId, SurgeryType = result.SurgeryType });
        }

        [HttpPut("patient/{identificationNumber}/cancel/{bookingId}")]
        public IActionResult CancelBooking(long identificationNumber, Guid bookingId)
        {
            try
            {
                _appointmentService.CancelAppointment(identificationNumber, bookingId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Appointment cancelled");
        }

        [HttpPost()]
        public IActionResult AddBooking(AddAppointmentRequest newAppointment)
        {
            try
            {
                return Ok(_appointmentService.AddNewAppointment(newAppointment));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        public class NewBooking
        {
            public Guid Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public long PatientId { get; set; }
            public long DoctorId { get; set; }
        }

        private static MyOrderResult UpdateLatestBooking(List<Order> bookings2, int i)
        {
            MyOrderResult latestBooking;
            latestBooking = new MyOrderResult();
            latestBooking.Id = bookings2[i].Id;
            latestBooking.DoctorId = bookings2[i].DoctorId;
            latestBooking.StartTime = bookings2[i].StartTime;
            latestBooking.EndTime = bookings2[i].EndTime;
            latestBooking.PatientId = bookings2[i].PatientId;
            latestBooking.SurgeryType = (int)bookings2[i].GetSurgeryType();

            return latestBooking;
        }

        private class MyOrderResult
        {
            public Guid Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public long PatientId { get; set; }
            public long DoctorId { get; set; }
            public int SurgeryType { get; set; }
        }
    }
}