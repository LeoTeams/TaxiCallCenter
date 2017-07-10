using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp.Events
{
    public class RecordingCompleteEventArgs : EventArgs
    {
        public RecordingCompleteEventArgs(Byte[] recordingBytes)
        {
            this.RecordingBytes = recordingBytes;
        }

        public Byte[] RecordingBytes { get; }
    }
}
