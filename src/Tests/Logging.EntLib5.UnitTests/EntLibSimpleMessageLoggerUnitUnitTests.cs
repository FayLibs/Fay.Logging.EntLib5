using System;
using System.Diagnostics;
using System.Reflection;
using Fay.Logging;
using Fay.Logging.EntLib5;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Ploeh.AutoFixture.Xunit2;
using Shouldly;
using Xunit;

namespace Logging.EntLib5.UnitTests
{
    public class EntLibSimpleMessageLoggerUnitUnitTests : EntLibLoggerUnitTests
    {
        [Theory]
        [InlineAutoData("Critical")]
        [InlineAutoData("Error")]
        [InlineAutoData("Warning")]
        [InlineAutoData("Information")]
        [InlineAutoData("Verbose")]
        public void LogWriterWriteProvidesValidLogEntryGivenInScopeMessage(string methodName, string category, string expectedMessage)
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LogEntry actualLogEntry = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = {Tuple.Create(category, SourceLevels.All)};
                var logWriter = CreateShimLogWriterImpl(le => { actualLogEntry = le; }, categoryAndSourceLevels);
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);
                Func<string> writeLogEntry = () => expectedMessage;

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });
                
                // Assert
                actualLogEntry.ShouldNotBeNull();
                actualLogEntry.Message.ShouldBe(expectedMessage);
                actualLogEntry.Categories.Count.ShouldBe(1);
                actualLogEntry.Categories.ShouldContain(category);
            }
        }

        [Theory]
        [InlineAutoData("CriticalException")]
        [InlineAutoData("ErrorException")]
        public void LogWriterWriteProvidesValidLogEntryGivenInScopeMessageWithException(string methodName, string category, string expectedMessage)
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LogEntry actualLogEntry = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, SourceLevels.All) };
                var logWriter = CreateShimLogWriterImpl(le => { actualLogEntry = le; }, categoryAndSourceLevels);
                Func<string> writeLogEntry = () => expectedMessage;
                Exception ex = new Exception(methodName);
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry, ex });

                // Assert
                actualLogEntry.ShouldNotBeNull();
                actualLogEntry.Message.ShouldStartWith(expectedMessage);
                actualLogEntry.Message.ShouldContain(methodName);
                actualLogEntry.Categories.Count.ShouldBe(1);
                actualLogEntry.Categories.ShouldContain(category);
            }
        }

        [Fact]
        public void ExceptionProvidesValidLogEntryGivenInScopeMessageWithOutException()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LogEntry actualLogEntry = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create("General", SourceLevels.All) };
                var logWriter = CreateShimLogWriterImpl(le => { actualLogEntry = le; }, categoryAndSourceLevels);
                Func<string> writeLogEntry = () => "Test";
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);

                // Act
                sut.Exception(LogSeverity.Critical, writeLogEntry, null);

                // Assert
                actualLogEntry.ShouldNotBeNull();
                actualLogEntry.Message.ShouldBe("Test");
                actualLogEntry.Categories.Count.ShouldBe(1);
                actualLogEntry.Categories.ShouldContain("General");
            }
        }
       
        [Theory]
        [InlineAutoData(LogSeverity.Critical, true, true, SourceLevels.All)]
        [InlineAutoData(LogSeverity.Error, true, true, SourceLevels.All)]
        [InlineAutoData(LogSeverity.Warning, true, true, SourceLevels.All)]
        [InlineAutoData(LogSeverity.Information, true, true, SourceLevels.All)]
        [InlineAutoData(LogSeverity.Verbose, true, true, SourceLevels.All)]
        [InlineAutoData(LogSeverity.All, true, true, SourceLevels.All)]
        [InlineAutoData(LogSeverity.Off, true, false, SourceLevels.All)]
        [InlineAutoData(LogSeverity.Off, true, true, SourceLevels.Off)]
        [InlineAutoData(LogSeverity.Off, false, true, SourceLevels.Off)]
        [InlineAutoData((LogSeverity)5000, true, true, SourceLevels.All)]
        [InlineAutoData((LogSeverity)3, true, false, SourceLevels.Error)]
        [InlineAutoData((LogSeverity)(short)-1, true, false, SourceLevels.All, null)]
        public void IsSeverityInScopeReturnsValid(LogSeverity severity, bool isEnabled, bool expectedResult, SourceLevels categoryLevel, string category)
        {
            using (ShimsContext.Create())
            {
                // Arrange                
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, categoryLevel) };
                var logWriter = CreateShimLogWriterImpl(le => { }, categoryAndSourceLevels, isEnabled);
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);

                // Act
                bool result = sut.IsSeverityInScope(severity, null);

                // Assert
                result.ShouldBe(expectedResult);
            }
        }

        [Theory]
        [InlineAutoData("Critical", SourceLevels.Off )]
        [InlineAutoData("Error", SourceLevels.Critical)]
        [InlineAutoData("Warning", SourceLevels.Error)]
        [InlineAutoData("Information", SourceLevels.Warning)]
        [InlineAutoData("Verbose", SourceLevels.Information)]
        public void LogWriterWriteNotCalledGivenOutOfScopeMessage(string methodName, SourceLevels categoryLevel, string category)
        {
            using (ShimsContext.Create())
            {
                // Arrange                  
                bool? writeLogEntryCalled = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, categoryLevel) };
                var logWriter = CreateShimLogWriterImpl(le => { writeLogEntryCalled = true; }, categoryAndSourceLevels);
                Func<string> writeLogEntry = () => string.Empty;                
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });

                // Assert
                writeLogEntryCalled.ShouldBeNull();                               
            }
        }

        [Theory]
        [InlineAutoData("Critical", SourceLevels.Critical)]
        [InlineAutoData("Error", SourceLevels.Error)]
        [InlineAutoData("Warning", SourceLevels.Warning)]
        [InlineAutoData("Information", SourceLevels.Information)]
        [InlineAutoData("Verbose", SourceLevels.Verbose)]
        public void LogWriterWriteNotCalledGivenInScopeEmptyMessage(string methodName, SourceLevels categoryLevel, string category)
        {
            using (ShimsContext.Create())
            {
                // Arrange                  
                bool? writeLogEntryCalled = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, categoryLevel) };
                var logWriter = CreateShimLogWriterImpl(le => { writeLogEntryCalled = true; }, categoryAndSourceLevels);
                Func<string> writeLogEntry = () => string.Empty;
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });

                // Assert
                writeLogEntryCalled.ShouldBeNull();
            }
        }

        [Fact]
        public void ExceptionDoesNotLogGivenNulldelegateAndException()
        {
            using (ShimsContext.Create())
            {
                // Arrange  
                bool? writeLogEntryCalled = null;
                var logWriter = CreateShimLogWriterImpl(le => { writeLogEntryCalled = true; });
                IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter);

                // Act
                sut.Exception(LogSeverity.Critical, null, null);

                // Assert
                writeLogEntryCalled.ShouldBeNull();
            }
        }        
    }
}