using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
    1)Написать реализацию генерации таких графических примитивов как вектор, окружность, элипс. используя только методы типа putpixel()
    Алгоритмы для реализации:
    -простой (для векторов), несимметричный цда (цифровой дифференциальный анализатор)
    -Алгоритм Брезенхэма (целочисленный), для векторов
    -Алгоритм Брезенхэма для окружностей
    -Алгоритм Ву
    2)Нарисовать при помощи вышеобозначенного собственные инициалы
    3)Оценить быстродействие алгоритмов
*/

namespace graphics1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
