using ContactsCore.Common.Interfaces;
using ContactsCore.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ContactsCore.Model.Models
{
    public class ContactDetail : Base, IContactDetail
    {        
        public string Description { get; set; }
        public string Value { get; set; }
        
        public ContactDetailType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ContactDetailType TypeDescription => Type;

        //public Guid ContactUid { get; set; }
    }
}
