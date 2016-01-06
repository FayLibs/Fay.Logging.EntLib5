using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Fakes;

namespace Logging.EntLib5.UnitTests
{
    public class EntLibLoggerUnitTests
    {
        protected ShimLogWriterImpl CreateShimLogWriterImpl(Action<LogEntry> writeLogEntry, Tuple<string, SourceLevels>[] categoryAndSourceLevels = null, bool isEnable = true)
        {
            ShimLogWriterImpl logWriter = new ShimLogWriterImpl
            {
                WriteLogEntry = le => { writeLogEntry(le); },
                IsLoggingEnabled = () => isEnable,
                TraceSourcesGet = () =>
                {
                    Dictionary<string, LogSource> dictionary = new Dictionary<string, LogSource>();

                    try
                    {
                        if (categoryAndSourceLevels == null || categoryAndSourceLevels.Length == 0 || (categoryAndSourceLevels.Length == 1 && categoryAndSourceLevels[0].Item1 == null))
                            return dictionary;
                    
                        foreach (var level in categoryAndSourceLevels)
                        {
                            dictionary.Add(level.Item1, new LogSource(level.Item1, level.Item2));
                        }
                        return dictionary;
                    }
                    catch (Exception)
                    {
                        return dictionary;                     
                    }
                },
                DisposeBoolean = disposing => { },
            };

            return logWriter;
        }
        
        protected Tuple<string, SourceLevels>[] CreateCategoriesAndSourceLevelsTuple(string[] categories = null, SourceLevels[] sourceLevels = null)
        {
            if (categories == null || categories.Length == 0)
                return new Tuple<string, SourceLevels>[0];

            Tuple<string, SourceLevels>[] catAndSrcLvls = new Tuple<string, SourceLevels>[categories.Length];
            for (int i = 0; i < categories.Length; i++)
            {
                string category = categories[i];

                if (sourceLevels == null || sourceLevels.Length < i)
                    catAndSrcLvls[i] = Tuple.Create(category, SourceLevels.All);
                else
                    catAndSrcLvls[i] = Tuple.Create(category, sourceLevels[i]);
            }

            return catAndSrcLvls;
        }
    }
}