//ArchiveFile.cs

using System;

namespace Pakker
{
    /// <summary>
    /// Represents a file inside of a packed archive
    /// </summary>
    public class ArchiveFile : ArchiveItem
    {
        private long startByte; //The start pos of this file's data in its archive
        public long getStartByte() { return startByte; }

        public ArchiveFile(string name, string path, int size, long startByte)
            : base(name, path, size)
        {
            this.startByte = startByte;
        }

        /// <summary>
        /// Renames the <see cref="ArchiveFile"/>.
        /// </summary>
        /// <param name="name">The new name for this file.</param>
        public void rename(string name)
        {
            this.name = name;
            string[] folders = this.path.Split('\\');
            folders[folders.Length - 1] = name; //Last entry is filename            
            this.path = String.Join("\\", folders); //Update with renamed path
        }

        /// <summary>
        /// Changes the <see cref="ArchiveFile"/>'s parent path.
        /// </summary>
        /// <param name="path">The new parent path.</param>
        public void changeParentPath(string path)
        {
            this.path = path + "\\" + this.name;
        }
    }
}
