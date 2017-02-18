using System.Collections.Generic;

namespace ContactsCore.Model.ViewModels
{
    public class ContactWithDetailsViewModel : Models.Contact
    {
        public List<Models.ContactDetail> ContactDetails { get; set; }
    }
}
