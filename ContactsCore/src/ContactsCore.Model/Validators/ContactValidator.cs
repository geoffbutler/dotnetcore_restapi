using ContactsCore.Model.Models;
using FluentValidation;

namespace ContactsCore.Model.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            //RuleFor(o => o.Uid)

            RuleFor(o => o.FirstName).NotEmpty().Length(1, 50);
            RuleFor(o => o.LastName).NotEmpty().Length(1, 50);
        }
    }
}
