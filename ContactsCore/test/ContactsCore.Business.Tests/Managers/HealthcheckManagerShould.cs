using ContactsCore.Business.Managers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Data.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ContactsCore.Data;
using ContactsCore.Data.Helpers;
using ContactsCore.Business.Tests.TestHelpers;

namespace ContactsCore.Business.Tests.Managers
{
    public class HealthcheckManagerShould
    {
        private readonly Mock<ILogger<HealthcheckManager>> _fakeLogger;
        private readonly Mock<IAdoHelper> _fakeAdoHelper;
        private readonly Mock<DbCommand> _fakeDbCommand;
        private readonly Mock<DbConnection> _fakeDbConn;        
        private readonly object _fakeScalarResult;

        private readonly HealthcheckManager _target;
        

        public HealthcheckManagerShould()
        {
            _fakeLogger = new Mock<ILogger<HealthcheckManager>>();

            var fakeDbContext = new Mock<ContactsContext>();
            _fakeAdoHelper = new Mock<IAdoHelper>();
            var fakeDb = new Mock<DatabaseFacade>(fakeDbContext.Object);
            _fakeDbConn = new Mock<DbConnection>();
            _fakeDbCommand = new Mock<DbCommand>();
            _fakeScalarResult = new object();

            fakeDbContext.SetupGet(o => o.Database).Returns(fakeDb.Object);
            _fakeAdoHelper.Setup(o => o.GetConnection(fakeDbContext.Object)).Returns(_fakeDbConn.Object);
            _fakeAdoHelper.Setup(o => o.CreateCommand(_fakeDbConn.Object)).Returns(_fakeDbCommand.Object);
            

            _target = new HealthcheckManager(_fakeLogger.Object, fakeDbContext.Object, _fakeAdoHelper.Object);
        }

        [Fact]
        public async Task ReturnApiVersionAndOkDbStatusOnGet()
        {
            // arrange
            _fakeAdoHelper.Setup(o => o.ExecuteScalarAsync(_fakeDbCommand.Object)).ReturnsAsync(_fakeScalarResult);

            // act
            var result = await _target.Get();

            // assert
            result.ApiVersion.Should().NotBeNullOrWhiteSpace();
            result.DbStatus.Should().Be("OK");
            
            _fakeAdoHelper.Verify(o => o.ExecuteScalarAsync(_fakeDbCommand.Object), Times.Once);
            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
        }

        [Fact]
        public async Task ReturnNotOkDbStatusAndLogErrorOnGetWhenExceptionThrownOnOpenConnection()
        {
            // arrange
            var expectedException = new Exception("test");
            _fakeAdoHelper.Setup(o => o.OpenConnectionAsync(_fakeDbConn.Object)).Throws(expectedException);

            // act
            var result = await _target.Get();

            // assert
            result.DbStatus.Should().NotBe("OK");

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
            LogAssert.AssertError(_fakeLogger, expectedException, Times.Once);
        }

        [Fact]
        public async Task ReturnNotOkDbStatusAndLogErrorOnGetWhenExceptionThrownOnExecuteScalar()
        {
            // arrange
            var expectedException = new Exception("test");            
            _fakeAdoHelper.Setup(o => o.ExecuteScalarAsync(_fakeDbCommand.Object)).ThrowsAsync(expectedException);

            // act
            var result = await _target.Get();

            // assert
            result.DbStatus.Should().NotBe("OK");

            LogAssert.AssertInfo(_fakeLogger, Times.AtLeastOnce);
            LogAssert.AssertError(_fakeLogger, expectedException, Times.Once);
        }
    }
}
