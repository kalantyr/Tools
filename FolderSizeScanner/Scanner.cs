using System;
using System.IO;
using System.Linq;
using System.Threading;
using FolderSizeScanner.Model;

namespace FolderSizeScanner
{
    public class Scanner
    {
        private readonly ScanContext _scanContext;
        private Thread _thread;
        private volatile bool _stopRequested;

        public Action<Scanner> Completed;

        public Scanner(ScanContext scanContext)
        {
            _scanContext = scanContext ?? throw new ArgumentNullException(nameof(scanContext));
        }

        public bool ScanIsCompletted { get; private set; }

        public void Start()
        {
            _scanContext.Result = new ScanResult(_scanContext.Root.FullName);
            _thread = new Thread(() => { Scan(_scanContext.Result); });
            _thread.Start();
        }

        public void Scan(ScanResult scanResult)
        {
            try
            {
                if (_stopRequested)
                    return;

                scanResult.Size = new DirectoryInfo(scanResult.Folder)
                    .GetFiles(_scanContext.SearchPattern)
                    .Sum(fi => fi.Length);

                foreach (var directoryInfo in new DirectoryInfo(scanResult.Folder).GetDirectories())
                {
                    if (_stopRequested)
                        return;

                    var child = new ScanResult(directoryInfo.FullName);
                    scanResult.InnerResults.Add(child);
                    Scan(child);
                }
            }
            catch (Exception e)
            {
                scanResult.Exception = e;
            }

            if (scanResult == _scanContext.Result)
            {
                ScanIsCompletted = true;
                Completed?.Invoke(this);
            }
        }

        public void Stop()
        {
            _stopRequested = true;
        }
    }
}
