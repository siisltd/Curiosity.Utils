// using System.Collections.Generic;
// using System.Net;
// using Curiosity.Configuration;
//
// namespace Curiosity.Tools.Web.Middleware.IPFilter
// {
//     /// <summary>
//     /// Настройки фильтрации HTTP запросов по IP
//     /// </summary>
//     public class IPFilterOptions : ILoggableOptions, IValidatableOptions
//     {
//         /// <summary>
//         /// Список разрешённых IP, с которых можно обращаться к нам
//         /// </summary>
//         public string[]? AllowedIP { get; set; }
//
//         public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
//         {
//             var errors = new ConfigurationValidationErrorCollection(prefix);
//
//             if (AllowedIP != null && AllowedIP.Length > 0)
//             {
//                 foreach (var ip in AllowedIP)
//                 {
//                     // тут может быть либо конкретный IP, либо сеть, проверим оба варианта
//                     if (!IPAddress.TryParse(ip, out _))
//                     {
//                         // !IPNetwork.TryParse(ip, out _)
//                         errors.AddErrorIf(!IPAddress.TryParse(ip, out _), nameof(AllowedIP), $"\"{ip}\" - некорректный IP");
//                     }
//                 }
//             }
//             
//             return errors;
//         }
//     }
// }