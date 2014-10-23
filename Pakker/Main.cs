using System;
using System.Windows.Forms;

namespace Pakker
{
    public partial class Main : Form, IPakInteractor
    {
        private Archive pak;
        private PakWorker worker;

        public PakWorker getWorker() { return worker; }

        public Main()
        {
            InitializeComponent();
            
            //Add view's context menu options to the actions dropdown
            foreach (ToolStripMenuItem item in archiveContentView.getContextMenuItems())
                actionsToolStripMenuItem.DropDownItems.Add(new MultiMenuItem(item));

            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            folderBrowserDialog.SelectedPath = Environment.CurrentDirectory;
            worker = new PakWorker(archiveContentView.getProgressBar(), new PakWorker.DoneCallback(doneIO));
        }

        /// <summary>
        /// Updates the form when an IO operation completes.
        /// </summary>
        /// <param name="success">Whether or not the IO operation was successful.</param>
        public void doneIO(bool success)
        {
            if (!success)
                pak = null;

            //Load the archive in the view
            archiveContentView.changeArchive(this.pak);
            archiveContentView.doneIO(success);
            update();
        }

        /// <summary>
        /// Updates the view.
        /// </summary>
        public void update()
        {
            openToolStripMenuItem.Enabled = !PakWorker.isIOLocked();
            createArchiveToolStripMenuItem.Enabled = !PakWorker.isIOLocked();
            extToolStripMenuItem.Enabled = !PakWorker.isIOLocked();
        }

        private void openArchive(Archive pak)
        {
            this.pak = pak;
            worker.runMethodAsync(pak.load);
            archiveContentView.setStatus("Opening archive");
            update();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                openArchive(new Archive(openFileDialog.FileName));
        }

        private void createArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK && saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Creates and opens the new archive
                this.pak = new Archive(saveFileDialog.FileName);
                worker.runArgMethodAsync(this.pak.createPak, folderBrowserDialog.SelectedPath);
                archiveContentView.setStatus("Creating archive");
                update();
            }
        }

        private void extToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Toggle packing without extensions
            ToolStripMenuItem s = ((ToolStripMenuItem)sender);
            s.Checked = !s.Checked;
            Archive.usingExtensions = !Archive.usingExtensions;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Cancel async jobs and get the hell out of here
            archiveContentView.getWorker().cancel();
            worker.cancel();
            Application.Exit();
        }
    }
}
