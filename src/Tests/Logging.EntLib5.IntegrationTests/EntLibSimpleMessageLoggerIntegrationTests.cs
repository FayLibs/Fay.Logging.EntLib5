using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Fay.Logging;
using Fay.Logging.EntLib5;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Ploeh.AutoFixture.Xunit2;
using Shouldly;
using Xunit;

namespace Logging.EntLib5.IntegrationTests
{
    public class EntLibSimpleMessageLoggerIntegrationTests : EntLibLoggerTests, IDisposable
    {
        private readonly string _tempFilename;

        public EntLibSimpleMessageLoggerIntegrationTests()
        {
            _tempFilename = Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (_tempFilename != null && File.Exists(_tempFilename))
                File.Delete(_tempFilename);
        }

        [Theory]
        [InlineAutoData("Critical")]
        [InlineAutoData("Error")]
        [InlineAutoData("Warning")]
        [InlineAutoData("Information")]
        [InlineAutoData("Verbose")]
        public void LogWriterWriteProvidesValidLogEntryGivenInScopeMessage(string methodName, string category, string expectedMessage)
        {
            // Arrange            
            Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, SourceLevels.All) };            
            LogWriter logWriter = CreateLogWriter(_tempFilename, categoryAndSourceLevels);
            Func<string> writeLogEntry = () => expectedMessage;

            // Act
            using (IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });
            string[] logLines = File.ReadAllLines(_tempFilename);

            // Assert
            logLines.ShouldNotBeNull();
            logLines.Length.ShouldBe(1);
            logLines[0].ShouldBe(string.Format(LogLineFormatter, methodName, category, expectedMessage));
        }


        [Theory]
        [InlineAutoData("CriticalException")]
        [InlineAutoData("ErrorException")]
        public void LogWriterWriteProvidesValidLogEntryGivenInScopeMessageWithException(string methodName, string category, string expectedMessage)
        {
            // Arrange
            Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, SourceLevels.All) };
            LogWriter logWriter = CreateLogWriter(_tempFilename, categoryAndSourceLevels);
            Func<string> writeLogEntry = () => expectedMessage;
            Exception ex = new Exception(methodName);

            // Act
            using (IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry, ex });
            string[] logLines = File.ReadAllLines(_tempFilename);

            // Assert
            logLines.ShouldNotBeNull();
            logLines.Length.ShouldBe(4);
            logLines[0].ShouldBe(string.Format(LogLineFormatter, methodName.Replace("Exception", string.Empty), category, expectedMessage));
            logLines[1].ShouldBe(EntLibSimpleMessageLogger.BeginExceptionDetailsText);
            logLines[2].ShouldBe(ex.ToString());
            logLines[3].ShouldBe(EntLibSimpleMessageLogger.EndExceptionDetailsText);
        }

        [Fact]
        public void ExceptionProvidesValidLogEntryGivenInScopeMessageWithOutException()
        {
            // Arrange
            Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create("General", SourceLevels.All) };
            LogWriter logWriter = CreateLogWriter(_tempFilename, categoryAndSourceLevels);
            Func<string> writeLogEntry = () => "Test";


            // Act
            using (IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                sut.Exception(LogSeverity.Critical, writeLogEntry, null);
            string[] logLines = File.ReadAllLines(_tempFilename);

            // Assert
            logLines[0].ShouldBe(string.Format(LogLineFormatter, "Critical", "General", "Test"));
        }

        [Theory]
        [InlineData(LogSeverity.Critical, SourceLevels.All, "General", true, true)]
        [InlineData(LogSeverity.Error, SourceLevels.All, "General", true, true)]
        [InlineData(LogSeverity.Warning, SourceLevels.All, "General", true, true)]
        [InlineData(LogSeverity.Information, SourceLevels.All, "General", true, true)]
        [InlineData(LogSeverity.Verbose, SourceLevels.All, "General", true, true)]
        [InlineData(LogSeverity.All, SourceLevels.All, "General", true, true)]
        [InlineData(LogSeverity.Off, SourceLevels.All, "General", true, false)]
        [InlineData(LogSeverity.Off, SourceLevels.Off, "General", true, true)]
        [InlineData(LogSeverity.Off, SourceLevels.Off, "General", false, true)]
        [InlineData((LogSeverity)5000, SourceLevels.All, "General", true, true)]
        [InlineData((LogSeverity)3, SourceLevels.Error, "General", true, false)]
        [InlineData((LogSeverity) (short) -1, SourceLevels.All, null, true, false)]
        public void IsSeverityInScopeReturnsValid(LogSeverity severity, SourceLevels categoryLevel, string category, bool isEnabled, bool expectedResult)
        {
            // Arrange                
            Tuple<string, SourceLevels>[] categoryAndSourceLevels = {Tuple.Create(category, categoryLevel)};
            LogWriter logWriter = CreateLogWriter(_tempFilename, categoryAndSourceLevels, isEnabled);
            
            // Act
            bool result;
            using (IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                 result = sut.IsSeverityInScope(severity, null);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineAutoData("Critical", SourceLevels.Off)]
        [InlineAutoData("Error", SourceLevels.Critical)]
        [InlineAutoData("Warning", SourceLevels.Error)]
        [InlineAutoData("Information", SourceLevels.Warning)]
        [InlineAutoData("Verbose", SourceLevels.Information)]
        public void LogWriterWriteNotCalledGivenOutOfScopeMessage(string methodName, SourceLevels categoryLevel, string category)
        {
            // Arrange                  
            Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, categoryLevel) };
            LogWriter logWriter = CreateLogWriter(_tempFilename, categoryAndSourceLevels);
            Func<string> writeLogEntry = () => string.Empty;
            

            // Act
            using(IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });
            string[] logLines = File.ReadAllLines(_tempFilename);

            // Assert
            logLines.ShouldNotBeNull();
            logLines.Length.ShouldBe(0);
        }

        [Theory]
        [InlineAutoData("Critical", SourceLevels.Critical)]
        [InlineAutoData("Error", SourceLevels.Error)]
        [InlineAutoData("Warning", SourceLevels.Warning)]
        [InlineAutoData("Information", SourceLevels.Information)]
        [InlineAutoData("Verbose", SourceLevels.Verbose)]
        public void LogWriterWriteNotCalledGivenInScopeEmptyMessage(string methodName, SourceLevels categoryLevel, string category)
        {
            // Arrange                              
            Tuple<string, SourceLevels>[] categoryAndSourceLevels = { Tuple.Create(category, categoryLevel) };
            LogWriter logWriter = CreateLogWriter(_tempFilename, categoryAndSourceLevels);
            Func<string> writeLogEntry = () => string.Empty;
            
            // Act
            using (IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });
            string[] logLines = File.ReadAllLines(_tempFilename);

            // Assert
            logLines.ShouldNotBeNull();
            logLines.Length.ShouldBe(0);
        }
        
        [Fact]
        public void ExceptionDoesNotLogGivenNulldelegateAndException()
        {
            // Arrange  
            LogWriter logWriter = CreateLogWriter(_tempFilename);
            
            // Act
            using (IDelegateLogger<string> sut = new EntLibSimpleMessageLogger(logWriter))
                sut.Exception(LogSeverity.Critical, null, null);
            string[] logLines = File.ReadAllLines(_tempFilename);

            // Assert
            logLines.ShouldNotBeNull();
            logLines.Length.ShouldBe(0);
        }


    }
}
