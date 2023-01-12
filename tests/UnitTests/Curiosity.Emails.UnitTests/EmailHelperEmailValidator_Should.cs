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
        [InlineData("john+1@siisltd.ru", true)]
        [InlineData("иван@siisltd.ru", true)]
        [InlineData("ИВАН@siisltd.ru", true)]
        [InlineData("test.test@-com", false)]
        [InlineData("TESt.TEST@-com", false)]
        [InlineData("mail.mail-test", false)]
        [InlineData("adil.mir762@gmail-com", false)]
        public void ValidateEmail(string testedEmail, bool expectedResult)
        {
            //act
            var actualResult = EmailHelper.IsEmailValid(testedEmail);

            //assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
