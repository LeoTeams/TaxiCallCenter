using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public class OrderInfo
    {
        public String AddressFromStreet { get; set; }

        public String AddressFromHouse { get; set; }

        public String AddressToStreet { get; set; }

        public String AddressToHouse { get; set; }

        public String DateTime { get; set; }

        public String Phone { get; set; }

        public String AdditionalInfo { get; set; }

        public DateTime TrueDateTime { get; set; }
    }
}
