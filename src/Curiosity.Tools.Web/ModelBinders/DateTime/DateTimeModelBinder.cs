using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;

        public DateTimeModelBinder(Type modelType, ILoggerFactory loggerFactory)
        {
            _baseBinder = new SimpleTypeModelBinder(modelType, loggerFactory);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;

                // Use invariant culture
                if (DateTime.TryParse(valueAsString, Thread.CurrentThread.CurrentUICulture, DateTimeStyles.None, out var result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
            return _baseBinder.BindModelAsync(bindingContext);
        }
    }
}