using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    /// <summary>
    /// Provider of <see cref="DateTimeModelBinder"/>.
    /// </summary>
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?)))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new DateTimeModelBinder(context.Metadata.ModelType, loggerFactory);
            }

            return null;
        }
    }
}
