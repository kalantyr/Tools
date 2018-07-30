using System;
using System.IO;

namespace FileRenamer
{
    public class SimpleReplaceRule : IReplaceRule
    {
        private readonly string _oldValue;
        private readonly string _newValue;
        private readonly bool _equals;
        private static readonly string[] ExcludeExtensions = { ".dll", ".exe", ".pdb", ".nupkg" };

        public SimpleReplaceRule(string oldValue, string newValue, bool equals = false)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            _equals = @equals;
        }

        public string Apply(FileInfo fileInfo)
        {
            foreach (var ext in ExcludeExtensions)
                if (string.Equals(fileInfo.Extension, ext, StringComparison.InvariantCultureIgnoreCase))
                    return fileInfo.FullName;

            if (_equals && Path.GetFileNameWithoutExtension(fileInfo.Name) != _oldValue)
                return fileInfo.FullName;

            if (!fileInfo.Name.Contains(_oldValue))
                return fileInfo.FullName;

            return Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(_oldValue, _newValue));
        }

        public string Apply(DirectoryInfo directoryInfo)
        {
            if (_equals && directoryInfo.Name != _oldValue)
                   return directoryInfo.FullName;

            if (!directoryInfo.Name.Contains(_oldValue))
                return directoryInfo.FullName;

            return Path.Combine(directoryInfo.Parent.FullName, directoryInfo.Name.Replace(_oldValue, _newValue));
        }

        public string Apply(string text)
        {
            if (!text.Contains(_oldValue))
                return text;

            return text.Replace(_oldValue, _newValue);
        }
    }
}
