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
        public void ValidateEmail(string testedEmail, bool expectedResult)
        {
            //act
            var actualResult = EmailHelper.IsEmailValid(testedEmail);

            //assert
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
