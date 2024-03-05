using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class DownloadProgressEntity
    {
        public long BytesReceived { get; set; }
        public long TotalBytes { get; set; }
        public double ProgressPercentage { get; set; }
        public double Speed { get; set; }
    }
}
