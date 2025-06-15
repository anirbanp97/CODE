using System.ComponentModel.DataAnnotations;

namespace MinimalAPIDemo.Models
{
    public static class ValidationHelper
    {
        // Validates the specified model instance.
        // T: Type of the model to validate
        // model: Model instance to validate.
        // validationResults: List of validation errors, if any.
        // returns True if the model is valid; otherwise, false.
        public static bool TryValidate<T>(T model, out List<ValidationResult> validationResults)
        {
            // Initialize the list to hold validation error results
            validationResults = new List<ValidationResult>();

            // If the model is null, return false and add a validation error
            if (model == null)
            {
                validationResults.Add(new ValidationResult("Model instance cannot be null."));
                return false;
            }

            // Create a ValidationContext for the model, which includes the model's metadata.
            var validationContext = new ValidationContext(model);

            // Perform validation and return the result.
            // Validate the model's properties based on their Data Annotation attributes

            // model: The instance of the object we want to validate.
            // validationContext: Provides metadata and context information required for the validation process.
            // validationResults: An output parameter (list) that will contain all validation errors found during the process.
            // true (validateAllProperties): When set to true, this tells the validator to check all properties of the object,
            //      not just those directly involved in the current context.
            //      This means nested objects and all annotated properties are validated.
            return Validator.TryValidateObject(model, validationContext, validationResults, true);
        }
    }
}
