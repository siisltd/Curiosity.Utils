using System;
using System.Linq;
using Curiosity.Tools.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Provider of <see cref="TrimStringModelBinder"/>.
    /// </summary>
    public class TrimStringModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // We check that the type is a string and that it has a parent container.
            var metadata = context.Metadata;
            if (metadata.IsComplexType || metadata.ModelType != typeof(string) || metadata.ContainerType == null)
                return null;

            // We check that there is our attribute.
            var property = metadata.ContainerType
                .GetProperties()
                .FirstOrDefault(propertyInfo => propertyInfo.Name == metadata.PropertyName);
            if (property == null || !property.GetCustomAttributes(true).OfType<TrimStringAttribute>().Any())
                return null;

            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            // Everything worked out successfully-we return the correct Binder.
            return new TrimStringModelBinder(metadata.ModelType, loggerFactory);
        }
    }
}
