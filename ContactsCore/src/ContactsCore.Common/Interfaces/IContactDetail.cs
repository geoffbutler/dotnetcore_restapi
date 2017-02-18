using ContactsCore.Common.Enums;

namespace ContactsCore.Common.Interfaces
{
    public interface IContactDetail : IBase
    {
        string Description { get; set; }
        string Value { get; set; }
        ContactDetailType Type { get; set; }
    }
}
