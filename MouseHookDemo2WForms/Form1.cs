using System;
using System.Windows.Forms;

namespace MouseHookDemo2WForms
{
    public partial class Form1 : Form
    {
        //MouseHook mh;
        MouseHookAdapter mh;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            mh = new MouseHookAdapter();
            mh.MouseClickEvent += () => richTextBox1.AppendText($"clicked\n");

            //mh = new MouseHook();
            //mh.SetHook();
            //mh.MouseClickEvent += mh_MouseClickEvent;
            //mh.MouseDownEvent += mh_MouseDownEvent;

            //mh.MouseMoveEvent += mh_MouseMoveEvent;            
            //mh.MouseUpEvent += mh_MouseUpEvent;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mh.UnHook();
        }
        //private void mh_MouseClickEvent(object sender, MouseEventArgs e)
        //{
        //    //MessageBox.Show(e.X + "-" + e.Y);
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        string sText = "(" + e.X.ToString() + "," + e.Y.ToString() + ")";
        //        richTextBox1.AppendText($"clicked {sText}\n");
        //    }
        //}
        //private void mh_MouseDownEvent(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        //richTextBox1.AppendText($"clicked {e.X},{e.Y}\n");
        //    }
        //}
    }
}
