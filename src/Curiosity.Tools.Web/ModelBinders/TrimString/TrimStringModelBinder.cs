using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Model binder that binds string fields and trim it before binding.
    /// </summary>
    public class TrimStringModelBinder: IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;

        /// <inheritdoc cref="TrimStringModelBinder"/>
        public TrimStringModelBinder(Type modelType, ILoggerFactory loggerFactory)
        {
            _baseBinder = new SimpleTypeModelBinder(modelType, loggerFactory);
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));
            
            // Check that there is data
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None) 
                return _baseBinder.BindModelAsync(bindingContext);
            
            // Check that the string is not empty.
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            var boundValue = valueProviderResult.FirstValue;
            if (String.IsNullOrEmpty(boundValue))
                return _baseBinder.BindModelAsync(bindingContext);
            
            // Cut it off, return it - everyone is happy.
            bindingContext.Result = ModelBindingResult.Success(boundValue.Trim());
            return Task.CompletedTask;
        }
    }
}
