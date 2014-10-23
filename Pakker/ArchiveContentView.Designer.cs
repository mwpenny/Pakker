namespace Pakker
{
    partial class ArchiveContentView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contentViewPane = new System.Windows.Forms.ListView();
            this.nameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pathHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unpackSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unpackAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.icons = new System.Windows.Forms.ImageList(this.components);
            this.pathBox = new System.Windows.Forms.TextBox();
            this.parentButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusSelectedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusSizeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.unpackProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.cancelButton = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentViewPane
            // 
            this.contentViewPane.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentViewPane.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentViewPane.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.pathHeader,
            this.sizeHeader});
            this.contentViewPane.ContextMenuStrip = this.contextMenuStrip;
            this.contentViewPane.LabelEdit = true;
            this.contentViewPane.LargeImageList = this.icons;
            this.contentViewPane.Location = new System.Drawing.Point(0, 29);
            this.contentViewPane.Name = "contentViewPane";
            this.contentViewPane.Size = new System.Drawing.Size(500, 247);
            this.contentViewPane.SmallImageList = this.icons;
            this.contentViewPane.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.contentViewPane.TabIndex = 0;
            this.contentViewPane.UseCompatibleStateImageBehavior = false;
            this.contentViewPane.View = System.Windows.Forms.View.Details;
            this.contentViewPane.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.contentViewPane_AfterLabelEdit);
            this.contentViewPane.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.contentViewPane_ColumnClick);
            this.contentViewPane.ItemActivate += new System.EventHandler(this.contentViewPane_ItemActivate);
            this.contentViewPane.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.contentViewPane_ItemSelectionChanged);
            this.contentViewPane.KeyDown += new System.Windows.Forms.KeyEventHandler(this.contentViewPane_KeyDown);
            // 
            // nameHeader
            // 
            this.nameHeader.Text = "Name";
            this.nameHeader.Width = 150;
            // 
            // pathHeader
            // 
            this.pathHeader.Text = "Relative path";
            this.pathHeader.Width = 250;
            // 
            // sizeHeader
            // 
            this.sizeHeader.Text = "Size (bytes)";
            this.sizeHeader.Width = 100;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilesMenuItem,
            this.addFolderMenuItem,
            this.unpackSelectedMenuItem,
            this.unpackAllMenuItem,
            this.renameMenuItem,
            this.deleteMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.Size = new System.Drawing.Size(161, 158);
            // 
            // addFilesMenuItem
            // 
            this.addFilesMenuItem.Enabled = false;
            this.addFilesMenuItem.Name = "addFilesMenuItem";
            this.addFilesMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addFilesMenuItem.Text = "Add &file(s) to archive";
            this.addFilesMenuItem.Click += new System.EventHandler(this.addFileMenuItem_Click);
            // 
            // addFolderMenuItem
            // 
            this.addFolderMenuItem.Enabled = false;
            this.addFolderMenuItem.Name = "addFolderMenuItem";
            this.addFolderMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addFolderMenuItem.Text = "Add fo&lder to archive";
            this.addFolderMenuItem.Click += new System.EventHandler(this.addFolderMenuItem_Click);
            // 
            // unpackSelectedMenuItem
            // 
            this.unpackSelectedMenuItem.Enabled = false;
            this.unpackSelectedMenuItem.Name = "unpackSelectedMenuItem";
            this.unpackSelectedMenuItem.Size = new System.Drawing.Size(160, 22);
            this.unpackSelectedMenuItem.Text = "&Unpack selected";
            this.unpackSelectedMenuItem.Click += new System.EventHandler(this.unpackSelectedMenuItem_Click);
            // 
            // unpackAllMenuItem
            // 
            this.unpackAllMenuItem.Enabled = false;
            this.unpackAllMenuItem.Name = "unpackAllMenuItem";
            this.unpackAllMenuItem.Size = new System.Drawing.Size(160, 22);
            this.unpackAllMenuItem.Text = "Unpack &archive";
            this.unpackAllMenuItem.Click += new System.EventHandler(this.unpackAllMenuItem_Click);
            // 
            // renameMenuItem
            // 
            this.renameMenuItem.Enabled = false;
            this.renameMenuItem.Name = "renameMenuItem";
            this.renameMenuItem.Size = new System.Drawing.Size(160, 22);
            this.renameMenuItem.Text = "&Rename";
            this.renameMenuItem.Click += new System.EventHandler(this.renameMenuItem_Click);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Enabled = false;
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(160, 22);
            this.deleteMenuItem.Text = "&Delete";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // icons
            // 
            this.icons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.icons.ImageSize = new System.Drawing.Size(16, 16);
            this.icons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pathBox
            // 
            this.pathBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathBox.Location = new System.Drawing.Point(36, 5);
            this.pathBox.Name = "pathBox";
            this.pathBox.ReadOnly = true;
            this.pathBox.Size = new System.Drawing.Size(461, 20);
            this.pathBox.TabIndex = 2;
            // 
            // parentButton
            // 
            this.parentButton.Enabled = false;
            this.parentButton.Location = new System.Drawing.Point(3, 3);
            this.parentButton.Name = "parentButton";
            this.parentButton.Size = new System.Drawing.Size(27, 23);
            this.parentButton.TabIndex = 1;
            this.parentButton.Text = "..";
            this.parentButton.UseVisualStyleBackColor = true;
            this.parentButton.Click += new System.EventHandler(this.parentButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusSelectedLabel,
            this.statusSizeLabel,
            this.statusLabel,
            this.unpackProgress,
            this.cancelButton});
            this.statusStrip.Location = new System.Drawing.Point(0, 276);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(500, 24);
            this.statusStrip.TabIndex = 3;
            // 
            // statusSelectedLabel
            // 
            this.statusSelectedLabel.Name = "statusSelectedLabel";
            this.statusSelectedLabel.Size = new System.Drawing.Size(99, 19);
            this.statusSelectedLabel.Text = "0 item(s) selected";
            // 
            // statusSizeLabel
            // 
            this.statusSizeLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusSizeLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
            this.statusSizeLabel.Name = "statusSizeLabel";
            this.statusSizeLabel.Size = new System.Drawing.Size(17, 19);
            this.statusSizeLabel.Text = "0";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(244, 19);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Done";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // unpackProgress
            // 
            this.unpackProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.unpackProgress.Name = "unpackProgress";
            this.unpackProgress.Size = new System.Drawing.Size(100, 18);
            // 
            // cancelButton
            // 
            this.cancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cancelButton.Enabled = false;
            this.cancelButton.Image = global::Pakker.Properties.Resources.cancel;
            this.cancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(23, 22);
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All files|*.*";
            this.openFileDialog.Multiselect = true;
            // 
            // ArchiveContentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.parentButton);
            this.Controls.Add(this.pathBox);
            this.Controls.Add(this.contentViewPane);
            this.Name = "ArchiveContentView";
            this.Size = new System.Drawing.Size(500, 300);
            this.contextMenuStrip.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView contentViewPane;
        private System.Windows.Forms.ColumnHeader nameHeader;
        private System.Windows.Forms.ColumnHeader pathHeader;
        private System.Windows.Forms.ColumnHeader sizeHeader;
        private System.Windows.Forms.ImageList icons;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.Button parentButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar unpackProgress;
        private System.Windows.Forms.ToolStripStatusLabel statusSelectedLabel;
        private System.Windows.Forms.ToolStripStatusLabel statusSizeLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem unpackSelectedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unpackAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripButton cancelButton;
        private System.Windows.Forms.ToolStripMenuItem addFilesMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem addFolderMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
