using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GameOfLife1
{
    //delegate void DelGoLife();
    public partial class Form1 : Form
    {
        const int i_size = 90;
        const int j_size = 50;
        const int cellSize = 10;
        const int generations = 10;
        const int butLength = 60;
        const int butHeight = 30;
        private bool stopflag = false; //переключатель конца игрового процесса
        private byte[,] mass = new byte[i_size, j_size]; //массив значений мертвый-живой для ячеек игрового поля
        private byte[,] tempmass = new byte[i_size, j_size]; // Массив временных значений
        private Button[,] massbut = new Button[i_size, j_size]; // Массив кнопок-ячеек игрового поля
        private int generation = 0; //Счетчик поколений
        public delegate void GoLifeDelegate();
        public Form1()
        {
            InitializeComponent();
            Button butnClear = new Button();
            butnClear.Size = new Size(butLength, butHeight);
            butnClear.Location = new Point(0, 0);
            butnClear.Click += new System.EventHandler(this.butnClearClick);
            butnClear.Text = "Clear";
            butnClear.BackColor = Color.Red;
            butnClear.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(butnClear);
            Button butnGo = new Button();
            butnGo.Size = new Size(butLength, butHeight);
            butnGo.Location = new Point(butLength, 0);
            butnGo.Click += new System.EventHandler(this.butnGoClick);
            butnGo.Text = "Go";
            butnGo.BackColor = Color.Red;
            butnGo.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(butnGo);
            Button butnRand = new Button();
            butnRand.Size = new Size(butLength, butHeight);
            butnRand.Location = new Point(butLength*2, 0);
            butnRand.Click += new System.EventHandler(this.butnRandomLifeClick);
            butnRand.Text = "Random";
            butnRand.BackColor = Color.Red;
            butnRand.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(butnRand);
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    massbut[i, j] = new Button();
                    massbut[i, j].Size = new Size(cellSize, cellSize);
                    massbut[i, j].Location = new Point(i * cellSize + 30, j * cellSize + 30);
                    massbut[i, j].Click += new System.EventHandler(this.massbutClick);
                    massbut[i, j].Tag = new Coords(i, j); // .Tag для привязки кнопок-ячеек к массиву значений мертвый-живой  
                    massbut[i, j].FlatStyle = FlatStyle.Flat;
                    this.Controls.Add(massbut[i, j]);
                }
            }

            NewLife();
        }
        private void massbutClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Coords co = (Coords)btn.Tag;
            btn.BackColor = Color.White;
            mass[co.i, co.j] = 1;
        }

        //Нажатие кнопки GO запускаем игровой процесс в отдельном потоке
        private void butnGoClick(object sender, EventArgs e)
        {
            Thread LifeThread = new Thread(MyFunc);//почему не работает просто передача метода
            //LifeThread.IsBackground = true;
            LifeThread.Start();
        }
        private void butnClearClick(object sender, EventArgs e)
        {
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    mass[i, j] = 0;
                    massbut[i, j].BackColor = Color.Black;
                }
            }
            Refresh();
        }
        private void butnRandomLifeClick(object sender, EventArgs e)
        {
            Random rand = new Random();
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    mass[i, j] = (byte)rand.Next(0,2);
                    tempmass[i, j] = mass[i, j];
                    massbut[i, j].BackColor = mass[i, j] == 1? Color.White : Color.Black;
                }
            }
        }
        //Начало новой игры
        private void NewLife()
        {
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    mass[i, j] = 0;
                    tempmass[i, j] = 0;
                    massbut[i, j].BackColor = Color.Black;
                }
            }
            generation = 0;
            //label1.Text = string.Empty;
            this.Refresh();
        }

        private void MyFunc()
        {
            GoLifeDelegate d = new GoLifeDelegate(GoLife);
            Invoke(d);
        }
        //метод выполняется в отдельном потоке, вызываем метод генерации нового поколения 
        //и проверяем состояние переключателя конца процесса
        private void GoLife()
        {
            for (int i = 0; i < generations; i++)
            {
                NextGeneration();
                generation++;
                //label1.Text = generation.ToString();
                this.Refresh();
            }
            if (stopflag) 
            { 
                stopflag = false; 
                //NewLife();
                Application.Exit();
            }
        }
        //Генерирует новое поколение
        public void NextGeneration()
        {
            SuspendLayout();

            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    if ((sosediCount(i, j) == 3) && (mass[i, j] == 0)) { tempmass[i, j] = 1; }
                    if (((sosediCount(i, j) == 3) || (sosediCount(i, j) == 2)) && (mass[i, j] == 1)) { tempmass[i, j] = 1; }
                    if ((sosediCount(i, j) < 2) && (mass[i, j] == 1)) { tempmass[i, j] = 0; }
                    if ((sosediCount(i, j) > 3) && (mass[i, j] == 1)) { tempmass[i, j] = 0; }
                }
            }
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    mass[i, j] = tempmass[i, j];
                    if (mass[i, j] == 1) { massbut[i, j].BackColor = Color.White; } else { massbut[i, j].BackColor = Color.Black; }
                }
            }
            ResumeLayout();

        }
        //функция расчета количества соседей у особи
        private int sosediCount(int a, int b)
        {
            int sosedi = 0;
            if (a > 0 && mass[a - 1, b] == 1) { sosedi++; }
            if (a > 0 && b < 49 && mass[a - 1, b + 1] == 1) { sosedi++; }
            if (b < 49 && mass[a, b + 1] == 1) { sosedi++; };
            if (a < 49 && b < 49 && mass[a + 1, b + 1] == 1) { sosedi++; }
            if (a < 49 && mass[a + 1, b] == 1) { sosedi++; }
            if (a < 49 && b > 0 && mass[a + 1, b - 1] == 1) { sosedi++; }
            if (b > 0 && mass[a, b - 1] == 1) { sosedi++; }
            if (a > 0 && b > 0 && mass[a - 1, b - 1] == 1) { sosedi++; }
            return sosedi;
        }
    }
    //класс объектов, хранящих индексы массива для Button.Tag
    public class Coords
    {
        private int _i;
        private int _j;
        public int i
        {
            get { return _i; }
            set { _i = i; }
        }
        public int j
        {
            get { return _j; }
            set { _j = j; }
        }
        public Coords(int a, int b)
        {
            _i = a; _j = b;
        }

    }


}

