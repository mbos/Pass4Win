using System.Collections.Generic;
using System.IO;
using Autofac;
using Moq;
using NUnit.Framework;
using Pass4Win;

namespace Pass4WinTests
{
    [TestFixture]
    public class FileSystemInterfaceTests
    {
        [SetUp]
        public void BeforeTest()
        {
            Setup.InitializeContainer();
        }
       
        [Test]
        public void FindMixedCaseFileWIthLowerCaseSearch()
        {
            var fileNameToFind = "caseSensitiveFile.gpg";
            var anotherFile = "somefile.gpg";
            var fileMockOne = new Mock<IFileProvider>();
            var fileMockTwo = new Mock<IFileProvider>();
            fileMockOne.SetupAllProperties();
            fileMockOne.Object.Name = fileNameToFind;
            fileMockOne.Object.FullName = fileNameToFind;
            fileMockTwo.SetupAllProperties();
            fileMockTwo.Object.Name = anotherFile;
            fileMockTwo.Object.FullName = anotherFile;
            var fileProviderMockList = new List<IFileProvider>
            {
                fileMockOne.Object,
                fileMockTwo.Object
            };

            var directoryProviderMock = Setup.Scope.Resolve<Mock<IDirectoryProvider>>();
            directoryProviderMock.Setup(
                m => m.GetFiles(It.IsAny<string>(), It.Is<SearchOption>(so => so == SearchOption.AllDirectories)))
                .Returns(fileProviderMockList.ToArray());
            var fsi = Setup.Scope.Resolve<FileSystemInterface>();
            fsi.Search(fileNameToFind.ToLower());
            Assert.AreEqual(1, fsi.SearchList.Count);
            Assert.AreEqual(fileNameToFind, fsi.SearchList[0]);
        }

        [Test]
        public void FindMixedCaseFileWithMixedCaseSearch()
        {
            var fileNameToFind = "caseSensitiveFile.gpg";
            var anotherFile = "somefile.gpg";
            var fileMockOne = new Mock<IFileProvider>();
            var fileMockTwo = new Mock<IFileProvider>();
            fileMockOne.SetupAllProperties();
            fileMockOne.Object.Name = fileNameToFind;
            fileMockOne.Object.FullName = fileNameToFind;
            fileMockTwo.SetupAllProperties();
            fileMockTwo.Object.Name = anotherFile;
            fileMockTwo.Object.FullName = anotherFile;
            var fileProviderMockList = new List<IFileProvider>
            {
                fileMockOne.Object,
                fileMockTwo.Object
            };

            var directoryProviderMock = Setup.Scope.Resolve<Mock<IDirectoryProvider>>();
            directoryProviderMock.Setup(
                m => m.GetFiles(It.IsAny<string>(), It.Is<SearchOption>(so => so == SearchOption.AllDirectories)))
                .Returns(fileProviderMockList.ToArray());
            var fsi = Setup.Scope.Resolve<FileSystemInterface>();
            fsi.Search(fileNameToFind);
            Assert.AreEqual(1, fsi.SearchList.Count);
            Assert.AreEqual(fileNameToFind, fsi.SearchList[0]);
        }

        [Test]
        public void FindAllFilesWithWildcardSearch()
        {
            var file = "caseSensitiveFile.gpg";
            var anotherFile = "somefile.gpg";
            var fileMockOne = new Mock<IFileProvider>();
            var fileMockTwo = new Mock<IFileProvider>();
            fileMockOne.SetupAllProperties();
            fileMockOne.Object.Name = file;
            fileMockOne.Object.FullName = file;
            fileMockTwo.SetupAllProperties();
            fileMockTwo.Object.Name = anotherFile;
            fileMockTwo.Object.FullName = anotherFile;
            var fileProviderMockList = new List<IFileProvider>
            {
                fileMockOne.Object,
                fileMockTwo.Object
            };

            var directoryProviderMock = Setup.Scope.Resolve<Mock<IDirectoryProvider>>();
            directoryProviderMock.Setup(
                m => m.GetFiles(It.IsAny<string>(), It.Is<SearchOption>(so => so == SearchOption.AllDirectories)))
                .Returns(fileProviderMockList.ToArray());
            var fsi = Setup.Scope.Resolve<FileSystemInterface>();
            fsi.Search("*.*");
            Assert.AreEqual(2, fsi.SearchList.Count);
            Assert.AreEqual(file, fsi.SearchList[0]);
            Assert.AreEqual(anotherFile, fsi.SearchList[1]);
        }

        /// <summary>
        ///     Tests creating of directory tree nodes
        /// </summary>
        [Test]
        public void CreateDirectoryTreeNodes()
        {
            var directoryOne = new Mock<IDirectoryProvider>();
            directoryOne.SetupAllProperties();
            directoryOne.Object.FullName = "directoryone";
            directoryOne.Object.Name = "directoryone";
            directoryOne.Setup(m => m.GetDirectories()).Returns(new List<IDirectoryProvider>());
            Setup.Scope.Resolve<Mock<IDirectoryProvider>>()
                .Setup(m => m.GetDirectories())
                .Returns(new List<IDirectoryProvider>
                {
                    directoryOne.Object
                });

            var fsi = Setup.Scope.Resolve<FileSystemInterface>();
            var nodes = fsi.UpdateDirectoryTree();
            Assert.IsNotNull(nodes);
            Assert.AreEqual(1, nodes.Length);
        }

        /// <summary>
        ///     Tests creating a list files from a given directory
        /// </summary>
        [Test]
        public void GetFileList()
        {
            var file = "afile.gpg";
            var anotherFile = "somefile.gpg";
            var fileMockOne = new Mock<IFileProvider>();
            var fileMockTwo = new Mock<IFileProvider>();
            fileMockOne.SetupAllProperties();
            fileMockOne.Object.Name = file;
            fileMockOne.Object.FullName = file;
            fileMockOne.Object.Extension = ".gpg";
            fileMockTwo.SetupAllProperties();
            fileMockTwo.Object.Name = anotherFile;
            fileMockTwo.Object.FullName = anotherFile;
            fileMockTwo.Object.Extension = ".gpg";
            var fileProviderMockList = new List<IFileProvider>
            {
                fileMockOne.Object,
                fileMockTwo.Object
            };

            var directoryProviderMock = Setup.Scope.Resolve<Mock<IDirectoryProvider>>();
            directoryProviderMock.Setup(
                m => m.GetFiles())
                .Returns(fileProviderMockList.ToArray());
            var fsi = Setup.Scope.Resolve<FileSystemInterface>();
            var list = fsi.UpdateDirectoryList(directoryProviderMock.Object);
            Assert.AreEqual(2, list.Count);
        }
    }
}