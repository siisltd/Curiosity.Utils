using Curiosity.Tools.Web.Resources;

namespace Curiosity.Tools.Web.Controllers
{
    /// <summary>
    /// Basic errors.
    /// </summary>
    public enum SiteError
    {
        ParameterMissing,
        InvalidParameter,
        ObjectMissing,
        Unauthorized,
        AccessDenied
    }
    
    public static class SiteErrorExtensions
    {
        /// <summary>
        /// Localized string with error description.
        /// </summary>
        public static string ToLocalizedString(this SiteError siteError)
        {
            switch (siteError)
            {
                case SiteError.ParameterMissing:
                    return LNG.SiteError_ParameterMissing;
                case SiteError.InvalidParameter:
                    return LNG.SiteError_InvalidParameter;
                case SiteError.ObjectMissing:
                    return LNG.SiteError_ObjectMissing;
                case SiteError.Unauthorized:
                    return LNG.SiteError_Unauthorized;
                case SiteError.AccessDenied:
                    return LNG.SiteError_AccessDenied;

                default:
                    return LNG.SiteError_UnknownError;
            }
        }
    }
}