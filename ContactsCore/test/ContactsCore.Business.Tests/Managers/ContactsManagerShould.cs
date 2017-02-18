using ContactsCore.Business.Managers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using ContactsCore.Data;
using ContactsCore.Data.Helpers;
using ContactsCore.Business.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ContactsCore.Common.Enums;
using ContactsCore.Data.Dao;
using System.Linq;

namespace ContactsCore.Business.Tests.Managers
{
    public class ContactsManagerShould : IDisposable
    {
        private readonly Mock<ILogger<ContactsManager>> _fakeLogger;
        private readonly ContactsContext _db;
        private readonly List<Contact> _expectedData;

        private readonly ContactsManager _target;
                

        public ContactsManagerShould()
        {
            _fakeLogger = new Mock<ILogger<ContactsManager>>();

            var options = new DbContextOptionsBuilder<ContactsContext>()
                .UseInMemoryDatabase(databaseName: "contacts")
                .Options;
            _db = new ContactsContext(options);
            var unitOfWork = new ContactsUnitOfWork(_db);

            var mapper = MapperConfig.Init().CreateMapper();
            var fakeDbExceptionHelper = new Mock<IDbExceptionHelper>();
            _target = new ContactsManager(_fakeLogger.Object, mapper, fakeDbExceptionHelper.Object, unitOfWork);


            // seed in-memory db
            _expectedData = new List<Contact>
            {
                new Contact { Id = 1, Uid = Guid.NewGuid(), IsDeleted = false, FirstName = "fname1", LastName = "lname1" },
                new Contact { Id = 2, Uid = Guid.NewGuid(), IsDeleted = false, FirstName = "fname2", LastName = "lname2" },
                new Contact { Id = 3, Uid = Guid.NewGuid(), IsDeleted = false, FirstName = "fname3", LastName = "lname3" },
                new Contact { Id = 4, Uid = Guid.NewGuid(), IsDeleted = true, FirstName = "fname4", LastName = "lname4" },
            };
            _db.Contacts.AddRange(_expectedData);
            _db.SaveChanges();            
        }

        public void Dispose()
        {
            // clean up in-memory db (in-memory state is not cleared between tests)
            _db.Contacts.RemoveRange(_db.Contacts);
            _db.SaveChanges();
        }

        [Fact]
        public async Task ReturnPagedDataOnGet()
        {
            // arrange
            var expectedResults = new List<Model.Models.Contact>
            {
                new Model.Models.Contact { Uid = _expectedData[0].Uid, FirstName = _expectedData[0].FirstName, LastName = _expectedData[0].LastName },
                new Model.Models.Contact { Uid = _expectedData[1].Uid, FirstName = _expectedData[1].FirstName, LastName = _expectedData[1].LastName },
                new Model.Models.Contact { Uid = _expectedData[2].Uid, FirstName = _expectedData[2].FirstName, LastName = _expectedData[2].LastName },
            };

            const int expectedPageNumber = 1;
            const int expectedPageSize = 10;
            var expectedTotal = _expectedData.Count(o => !o.IsDeleted);

            // act
            var result = await _target.Get(expectedPageNumber, expectedPageSize);

            // assert
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.Success);
            result.Result.ShouldBeEquivalentTo(expectedResults);

            result.PageMeta.Should().NotBeNull();
            result.PageMeta.PageNumber.Should().Be(expectedPageNumber);
            result.PageMeta.PageSize.Should().Be(expectedPageSize);
            result.PageMeta.Total.Should().Be(expectedTotal);

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReturnDataFromSecondPageOnGet()
        {
            // arrange
            var expectedResults = new List<Model.Models.Contact>
            {                
                new Model.Models.Contact { Uid = _expectedData[1].Uid, FirstName = _expectedData[1].FirstName, LastName = _expectedData[1].LastName },
            };

            const int expectedPageNumber = 2;
            const int expectedPageSize = 1;
            var expectedTotal = _expectedData.Count(o => !o.IsDeleted);

            // act
            var result = await _target.Get(expectedPageNumber, expectedPageSize);

            // assert
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.Success);
            result.Result.ShouldBeEquivalentTo(expectedResults);

            result.PageMeta.Should().NotBeNull();
            result.PageMeta.PageNumber.Should().Be(expectedPageNumber);
            result.PageMeta.PageSize.Should().Be(expectedPageSize);
            result.PageMeta.Total.Should().Be(expectedTotal);

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReturnNoDataFromSecondLargePageOnGet()
        {
            // arrange
            var expectedResults = new List<Model.Models.Contact>();

            const int expectedPageNumber = 2;
            const int expectedPageSize = 10;
            var expectedTotal = _expectedData.Count(o => !o.IsDeleted);

            // act
            var result = await _target.Get(expectedPageNumber, expectedPageSize);

            // assert
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.Success);
            result.Result.ShouldBeEquivalentTo(expectedResults);

            result.PageMeta.Should().NotBeNull();
            result.PageMeta.PageNumber.Should().Be(expectedPageNumber);
            result.PageMeta.PageSize.Should().Be(expectedPageSize);
            result.PageMeta.Total.Should().Be(expectedTotal);

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReturnNoDeletedDataOnGet()
        {
            // arrange
            var expectedResults = new List<Model.Models.Contact>();

            const int expectedPageNumber = 2;
            const int expectedPageSize = 3;
            var expectedTotal = _expectedData.Count(o => !o.IsDeleted);

            // act
            var result = await _target.Get(expectedPageNumber, expectedPageSize);

            // assert
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.Success);
            result.Result.ShouldBeEquivalentTo(expectedResults);

            result.PageMeta.Should().NotBeNull();
            result.PageMeta.PageNumber.Should().Be(expectedPageNumber);
            result.PageMeta.PageSize.Should().Be(expectedPageSize);
            result.PageMeta.Total.Should().Be(expectedTotal);

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
        }


        [Fact]
        public async Task ReturnDataOnGetByUid()
        {
            // arrange
            var expectedResults = new List<Model.Models.Contact>
            {                
                new Model.Models.Contact { Uid = _expectedData[2].Uid, FirstName = _expectedData[2].FirstName, LastName = _expectedData[2].LastName },
            };

            // act
            var result = await _target.GetByUid(_expectedData[2].Uid);

            // assert
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.Success);
            result.Result.ShouldBeEquivalentTo(expectedResults);

            result.PageMeta.Should().BeNull();

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReturnNoDataOnGetByUidForMissingRecord()
        {
            // arrange
            

            // act
            var result = await _target.GetByUid(Guid.NewGuid());

            // assert            
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.NotFound);

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
            LogAssert.AssertWarn(_fakeLogger, Times.Once);
        }

        [Fact]
        public async Task ReturnNoDeletedDataOnGetByUid()
        {
            // arrange

            // act
            var result = await _target.GetByUid(_expectedData[3].Uid);

            // assert            
            result.Should().NotBeNull();
            result.ResultStatus.Should().Be(ManagerResponseResult.NotFound);

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
            LogAssert.AssertWarn(_fakeLogger, Times.Once);
        }

        
        // TODO        
    }
}
