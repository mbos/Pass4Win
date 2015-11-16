/*
 * Copyright (C) 2015 by Mike Bos
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
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using Autofac;
    using SharpConfig;

    /// <summary>
    /// Class to interface with the file system ie read directories and file operations
    /// </summary>
    public class FileSystemInterface
    {
        private readonly Config _config;
        private readonly IDirectoryProvider _directoryProvider;

        /// <summary>
        /// Gets the Search list
        /// </summary>
        public List<string> SearchList { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemInterface"/> class.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="directoryProvider"></param>
        public FileSystemInterface(Config config, IDirectoryProvider directoryProvider)
        {
            _config = config;
            _directoryProvider = directoryProvider;

            SearchList = new List<string> { "No", "Value" };

            //// Filling the Directory and search datatable
            this.UpdateDirectoryList(directoryProvider);
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
            foreach (var file in _directoryProvider.GetFiles("*.gpg", SearchOption.AllDirectories))
            {
                if(Regex.IsMatch(file.Name, this.WildcardToRegex(searchtext), RegexOptions.IgnoreCase))
                {
                    this.SearchList.Add(file.FullName);
                }
            }
            if (SearchList.Count == 0)
            {
                this.SearchList.Add("No Value");
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
        public TreeNode[] UpdateDirectoryTree(IDirectoryProvider path = null)
        {
            if (path == null)
            {
                path = _directoryProvider;
            }

            List<TreeNode> nodeList = new List<TreeNode>();

            var nodeName = new StringBuilder();
            nodeName.Append(path.Name);
            nodeName.AppendFormat(" ({0})", path.EnumerateFiles().Count());
            var node = new TreeNode(nodeName.ToString()) { Tag = path.FullName };

            foreach (var directory in path.GetDirectories())
            {
                if (!directory.Name.StartsWith("."))
                {
                    var childNodes = UpdateDirectoryTree(directory);
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
        public List<string> UpdateDirectoryList(IDirectoryProvider directoryInfo)
        {
            List<string> list = new List<string>();
            foreach (var ffile in directoryInfo.GetFiles())
            {
                if (!ffile.Name.StartsWith("."))
                {
                    if (ffile.Extension.ToLower() == ".gpg")
                    {
                        list.Add(Path.GetFileNameWithoutExtension(ffile.Name));
                    }
                }
            }
            if (list.Count == 0)
            {
                return null;
            }
            return list;
        }

        private string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }
    }
}
