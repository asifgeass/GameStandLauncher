namespace TaskbarHide
{
	partial class frmMain
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.cmdHide = new System.Windows.Forms.Button();
            this.cmdShow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdHide
            // 
            this.cmdHide.Location = new System.Drawing.Point(12, 12);
            this.cmdHide.Name = "cmdHide";
            this.cmdHide.Size = new System.Drawing.Size(75, 23);
            this.cmdHide.TabIndex = 0;
            this.cmdHide.Text = "Hide";
            this.cmdHide.UseVisualStyleBackColor = true;
            this.cmdHide.Click += new System.EventHandler(this.cmdHide_Click);
            // 
            // cmdShow
            // 
            this.cmdShow.Location = new System.Drawing.Point(93, 12);
            this.cmdShow.Name = "cmdShow";
            this.cmdShow.Size = new System.Drawing.Size(75, 23);
            this.cmdShow.TabIndex = 1;
            this.cmdShow.Text = "Show";
            this.cmdShow.UseVisualStyleBackColor = true;
            this.cmdShow.Click += new System.EventHandler(this.cmdShow_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 49);
            this.Controls.Add(this.cmdShow);
            this.Controls.Add(this.cmdHide);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMain";
            this.Text = "TaskbarHide";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cmdHide;
		private System.Windows.Forms.Button cmdShow;
	}
}

