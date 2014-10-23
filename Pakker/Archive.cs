/* Archive.cs
 * 
 * .PAK archive structure (or at least this implementation of it).
 * Originally made to extract assets from the game FEZ.
 * 
 * Header:
 * Int32 (4 bytes) representing the number of files
 * 
 * For each file:
 *     length-prefixed string representing the relative path to the file
 *     Int32 (4 bytes) representing file size in bytes
 *     [filesize] bytes representing the file's data
 *     
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace Pakker
{
    /// <summary>
    /// A packed .pak archive.
    /// </summary>
    public class Archive : ArchiveItem
    {
        //Whether or not to use file extensions when adding to archives
        public static bool usingExtensions = true;
        private string unpackPath;
        private ArchiveDirectory root;
        public ArchiveDirectory getRoot() { return root; }

        /// <param name="path">The path to the <see cref="Archive"/>'s .pak file (including file name).</param>
        /// <param name="unpackPath">The directory to unpack the contents of the <see cref="Archive"/> to.</param>
        public Archive(string path, string unpackPath = "unpacked")
            : base(Path.GetFileNameWithoutExtension(path), path, 0)
        {
            this.unpackPath = unpackPath;
            this.root = new ArchiveDirectory("", "", null);
        }

        /// <summary>
        /// Loads the <see cref="Archive"/>'s metadata from its .pak file.
        /// </summary>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        public void load(PakWorker.CancelCallback cancelling=null, PakWorker.ProgressCallback progress=null)
        {
            if (cancelling != null && cancelling()) //Check for cancel
                return;

            ArchiveFile[] files = loadPak(cancelling, progress); //Get file list from packed file
            updateSize();
            this.root = this.root ?? new ArchiveDirectory("", "", null); //Update root if not null
            getDirTree(files, ref this.root);
        }

        /// <summary>
        /// Writes a list of files from the <see cref="Archive"/>'s .pak file to disk.
        /// </summary>
        /// <param name="files">The list of <see cref="ArchiveFile"/>s to write.</param>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        public void writeFiles(object files, PakWorker.CancelCallback cancelling=null, PakWorker.ProgressCallback progress=null)
        {
            List<ArchiveFile> fs = (List<ArchiveFile>)files;

            using (FileStream f = File.OpenRead(this.path))
            {
                using (BinaryReader pak = new BinaryReader(f))
                {
                    for (int i = 0; i < fs.Count; i++)
                    {
                        //Move to file location in packed file and write the bytes to their own file
                        f.Seek(fs[i].getStartByte(), SeekOrigin.Begin);
                        writeBytes(fs[i].getPath(), pak.ReadBytes(fs[i].getSize()));

                        if (progress != null) //Report progress
                            progress(PakWorker.calcPercent(i, fs.Count));

                        if (cancelling != null && cancelling()) //Check for cancel
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Writes every file in the <see cref="Archive"/>'s .pak file to disk.
        /// </summary>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        public void writeAllFiles(PakWorker.CancelCallback cancelling=null, PakWorker.ProgressCallback progress=null)
        {
            using (FileStream f = File.OpenRead(this.path))
            {
                using (BinaryReader pak = new BinaryReader(f))
                {
                    int filecount = pak.ReadInt32();
                    for (int i = 0; i < filecount; i++)
                    {
                        //Read file metadata from the packed file
                        string filename = pak.ReadString();
                        int filesize = pak.ReadInt32();
                        writeBytes(filename, pak.ReadBytes(filesize));

                        if (progress != null) //Report progress
                            progress(PakWorker.calcPercent(i, filecount));

                        if (cancelling != null && cancelling()) //Check for cancel
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a list of files to this <see cref="Archive"/>.
        /// </summary>
        /// <param name="rootNpaths">A list of file paths to write, with the first string being the relative path in the <see cref="Archive"/>.</param>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        public void addFiles(object rootNpaths, PakWorker.CancelCallback cancelling = null, PakWorker.ProgressCallback progress = null)
        {
            //PakWorkerArgWork only allows one argument, so take the root and rel paths strings out of rootNPaths
            List<string> paths = (List<string>)rootNpaths;
            string rootpath = paths[0];
            string relpath = paths[1];
            paths.RemoveRange(0, 2);

            using (FileStream f = File.Open(this.path, FileMode.Open))
            {
                using (BinaryWriter pak = new BinaryWriter(f))
                {
                    pak.Write(this.root.subFiles().Count + paths.Count); //Write file count
                    f.Seek(0, SeekOrigin.End); //Go to end of file
                    writeToPak(paths.ToArray(), rootpath, relpath, pak, cancelling, progress); //Write files
                }
            }

            load(cancelling, progress);
        }

        /// <summary>
        /// Creates a .pak file and loads its metadata.
        /// </summary>
        /// <param name="root">The root directory of the files.</param>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        public void createPak(object root, PakWorker.CancelCallback cancelling=null, PakWorker.ProgressCallback progress=null)
        {
            string rootpath = (string)root;
            string[] paths = Directory.GetFiles(rootpath, "*", SearchOption.AllDirectories);

            using (FileStream f = File.OpenWrite(this.path))
            {
                using (BinaryWriter pak = new BinaryWriter(f))
                {
                    pak.Write(paths.Length); //Write file count
                    writeToPak(paths, rootpath, "", pak, cancelling, progress); //Write files
                }
            }
            load(cancelling, progress); //Load newly created pak
        }

        /// <summary>
        /// Re-saves this <see cref="Archive"/>'s files, changing names if they were changed,
        /// and omitting deleted files.
        /// </summary>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        public void updatePak(PakWorker.CancelCallback cancelling=null, PakWorker.ProgressCallback progress=null)
        {
            //Rename the archive using a GUID
            string oldFilePath = Path.Combine(Path.GetDirectoryName(this.path), Guid.NewGuid().ToString());
            File.Move(this.path, oldFilePath);

            using (FileStream newfile = File.OpenWrite(this.path))
            {
                using (BinaryWriter destPak = new BinaryWriter(newfile))
                {
                    List<ArchiveFile> files = this.root.subFiles();
                    destPak.Write(files.Count);
                    using (FileStream oldfile = File.OpenRead(oldFilePath))
                    {
                        using (BinaryReader srcPak = new BinaryReader(oldfile))
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                //Move to location of data bytes in source and write to the dest file
                                oldfile.Seek(files[i].getStartByte(), SeekOrigin.Begin);
                                destPak.Write(files[i].getPath());
                                destPak.Write(files[i].getSize());
                                destPak.Write(srcPak.ReadBytes(files[i].getSize()));

                                if (progress != null) //Report progress
                                    progress(PakWorker.calcPercent(i, files.Count));

                                if (cancelling != null && cancelling()) //Check for cancel
                                    break;
                            }
                        }
                    }
                }
            }

            if (cancelling == null || !cancelling())
                File.Delete(oldFilePath); //The old file is no longer needed
            else
            {
                File.Delete(this.path); //Restore old file
                File.Move(oldFilePath, this.path);
            }
            updateSize();
        }

        /// <summary>
        /// Writes a list of files to a .pak file
        /// </summary>
        /// <remarks>
        /// Assumes <paramref name="rootpath"/> is actually a part of each path in <paramref name="paths"/>.
        /// </remarks>
        /// <param name="paths">The paths to the files to write to the .pak file.</param>
        /// <param name="rootpath">The root directory of the files.</param>
        /// <param name="relpath">The path in the pak file to store the files.</param>
        /// <param name="pak">The <see cref="BinaryWriter"/> used to write the file.</param>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        private void writeToPak(string[] paths, string rootpath, string relpath, BinaryWriter pak,
                PakWorker.CancelCallback cancelling = null, PakWorker.ProgressCallback progress = null)
        {
            for (int i = 0; i < paths.Length; i++) //Write info for each file 
            {
                //Don't use \ if top-level root dir
                string sep = !String.IsNullOrEmpty(relpath) ? "\\" : "";
                string filePath = relpath + sep + paths[i].Substring(rootpath.Length + 1);
                
                if (!Archive.usingExtensions)
                    filePath = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath);

                //Write file to pak file
                byte[] data = File.ReadAllBytes(paths[i]);
                pak.Write(filePath);
                pak.Write(data.Length);
                pak.Write(data);

                if (progress != null) //Report progress
                    progress(PakWorker.calcPercent(i, paths.Length));

                if (cancelling != null && cancelling()) //Check for cancel
                    i = paths.Length;

                data = null;
                GC.Collect();
            }
        }

        /// <summary>
        /// Gets all <see cref="ArchiveFile"/>s contained in this <see cref="Archive"/>.
        /// </summary>
        /// <param name="cancelling">The <see cref="CancellCallback"/> to check for a cancel message from.</param>
        /// <param name="progress">The <see cref="ProgressCallback"/> to report back to.</param>
        /// <returns>An array of all <see cref="ArchiveFile"/>s in this <see cref="Archive"/>.</returns>
        private ArchiveFile[] loadPak(PakWorker.CancelCallback cancelling=null, PakWorker.ProgressCallback progress=null)
        {
            ArchiveFile[] files;

            using (FileStream f = File.OpenRead(this.path))
            {
                using (BinaryReader pak = new BinaryReader(f))
                {
                    files = new ArchiveFile[pak.ReadInt32()]; //Get file count from packed file
                    for (int i = 0; i < files.Length; i++)
                    {
                        string fPath = pak.ReadString();
                        string fName = Path.GetFileName(fPath);
                        int bCount = pak.ReadInt32();
                        files[i] = new ArchiveFile(fName, fPath, bCount, f.Position); //Add file

                        //Skip over the data bytes since we're just reading metadata
                        f.Seek(bCount, SeekOrigin.Current);

                        if (progress != null) //Report progress
                            progress(PakWorker.calcPercent(i, files.Length));

                        if (cancelling != null && cancelling()) //Check for cancel
                            return null;
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// Forms the file/directory hierarchy of this <see cref="Archive"/>, with subfiles and subdirectories
        /// in their proper places.
        /// </summary>
        /// <param name="files">The <see cref="ArchiveFile"/>s to generate the tree of.</param>
        /// <param name="root">The <see cref="ArchiveDirectory"/> to use as the root node.</param>
        private void getDirTree(ArchiveFile[] files, ref ArchiveDirectory root)
        {
            foreach (ArchiveFile f in files)
            {
                ArchiveDirectory cd = root;

                //If the file is in a subdirectory
                if (f.getPath().Contains("\\"))
                {
                    string[] folders = f.getPath().Split('\\');                    

                    //Move deeper in the path, one directory at a time
                    for (int i = 0; i < folders.Length; i++)
                    {
                        if (i < folders.Length-1)
                        {
                            ArchiveDirectory d;

                            //Work through the path adding new directories to the parent as necessary
                            if (!cd.getDirs().ContainsKey(folders[i]))
                            {
                                string sep = !String.IsNullOrEmpty(cd.getPath()) ? "\\" : ""; //Don't add \ for top-level dirs
                                d = new ArchiveDirectory(folders[i], cd.getPath() + sep + folders[i], cd);
                                cd.getDirs().Add(folders[i], d);
                            }
                            else
                                d = cd.getDirs()[folders[i]];
                            cd = d; //This is now the current directory
                        }

                        //Add new files when on last directory in the path
                        else if (i == folders.Length-1 && !cd.getFiles().ContainsKey(f.getName()))
                            cd.addFile(f);
                    }
                }
                else if (!root.getFiles().ContainsKey(f.getName())) //The file is in this directory
                    root.addFile(f);
            }
        }

        //Helper method
        private void writeBytes(string path, byte[] data)
        {
            //Create missing directories as necessary and write the file
            string writePath = Path.Combine(this.unpackPath, this.name);
            writePath = Path.Combine(writePath, path);

            string dir = Path.GetDirectoryName(writePath);
            if (dir.Length > 0 && !Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            //Add file extension if none present (prevents file/directory name collision)
            string ext = String.IsNullOrEmpty(Path.GetExtension(path)) ? ".unpacked" : "";
            File.WriteAllBytes(writePath + ext, data);
        }

        private void updateSize()
        {
            this.size = (int)(new FileInfo(path).Length);
        }
    }
}
