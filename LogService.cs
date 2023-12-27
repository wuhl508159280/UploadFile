using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteDataManager.Services
{
    public static class LogService
    {
        public static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }
}
