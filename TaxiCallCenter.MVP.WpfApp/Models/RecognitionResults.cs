using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Models
{
    public class RecognitionResults
    {
        public Boolean Success { get; set; }

        public List<RecognitionVariant> Variants { get; } = new List<RecognitionVariant>();
    }

    public class RecognitionVariant
    {
        public Double Confidence { get; set; }

        public String Text { get; set; }
    }
}
