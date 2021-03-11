using Curiosity.Tools.Attributes;
using Xunit;

namespace Curiosity.Tools.UnitTests.Attributes
{
    public class ValidEmailAttributeTests
    {
        [Theory]
        [InlineData(@"NotAnEmail", false)]
        [InlineData(@"@NotAnEmail", false)]
        [InlineData(@"""test\\blah""@example.com", true)]
        [InlineData(@"""test\blah""@example.com", false)]
        [InlineData("\"test\\\rblah\"@example.com", true)]
        [InlineData("\"test\rblah\"@example.com", false)]
        [InlineData(@"""test\""blah""@example.com", true)]
        [InlineData(@"""test""blah""@example.com", false)]
        [InlineData(@"customer/department@example.com", true)]
        [InlineData(@"$A12345@example.com", true)]
        [InlineData(@"!def!xyz%abc@example.com", true)]
        [InlineData(@"_Yosemite.Sam@example.com", true)]
        [InlineData(@"~@example.com", true)]
        [InlineData(@".wooly@example.com", false)]
        [InlineData(@"wo..oly@example.com", false)]
        [InlineData(@"pootietang.@example.com", false)]
        [InlineData(@".@example.com", false)]
        [InlineData(@"""Austin@Powers""@example.com", true)]
        [InlineData(@"Ima.Fool@example.com", true)]
        [InlineData(@"""Ima.Fool""@example.com", true)]
        [InlineData(@"""Ima Fool""@example.com", true)]
        [InlineData(@"Ima Fool@example.com", false)]
        public void CheckEmail(string email, bool expected)
        {
            // act
            var result =  new ValidEmailAttribute().IsValid(email);

            // assert
            Assert.Equal(result, expected);
        }
    }
}