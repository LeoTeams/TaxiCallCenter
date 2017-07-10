using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiCallCenter.MVP.WpfApp
{
    public class AudioSaver
    {
        private readonly String basePath = "D:\\DevGuild\\NPK\\Vera\\Data";

        public void SaveBytes(Guid sessionId, String type, Guid messageId, Byte[] bytes)
        {
            var fileName = $"{sessionId:N}-{type}-{messageId:N}.wav";
            using (var fileStream = new FileStream(Path.Combine(this.basePath, fileName), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
