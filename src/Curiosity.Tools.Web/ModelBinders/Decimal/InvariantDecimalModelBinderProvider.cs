using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.ModelBinders
{
    public class InvariantDecimalModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory; 
        
        public InvariantDecimalModelBinderProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?)))
            {
                return new InvariantDecimalModelBinder(context.Metadata.ModelType, _loggerFactory);
            }

            return null;
        }
    }
}