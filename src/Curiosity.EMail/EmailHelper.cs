using System;
using System.Linq;
using System.Net.Mail;

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
                try
                {
                    // Sometimes usual mail address can't handle all cases, need extra validation
                    new MailAddress(email);
                    // new MailboxAddress(email);
                }
                catch
                {
                    return false;
                }
            }
            
            return true;
        }

        // public static IReadOnlyCollection<MailboxAddress> ToEmailsList(this string emails)
        // {
        //     return emails
        //         .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
        //         .Select(x => new MailboxAddress(x.Trim()))
        //         .ToArray();
        // }

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
    }
}