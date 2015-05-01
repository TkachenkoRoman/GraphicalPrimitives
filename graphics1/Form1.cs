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
        void PutPixel(Graphics g, int x, int y, int size, Color c, int alpha = 255)
        {
            //Bitmap bm = new Bitmap(size, size);
            //for (int i = 0; i < size; i++)
            //    for (int j = 0; j < size; j++)
            //        bm.SetPixel(i, j, c);
            //g.DrawImageUnscaled(bm, x, y);

            SolidBrush myBrush = new SolidBrush(Color.FromArgb(alpha,c));
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
                BresenhamCircle(g, 100, 60, 50, pixelSize, Color.Green);
                BresenhamCircle(g, 20, 20, 30, pixelSize, Color.Green);
                BresenhamCircle(g, 20, 20, 10, pixelSize, Color.Green);
                BresenhamEllipse(g, 50, 90, 50, 20, pixelSize, Color.Indigo);
                DrawWuLine(g, Color.Gold, 90, 0, 190, 60, pixelSize);
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

            double dx = (x2 - x1) / L;
            double dy = (y2 - y1) / L;
            double x = x1;
            double y = y1;

            PutPixel(g, (int)x, (int)y, size, col);
            for (int i = 0; i < L; i += 1)
            {
                if (x1 != x2)
                {
                    x = x + dx;
                }

                if (y1 != y2)
                {
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
            x0 *= size;
            y0 *= size;
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

        private void BresenhamEllipse(Graphics g, int xc, int yc, int width, int height, int size, Color col)
        {
            xc *= size;
            yc *= size;
            width *= size;
            height *= size;


            int a2 = width * width;
            int b2 = height * height;
            int fa2 = 4 * a2, fb2 = 4 * b2;
            int x, y, sigma;

            /* first half */
            for (x = 0, y = height, sigma = 2 * b2 + a2 * (size - 2 * height); b2 * x <= a2 * y; x+=size)
            {
                PutPixel(g, RoundToPixelSize(xc + x, size), RoundToPixelSize(yc + y, size), size, col);
                PutPixel(g, RoundToPixelSize(xc - x, size), RoundToPixelSize(yc + y, size), size, col);
                PutPixel(g, RoundToPixelSize(xc + x, size), RoundToPixelSize(yc - y, size), size, col);
                PutPixel(g, RoundToPixelSize(xc - x, size), RoundToPixelSize(yc - y, size), size, col);

                if (sigma >= 0)
                {
                    sigma += fa2 * (size - y);
                    y -= size;
                }
                sigma += b2 * ((4 * x) + 6 * size);
            }

            /* second half */
            for (x = width, y = 0, sigma = 2 * a2 + b2 * (size - 2 * width); a2 * y <= b2 * x; y+=size)
            {

                PutPixel(g, RoundToPixelSize(xc + x, size), RoundToPixelSize(yc + y, size), size, col);
                PutPixel(g, RoundToPixelSize(xc - x, size), RoundToPixelSize(yc + y, size), size, col);
                PutPixel(g, RoundToPixelSize(xc + x, size), RoundToPixelSize(yc - y, size), size, col);
                PutPixel(g, RoundToPixelSize(xc - x, size), RoundToPixelSize(yc - y, size), size, col);

                if (sigma >= 0)
                {
                    sigma += fb2 * (size - x);
                    x -= size;
                }
                sigma += a2 * ((4 * y) + 6 * size);
            }
        }



        public void DrawWuLine(Graphics g, Color clr, int x0, int y0, int x1, int y1, int size)
        {
            x0 *= size;
            y0 *= size;
            x1 *= size;
            y1 *= size;
            //Вычисление изменения координат
            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
            //Если линия параллельна одной из осей, рисуем обычную линию - заполняем все пикселы в ряд
            if (dx == 0 || dy == 0)
            {
                g.DrawLine(new Pen(clr), x0, y0, x1, y1);
                return;
            }

            //Для Х-линии (коэффициент наклона < 1)
            if (dy < dx)
            {
                //Первая точка должна иметь меньшую координату Х
                if (x1 < x0)
                {
                    x1 += x0; x0 = x1 - x0; x1 -= x0;
                    y1 += y0; y0 = y1 - y0; y1 -= y0;
                }
                //Относительное изменение координаты Y
                float grad = (float)dy / dx;
                //Промежуточная переменная для Y
                float intery = y0 + grad;
                //Первая точка
                PutPixel(g, RoundToPixelSize(x0, size), RoundToPixelSize(y0, size), size, clr);

                for (int x = x0 + 1; x < x1; x++)
                {
                    //Верхняя точка
                    PutPixel(g, RoundToPixelSize(x, size), RoundToPixelSize(IPart(intery), size), size, clr, (int)(255 - FPart(intery) * 255));
                    //PutPixel(g, clr, x, IPart(intery), (int)(255 - FPart(intery) * 255));
                    //Нижняя точка
                    PutPixel(g, RoundToPixelSize(x, size), RoundToPixelSize(IPart(intery) + 1, size), size, clr, (int)(FPart(intery) * 255));
                    //PutPixel(g, clr, x, IPart(intery) + 1, (int)(FPart(intery) * 255));
                    //Изменение координаты Y
                    intery += grad;
                }
                //Последняя точка
                PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, clr);
            }
            //Для Y-линии (коэффициент наклона > 1)
            else
            {
                //Первая точка должна иметь меньшую координату Y
                if (y1 < y0)
                {
                    x1 += x0; x0 = x1 - x0; x1 -= x0;
                    y1 += y0; y0 = y1 - y0; y1 -= y0;
                }
                //Относительное изменение координаты X
                float grad = (float)dx / dy;
                //Промежуточная переменная для X
                float interx = x0 + grad;
                //Первая точка
                PutPixel(g, RoundToPixelSize(x0, size), RoundToPixelSize(y0, size), size, clr);

                for (int y = y0 + 1; y < y1; y++)
                {
                    //Верхняя точка
                    PutPixel(g, RoundToPixelSize(y, size), RoundToPixelSize(IPart(interx), size), size, clr, (int)(255 - FPart(interx) * 255));
                    //Нижняя точка
                    PutPixel(g, RoundToPixelSize(y, size), RoundToPixelSize(IPart(interx) + 1, size), size, clr, (int)(FPart(interx) * 255));
                    //Изменение координаты X
                    interx += grad;
                }
                //Последняя точка
                PutPixel(g, RoundToPixelSize(x1, size), RoundToPixelSize(y1, size), size, clr);
            }
        }
        //Целая часть числа
        private  int IPart(float x)
        {
            return (int)x;
        }
        //дробная часть числа
        private  float FPart(float x)
        {
            while (x >= 0)
                x--;
            x++;
            return x;
        }

    }
}
