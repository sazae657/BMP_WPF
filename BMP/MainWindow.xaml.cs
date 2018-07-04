using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMP
{
    enum BIT_SIZE
    {
        Size8 = ((sizeof(byte)) * 8),
        Size16 = (sizeof(UInt16) * 8),
        Size32 = (sizeof(UInt32) * 8),
        Size64 = (sizeof(UInt64) * 8),
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush 元の色;

        public MainWindow() {
            InitializeComponent();
            元の色 = inputBox.Background;
            設定("");
        }

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (inputBox.Text.Length == 0) {
                return;
            }
            if (!Regex.IsMatch(inputBox.Text, @"\A[0-9,]+\Z")) {
                var bc = new BrushConverter();
                inputBox.Background = (Brush)bc.ConvertFrom("#AAFF0000");
                return;
            }
            inputBox.Background = 元の色;
            設定(inputBox.Text.Trim());
        }

        private void ﾀﾞﾎﾞーｸﾘｯｸ(object sender, MouseButtonEventArgs e) {
            var s = sender as TextBox;
            Clipboard.SetData(DataFormats.Text, s.Text.Trim());
        }

        bool 設定(string str) {
            try {
                ulong bytes = 八ﾏｽｸ(str);
                binDisp8.Text = 二進に変換(bytes, BIT_SIZE.Size8);
                settyText8.Text = $"0x{bytes:x2}";

                bytes = 十六ﾏｽｸ(str);
                binDisp16.Text = 二進に変換(bytes, BIT_SIZE.Size16);
                settyText16.Text = $"0x{bytes:x4}";

                bytes = 三十二ﾏｽｸ(str);
                binDisp32.Text = 二進に変換(bytes, BIT_SIZE.Size32);
                settyText32.Text = $"0x{bytes:x8}";

                bytes = 六十四ﾏｽｸ(str);
                binDisp64.Text = 二進に変換(bytes, BIT_SIZE.Size64);
                settyText64.Text = $"0x{bytes:x16}";
            }
            catch (Exception e) {
                return false;
            }
            return true;
        }

        string 二進に変換(ulong x, BIT_SIZE BitSize) {
            ulong bit = 1;
            int i;
            var bmp = new char[(int)BIT_SIZE.Size64];

            int off = 0;
            for (i = 0; i < (int)BitSize; i++) {
                bmp[off++] = ((x & bit) == 0) ? '0' : '1';
                bit <<= 1;
            }

            var ret = "";
            off = 0;
            for (i = (int)BitSize - 1; i >= 0; i--) {
                ret += bmp[i];
                if (0 == (i % 4)) {
                    ret += " ";
                }
            }
            return string.Join("", ret.Reverse());
        }

        byte 八ﾏｽｸ(string arg) =>
            (byte)(六十四ﾏｽｸ(arg) & 0xff);

        UInt16 十六ﾏｽｸ(string arg) =>
            (UInt16)(六十四ﾏｽｸ(arg) & 0xffff);
        

        UInt32 三十二ﾏｽｸ(string arg) =>
            (UInt32)(六十四ﾏｽｸ(arg) & 0xffffffff);


        UInt64 六十四ﾏｽｸ(string arg) 
        {
            int l, u, tok;
            var buffer = arg;
            l = u = tok = 0;
            if (0 == arg.Length) {
                return 0;
            }

            l = u = tok = 0;

            foreach (var w in buffer.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                try {
                    tok = int.Parse(w);
                }
                catch {
                    continue;//throw e;
                }

                if (tok <= 32) {
                    tok -= 1;
                    l |= (1 << (tok & 0xff));
                }
                else {
                    tok -= 33;
                    u |= (1 << (tok & 0xff));
                }
            }
            return (UInt64)((uint)l | ((UInt64)u) << 32);
        }
    }
}
