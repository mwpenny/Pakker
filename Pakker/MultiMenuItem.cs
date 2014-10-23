/* MultiMenuItem.cs
 * 
 * One of the limitations of the .NET library is that a
 * ToolStripMenuItem can only belong to one menu.
 * This class is a very simple workaround. Instances of
 * it have a ToolStripMenuItem associated with them.
 * 
 * This class overrides a couple of basic properties so that
 * changes made to instances of it are reflected in the original
 * ToolStripMenuItem, and vice versa.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pakker
{
    /// <summary>
    /// A <see cref="ToolStripMenuItem"/> that maintains some of the same
    /// data as another one.
    /// </summary>
    public class MultiMenuItem : ToolStripMenuItem
    {
        private ToolStripMenuItem original; //The ToolStripMenuItem to mimic

        /// <param name="original">The ToolStripMenuItem to mimic</param>
        public MultiMenuItem(ToolStripMenuItem original)
        {
            this.original = original;
        }

        public override bool Enabled
        {
            get { return original.Enabled; }
            set { original.Enabled = value; }
        }

        public override string Text
        {
            get { return original.Text; }
            set { original.Text = value; }
        }

        protected override void OnClick(EventArgs e) { original.PerformClick(); }
    }
}
