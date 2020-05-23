using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

            List<FileChecksum> firstDirectoryChecksums = GetChecksums(firstDirectoryFiles);
            List<FileChecksum> secondDirectoryChecksums = GetChecksums(secondDirectoryFiles);

            foreach (var checksum in firstDirectoryChecksums)
            {
                Console.WriteLine($"{checksum.filePath} -> {checksum.checksum}");
            }

            var fileComparisons = CompareFiles(firstDirectoryChecksums, secondDirectoryChecksums);

            stopwatch.Stop();

            OutputElapsedTime(stopwatch, "Finished");

            Console.ReadLine();
        }


        static List<FileComparison> CompareFiles(List<FileChecksum> firstList, List<FileChecksum> secondList)
        {
            List<FileComparison> fileComparisons = new List<FileComparison>();

            // First, iterate through first list
            foreach (FileChecksum firstFile in firstList)
            {
                FileChecksum? secondFile = secondList.Where(x => x.filePath == firstFile.filePath).FirstOrDefault();

                if (secondFile is null)
                {
                    fileComparisons.Add(new FileComparison(firstFile.filePath, null, FileComparisonResult.SecondFileMissing));
                }
                else
                {
                    FileComparisonResult result = firstFile.checksum == secondFile.checksum
                    ? FileComparisonResult.FilesMatch
                    : FileComparisonResult.FilesDiffer;

                    fileComparisons.Add(new FileComparison(firstFile.filePath, secondFile.filePath, result));
                    secondList.Remove(secondFile);
                }
                firstList.Remove(firstFile);
            }

            // Second, iterate through remaining items in second list
            foreach (FileChecksum secondFile in secondList)
            {
                fileComparisons.Add(new FileComparison(null, secondFile.filePath, FileComparisonResult.FirstFileMissing));
            }

            return fileComparisons;
        }

        static List<FileChecksum> GetChecksums(List<string> files)
        {
            List<FileChecksum> checksums = new List<FileChecksum>();


#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
            using MD5 md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
            foreach (string file in files)
            {
                using FileStream stream = File.OpenRead(file);
                checksums.Add(new FileChecksum(file, md5.ComputeHash(stream)));
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
