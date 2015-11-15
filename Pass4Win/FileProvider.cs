using System.IO;

namespace Pass4Win
{
    public interface IFileProvider
    {
        string Name { get; set; }
        string FullName { get; set; }
        string Extension { get; set; }
    }
    public class FileProvider : IFileProvider
    {
        private readonly FileInfo _fileInfo;

        public FileProvider(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public string Name
        {
            get { return _fileInfo.Name; }
            set { }
        }

        public string FullName
        {
            get { return _fileInfo.FullName; }
            set { }
        }

        public string Extension
        {
            get { return _fileInfo.Extension; }
            set { }
        }
    }
}
