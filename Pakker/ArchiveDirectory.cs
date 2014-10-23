//ArchiveDirectory.cs

using System;
using System.Collections.Generic;

namespace Pakker
{
    /// <summary>
    /// Represents a folder inside of a packed archive.
    /// </summary>
    /// <remarks>
    /// Contains subfiles and subdirectories.
    /// </remarks>
    public class ArchiveDirectory : ArchiveItem
    {
        private ArchiveDirectory parent;
        private Dictionary<string, ArchiveDirectory> dirs;
        private Dictionary<string, ArchiveFile> files;
        
        public ArchiveDirectory getParent() { return parent; }
        public Dictionary<string, ArchiveDirectory> getDirs() { return dirs; }
        public Dictionary<string, ArchiveFile> getFiles() { return files; }
        public void addDir(ArchiveDirectory d)
        {
            if (!dirs.ContainsKey(d.name))
                dirs.Add(d.name, d);
        }
        public void addFile(ArchiveFile f)
        {
            if (!files.ContainsKey(f.getName()))
                files.Add(f.getName(), f);
        }

        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        public ArchiveDirectory(string name, string path, ArchiveDirectory parent)
            : base(name, path, 0)
        {
            this.parent = parent;
            this.dirs = new Dictionary<string, ArchiveDirectory>();
            this.files = new Dictionary<string, ArchiveFile>();
        }

        /// <summary>
        /// Renames this directory, all subdirectories, and all subfiles.
        /// </summary>
        /// <param name="name">The new name for this directory.</param>
        public void rename(string name)
        {
            this.name = name;
            string[] folders = this.path.Split('\\'); //Get path parts (like breadcrumbs)
            folders[folders.Length-1] = name; //Last entry is dir name
            this.path = String.Join("\\", folders, 0, folders.Length-1);
            changeParentPath(path);
        }

        /// <summary>
        /// Changes the parent path of this directory, all subdirectories, and all subfiles.
        /// </summary>
        /// <param name="path">The new parent path of this directory.</param>
        public void changeParentPath(string path)
        {
            string sep = !String.IsNullOrEmpty(path) ? "\\" : ""; //Don't add \ for top-level dirs
            this.path = path + sep + this.name;
            
            //Change paths of contained folders and files
            foreach (ArchiveDirectory d in dirs.Values)
                d.changeParentPath(this.path);
            foreach (ArchiveFile f in files.Values)
                f.changeParentPath(this.path);
        }

        /// <summary>
        /// Returns a list of all files in this directory and its subdirectories.
        /// </summary>
        /// <returns>Subfiles of this directory.</returns>
        public List<ArchiveFile> subFiles()
        {
            List<ArchiveFile> retFiles = new List<ArchiveFile>();
            retFiles.AddRange(files.Values); //Add subfiles

            //Recursively crawl child directories for files
            foreach (ArchiveDirectory d in dirs.Values)
                retFiles.AddRange(d.subFiles());

            return retFiles;
        }

        /// <summary>
        /// Returns a list of all directories in this directory and its subdirectories.
        /// </summary>
        /// <returns>Subdirectories of this directory.</returns>
        public List<ArchiveDirectory> subDirs()
        {
            List<ArchiveDirectory> retDirs = new List<ArchiveDirectory>();
            retDirs.AddRange(dirs.Values); //Add subdirectories

            //Recursively crawl child directories for directories
            foreach (ArchiveDirectory d in dirs.Values)
                retDirs.AddRange(d.subDirs());

            return retDirs;
        }
    }
}
