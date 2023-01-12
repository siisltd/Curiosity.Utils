using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Curiosity.Tools.Web.ModelState
{
    /// <summary>
    /// Extensions methods for <see cref="ModelStateDictionary"/>
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Adds error information by the condition
        /// </summary>
        public static ModelStateDictionary AddModelErrorIf(
            this ModelStateDictionary modelState,
            bool condition,
            string fieldName, 
            string errorDescription)
        {
            if (!condition) return modelState;
            if (String.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));
            if (String.IsNullOrWhiteSpace(errorDescription))
                throw new ArgumentNullException(nameof(errorDescription));
            
            modelState.AddModelError(fieldName, errorDescription);
            
            return modelState ?? throw new ArgumentNullException(nameof(modelState));
        }
    }
}