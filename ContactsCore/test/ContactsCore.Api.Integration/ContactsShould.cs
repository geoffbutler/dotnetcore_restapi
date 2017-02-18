using System;
using System.Threading.Tasks;
using Xunit;
using ContactsCore.Model.Models;

namespace ContactsCore.Api.Integration
{
    public class ContactsShould : CrudTest<Contact>, ICrudTest
    {
        public ContactsShould()
            : base("contacts")
        {
        }
        
        [Fact]
        public async Task CreateAndReadByUid()
        {
            await CreateAndReadByUidImpl();
        }

        [Fact]
        public async Task CreateAndRead()
        {
            await CreateAndReadImpl();
        }

        [Fact]
        public async Task CreateAndUpdate()
        {
            await CreateAndUpdateImpl();
        }

        [Fact]
        public async Task CreateAndDelete()
        {
            await CreateAndDeleteImpl();
        }

        protected override Contact GetPostModel()
        {
            var uid = Guid.NewGuid();
            var nonce = $" {uid.ToString("N").Substring(28)}"; // last four chars
            var model = new Contact
            {
                Uid = uid,
                FirstName = "foo" + nonce,
                LastName = "bar" + nonce
            };
            return model;
        }

        protected override Contact GetPutModel(Contact model)
        {
            model.FirstName = model.FirstName + " u";
            model.LastName = model.LastName + " u";
            return model;
        }
    }
}