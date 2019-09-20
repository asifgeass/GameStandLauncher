using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logic.MouseHookFolder;

namespace Logic
{
    public class MouseHookAdapter
    {
        private MouseHook mh;
        public event Action MouseClickEvent = delegate { };

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
