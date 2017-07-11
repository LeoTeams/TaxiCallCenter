using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public class Address
    {
        public String StreetType { get; set; }

        public String StreetName { get; set; }

        public String StreetNumber { get; set; }

        public override String ToString()
        {
            return $"{this.StreetType} {this.StreetName} дом {this.StreetNumber}";
        }
    }

    public static class AddressParser
    {
        private static readonly Regex Pattern1 = new Regex(@"(?<streetType>улица|проспект|переулок|площадь) (?<streetName>.+) дом (?<streetNumber>\d+)$");
        private static readonly Regex Pattern2 = new Regex(@"(?<streetType>улица|проспект|переулок|площадь) (?<streetName>.+) (?<streetNumber>\d+)$");

        public static Address ParseAddress(String address)
        {
            Match match;
            if ((match = AddressParser.Pattern1.Match(address)).Success)
            {
                return new Address
                {
                    StreetType = match.Groups["streetType"].Value,
                    StreetName = match.Groups["streetName"].Value,
                    StreetNumber = match.Groups["streetNumber"].Value
                };
            }

            if ((match = AddressParser.Pattern2.Match(address)).Success)
            {
                return new Address
                {
                    StreetType = match.Groups["streetType"].Value,
                    StreetName = match.Groups["streetName"].Value,
                    StreetNumber = match.Groups["streetNumber"].Value
                };
            }

            return null;
        }
    }
}
