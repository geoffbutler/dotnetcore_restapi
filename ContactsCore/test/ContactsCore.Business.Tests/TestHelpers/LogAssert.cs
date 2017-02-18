using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace ContactsCore.Business.Tests.TestHelpers
{
    public static class LogAssert
    {
        public static void AssertInfo<T>(Mock<ILogger<T>> fakeLogger, Func<Times> times)
            where T : class
        {
            fakeLogger.Verify(o => o.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()), times);
        }

        public static void AssertWarn<T>(Mock<ILogger<T>> fakeLogger, Func<Times> times)
            where T : class
        {
            fakeLogger.Verify(o => o.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()), times);
        }

        public static void AssertError<T>(Mock<ILogger<T>> fakeLogger, Exception expectedException, Func<Times> times)
            where T : class
        {            
            fakeLogger.Verify(o => o.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()), times);
        }
    }
}
