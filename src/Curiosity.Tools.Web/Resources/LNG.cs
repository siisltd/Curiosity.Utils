using System.Reflection;
using System.Resources;

namespace Curiosity.Tools.Web.Resources
{
    internal static class LNG
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Curiosity.Tools.Web.Resources.LNG", typeof(LNG).GetTypeInfo().Assembly);

        public static string SiteError_ParameterMissing => GetString("SiteError_ParameterMissing");
        public static string SiteError_InvalidParameter => GetString( "SiteError_InvalidParameter");
        public static string SiteError_ObjectMissing => GetString( "SiteError_ObjectMissing");
        public static string SiteError_Unauthorized => GetString( "SiteError_Unauthorized");
        public static string SiteError_AccessDenied => GetString( "SiteError_AccessDenied");
        public static string SiteError_UnknownError => GetString( "SiteError_UnknownError");
        
        public static string UnexpectedError => GetString( "UnexpectedError");
        
        private static string GetString(string name, params string[]? formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}