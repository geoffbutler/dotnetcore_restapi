using FluentValidation;

namespace ContactsCore.Model.Validators
{
    public class ContactDetailValidator : AbstractValidator<Models.ContactDetail>
    {
        public ContactDetailValidator()
        {
            //RuleFor(o => o.Uid)

            RuleFor(o => o.Description).NotEmpty().Length(0, 50);
            RuleFor(o => o.Value).NotEmpty().Length(0, 50);
            RuleFor(o => o.Type).IsInEnum();
        }
    }
}
