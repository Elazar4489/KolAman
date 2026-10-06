using ClassificationComponent.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace ClassificationComponent.Services
{
    public class ValidatorE
    {
        private readonly ILogger<ValidatorE> _logger;
        public ValidatorE(ILogger<ValidatorE> logger) { _logger = logger; }
        public bool IsValid(Alert alert)
        {
            if (alert == null) { return false; }
            var validationContext = new ValidationContext(alert);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(alert, validationContext, validationResults, validateAllProperties: true);
            if (!isValid)
            {
                foreach (var E in validationResults)
                {
                    _logger.LogError("alert feild {message}", E.ErrorMessage);
                }
                return false;
            }
            return true;
        }
    }
}

