/*
 * Copyright (C) 2105 by Mike Bos
 *
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation;
 * either version 3 of the License, or any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE. See the GNU General Public License for more details.
 *
 * A copy of the license is obtainable at http://www.gnu.org/licenses/gpl-3.0.en.html#content
*/



namespace Pass4Win
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;

    /// <summary>
    /// Class to interface with the file system ie read directories and file operations
    /// </summary>
    public class FileSystemInterface
    {
        /// <summary>
        /// Holder for the Password Store location
        /// </summary>
        private readonly string passWordStore;

        /// <summary>
        /// Gets the Search list
        /// </summary>
        public List<string> SearchList { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemInterface"/> class.
        /// </summary>
        /// <param name="passWordStore">
        /// The password store location
        /// </param>
        public FileSystemInterface(string passWordStore)
        {
            this.passWordStore = passWordStore;
            this.SearchList = new List<string> { "No", "Value" };

            //// Setting up the path to the password store
            DirectoryInfo storePath = new DirectoryInfo(passWordStore);

            //// Filling the Directory and search datatable
            this.UpdateDirectoryList(storePath);
        }

        /// <summary>
        /// Searchs through all the entries
        /// </summary>
        /// <param name="searchtext">
        /// The searchtext.
        /// </param>
        public void Search(string searchtext)
        {
            this.SearchList.Clear();
            foreach (FileInfo tmpFileInfo in new DirectoryInfo(this.passWordStore).GetFiles("*.gpg", SearchOption.AllDirectories))
            {
                if (Operators.LikeString(tmpFileInfo.Name, searchtext, CompareMethod.Text))
                {
                    this.SearchList.Add(tmpFileInfo.FullName);
                }
            }
        }

        /// <summary>
        /// Rebuilds the directory tree
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode"/>.
        /// </returns>
        public TreeNode[] UpdateDirectoryTree(DirectoryInfo path)
        {
            List<TreeNode> nodeList = new List<TreeNode>();

            var nodeName = new StringBuilder();
            nodeName.Append(path.Name);
            nodeName.AppendFormat(" ({0})", path.EnumerateFiles().Count());
            var node = new TreeNode(nodeName.ToString()) { Tag = path.FullName };

            foreach (var directory in path.GetDirectories())
            {
                if (!directory.Name.StartsWith("."))
                {
                    var childNodes = this.UpdateDirectoryTree(directory);
                    node.Nodes.AddRange(childNodes);
                }
            }

            nodeList.Add(node);
            TreeNode[] nodeArr = nodeList.ToArray();

            return nodeArr;
        }

        /// <summary>
        /// Get's all the relevant files from the given directory
        /// </summary>
        /// <param name="directoryInfo">
        /// The directory info structure of the location
        /// </param>
        /// <returns>
        /// Returns a list with text, fullname, path
        /// </returns>
        public List<string> UpdateDirectoryList(DirectoryInfo directoryInfo)
        {
            return (from ffile in directoryInfo.GetFiles() where !ffile.Name.StartsWith(".") where ffile.Extension.ToLower() == ".gpg" select Path.GetFileNameWithoutExtension(ffile.Name)).ToList();
        }
    }
}
