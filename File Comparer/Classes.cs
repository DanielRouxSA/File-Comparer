using System;

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

    enum FileComparisonResult
    {
        FilesMatch,
        FilesDiffer,
        FirstFileMissing,
        SecondFileMissing
    }

    class FileComparison
    {

        public string?              firstFile   { get; set; }
        public string?              secondFile  { get; set; }
        public FileComparisonResult result      { get; set; }

        public FileComparison(string? firstFile, string? secondFile, FileComparisonResult result)
        {
            this.firstFile  = firstFile;
            this.secondFile = secondFile;
            this.result     = result;
        }
    }
}
