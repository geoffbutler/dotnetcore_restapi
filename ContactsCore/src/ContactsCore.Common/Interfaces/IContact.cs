namespace ContactsCore.Common.Interfaces
{
    public interface IContact : IBase
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
