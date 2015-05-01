using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphics1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Graphics g = null;
        private int pixelSize = 5;
        void PutPixel(Graphics g, int x, int y, int size, Color c)
        {
            //Bitmap bm = new Bitmap(size, size);
            //for (int i = 0; i < size; i++)
            //    for (int j = 0; j < size; j++)
            //        bm.SetPixel(i, j, c);
            //g.DrawImageUnscaled(bm, x, y);

            SolidBrush myBrush = new SolidBrush(c);
            g.FillRectangle(myBrush, new Rectangle(new Point() { X = x, Y = y }, new Size() {  Width = size, Height = size }));
        }

        private void buttonDraw_Click(object sender, EventArgs e)
        {
            if (g != null)
                g.Clear(Color.White);

            g = this.pictureBoxMain.CreateGraphics();
            pixelSize = Convert.ToInt16(this.maskedTextBoxPixelSize.Text);
            if (pixelSize > 0)
            {
                DDALine(g, 0, 0, 100, 60, pixelSize, Color.Black);
                NonSymDDALine(g, 30, 0, 130, 60, pixelSize, Color.Red);
                BrezengamLine(g, 60, 0, 160, 60, pixelSize, Color.Blue);
                BresenhamCircle(g, 150, 200, 50, pixelSize, Color.Green);
                BresenhamCircle(g, 150, 200, 30, pixelSize, Color.Green);
                BresenhamCircle(g, 150, 200, 10, pixelSize, Color.Green);
            }
        }

        private int RoundToPixelSize(double d, int size)
        {
            int i = Convert.ToInt16(d);
            if (i % size != 0) i = (i / size) * size + size;
            return i;
        }

        private void DDALine(Graphics g, double _x1, double _y1, double _x2, double _y2, int size, Color col)
        {
            double x1 = _x1 * size;
            double y1 = _y1 * size;
            double x2 = _x2 * size;
            double y2 = _y2 * size;
            double L;

            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))
            {
                L = Math.Abs(x2 - x1);
            }
            else
            {
                L = Math.Abs(y2 - y1);
            }

            double x = x1;
            double y = y1;

            PutPixel(g, (int)x, (int)y, size, col);
            for (int i = 0; i < L; i += 1)
            {
                if (x1 != x2)
                {
                    double dx = (x2 - x1) / L;
                    x = x + dx;
                }

                if (y1 != y2)
                {
                    double dy = (y2 - y1) / L;
                    y = y + dy;
                }
                PutPixel(g, RoundToPixelSize(x, size), RoundToPixelSize(y, size), size, col);
            }
        }

        private void NonSymDDALine(Graphics g, int _x1, int _y1, int _x2, int _y2, int size, Color col)
        {
            int x1 = _x1 * size;
            int y1 = _y1 * size;
            int x2 = _x2 * size;
            int y2 = _y2 * size;

            int dx;
            int dy;
            int s;
            if (x1 > x2)
            {
                s = x1; x1 = x2; x2 = s;
                s = y1; y1 = y2; y2 = s;
            }
            dx = x2 - x1; dy = y2 - y1;
            PutPixel(g, (int)x1, (int)y1, size, col);
            if (dx == 0 && dy == 0) return;

            /* Вычисление количества позиций по X и Y */
            dx = dx + size; dy = dy + size;

            if (dy == dx)
            {                 /* Наклон == 45 градусов */
                while (x1 < x2)
                {
                    x1 = x1 + size;
                    PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, col);
                }
            }
            else if (dx > dy)
            {           /* Наклон <  45 градусов */
                s = 0;
                while (x1 < x2)
                {
                    x1 = x1 + size;
                    s = s + dy;
                    if (s >= dx) { s = s - dx; y1 = y1 + size; }
                    PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, col);
                }
            }
            else
            {                        /* Наклон >  45 градусов */
                s = 0;
                while (y1 < y2)
                {
                    y1 = y1 + size;
                    s = s + dx;
                    if (s >= dy) { s = s - dy; x1 = x1 + size; }
                    PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, col);
                }
            }
        }

        private void BrezengamLine(Graphics g, int _x1, int _y1, int _x2, int _y2, int size, Color col)
        {
            int x1 = _x1 * size;
            int y1 = _y1 * size;
            int x2 = _x2 * size;
            int y2 = _y2 * size;

            int dx, dy, s, sx, sy, kl, swap, incr1, incr2;
            sx = 0;
            /* Вычисление приращений и шагов */
            if ((dx = x2 - x1) < 0) { dx = -dx; sx -= size; } else if (dx > 0) sx += size;
            sy = 0;
            if ((dy = y2 - y1) < 0) { dy = -dy; sy -= size; } else if (dy > 0) sy += size;

            /* Учет наклона */
            swap = 0;
            if ((kl = dx) < (s = dy))
            {
                dx = s; dy = kl; kl = s; ++swap;
            }
            s = (incr1 = 2 * dy) - dx; /* incr1 - констан. перевычисления */
            /* разности если текущее s < 0  и  */
            /* s - начальное значение разности */
            incr2 = 2 * dx;         /* Константа для перевычисления    */
            /* разности если текущее s >= 0    */
            /* Первый  пиксел вектора       */
            PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, col);
            while ((kl -= size) >= 0)
            {
                if (s >= 0)
                {
                    if (swap > 0) x1 += sx; else y1 += sy;
                    s -= incr2;
                }
                if (swap > 0) y1 += sy; else x1 += sx;
                s += incr1;
                PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, col); /* Текущая  точка  вектора   */
            }
        }

        private void BresenhamCircle(Graphics g, int x0, int y0, int radius, int size, Color col)
        {
            int x = radius * size;
            int y = 0;
            int radiusError = size - x;
            while (x >= y)
            {
                PutPixel(g, RoundToPixelSize(x + x0, size), RoundToPixelSize(y + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(y + x0, size), RoundToPixelSize(x + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(-x + x0, size), RoundToPixelSize(y + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(-y + x0, size), RoundToPixelSize(x + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(-x + x0, size), RoundToPixelSize(-y + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(-y + x0, size), RoundToPixelSize(-x + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(x + x0, size), RoundToPixelSize(-y + y0, size), size, col);
                PutPixel(g, RoundToPixelSize(y + x0, size), RoundToPixelSize(-x + y0, size), size, col);
                y += size;
                if (radiusError < 0)
                {
                    radiusError += 2 * y + size;
                }
                else
                {
                    x -= size;
                    radiusError += 2 * (y - x + size);
                }
            }
        }
    }
}
