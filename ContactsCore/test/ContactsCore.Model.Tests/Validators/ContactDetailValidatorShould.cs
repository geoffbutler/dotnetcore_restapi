using ContactsCore.Model.Models;
using ContactsCore.Model.Validators;
using System.Threading.Tasks;
using Xunit;

namespace ContactsCore.Model.Tests.Validators
{
    public class ContactDetailValidatorShould
    {
        private readonly ContactDetail _validModel = new ContactDetail
        {
            //Uid = 
            Description = "foo",
            Value = "bar",
            Type = Common.Enums.ContactDetailType.Email,
            //ContactUid = 
        };

        private readonly ContactDetailValidator _target = new ContactDetailValidator();

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
        public async Task ReturnErrorsForInvalidDescription(string value)
        {
            // arrange
            var model = _validModel;
            model.Description = value;

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
        public async Task ReturnErrorsForInvalidValue(string value)
        {
            // arrange
            var model = _validModel;
            model.Value = value;

            // act
            var result = await _target.ValidateAsync(model);

            // assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Theory]
        [InlineData((Common.Enums.ContactDetailType)(-1))] // invalid
        [InlineData((Common.Enums.ContactDetailType)99)] // invalid                
        public async Task ReturnErrorsForInvalidType(Common.Enums.ContactDetailType value)
        {
            // arrange
            var model = _validModel;
            model.Type = value;

            // act
            var result = await _target.ValidateAsync(model);

            // assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }
    }
}
