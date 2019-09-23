using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchHook;

namespace Logic
{
    //==From Logic Ctrl+X==
    class TouchHook1
    {
        public static void test()
        {
           var hook = new WM_TouchHook(IntPtr.Zero, HookType.WH_CALLWNDPROC);
            hook.InstallHook();
            hook.TouchDown += Hook_TouchDown;            
        }

        private static void Hook_TouchDown(object sender, TouchHook.TouchEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
