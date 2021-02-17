using System.Reflection;
using System.Resources;

namespace Curiosity.SFTP.SSH.Net
{
    internal static class LNG
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager(Assembly.GetExecutingAssembly().FullName, typeof(LNG).GetTypeInfo().Assembly);

        public static string SftpClient_SameFileAlreadyExists => GetString("SameFileAlreadyExists");
        public static string SftpClient_DifferentFileAlreadyExists => GetString( "DifferentFileAlreadyExists");
        public static string SftpClient_Overwriting => GetString( "Overwriting");
        public static string SftpClient_AuthMethodsNotSpecified => GetString( "AuthMethodsNotSpecified");
        public static string SftpClient_CanNotBeEmpty => GetString( "CanNotBeEmpty");
        
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