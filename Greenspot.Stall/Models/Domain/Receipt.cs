using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public class ReceiptFont
    {
        private ReceiptFont() { }
        public static Font Font16 = new Font("MS Gothic", 16, FontStyle.Bold);
        public static Font Font20 = new Font("MS Gothic", 20, FontStyle.Bold);
        public static Font Font24 = new Font("MS Gothic", 24, FontStyle.Bold);
        public static Font Font28 = new Font("MS Gothic", 28, FontStyle.Bold);
        public static Font Font32 = new Font("MS Gothic", 32, FontStyle.Bold);

        public enum Fonts { FONT16, FONT20, FONT24, FONT28, FONT32 }
        public static int CharsInRow(Font font)
        {
            switch ((int)font.Size)
            {
                case 16: return 33;
                case 20: return 26;
                case 24: return 22;
                case 28: return 19;
                case 32: return 16;
            }

            return 0;
        }

        public static float FontHeight(Font font)
        {
            switch ((int)font.Size)
            {
                case 16: return 24;
                case 20: return 30;
                case 24: return 36;
                case 28: return 42;
                case 32: return 48;
            }

            return 0;
        }
    }
    public class ReceiptSpace
    {
        public ReceiptSpace() { }
        public ReceiptSpace(float top, float right, float bottom, float left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
        public float Top { get; set; } = 0.0f;
        public float Right { get; set; } = 0.0f;
        public float Bottom { get; set; } = 0.0f;
        public float Left { get; set; } = 0.0f;
    }

    public class ReceiptRow
    {
        public static IList<ReceiptRow> CreateRows(string text, Font font)
        {
            var rows = new List<ReceiptRow>();
            var textArr = text.Split('\n');
            var rowLength = ReceiptFont.CharsInRow(font);
            foreach (var str in textArr)
            {
                var s = str;
                while (!string.IsNullOrEmpty(s))
                {
                    rows.Add(new ReceiptRow()
                    {
                        Text = CutContent(ref s, rowLength),
                        Font = font
                    });
                }
            }
            return rows;
        }

        private static string CutContent(ref string content, int rowLen)
        {
            var str = "";
            int len = 0;

            if (GetContentLength(content) <= rowLen)
            {
                str = content;
                content = "";
                return str;
            }

            while (len < rowLen && content.Length > 0)
            {
                var c = content[0];
                len += c < 128 ? 1 : 2;
                if (len > rowLen)
                {
                    break;
                }
                str += c;
                content = content.Remove(0, 1);
            }
            return str;
        }
        private static int GetContentLength(string content)
        {
            int len = 0;
            foreach (var c in content)
            {
                len += c < 128 ? 1 : 2;
            }
            return len;
        }

        public string Text { get; set; }
        public Font Font { get; set; }
        public ReceiptSpace Padding { get; set; } = new ReceiptSpace();
        public float Height
        {
            get
            {
                return ReceiptFont.FontHeight(Font) + Padding.Top + Padding.Bottom;
            }
        }
    }
    public class Receipt
    {
        private const int MAX_WIDTH = 384;

        private IList<ReceiptRow> _rows = new List<ReceiptRow>();
        public void AddRow(string text, Font font)
        {
            var rows = ReceiptRow.CreateRows(text, font);
            foreach (var r in rows)
            {
                _rows.Add(r);
            }
        }

        public Receipt(Order order)
        {
            //logo

            //order number
            AddRow(string.Format("{0} #{1}", order.Stall.StallName, order.Id), ReceiptFont.Font20);

            //order time
            AddRow(string.Format("{0:HH:mm:ss ddMMM, yyyy}", order.CreateTime), ReceiptFont.Font16);

            //separator
            AddRow("".PadRight(ReceiptFont.CharsInRow(ReceiptFont.Font24), '-'), ReceiptFont.Font24);

            //receiver
            AddRow(order.Receiver, ReceiptFont.Font16);
            AddRow(order.DeliveryAddress, ReceiptFont.Font16);
            if (order.DeliveryTimeStart != null && order.DeliveryTimeEnd != null)
            {
                AddRow(string.Format("{0:ddd, ddMMM HH:mm}-{1:HH:mm}", order.DeliveryTimeStart, order.DeliveryTimeEnd), ReceiptFont.Font16);
            }

            //separator
            AddRow("".PadRight(ReceiptFont.CharsInRow(ReceiptFont.Font24), '-'), ReceiptFont.Font24);

            //detail
            foreach (var item in order.Items)
            {
                AddRow(item.Name, ReceiptFont.Font24);
                AddRow(string.Format("x{0} @ {1:$0.00}", item.Quantity, item.Price * item.Quantity), ReceiptFont.Font24);
            }

            //separator
            AddRow("".PadRight(ReceiptFont.CharsInRow(ReceiptFont.Font24), '-'), ReceiptFont.Font24);

            //total
            AddRow(string.Format("总计:{0:$0.0}", order.StallAmount), ReceiptFont.Font24);

            //separator
            AddRow("".PadRight(ReceiptFont.CharsInRow(ReceiptFont.Font16), ' '), ReceiptFont.Font16);

            //note
            AddRow("备注:" + order.Note, ReceiptFont.Font16);

            //separator
            AddRow("".PadRight(ReceiptFont.CharsInRow(ReceiptFont.Font16), ' '), ReceiptFont.Font16);

            //slogan
            AddRow("------久等了点餐 让您久等了------", ReceiptFont.Font16);

            //gst
            //AddRow("GST", string.Format("{0:$0.00}", order.GST), ReceiptFont.Font24);

        }

        public float Height
        {
            get
            {
                float height = 0;
                foreach (var r in _rows)
                {
                    height += r.Height + r.Padding.Top + r.Padding.Bottom;
                }

                return height;
            }
        }

        public Bitmap Image
        {
            get
            {
                float currY = 0;
                int logoHeight = 120;
                Bitmap bmp = new Bitmap(MAX_WIDTH, (int)Height + logoHeight);
                var logo = Bitmap.FromFile(Greenspot.Configuration.GreenspotConfiguration.AppSettings["ReceiptLogoPath"].Value);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    //draw logo
                    g.DrawImage(logo, 130, currY);
                    currY += logoHeight;

                    foreach (var r in _rows)
                    {
                        currY += r.Padding.Top;
                        g.DrawString(r.Text, r.Font, Brushes.Black, 0, currY);
                        currY += r.Height + r.Padding.Bottom;
                    }
                }

                return bmp;
            }
        }

    }
}