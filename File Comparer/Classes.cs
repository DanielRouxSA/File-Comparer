using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileComparer
{
    class FileChecksum
    {
        public string filePath { get; set; }
        public byte[] checksum { get; set; }

        public string checksumString => BitConverter.ToString(checksum);

        public FileChecksum(string filePath, byte[] checksum)
        {
            this.filePath = filePath;
            this.checksum = checksum;
        }
    }

    public enum FileComparisonResult
    {
        FilesMatch,
        FilesDiffer,
        FirstFileMissing,
        SecondFileMissing
    }

    public class FileComparison
    {

        public string?              firstFile   { get; set; }
        public string?              secondFile  { get; set; }
        public FileComparisonResult result      { get; set; }

        public string resultString
        {
            get
            {
                return result switch
                {
                    FileComparisonResult.FilesMatch         => "Files match",
                    FileComparisonResult.FilesDiffer        => "Files differ",
                    FileComparisonResult.FirstFileMissing   => "First file missing",
                    FileComparisonResult.SecondFileMissing  => "Second file missing",
                    _                                       => ""
                };
            }
        }

        public FileComparison(string? firstFile, string? secondFile, FileComparisonResult result)
        {
            this.firstFile  = firstFile;
            this.secondFile = secondFile;
            this.result     = result;
        }
    }

    [Table("file_comparison_results")]
    public class FileComparisonItem
    {
        [Key]
        public int      recordNo    { get; set; }
        public string?  firstFile   { get; set; }
        public string?  secondFile  { get; set; }
        public string   result      { get; set; }

        public FileComparisonItem(FileComparison fileComparison)
        {
            firstFile   = fileComparison.firstFile;
            secondFile  = fileComparison.secondFile;
            result      = fileComparison.resultString;
        }
    }

    public class Context : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

        private DbSet<FileComparisonItem> fileComparisonResults { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public bool TrySaveFileComparisonResults(List<FileComparisonItem> fileComparisonItems)
        {
            try
            {
                Database.ExecuteSqlRaw("TRUNCATE TABLE file_comparison_results");
                fileComparisonResults.AddRange(fileComparisonItems);
                SaveChanges();
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return false;
            }
        }
    }
}
