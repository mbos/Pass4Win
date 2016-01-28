using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pass4Win
{
    public interface IDirectoryProvider
    {
        IFileProvider[] GetFiles();
        IFileProvider[] GetFiles(string searchPattern, SearchOption searchOption);
        string Name { get; set; }
        string FullName { get; set; }
        IEnumerable<FileInfo> EnumerateFiles();
        IEnumerable<IDirectoryProvider> GetDirectories();
    }
    public class DirectoryProvider : IDirectoryProvider
    {
        private readonly ConfigHandling _config;
        private DirectoryInfo _directoryInfo;

        public DirectoryProvider(ConfigHandling config)
        {
            _config = config;

            try
            {
                _directoryInfo = new DirectoryInfo(_config["PassDirectory"]);
            }
            catch (ArgumentException)
            {
            }
        }

        public DirectoryProvider(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public IFileProvider[] GetFiles()
        {
            if (_directoryInfo?.Exists == false)
                _directoryInfo.Create();
            return _directoryInfo?.GetFiles().Select(f=>(IFileProvider)new FileProvider(f)).ToArray() ?? new IFileProvider[0];
        }

        public IFileProvider[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return _directoryInfo?.GetFiles(searchPattern, searchOption).Select(f=>(IFileProvider) new FileProvider(f)).ToArray() ?? new IFileProvider[0];
        }

        public string Name
        {
            get { return _directoryInfo.Name; }
            set { }
        }

        public string FullName
        {
            get { return _directoryInfo.FullName; }
            set { }
        }

        public IEnumerable<FileInfo> EnumerateFiles()
        {
            return _directoryInfo.EnumerateFiles();
        }

        public IEnumerable<IDirectoryProvider> GetDirectories()
        {
            return _directoryInfo.GetDirectories().Select(d=>new DirectoryProvider(d));
        }
    }
}
