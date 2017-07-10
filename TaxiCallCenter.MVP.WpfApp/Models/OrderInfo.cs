using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public class OrderInfo
    {
        public String AddressFrom { get; set; }

        public String AddressTo { get; set; }

        public String DateTime { get; set; }

        public String Phone { get; set; }

        public String AdditionalInfo { get; set; }
    }
}
