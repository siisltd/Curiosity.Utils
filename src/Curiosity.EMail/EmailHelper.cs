using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MimeKit;

namespace Curiosity.EMail
{
    /// <summary>
    /// Helpers for sending EMails.
    /// </summary>
    public static class EmailHelper
    {
        private static readonly char[] Separators = { ';', ',', ' ' };
        
        /// <summary>
        /// Checks that specified EMails are valid.
        /// </summary>
        /// <param name="emails">List of EMails.</param>
        /// <param name="allowEmptyString"></param>
        /// <returns></returns>
        public static bool IsValidEmailsList(this string emails, bool allowEmptyString = true)
        {
            if (String.IsNullOrWhiteSpace(emails))
                return allowEmptyString;

            var splittedEmails = emails.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            
            for (var i = 0; i < splittedEmails.Length; i++)
            {
                var email = splittedEmails[i].Trim();
                if (!IsEmailValid(email))
                {
                    return false;
                }
            }
            
            return true;
        }

        public static IReadOnlyList<MailboxAddress> ToEmailsList(this string emails)
        {
            return emails
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => MailboxAddress.Parse(x.Trim()))
                .ToArray();
        }

        public static string? ToNormalizedEmailsListString(this string? emails)
        {
            if (String.IsNullOrWhiteSpace(emails))
                return null;
                
            var emailsList = emails
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLower())
                .ToArray();

            return String.Join(", ", emailsList);
        }
        
        /// <summary>
        /// Regex pattern for email validation.
        /// </summary>
        /// <remarks>
        /// Source link: https://stackoverflow.com/a/8829363/7087479
        /// </remarks>
        private const string EmailPattern = @"^[a-zA-Zа-яА-Я0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        /// <summary>
        /// Checks that email address is valid.
        /// </summary>
        public static bool IsEmailValid(string? email)
        {
            var mailboxResult = MailboxAddress.TryParse(ParserOptions.Default, email ?? "", out _);
            var isRegexMatch = Regex.Match(email ?? "", EmailPattern).Success;

            return mailboxResult && isRegexMatch;
        }
    }
}
