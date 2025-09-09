using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DAValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ProductManagementApp.Common
{
    public static class ValidationHelper
    {
        public static IList<DAValidationResult> Validate(object instance)
        {
            var ctx = new ValidationContext(instance);
            var results = new List<DAValidationResult>();
            Validator.TryValidateObject(instance, ctx, results, validateAllProperties: true);
            return results;
        }
    }
}