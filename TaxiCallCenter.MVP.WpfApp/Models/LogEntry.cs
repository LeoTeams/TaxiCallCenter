using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public class LogEntry
    {
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public String Text { get; set; }
    }
}
