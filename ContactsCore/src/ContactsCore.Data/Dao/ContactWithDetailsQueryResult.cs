using System.Collections.Generic;

namespace ContactsCore.Data.Dao
{
    public class ContactWithDetailsQueryResult
    {
        public Contact Contact { get; set; }
        public IEnumerable<ContactDetail> Details { get; set; }
    }
}
