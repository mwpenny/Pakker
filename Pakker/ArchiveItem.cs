//ArchiveItem.cs

using System;

namespace Pakker
{
    /// <summary>
    /// Represents an item inside of a packed archive.
    /// </summary>
    public abstract class ArchiveItem
    {
        protected string name, path;
        protected int size; //In bytes

        public string getName() { return name; }
        public string getPath() { return path; }
        public int getSize() { return size; }

        /// <param name="name">The name of the item.</param>
        /// <param name="path">The path to the item (including its name).</param>
        /// <param name="size">The size of the item (in bytes).</param>
        public ArchiveItem(string name, string path, int size=0)
        {
            this.name = name;
            this.path = path.Replace('/', '\\'); //Ensure windows-friendly path
            this.size = size;
        }
    }
}
