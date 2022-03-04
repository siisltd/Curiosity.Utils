using System.Text.RegularExpressions;
using Curiosity.EMail;
using Xunit;

namespace Curiosity.Emails.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="EmailHelper"/> for method <see cref="EmailHelper.IsEmailValid"/>.
    /// </summary>
    public class EmailHelper_Should
    {
        [Theory]
        [InlineData("john@siisltd.ru", true)]
        [InlineData("test.test@-com", false)]
        [InlineData("mail.mail-test", false)]
        // [InlineData("adil.mir762@gmail-com", false)]
        public void ValidateEmail(string testedEmail, bool expectedResult)
        {
            //act
            var actualResult = EmailHelper.IsEmailValid(testedEmail);

            //assert
            Assert.Equal(expectedResult, actualResult);
        }

        // [Theory]
        // [InlineData("john@siisltd.ru", true)]
        // [InlineData("test.test@-com", false)]
        // [InlineData("mail.mail-test", false)]
        // [InlineData("adil.mir762@gmail-com", false)]
        // public void ValidateEmail1(string testedEmail, bool expectedResult)
        // {
        //      var regexp = new Regex("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
        //
        //      var isMatch = regexp.IsMatch(testedEmail);
        //
        //      Assert.Equal(expectedResult, isMatch);
        // }
        //
        // [Theory]
        // [InlineData("john@siisltd.ru", true)]
        // [InlineData("test.test@-com", false)]
        // [InlineData("mail.mail-test", false)]
        // [InlineData("adil.mir762@gmail-com", false)]
        // [InlineData("adil.mir762@gmail.com", true)]
        // public void ValidateEmail2(string testedEmail, bool expectedResult)
        // {
        //      var regexp = new Regex("^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&\'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$");
        //
        //      var isMatch = regexp.IsMatch(testedEmail);
        //
        //      Assert.Equal(expectedResult, isMatch);
        // }
    }
}
