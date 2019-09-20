using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Logic
{
    public static class ExtensionsOld
    {
        private static readonly string n = Environment.NewLine;

        public static ImageSource ToImageSource(this Icon icon)
        {
            if(icon==null)
            { return null; }
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public static ImageSource ToImageSource(this Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public static void RunParallel(this Task task) { }
        public static string trySubstring(this String inc, int startIndex, int length)
        {
            try
            {
                return inc.Substring(startIndex, length);
            }
            catch
            { return inc; }
        }
        public static string trySubstring(this String inc, int length)
        {
            return trySubstring(inc, 0, length);
        }
        public static string space(this String inc, int length = 45)
        {
            //length=45 for textbox
            string result = inc;
            Regex regex = new Regex($@"[^\s]{{{length},}}");
            while (regex.IsMatch(result))
            {
                Match matchReg = regex.Match(result);
                int sizeFound = matchReg.Length;
                var index = matchReg.Index;
                result = result.Insert(index + length - 1, " ");
            }
            return result;
        }
        public static string prefix(this String mainMsg, string prefixMsg, int countSeparators = 1)
        {
            string separator = "";
            for (int i = 0; i < countSeparators; i++)
            {
                separator += n;
            }
            if (separator == "")
                separator = " ";
            return (string.IsNullOrEmpty(prefixMsg)) ? mainMsg : prefixMsg + separator + mainMsg;
        }
    }
}
