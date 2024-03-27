using FluentAssertions;
using Xunit;

namespace Curiosity.Tools.UnitTests.UniqueId
{
    public class PublicIdTests
    {
        // can detect that line is 16 chars hex and parse it to true long value
        [Fact]
        public void TryParse_Parse16CharsHexLine_LongValue()
        {
            // arrange
            const long value = 10752219502637056;
            const string hex = "0026331630007000"; // 16 chars hex
            
            // act
            var canParse = PublicId.TryParse(hex, out var id);

            // assert
            canParse.Should().Be(true);
            id.Should().Be(value);
        }
        
        // can detect that line is 17 chars hex and parse it to true long value
        [Fact]
        public void TryParse_Parse17CharsHexLine_LongValue()
        {
            // arrange
            const long value = 10752219502637056;
            const string hex = "00026331630007000"; // 17 chars hex
            
            // act
            var canParse = PublicId.TryParse(hex, out var id);

            // assert
            canParse.Should().Be(true);
            id.Should().Be(value);
        }
        
        // can detect that line is dec and parse it to true long value
        [Fact]
        public void TryParse_ParseDecLine_LongValue()
        {
            // arrange
            const long value = 10752219502637056;
            const string dec = "10752219502637056"; // dec line
            
            // act
            var canParse = PublicId.TryParse(dec, out var id);

            // assert
            canParse.Should().Be(true);
            id.Should().Be(value);
        }
    }
}