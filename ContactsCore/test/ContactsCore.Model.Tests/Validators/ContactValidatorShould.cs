using ContactsCore.Model.Models;
using ContactsCore.Model.Validators;
using System.Threading.Tasks;
using Xunit;

namespace ContactsCore.Model.Tests.Validators
{
    public class ContactValidatorShould
    {
        private readonly Contact _validModel = new Contact
        {
            //Uid = 
            FirstName = "foo",
            LastName = "bar"
        };

        private readonly ContactValidator _target = new ContactValidator();

        [Fact]
        public async Task ReturnNoErrorsForValidModel()
        {
            // arrange
            var model = _validModel;

            // act
            var result = await _target.ValidateAsync(model);

            // assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData(null)] // empty
        [InlineData("")] // empty
        [InlineData(" ")] // empty
        [InlineData("123456789012345678901234567890123456789012345678901")] // greater than max
        public async Task ReturnErrorsForInvalidFirstName(string value)
        {
            // arrange
            var model = _validModel;
            model.FirstName = value;

            // act
            var result = await _target.ValidateAsync(model);

            // assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Theory]
        [InlineData(null)] // empty
        [InlineData("")] // empty
        [InlineData(" ")] // empty
        [InlineData("123456789012345678901234567890123456789012345678901")] // greater than max
        public async Task ReturnErrorsForInvalidLastName(string value)
        {
            // arrange
            var model = _validModel;
            model.LastName = value;

            // act
            var result = await _target.ValidateAsync(model);

            // assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }
    }
}
