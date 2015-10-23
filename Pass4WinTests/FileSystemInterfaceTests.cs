

namespace Pass4Win.Tests
{
    using System;
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

        /// <summary>
        /// Tests for case-sensitive searching for files in a given directory
        /// </summary>
        [Test()]
        public void CaseSensitiveFileSearch()
        {
            string fileName = "caseSensitiveFile.gpg";
            string caseSensitiveFile = Application.LocalUserAppDataPath + "\\" + fileName;
            try
            {
                FileInfo fileInfo = new FileInfo(caseSensitiveFile);
                using (File.Create(Path.Combine(caseSensitiveFile)))
                {
                }

                FileSystemInterface fsi = new FileSystemInterface(Application.LocalUserAppDataPath);
                fsi.Search(fileName.ToLower());

                Assert.AreEqual(1, fsi.SearchList.Count);
                foreach (var row in fsi.SearchList)
                {
                    Assert.IsNotNull(row);
                    FileInfo tmpFileInfo = new FileInfo(row);
                    Assert.True(tmpFileInfo.Exists);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                File.Delete(caseSensitiveFile);
            }
        }

        /// <summary>
        /// Tests for wildcard searching for files in a given directory
        /// </summary>
        [Test()]
        public void WildCardFileSearch()
        {
            string fileName = "caseSensitiveFile.gpg";
            string caseSensitiveFile = Application.LocalUserAppDataPath + "\\" + fileName;
            try
            {
                FileInfo fileInfo = new FileInfo(caseSensitiveFile);
                using (File.Create(Path.Combine(caseSensitiveFile)))
                {
                }

                FileSystemInterface fsi = new FileSystemInterface(Application.LocalUserAppDataPath);
                fsi.Search("*.*");

                Assert.AreEqual(1, fsi.SearchList.Count);
                foreach (var row in fsi.SearchList)
                {
                    Assert.IsNotNull(row);
                    FileInfo tmpFileInfo = new FileInfo(row);
                    Assert.True(tmpFileInfo.Exists);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                File.Delete(caseSensitiveFile);
            }
        }
    }
}