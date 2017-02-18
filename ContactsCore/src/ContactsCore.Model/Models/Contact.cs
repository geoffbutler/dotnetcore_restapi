using ContactsCore.Common.Interfaces;

namespace ContactsCore.Model.Models
{
    public class Contact : Base, IContact
    {        
        public string FirstName { get; set; }
        public string LastName { get; set; }        
    }
}
