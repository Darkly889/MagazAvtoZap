using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MagazAvtoZap.BusinessLogic
{
    public class ValidationService
    {
        public bool ValidateObject(object obj, out List<ValidationResult> validationResults)
        {
            var context = new ValidationContext(obj);
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, context, validationResults, true);
        }
    }
}
