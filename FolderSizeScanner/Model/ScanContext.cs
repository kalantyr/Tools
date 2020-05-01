using System.IO;

namespace FolderSizeScanner.Model
{
    public class ScanContext
    {
        public DirectoryInfo Root { get; set; }

        public ScanResult Result { get; set; }

        public string SearchPattern => "*.*";
    }
}
