using System;
using System.Diagnostics;
using System.Reflection;
using Fay.Logging;
using Fay.Logging.EntLib5;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Fakes;
using Shouldly;
using Xunit;

namespace Logging.EntLib5.UnitTests
{
    public class EntLibSimpleCategoryLoggerUnitUnitTests : EntLibLoggerUnitTests
    {
        [Theory]
        [InlineData("Critical", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All })]
        [InlineData("Error", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All })]
        [InlineData("Warning", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All })]
        [InlineData("Information", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All })]
        [InlineData("Verbose", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All })]
        [InlineData("Verbose", "Test", null, null, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All })]
        public void LogWriterWriteProvidesValidLogEntryGivenInScopeMessage(string methodName, string expectedMessage, string[] expectedCategories, SourceLevels[] expectedSourceLevels, string[] totalCategories, SourceLevels[] totalSourceLevels)
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LogEntry actualLogEntry = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = CreateCategoriesAndSourceLevelsTuple(totalCategories, totalSourceLevels);
                ShimLogWriterImpl logWriter = CreateShimLogWriterImpl(le => { actualLogEntry = le; }, categoryAndSourceLevels);
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);
                Func<MessageWithCategories> writeLogEntry = () => new MessageWithCategories(expectedMessage, expectedCategories);

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });

                // Assert
                actualLogEntry.ShouldNotBeNull();
                actualLogEntry.Message.ShouldBe(expectedMessage);
                actualLogEntry.Categories.Count.ShouldBe(expectedCategories?.Length ?? 1);
                actualLogEntry.Categories.ShouldContain(expectedCategories == null ? "General" : expectedCategories[0]);
            }
        }


        [Theory]
        [InlineData("CriticalException", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All })]
        [InlineData("ErrorException", "Test", new[] { "General", "Special" }, new[] { SourceLevels.All, SourceLevels.All })]
        public void LogWriterWriteProvidesValidLogEntryGivenInScopeMessageWithException(string methodName, string expectedMessage, string[] expectedCategories, SourceLevels[] expectedSourceLevels)
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LogEntry actualLogEntry = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = CreateCategoriesAndSourceLevelsTuple(expectedCategories, expectedSourceLevels);
                ShimLogWriterImpl logWriter = CreateShimLogWriterImpl(le => { actualLogEntry = le; }, categoryAndSourceLevels);
                Func<MessageWithCategories> writeLogEntry = () => new MessageWithCategories(expectedMessage, expectedCategories);
                Exception ex = new Exception(methodName);
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry, ex });

                // Assert
                actualLogEntry.ShouldNotBeNull();
                actualLogEntry.Message.ShouldStartWith(expectedMessage);
                actualLogEntry.Message.ShouldContain(methodName);
                actualLogEntry.Categories.Count.ShouldBe(expectedCategories.Length);
                actualLogEntry.Categories.ShouldContain(expectedCategories[0]);
            }
        }

        [Fact]
        public void ExceptionProvidesValidLogEntryGivenInScopeMessageWithOutException()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LogEntry actualLogEntry = null;
                string expectedMessage = "Test";
                string[] categories = new[] { "General", "Special" };
                SourceLevels[] sourceLevels = new[] { SourceLevels.All, SourceLevels.All };
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = CreateCategoriesAndSourceLevelsTuple(categories, sourceLevels);
                ShimLogWriterImpl logWriter = CreateShimLogWriterImpl(le => { actualLogEntry = le; }, categoryAndSourceLevels);
                Func<MessageWithCategories> writeLogEntry = () => new MessageWithCategories(expectedMessage, categories);
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);

                // Act
                sut.Exception(LogSeverity.Critical, writeLogEntry, null);

                // Assert
                actualLogEntry.ShouldNotBeNull();
                actualLogEntry.Message.ShouldBe(expectedMessage);
                actualLogEntry.Categories.Count.ShouldBe(categories.Length);
                actualLogEntry.Categories.ShouldBe(categories);
            }
        }

        [Theory]
        [InlineData(LogSeverity.Critical, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Error, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Warning, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Information, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Verbose, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.All, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Off, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, false)]
        [InlineData(LogSeverity.Off, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Off, SourceLevels.Off, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Off, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Off, SourceLevels.Off, SourceLevels.All }, false, true)]
        [InlineData((LogSeverity)5000, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData((LogSeverity)3, new[] { "General", "Special" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Error, SourceLevels.Error, SourceLevels.All }, true, false)]
        [InlineData((LogSeverity)(short)-1, null, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, false)]       
        [InlineData((LogSeverity)(short)-1, null, null, null, true, false)]
        [InlineData(LogSeverity.Verbose, null, null, null, true, false)]
        [InlineData(LogSeverity.Verbose, null, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, true)]
        [InlineData(LogSeverity.Verbose, null, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Critical, SourceLevels.All, SourceLevels.All }, true, false)]
        [InlineData(LogSeverity.Information, new[] { "Fake", "Nothing" }, new[] { "General", "Special", "Extra" }, new[] { SourceLevels.All, SourceLevels.All, SourceLevels.All }, true, false)]
        public void IsSeverityInScopeReturnsValid(LogSeverity severity, string[] expectedCategories, string[] totalCategories, SourceLevels[] totalSourceLevels, bool isEnabled, bool expectedResult)
        {
            using (ShimsContext.Create())
            {
                // Arrange                
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = CreateCategoriesAndSourceLevelsTuple(totalCategories, totalSourceLevels);
                var logWriter = CreateShimLogWriterImpl(le => { }, categoryAndSourceLevels, isEnabled);
                Func<MessageWithCategories> writeLogEntry = () => new MessageWithCategories(string.Empty, expectedCategories);
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);

                // Act
                bool result = sut.IsSeverityInScope(severity, writeLogEntry);

                // Assert
                result.ShouldBe(expectedResult);
            }
        }

        [Theory]
        [InlineData("Critical", new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Off, SourceLevels.Off, SourceLevels.All })]
        [InlineData("Error", new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Critical, SourceLevels.Critical, SourceLevels.All })]
        [InlineData("Warning", new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Error, SourceLevels.Error, SourceLevels.All })]
        [InlineData("Information", new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Warning, SourceLevels.Warning, SourceLevels.All })]
        [InlineData("Verbose", new[] { "General", "Special", "Extra" }, new[] { SourceLevels.Information, SourceLevels.Information, SourceLevels.All })]
        public void LogWriterWriteNotCalledGivenOutOfScopeMessage(string methodName, string[] totalCategories, SourceLevels[] totalSourceLevels)
        {
            using (ShimsContext.Create())
            {
                // Arrange                  
                bool? writeLogEntryCalled = null;
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = CreateCategoriesAndSourceLevelsTuple(totalCategories, totalSourceLevels);
                var logWriter = CreateShimLogWriterImpl(le => { writeLogEntryCalled = true; }, categoryAndSourceLevels);
                Func<MessageWithCategories> writeLogEntry = () => new MessageWithCategories(string.Empty, "General", "Special");
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);

                // Act
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });

                // Assert
                writeLogEntryCalled.ShouldBeNull();
            }
        }

        [Theory]
        [InlineData("Critical", SourceLevels.Critical)]
        [InlineData("Error", SourceLevels.Error)]
        [InlineData("Warning", SourceLevels.Warning)]
        [InlineData("Information", SourceLevels.Information)]
        [InlineData("Verbose", SourceLevels.Verbose)]
        public void LogWriterWriteNotCalledGivenInScopeEmptyMessage(string methodName, SourceLevels defaultSeverity)
        {
            using (ShimsContext.Create())
            {
                // Arrange                  
                bool? writeLogEntryCalled = null;
                string[] categories = new[] { "General", "Special" };
                SourceLevels[] sourceLevels = new[] { SourceLevels.All, SourceLevels.All };
                Tuple<string, SourceLevels>[] categoryAndSourceLevels = CreateCategoriesAndSourceLevelsTuple(categories, sourceLevels);
                var logWriter = CreateShimLogWriterImpl(le => { writeLogEntryCalled = true; }, categoryAndSourceLevels);
                Func<MessageWithCategories> writeLogEntry = () => new MessageWithCategories(string.Empty, categories);
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);

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
                IDelegateLogger<MessageWithCategories> sut = new EntLibSimpleCategoryLogger(logWriter);

                // Act
                sut.Exception(LogSeverity.Critical, null, null);

                // Assert
                writeLogEntryCalled.ShouldBeNull();
            }
        }
    }
}