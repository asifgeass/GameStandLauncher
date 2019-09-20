/*
 *	Copyright (c) 2008..11 by Simon Baer
 * 
 *  You may use this code for whatever you want.
 */

using System;
using System.Windows.Forms;

namespace TaskbarHide
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}
