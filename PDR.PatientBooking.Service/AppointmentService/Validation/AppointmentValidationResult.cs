using System;
using System.Collections.Generic;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.Validation;

namespace PDR.PatientBooking.Service.AppointmentService.Validation
{
    public class AppointmentValidationResult : PdrValidationResult
    {
        public Patient AppointmentPatient { get; set; }

        public Doctor AppointmentDoctor { get; set; }

        public SurgeryType AppointmentSurgeryType { get; set; }


        public AppointmentValidationResult(bool passedValidation) : base(passedValidation)
        {
        }

        public AppointmentValidationResult(bool passedValidation, string error) : base(passedValidation, error)
        {
        }

        public AppointmentValidationResult(bool passedValidation, List<string> errors) : base(passedValidation, errors)
        {
        }
    }
}
