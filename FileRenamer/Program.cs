using System;
using System.Collections.Generic;
using System.IO;

namespace FileRenamer
{
    public class Program
    {
        static void Main()
        {
            var rootFoler = new DirectoryInfo(@"Q:\Work\_Scr");
            var rules = new []
            {
                new SimpleReplaceRule("Чёрное", "Белое")
            };

            Rename(rootFoler, rules);
            Console.Out.WriteLine(Environment.NewLine + "Renaming done.");

            Console.Out.WriteLine(Environment.NewLine + "Replace in text? Y/N");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Replace(rootFoler, rules);
                Console.Out.WriteLine(Environment.NewLine + "Replacing done.");
            }

            Console.Out.WriteLine(Environment.NewLine + "Press any key to exit");
            Console.ReadKey();
        }

        private static void Rename(DirectoryInfo rootFoler, IReadOnlyCollection<IReplaceRule> rules)
        {
            foreach (var directoryInfo in rootFoler.GetDirectories())
                Rename(directoryInfo, rules);

            foreach (var renameRule in rules)
                foreach (var fileInfo in rootFoler.GetFiles())
                    try
                    {
                        var newName = renameRule.Apply(fileInfo);
                        if (newName != fileInfo.FullName)
                        {
                            File.Move(fileInfo.FullName, newName);
                            Console.Out.WriteLine(fileInfo.FullName + " -> " + newName);
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.Error.WriteLine(exc.Message);
                    }

            foreach (var renameRule in rules)
                foreach (var directoryInfo in rootFoler.GetDirectories())
                    try
                    {
                        var newName = renameRule.Apply(directoryInfo);
                        if (newName != directoryInfo.FullName)
                        {
                            Directory.Move(directoryInfo.FullName, newName);
                            Console.Out.WriteLine(directoryInfo.FullName + " -> " + newName);
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.Error.WriteLine(exc.Message);
                    }
        }

        private static void Replace(DirectoryInfo rootFoler, IReadOnlyCollection<IReplaceRule> rules)
        {
            foreach (var directoryInfo in rootFoler.GetDirectories())
                Replace(directoryInfo, rules);

            foreach (var rule in rules)
                foreach (var fileInfo in rootFoler.GetFiles())
                    try
                    {
                        var text = GetFileContent(fileInfo);
                        var modifiedText = rule.Apply(text);
                        if (modifiedText != text)
                        {
                            SetFileContent(fileInfo, modifiedText);
                            Console.Out.WriteLine(fileInfo.Name);
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.Error.WriteLine(exc.Message);
                    }
        }

        private static string GetFileContent(FileInfo fileInfo)
        {
            using (var file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(file))
                return reader.ReadToEnd();
        }

        private static void SetFileContent(FileInfo fileInfo, string text)
        {
            using (var file = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(file))
                writer.Write(text);
        }
    }
}
