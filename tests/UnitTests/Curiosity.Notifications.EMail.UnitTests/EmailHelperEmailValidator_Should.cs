using System.Net.Mail;
using System.Text.RegularExpressions;
using Curiosity.EMail;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;
using Xunit;

namespace Curiosity.Notifications.EMail.UnitTests
{
    /// <summary>
    /// Positive Unit tests for <see cref="EmailHelper"/>.
    /// </summary>
    public class EmailHelper_Should
    {
        [Fact]
        void EmailValidatorDiscardIncorrectEmails_Properly()
        {
            //arrange
            var correctTestEmail = "john@siisltd.ru";
            var incorrectTestEmail1 = "test.test@-com";
            var incorrectTestEmail2 = "mail.mail-test";

            IHelper helper = new EmailHelper();
            
            //act
            var correctEmailResult = helper.IsEmailValid(correctTestEmail);
            var incorrectEmailResult1 = helper.IsEmailValid(incorrectTestEmail1);
            var incorrectEmailResult2 = helper.IsEmailValid(incorrectTestEmail2);

            //assert
            Assert.True(correctEmailResult);
            Assert.False(incorrectEmailResult1);
            Assert.False(incorrectEmailResult2);
        }
    }
    
    public interface IHelper
    {
        public bool IsEmailValid(string? email);
    }
    
    public class EmailHelper : IHelper
    {
        private const string EmailPattern = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
        public bool IsEmailValid(string? email)
        {
            var mailboxResult = MailboxAddress.TryParse(ParserOptions.Default, email ?? "", out _);

            var isRegexMatch = Regex.Match(email ?? "", EmailPattern).Success;

            return mailboxResult && isRegexMatch;
        }
    }
}