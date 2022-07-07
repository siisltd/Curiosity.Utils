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
        private const string EmailPattern = @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*)@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";

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
