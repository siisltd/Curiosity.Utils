using System;
using System.Linq;
using System.Text;

namespace Curiosity.Tools.Web.Swagger
{
    /// <summary>
    /// Helps write a description in a Swagger UI
    /// </summary>
    public static class SwaggerDescriptor
    {
        /// <summary>
        /// Writes all enums values like a list with descriptions
        /// </summary>
        public static StringBuilder WriteEnum<T>(this StringBuilder sb, Func<T, string> describer) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T)} must be enum");

            sb.AppendLine();
            sb.AppendLine($"<b>{typeof(T).Name}:</b>");
            
            foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
            {
                sb.AppendLine($"- <b>[{Convert.ToInt32(value)}]</b> ({value}) - <b><i>{describer.Invoke(value)}</i></b>");
            }
            
            return sb;
        }

        /// <summary>
        /// Writes a block of details into HTML tag details
        /// </summary>
        public static StringBuilder WritePart(this StringBuilder sb, string partName, Action writeValueAction)
        {
            sb.AppendLine($"<details><summary><b>{partName}:</b></summary>");

            writeValueAction.Invoke();
            
            sb.AppendLine("</details>");
            sb.AppendLine();
            
            return sb;
        }
    }
}