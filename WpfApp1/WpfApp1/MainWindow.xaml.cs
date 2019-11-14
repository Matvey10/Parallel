using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int i_size = 400;
        const int j_size = 200;
        const int cellSize = 3;
        const int generations = 25;
        const int butWidth = 100;
        const int butHeight = 30;
        private byte[,] cellArray = new byte[i_size, j_size]; 
        private byte[,] tmpCellArray = new byte[i_size, j_size]; 
        private Button[,] butnArray = new Button[i_size, j_size]; 
        private int generation = 0; 
        public delegate void RepaintDelegate();
        public event RepaintDelegate RepaintDone;
        public MainWindow()
        {
            InitializeComponent();
            Button butnGo = new Button();
            butnGo.Height = butHeight;
            butnGo.Width = butWidth;
            butnGo.Content = "Go";
            butnGo.VerticalAlignment = VerticalAlignment.Top;
            butnGo.HorizontalAlignment = HorizontalAlignment.Left;
            butnGo.Margin = new Thickness(0, 0, 0, 0);
            butnGo.Click += butnGoClick;
            grid1.Children.Add(butnGo);
            Button butnClear = new Button();
            butnClear.Height = butHeight;
            butnClear.Width = butWidth;
            butnClear.Content = "Clear";
            butnClear.VerticalAlignment = VerticalAlignment.Top;
            butnClear.HorizontalAlignment = HorizontalAlignment.Left;
            butnClear.Margin = new Thickness(butWidth, 0, 0, 0);
            butnClear.Click += butnClearClick;
            grid1.Children.Add(butnClear);
            Button butnRandom = new Button();
            butnRandom.Height = butHeight;
            butnRandom.Width = butWidth;
            butnRandom.Content = "RandomLife";
            butnRandom.VerticalAlignment = VerticalAlignment.Top;
            butnRandom.HorizontalAlignment = HorizontalAlignment.Left;
            butnRandom.Margin = new Thickness(butWidth*2, 0, 0, 0);
            butnRandom.Click += butnRandomLifeClick;
            grid1.Children.Add(butnRandom);
            Button butnX= new Button();
            butnX.Height = butHeight;
            butnX.Width = butWidth;
            butnX.Content = "XLife";
            butnX.VerticalAlignment = VerticalAlignment.Top;
            butnX.HorizontalAlignment = HorizontalAlignment.Left;
            butnX.Margin = new Thickness(butWidth*3, 0, 0, 0);
            butnX.Click += butnXifeClick;
            grid1.Children.Add(butnX);
            for (int i = 0; i < i_size; i++)
                for (int j = 0; j < j_size; j++)
                {
                    butnArray[i, j] = new Button();
                    butnArray[i, j].Height = cellSize;
                    butnArray[i, j].Width = cellSize;
                    butnArray[i, j].VerticalAlignment = VerticalAlignment.Top;
                    butnArray[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                    butnArray[i, j].Margin = new Thickness(i * cellSize, j * cellSize + 40, 0, 0);
                    butnArray[i, j].Tag = new Coordinates(i, j);
                    butnArray[i, j].Click += Button_Click;
                    grid1.Children.Add(butnArray[i, j]);
                }
            NewLife();
            //RepaintDone += RepaintTextBox;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button butn = (Button)sender;
            Coordinates cs = (Coordinates)butn.Tag;
            butn.Background = new SolidColorBrush(Colors.White);
            cellArray[cs.i, cs.j] = 1;
            tmpCellArray[cs.i, cs.j] = cellArray[cs.i, cs.j];
        }
        private void butnXifeClick(object sender, EventArgs e)
        {
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    if (i == i_size / 2 | j == j_size/2)
                        cellArray[i, j] = 1;
                    else
                        cellArray[i, j] = 0;
                    tmpCellArray[i, j] = cellArray[i, j];
                    butnArray[i, j].Background = cellArray[i, j] == 1 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
                }
            }
        }

        /*private void ButnGo_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }*/
        /*private void DelForRepaint()
        {
            RepaintDelegate d = new RepaintDelegate(Repaint);
            d?.Invoke();
        }*/
        private void butnGoClick(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "Generation №0";
            textBox1.Background = new SolidColorBrush(Colors.Aquamarine);
            Thread LifeThread = new Thread(new ThreadStart(GoLife));                                    
            LifeThread.Start();
        }

        private void butnClearClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    if (cellArray[i,j] == 1)
                        butnArray[i, j].Background = new SolidColorBrush(Colors.Black);
                    cellArray[i, j] = 0;
                    tmpCellArray[i, j] = 0;
                }
            }
            textBox1.Text = "Game of Life";
            generation = 0;
        }
        private void butnRandomLifeClick(object sender, EventArgs e)
        {
            Random rand = new Random();
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    cellArray[i, j] = (byte)rand.Next(0, 2);
                    tmpCellArray[i, j] = cellArray[i, j];
                    butnArray[i, j].Background = cellArray[i, j] == 1 ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
                }
            }
        }
        private void NewLife()
        {
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    cellArray[i, j] = 0;
                    tmpCellArray[i, j] = 0;
                    butnArray[i, j].Background = new SolidColorBrush(Colors.Black);
                }
            }
            generation = 0;
        }
        private void GoLife()
        {
            for (int i1 = 0; i1 < generations; i1++)
            {
                generation++;
                NextGeneration();
                this.Dispatcher.Invoke(() => { textBox1.Text = "Generation №" + generation.ToString(); });
                Thread.Sleep(1000);
            }
        }
       /* public void RepaintTextBox()
        {
            this.Dispatcher.Invoke(() => { textBox1.Text = "Generation №" + generation.ToString(); });
        }*/
        public void NextGeneration()
        {
            ParallelLoopResult result = Parallel.For(0, i_size, Repaint);
            for (int i = 0; i < i_size; i++)
                for (int j = 0; j < j_size; j++)
                {
                    cellArray[i, j] = tmpCellArray[i, j];
                }
            //RepaintDone?.Invoke();
            /*if (result.IsCompleted)
                this.Dispatcher.Invoke(() => { textBox1.Text = "Generation №" + (generation).ToString(); });*/
            /*for (int i = 0; i < i_size; i++)
                for (int j = 0; j < j_size; j++)
                {
                    neighborCount(i, j, out int neighbors);
                    if ((neighbors == 3) && (cellArray[i, j] == 0))
                    {
                        tmpCellArray[i, j] = 1;
                        this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.White); });
                    }
                    if (((neighbors == 3) || (neighbors == 2)) && (cellArray[i, j] == 1))
                    {
                        tmpCellArray[i, j] = 1;
                    }
                    if ((neighbors < 2) && (cellArray[i, j] == 1))
                    {
                        tmpCellArray[i, j] = 0;
                        this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                    }
                    if ((neighbors > 3) && (cellArray[i, j] == 1))
                    {
                        tmpCellArray[i, j] = 0;
                        this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                    }
                }*/


            // this.Dispatcher.Invoke(() => { textBox1.Text = "Generation №" + (generation).ToString(); });
        }
        
        private void Repaint(int i)
        {
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw.Start();
            for (int j = 0; j < j_size; j++)
            {
                neighborCount(i, j, out int neighbors);
                if ((neighbors == 3) && (cellArray[i, j] == 0))
                {
                    tmpCellArray[i, j] = 1;
                    sw2.Start();
                    this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.White); });
                    sw2.Start();
                }
                else if (((neighbors == 3) || (neighbors == 2)) && (cellArray[i, j] == 1))
                {
                    tmpCellArray[i, j] = 1;
                }
                else if ((neighbors < 2) && (cellArray[i, j] == 1))
                {
                    tmpCellArray[i, j] = 0;
                    sw2.Start();
                    this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                    sw2.Stop();
                }
                else if ((neighbors > 3) && (cellArray[i, j] == 1))
                {
                    tmpCellArray[i, j] = 0;
                    sw2.Start();
                    this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                    sw2.Stop();
                }
            }

            sw.Stop();
            Console.WriteLine($"Sw:{sw.Elapsed}");
            Console.WriteLine($"Sw2:{sw2.Elapsed}");
        }
        private void neighborCount(int a, int b, out int neighbors)
        {
            neighbors = 0;
            if (a > 0 && cellArray[a - 1, b] == 1) { neighbors++; }
            if (a > 0 && b < j_size - 1 && cellArray[a - 1, b + 1] == 1) { neighbors++; }
            if (b < j_size-1 && cellArray[a, b + 1] == 1) { neighbors++; };
            if (a < i_size-1 && b < j_size - 1 && cellArray[a + 1, b + 1] == 1) { neighbors++; }
            if (a < i_size - 1 && cellArray[a + 1, b] == 1) { neighbors++; }
            if (a < i_size - 1 && b > 0 && cellArray[a + 1, b - 1] == 1) { neighbors++; }
            if (b > 0 && cellArray[a, b - 1] == 1) { neighbors++; }
            if (a > 0 && b > 0 && cellArray[a - 1, b - 1] == 1) { neighbors++; }
        }
    }
    class Coordinates
    {
        public int i { get; set; }
        public int j { get; set; }
        public Coordinates (int i, int j)
        {
            this.i = i;
            this.j = j;
        }
    }
}
 

