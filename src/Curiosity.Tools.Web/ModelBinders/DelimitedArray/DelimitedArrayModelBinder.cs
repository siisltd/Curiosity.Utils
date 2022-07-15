using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Class for binding arrays to model. Arrays are string, where items are delimited by some chars.
    /// </summary>
    public class DelimitedArrayModelBinder : IModelBinder
    {
        private readonly char[] _delimiters;

        /// <inheritdoc cref="DelimitedArrayModelBinder"/>
        public DelimitedArrayModelBinder(char[] delimiters)
        {
            _delimiters = delimiters ?? throw new ArgumentNullException(nameof(delimiters));
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            var values = valueProviderResult
                .ToString()
                .Split(_delimiters, StringSplitOptions.RemoveEmptyEntries);
            var elementType = bindingContext.ModelType.GetTypeInfo().GetElementType();
            if (elementType == null) throw new InvalidOperationException($"{nameof(elementType)} can't be null");

            if (values.Length == 0)
            {
                bindingContext.Result = ModelBindingResult.Success(Array.CreateInstance(elementType, 0));
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(elementType);
                var typedArray = Array.CreateInstance(elementType, values.Length);

                try
                {
                    for (var i = 0; i < values.Length; ++i)
                    {
                        var value = values[i];
                        var convertedValue = converter.ConvertFromString(value);
                        typedArray.SetValue(convertedValue, i);
                    }
                }
                catch (Exception exception)
                {
                    bindingContext.ModelState.TryAddModelError(
                        modelName,
                        exception,
                        bindingContext.ModelMetadata);
                }

                bindingContext.Result = ModelBindingResult.Success(typedArray);
            }

            return Task.CompletedTask;
        }
    }
}
