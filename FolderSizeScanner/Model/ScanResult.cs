using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderSizeScanner.Model
{
    public class ScanResult
    {
        public string Folder { get; }

        public long Size { get; set; }

        public long FullSize
        {
            get { return Size + InnerResults.Sum(r => r.FullSize); }
        }

        public ICollection<ScanResult> InnerResults { get; }

        public Exception Exception { get; set; }

        public ScanResult(string folder)
        {
            Folder = folder;
            InnerResults = new List<ScanResult>();
        }

        public override string ToString()
        {
            return new DirectoryInfo(Folder).Name;
        }
    }
}
