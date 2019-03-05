using System.IO;

namespace FileRenamer
{
    public interface IReplaceRule
    {
        string Apply(FileInfo fileInfo);

        string Apply(DirectoryInfo directoryInfo);

        string Apply(string text);
    }
}