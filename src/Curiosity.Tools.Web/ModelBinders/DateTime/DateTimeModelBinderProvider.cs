using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory; 
        
        public DateTimeModelBinderProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?)))
            {
                return new DateTimeModelBinder(context.Metadata.ModelType, _loggerFactory);
            }

            return null;
        }
    }
}