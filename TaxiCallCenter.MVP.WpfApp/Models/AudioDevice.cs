using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public class AudioDevice
    {
        public Int32 Id { get; set; }

        public String Name { get; set; }

        public override String ToString()
        {
            return this.Name;
        }
    }
}
