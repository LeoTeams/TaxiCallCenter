using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public static class PhoneParser
    {
        private static readonly Regex PhoneRegex = new Regex(@"^(?:7|8)(?<phone>9\d{9})$");

        public static String ParsePhone(String input)
        {
            var digits = new StringBuilder();
            foreach (var character in input)
            {
                if (Char.IsDigit(character))
                {
                    digits.Append(character);
                }
            }

            var number = digits.ToString();

            Match match;
            if ((match = PhoneParser.PhoneRegex.Match(number)).Success)
            {
                return match.Groups["phone"].Value;
            }

            return null;
        }
    }
}
