/*
 *	Copyright (c) 2008..11 by Simon Baer
 * 
 *  You may use this code for whatever you want.
 */

using System;
using System.Windows.Forms;

namespace TaskbarHide
{
    /// <summary>
    /// Demo application to hide/show the taskbar.
    /// </summary>
	public partial class frmMain : Form
	{
        /// <summary>
        /// Initialize form.
        /// </summary>
		public frmMain()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Hide the taskbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void cmdHide_Click(object sender, EventArgs e)
		{
			Taskbar.Hide();
		}

        /// <summary>
        /// Show the taskbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void cmdShow_Click(object sender, EventArgs e)
		{
			Taskbar.Show();
		}
	}
}
