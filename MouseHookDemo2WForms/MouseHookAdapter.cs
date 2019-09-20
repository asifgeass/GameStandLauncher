using System;
using System.Windows.Forms;

namespace MouseHookDemo2WForms
{
    public class MouseHookAdapter
    {
        private MouseHook mh;
        public event Action MouseClickEvent;

        public MouseHookAdapter()
        {
            mh = new MouseHook();
            mh.SetHook();
            mh.MouseClickEvent += mh_MouseClickEvent;
            mh.MouseDownEvent += mh_MouseDownEvent;
        }

        public void UnHook()
        {
            mh.UnHook();
        }
        private void mh_MouseClickEvent(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(e.X + "-" + e.Y);
            if (e.Button == MouseButtons.Left)
            {
                string sText = "(" + e.X.ToString() + "," + e.Y.ToString() + ")";
                //richTextBox1.AppendText($"clicked {sText}\n");
            }
        }
        private void mh_MouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X < 50 & e.Y < 50)
                {
                    MouseClickEvent();
                }
            }
        }
    }
}
