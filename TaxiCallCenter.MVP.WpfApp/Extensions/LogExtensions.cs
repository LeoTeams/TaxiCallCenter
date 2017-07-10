using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiCallCenter.MVP.WpfApp.Models;

namespace TaxiCallCenter.MVP.WpfApp.Extensions
{
    internal static class LogExtensions
    {
        internal static void LogEvent(this ICollection<LogEntry> log, String eventText)
        {
            log.Add(new LogEntry { Text = eventText });
        }
    }
}
