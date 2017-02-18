using System;
using System.Threading.Tasks;
using Xunit;
using ContactsCore.Model.Models;

namespace ContactsCore.Api.Integration
{
    public class ContactDetailsShould : CrudTest<ContactDetail>, ICrudTest
    {
        private const string ContactGuidString = "30910EC9-BE46-41B7-8D70-C35AFF00B3F3";

        private readonly Guid _contactGuid;
        private readonly string _contactPutRoute;
        
        public ContactDetailsShould()
            : base($"contactdetails/{ContactGuidString}")
        {
            var contactRoutes = new ApiRoutes("contacts");
            _contactGuid = new Guid(ContactGuidString);
            _contactPutRoute = contactRoutes.Put(_contactGuid);
        }

        private async Task EnsureContactExists()
        {
            using (var client = GetClient())
            {
                var nonce = $" {_contactGuid.ToString("N").Substring(28)}"; // last four chars
                var model = new Contact
                {
                    Uid = _contactGuid,
                    FirstName = "foo" + nonce,
                    LastName = "bar" + nonce
                };
                
                await PutModel(client, model, _contactPutRoute);
            }
        }

        [Fact]
        public async Task CreateAndReadByUid()
        {
            await EnsureContactExists();
            await CreateAndReadByUidImpl();
        }

        [Fact]
        public async Task CreateAndRead()
        {
            await EnsureContactExists();
            await CreateAndReadImpl();
        }

        [Fact]
        public async Task CreateAndUpdate()
        {
            await EnsureContactExists();
            await CreateAndUpdateImpl();
        }

        [Fact]
        public async Task CreateAndDelete()
        {
            await EnsureContactExists();
            await CreateAndDeleteImpl();
        }

        protected override ContactDetail GetPostModel()
        {
            var uid = Guid.NewGuid();
            var nonce = $" {uid.ToString("N").Substring(28)}"; // last four chars
            var model = new ContactDetail
            {
                Uid = uid, 
                Description = "foo" + nonce, 
                Value = "bar" + nonce, 
                Type = Common.Enums.ContactDetailType.Email
            };
            return model;
        }

        protected override ContactDetail GetPutModel(ContactDetail model)
        {
            model.Description = model.Description + " u";
            model.Value = model.Value + " u";
            model.Type = Common.Enums.ContactDetailType.Mobile;
            return model;
        }
    }
}