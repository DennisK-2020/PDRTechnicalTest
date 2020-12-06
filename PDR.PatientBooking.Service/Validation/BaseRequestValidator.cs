using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.Validation
{
    public class BaseRequestValidator
    {
        public bool IsEmailValid(string email, PdrValidationResult result)
        {
            result.PassedValidation = !string.IsNullOrEmpty(email) && new EmailAddressAttribute().IsValid(email);

            if (!result.PassedValidation)
                result.Errors.Add("Email must be a valid email address");

            return result.PassedValidation;
        }
    }
}
