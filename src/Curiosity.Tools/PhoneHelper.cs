using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Curiosity.Tools
{
    /// <summary>
    /// Assistant when working with phone numbers
    /// </summary>
    public static class PhoneHelper
    {
        /// <summary>
        /// Removes all non-numeric characters.
        /// </summary>
        public static string CleanupPhone(this string? phone)
        {
            if (phone == null) return String.Empty;

            var stringBuilder = new StringBuilder(phone.Length);

            for (var i = 0; i < phone.Length; i++)
            {
                var symbol = phone[i];
                if (!Char.IsDigit(symbol)) continue;

                stringBuilder.Append(symbol);
            }

            return stringBuilder.ToString();
        }

        public static bool IsCorrectPhone(this string phone)
        {
            if (String.IsNullOrWhiteSpace(phone)) return false;

            return phone.Length >= 4;
        }

        /// <summary>
        /// Converts phone to the format 8000000000000 if the string is recognized as a mobile number.
        /// </summary>
        /// <param name="phone">Phone number</param>
        public static string ResolvePhoneTo8Format(this string phone)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            phone = phone.CleanupPhone();
            if (phone.Length == 10)
            {
                return "8" + phone;
            }
        
            if (Regex.Match(phone, @"^((7)+([0-9]){10})$").Success)
            {
                return Regex.Replace(phone, @"^(7)", "8");
            }

            return phone;
        }
        
        /// <summary>
        /// Converts to the format 70000000000 if the string is recognized as a mobile number.
        /// </summary>
        /// <param name="phone">Phone number</param>
        public static string ResolvePhoneTo7Format(this string phone)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            phone = phone.CleanupPhone();
            if (phone.Length == 10)
            {
                return "7" + phone;
            }
        
            if (Regex.Match(phone, @"^((8|7)+([0-9]){10})$").Success)
            {
                return Regex.Replace(phone, @"^(8|7)", "7");
            }

            return phone;
        }

        /// <summary>
        /// Returns the mobile number to the format without 7 or 8
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string Remove7And8(this string phone)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            phone = phone.CleanupPhone();
            if (phone.Length <= 10) return phone;
            
            if (Regex.Match(phone, @"^((8|7)+([0-9]){10})$").Success)
            {
                return phone.Substring(1, phone.Length - 1);
            }

            return phone;
        }
        
        /// <summary>
        /// Returns true if the phone number matches the following formats:
        /// 8000000000000 or 70000000000.
        /// </summary>
        public static bool IsMobilePhoneFormat(this string phone)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            return Regex.Match(phone, @"^((8|7)+([0-9]){10})$").Success;
        }
    }
}