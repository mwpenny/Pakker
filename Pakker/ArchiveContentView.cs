//ArchiveContentView.cs

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace Pakker
{
    /// <summary>
    /// A control to view and edit the contents of a packed <see cref="Archive"/>.
    /// </summary>
    public partial class ArchiveContentView : UserControl, IPakInteractor
    {
        public enum EntryType { FILE, DIR }; //Used for image indicies in the view

        private ListViewColumnSorter lvc = new ListViewColumnSorter(0);
        private Archive archive; //The loaded archive
        private ArchiveDirectory cd; //The current directory
        private PakWorker worker;
        private IPakInteractor parent;

        public ToolStripItemCollection getContextMenuItems() { return contextMenuStrip.Items; }
        public Archive getArchive() { return archive; }
        public PakWorker getWorker() { return worker; }
        public ToolStripProgressBar getProgressBar() { return unpackProgress; }

        public ArchiveContentView(IPakInteractor parent)
        {
            InitializeComponent();
            this.parent = parent;
            worker = new PakWorker(unpackProgress, doneIO);

            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            folderBrowserDialog.SelectedPath = Environment.CurrentDirectory;
            contentViewPane.ListViewItemSorter = lvc;
            icons.Images.Add(Properties.Resources.file);
            icons.Images.Add(Properties.Resources.dir);
            update();
        }

        /// <summary>
        /// Loads an archive into the view.
        /// </summary>
        /// <param name="archive">The archive to load.</param>
        public void changeArchive(Archive archive)
        {
            //Reset everything
            this.archive = archive;
            this.cd = null;
            contentViewPane.Items.Clear();
            pathBox.Text = "";

            if (archive != null)
                changeDir(archive.getRoot());
        }

        /// <summary>
        /// Updates the control when an IO operation completes.
        /// </summary>
        /// <param name="success">Whether or not the IO operation was successful.</param>
        public void doneIO(bool success)
        {
            statusLabel.Text = "Done";

            //Unload the archive if there was a problem
            if (!success)
                changeArchive(null);

            changeDir(cd);
            update();
        }
        
        /// <summary>
        /// Updates the view.
        /// </summary>
        public void update()
        {
            //Update status strip stuff
            string text = contentViewPane.SelectedItems.Count + " item(s) selected";
            statusSelectedLabel.Text = text.PadRight(35);

            //Update file size label
            int selectedSize = 0;
            foreach (ListViewItem item in contentViewPane.SelectedItems)
                if (item.ImageIndex == (int)EntryType.FILE && cd.getFiles().ContainsKey(item.Text))
                    selectedSize += cd.getFiles()[item.Text].getSize();
            statusSizeLabel.Text = sizeFormat(selectedSize);

            //Enable/disable menu items and buttons
            addFilesMenuItem.Enabled = archive != null && !PakWorker.isIOLocked();
            addFolderMenuItem.Enabled = archive != null && !PakWorker.isIOLocked();
            unpackSelectedMenuItem.Enabled = contentViewPane.SelectedItems.Count > 0 && !PakWorker.isIOLocked();
            unpackAllMenuItem.Enabled = archive != null && !PakWorker.isIOLocked();
            renameMenuItem.Enabled = contentViewPane.SelectedItems.Count > 0 && !PakWorker.isIOLocked();
            deleteMenuItem.Enabled = contentViewPane.SelectedItems.Count > 0 && !PakWorker.isIOLocked();
            cancelButton.Enabled = PakWorker.isIOLocked();
            parent.update(); //Update parent
            Application.DoEvents();
        }

        private void changeDir(ArchiveDirectory root)
        {
            cd = root;
            pathBox.Text = "[" + archive.getName() + "]\\" + cd.getPath();
            parentButton.Enabled = !String.IsNullOrEmpty(cd.getPath());

            //Add file and directory entries
            contentViewPane.Items.Clear();
            foreach (ArchiveDirectory d in root.getDirs().Values)
                addEntry(d.getName(), d.getPath(), d.getSize(), 1);
            foreach (ArchiveFile f in root.getFiles().Values)
                addEntry(f.getName(), f.getPath(), f.getSize(), 0);

            update();
            contentViewPane.Select();
        }

        private void unpackSelectedAsync()
        {
            //Add all subfiles of this directory to the list of files to write
            List<ArchiveFile> files = new List<ArchiveFile>();
            foreach (ListViewItem item in contentViewPane.SelectedItems)
            {
                //Add each file in this dir
                if (item.ImageIndex == (int)EntryType.FILE)
                    files.Add(cd.getFiles()[item.Text]);
                //Recursively get files in this dir
                else if (item.ImageIndex == (int)EntryType.DIR)
                    files.AddRange(cd.getDirs()[item.Text].subFiles());
            }

            worker.runArgMethodAsync(archive.writeFiles, files);
            setStatus("Unpacking items");
        }

        private void addFiles()
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> rootNPaths = new List<string>();
                rootNPaths.Add(Path.GetDirectoryName(openFileDialog.FileNames[0])); //Root dir
                rootNPaths.Add(cd.getPath()); //Rel path
                rootNPaths.AddRange(openFileDialog.FileNames); //Files to write
                worker.runArgMethodAsync(archive.addFiles, rootNPaths); //Add file(s) to the archive
                setStatus("Adding file(s)");
            }
        }

        private void addFolder()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> rootNPaths = new List<string>();
                rootNPaths.Add(new DirectoryInfo(folderBrowserDialog.SelectedPath).Parent.FullName); //Root dir
                rootNPaths.Add(cd.getPath()); //Rel dir
                rootNPaths.AddRange(Directory.GetFiles(folderBrowserDialog.SelectedPath, "*", SearchOption.AllDirectories));
                worker.runArgMethodAsync(archive.addFiles, rootNPaths); //Add files to the archive
                setStatus("Adding folder");
            }
        }

        private void unpackAllAsync()
        {
            worker.runMethodAsync(archive.writeAllFiles);
            setStatus("Unpacking archive");
        }

        private void packAllAsync()
        {
            worker.runMethodAsync(archive.updatePak);
            setStatus("Updating archive");
        }

        private void addEntry(string name, string path, int size, int imageIndex)
        {
            ListViewItem entry = new ListViewItem(name);
            entry.SubItems.Add(path);
            entry.SubItems.Add(size.ToString());
            entry.ImageIndex = imageIndex;
            contentViewPane.Items.Add(entry);
        }

        private void removeEntry(ListViewItem entry)
        {
            if (entry.ImageIndex == (int)EntryType.DIR)
                cd.getDirs().Remove(entry.Text);
            else if (entry.ImageIndex == (int)EntryType.FILE)
                cd.getFiles().Remove(entry.Text);
            entry.Remove();
        }

        private void deleteSelected()
        {
            if (MessageBox.Show("Remove selected item(s) from archive?", "Delete confirmation", //Confirm remove
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (ListViewItem item in contentViewPane.SelectedItems)
                    removeEntry(item);
                packAllAsync(); //Update archive
            }
        }

        private void contentViewPane_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //Switch sort order if the sort column is clicked
            if (e.Column == lvc.sortCol)
                lvc.ascending = !lvc.ascending;
            //Use the clicked column to sort
            else
            {
                lvc.sortCol = e.Column;
                lvc.ascending = true;
            }

            ((ListView)sender).Sort();
        }

        private void contentViewPane_ItemActivate(object sender, EventArgs e)
        {
            ListView view = (ListView)sender;

            if (view.SelectedItems.Count > 0)
            {
                ListViewItem selected = view.SelectedItems[0];

                //Change into a directory when one is activated
                if (selected.ImageIndex == (int)EntryType.DIR)
                    changeDir(cd.getDirs()[selected.Text]);
            }
        }

        private void contentViewPane_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                ListViewItem item = ((ListView)sender).SelectedItems[0];
                
                //Rename file
                if (item.ImageIndex == (int)EntryType.FILE)
                {
                    //Update file in archive
                    cd.getFiles()[item.Text].rename(e.Label);
                    cd.addFile(cd.getFiles()[item.Text]);
                    cd.getFiles().Remove(item.Text);
                }

                //Rename directory
                else if (item.ImageIndex == (int)EntryType.DIR)
                {
                    //Update directory in archive
                    cd.getDirs()[item.Text].rename(e.Label);
                    cd.addDir(cd.getDirs()[item.Text]);
                    cd.getDirs().Remove(item.Text);
                }
                packAllAsync(); //Update archive
            }
        }

        private void contentViewPane_KeyDown(object sender, KeyEventArgs e)
        {
            //Go up a directory when backspace is pressed
            if (e.KeyCode == Keys.Back)
            {
                if (archive != null && cd != null && cd.getParent() != null)
                    changeDir(cd.getParent());
            }

            //Edit selected items when F2 is pressed
            else if (e.KeyCode == Keys.F2)
                renameMenuItem.PerformClick();

            //Prompt to delete when delete is pressed
            else if (e.KeyCode == Keys.Delete)
            {
                ListView view = ((ListView)sender);
                if (view.SelectedItems.Count > 0)
                    deleteSelected();
            }
        }

        //Simple event handlers
        public void setStatus(string status) { statusLabel.Text = status; update(); }
        private void parentButton_Click(object sender, EventArgs e) { changeDir(cd.getParent()); }
        private void contentViewPane_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) { update(); }
        private void unpackSelectedMenuItem_Click(object sender, EventArgs e) { unpackSelectedAsync(); }
        private void unpackAllMenuItem_Click(object sender, EventArgs e) { unpackAllAsync(); }
        private void deleteMenuItem_Click(object sender, EventArgs e) { deleteSelected(); }
        private void cancelButton_Click(object sender, EventArgs e) { setStatus("Cancelling"); worker.cancel(); parent.getWorker().cancel(); }
        private void addFileMenuItem_Click(object sender, EventArgs e) { addFiles(); }
        private void addFolderMenuItem_Click(object sender, EventArgs e) { addFolder(); }
        private void renameMenuItem_Click(object sender, EventArgs e)
        {
            if (contentViewPane.SelectedItems.Count > 0)
                contentViewPane.SelectedItems[0].BeginEdit();
        }

        //Helper method
        private string sizeFormat(int byteCount)
        {
            //Returns formatted file size
            if (byteCount > 0)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double power = Math.Floor(Math.Log(byteCount) / Math.Log(1024));
                double size = byteCount / (Math.Pow(1024.0, power));
                return Math.Round(size, 2) + sizes[(int)power];
            }
            return "0B";
        }
    }
}
