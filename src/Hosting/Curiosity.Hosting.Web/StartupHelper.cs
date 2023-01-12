using System.Reflection;
using Curiosity.Hosting.AppInitializer;
using Curiosity.Hosting.Web.Resources;
using Curiosity.Tools.IoC;
using Curiosity.Tools.Web.ModelBinders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Curiosity.Hosting.Web
{
    /// <summary>
    /// Helps configuring web apps startup.
    /// </summary>
    public static class StartupHelper
    {
        /// <summary>
        /// Adds pre-configured MVC to IoC with basic Curiosity services. 
        /// </summary>
        public static IMvcBuilder ConfigureCuriosityMvc(this IServiceCollection services, ICuriosityWebAppConfiguration configuration)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    // default culture for request
                    opts.DefaultRequestCulture = new RequestCulture(configuration.Culture.DefaultCulture);
                });
            
            var serviceProvider = services.BuildServiceProvider();

            var mvcBuilder = services
                .AddMvc(options =>
                {
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                    
                    options.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider());

                    var stringLocalizerFactory = serviceProvider.GetService<IStringLocalizerFactory>();
                    var assembly = Assembly.GetExecutingAssembly();
                    var localizer = stringLocalizerFactory.Create("ModelBinder", assembly.FullName);
                    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x =>
                        localizer[WebSiteResourceKeys.ModelBinder_MissingBindRequiredValueAccessor, x]);
                    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() =>
                        localizer[WebSiteResourceKeys.ModelBinder_MissingKeyOrValueAccessor]);
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x =>
                        localizer[WebSiteResourceKeys.ModelBinder_ValueMustNotBeNullAccessor, x]);
                    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) =>
                        localizer[WebSiteResourceKeys.ModelBinder_AttemptedValueIsInvalidAccessor, x, y]);
                    options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x =>
                        localizer[WebSiteResourceKeys.ModelBinder_UnknownValueIsInvalidAccessor, x]);
                    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x =>
                        localizer[WebSiteResourceKeys.ModelBinder_ValueIsInvalidAccessor, x]);
                    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x =>
                        localizer[WebSiteResourceKeys.ModelBinder_ValueMustBeANumberAccessor, x]);
                    options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() =>
                        localizer[WebSiteResourceKeys.ModelBinder_MissingRequestBodyRequiredValueAccessor]);
                    options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x =>
                        localizer[WebSiteResourceKeys.ModelBinder_NonPropertyAttemptedValueIsInvalidAccessor, x]);
                    options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
                        localizer[WebSiteResourceKeys.ModelBinder_NonPropertyUnknownValueIsInvalidAccessor]);
                    options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() =>
                        localizer[WebSiteResourceKeys.ModelBinder_NonPropertyValueMustBeANumberAccessor]);
                })
                .AddDataAnnotationsLocalization()
                .AddRazorRuntimeCompilation();

            services.AddMemoryCache();
            services.AddAppInitialization();
            services.AddSensitiveDataProtector(configuration.SensitiveDataFieldNames);
            
            return mvcBuilder;
        }
    }
}
