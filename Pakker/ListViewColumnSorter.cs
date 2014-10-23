//ListViewColumnSorter.cs

using System;
using System.Collections;
using System.Windows.Forms;

namespace Pakker
{

    /// <summary>
    /// Used to sort entries in a ListView using columns.
    /// Specifically, files and directories.
    /// </summary>
    /// <remarks>
    /// The ImageIndex property of ListViewItems is used
    /// to determine whether or not they are files or
    /// directories.
    /// </remarks>
    public class ListViewColumnSorter : IComparer
    {
        public int sortCol { get; set; }
        public bool ascending { get; set; }

        /// <param name="sortCol">The index of the column whose entries will be used for sorting.</param>
        /// <param name="ascending">Whether or not the sorted items should be ascending.</param>
        public ListViewColumnSorter(int sortCol, bool ascending=true)
        {
            this.sortCol = sortCol;
            this.ascending = ascending;
        }

        /// <summary>
        /// Compares 2 <see cref="ListViewItem"/> objects in column <see cref="sortCol"/>.
        /// </summary>
        /// <remarks>
        /// Keeps directories together, and the items must have subitems in column <see cref="sortCol"/>
        /// </remarks>
        /// <param name="o1">The first <see cref="ListViewItem"/>.</param>
        /// <param name="o2">The second <see cref="ListViewItem"/>.</param>
        /// <returns></returns>
        public int Compare(object o1, object o2)
        {
            ListViewItem i1 = (ListViewItem)o1;
            ListViewItem i2 = (ListViewItem)o2;
            
            //Directories before files
            if (i1.ImageIndex == (int)ArchiveContentView.EntryType.DIR &&
                    i2.ImageIndex == (int)ArchiveContentView.EntryType.FILE)
                return -1;

            string t1 = i1.SubItems[sortCol].Text;
            string t2 = i2.SubItems[sortCol].Text;

            //Toggle sort order depending on whether ascending/descending
            int multiplier = this.ascending ? 1 : -1;

            //If the fields to be compared are integers, compare them appropriately
            int s1, s2;
            if (Int32.TryParse(t1, out s1) && Int32.TryParse(t2, out s2))
                return s1.CompareTo(s2) * multiplier;
            return t1.CompareTo(t2) * multiplier;
        }        
    }
}
