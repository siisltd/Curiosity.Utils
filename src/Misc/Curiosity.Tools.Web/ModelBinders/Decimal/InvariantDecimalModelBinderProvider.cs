using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Provider of <see cref="InvariantDecimalModelBinder"/>
    /// </summary>
    public class InvariantDecimalModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?)))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new InvariantDecimalModelBinder(context.Metadata.ModelType, loggerFactory);
            }

            return null;
        }
    }
}
