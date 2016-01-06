Fay Logging EntLib5
===

What is it?
---
It is a [Fay Logging][FayLog] facade for the [Microsoft Enterprise Library Logging Application Block version 5][EntLib5Log]. This facade provides a simple delegate logging API making logging easier, while helping to make sure any of the code required to generate a log message is not executed unless the logging level is within scope.

Quick Start
---
Below is a simplified example for logging strings when you don't use categories. A more robust implementation may use a service locator or dependency inject framework to initialize the logger.

```cs
    // Initialize logger someplace
    IDelegateLogger<string> logger = new EntLibSimpleMessageLogger(Logger.Writer);
    
    // Use logger as needed
    logger.Critical(() => string.Format("Some text blah {0} blah", "blah"));
    
    logger.Verbose(() =>
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Foo");
        builder.Append(" ");
        builder.Append("Bar");
        return builder.ToString();
    });
    
    // When application is done needing the logger its recommend to dispose it
    logger.Dispose();
```
Below is a simple example for logging to categories.
```cs
    // Initialize logger someplace
    IDelegateLogger<MessageWithCategories> logger = new EntLibSimpleCategoryLogger(Logger.Writer);
    
    // Use logger as needed
    logger.Critical(() => new MessageWithCategories(string.Format("Some text blah {0} blah", "blah"), "General", "Foo"));
    
    logger.Verbose(() =>
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Foo");
        builder.Append(" ");
        builder.Append("Bar");
        return new MessageWithCategories(builder.ToString(), "General", "Foo");
    });
    
    // When application is done needing the logger its recommend to dispose it
    logger.Dispose();
```

[FayLog]:  https://github.com/FayLibs/Fay.Logging
[EntLib5Log]: http://msdn.microsoft.com/en-us/library/ff664569(PandP.50).aspx