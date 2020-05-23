using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace FileComparer
{
    class Program
    {
        static void Main()
        {
            Stopwatch stopwatch     = new Stopwatch();
            Stopwatch taskStopwatch = new Stopwatch();

            Console.WriteLine("Enter first directory:");
            string firstDirectory = Console.ReadLine();

            Console.WriteLine("Enter second directory:");
            string secondDirectory = Console.ReadLine();

            stopwatch.Start();

            taskStopwatch.Start();

            List<string> firstDirectoryFiles    = GetFiles(firstDirectory);
            List<string> secondDirectoryFiles   = GetFiles(secondDirectory);

            taskStopwatch.Stop();
            OutputElapsedTime(taskStopwatch, "Enumerating files completed");
            taskStopwatch.Reset();

            List<(string file, byte[] checksum)> firstDirectoryChecksums = GetChecksums(firstDirectoryFiles);
            List<(string file, byte[] checksum)> secondDirectoryChecksums = GetChecksums(secondDirectoryFiles);

            foreach (var checksum in firstDirectoryChecksums)
            {
                Console.WriteLine($"{checksum.file} -> {BitConverter.ToString(checksum.checksum)}");
            }

            stopwatch.Stop();

            OutputElapsedTime(stopwatch, "Finished");

            Console.ReadLine();
        }

        

        static List<(string file, byte[] checksum)> GetChecksums(List<string> files)
        {
            List<(string file, byte[] checksum)> checksums = new List<(string file, byte[] checksum)>();
            
            using MD5 md5 = MD5.Create();
            foreach (string file in files)
            {
                using FileStream stream = File.OpenRead(file);
                checksums.Add((file, md5.ComputeHash(stream)));
            }

            return checksums;
        }

        static List<string> GetFiles(string directory)
        {
            List<string> files = new List<string>();

            var subfolders = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (var subfolder in subfolders)
            {
                files.AddRange(GetFiles(subfolder));
            }
            
            files.AddRange(Directory.GetFiles(directory));

            return files;
        }

        static void OutputElapsedTime(Stopwatch stopwatch, string message)
        {
            TimeSpan elapsedTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

            string hours = elapsedTime.Hours >= 10 ? $"{elapsedTime.Hours}" : $"0{elapsedTime.Hours}";
            string minutes = elapsedTime.Minutes >= 10 ? $"{elapsedTime.Minutes}" : $"0{elapsedTime.Minutes}";
            string seconds = elapsedTime.Seconds >= 10 ? $"{elapsedTime.Seconds}" : $"0{elapsedTime.Seconds}";

            CustomConsole.WriteLineWithBreak($"{message} - elapsed time: {hours}:{minutes}:{seconds}");
        }
    }
}
