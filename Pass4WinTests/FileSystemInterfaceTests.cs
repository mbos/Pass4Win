namespace Pass4Win.Tests
{
    using System.IO;
    using System.Windows.Forms;

    using NUnit.Framework;

    /// <summary>
    /// The pwgen tests.
    /// </summary>
    [TestFixture()]
    public class FileSystemInterfaceTests
    {
        /// <summary>
        /// Tests creating of directory tree nodes
        /// </summary>
        [Test()]
        public void DirectoryTest()
        {

            FileSystemInterface fsi = new FileSystemInterface(Application.LocalUserAppDataPath);
            TreeNode[] nodes = fsi.UpdateDirectoryTree(new DirectoryInfo(Application.LocalUserAppDataPath));
            Assert.IsNotNull(nodes);
        }
        
        /// <summary>
        /// Tests creating a list files from a given directory
        /// </summary>
        [Test()]
        public void FileTest()
        {

            FileSystemInterface fsi = new FileSystemInterface(Application.LocalUserAppDataPath);
            var myList = fsi.UpdateDirectoryList(new DirectoryInfo(Application.LocalUserAppDataPath));
            foreach (var row in myList)
            {
                Assert.IsNotNull(row);
                FileInfo tmpFileInfo = new FileInfo(row);
                Assert.True(tmpFileInfo.Exists);
            }
        }

        /// <summary>
        /// Tests searching for files in a given directory
        /// </summary>
        [Test()]
        public void FileSearch()
        {

            FileSystemInterface fsi = new FileSystemInterface(Application.LocalUserAppDataPath);
            fsi.Search("*.*");
            foreach (var row in fsi.SearchList)
            {
                Assert.IsNotNull(row);
                FileInfo tmpFileInfo = new FileInfo(row);
                Assert.True(tmpFileInfo.Exists);
            }
        }
    }
}