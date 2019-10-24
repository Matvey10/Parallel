using System;
using System.Collections.Generic;
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
        const int i_size = 50;
        const int j_size = 50;
        const int cellSize = 7;
        const int generations = 10;
        const int butWidth = 100;
        const int butHeight = 30;
        private byte[,] cellArray = new byte[i_size, j_size]; 
        private byte[,] tmpCellArray = new byte[i_size, j_size]; 
        private Button[,] butnArray = new Button[i_size, j_size]; 
        private int generation = 0; 
        //public delegate void GoLifeDelegate();
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
            for (int i = 0; i < i_size; i++)
                for (int j = 0; j < j_size; j++)
                {
                    butnArray[i, j] = new Button();
                    butnArray[i, j].Height = cellSize;
                    butnArray[i, j].Width = cellSize;
                    butnArray[i, j].VerticalAlignment = VerticalAlignment.Top;
                    butnArray[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                    butnArray[i, j].Margin = new Thickness(i * cellSize, j * cellSize + 40, 0, 0);
                    grid1.Children.Add(butnArray[i, j]);
                }
            NewLife();
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
                    cellArray[i, j] = 0;
                    tmpCellArray[i, j] = 0;
                    butnArray[i, j].Background = new SolidColorBrush(Colors.Black);
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
            }
        }
        public void NextGeneration()
        {
            for (int i = 0; i < i_size; i++)
                for (int j = 0; j < j_size; j++)
                {
                    neighborCount(i, j, out int neighbors);
                    if ((neighbors == 3) && (cellArray[i, j] == 0))
                    {
                        tmpCellArray[i, j] = 1;
                        //this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.White); });
                    }
                    if (((neighbors == 3) || (neighbors == 2)) && (cellArray[i, j] == 1))
                    {
                        tmpCellArray[i, j] = 1;
                    }
                    if ((neighbors < 2) && (cellArray[i, j] == 1))
                    {
                        tmpCellArray[i, j] = 0;
                        //this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                    }
                    if ((neighbors > 3) && (cellArray[i, j] == 1))
                    {
                        tmpCellArray[i, j] = 0;
                        //this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                    }
                }

            for (int i = 0; i < i_size; i++)
                for (int j = 0; j < j_size; j++)
                {
                    cellArray[i, j] = tmpCellArray[i, j];
                    if (cellArray[i, j] == 1)
                        this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.White); });
                    else
                        this.Dispatcher.Invoke(() => { butnArray[i, j].Background = new SolidColorBrush(Colors.Black); });
                }
            this.Dispatcher.Invoke(() => { textBox1.Text = "Generation №" + (generation).ToString(); });
        }
        /*public void NextGeneration()
        {
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    neighborCount(i, j, out int neighbors);
                    if ((neighbors == 3) && (cellArray[i, j] == 0)) { tmpCellArray[i, j] = 1; }
                    if (((neighbors == 3) || (neighbors == 2)) && (cellArray[i, j] == 1)) { tmpCellArray[i, j] = 1; }
                    if ((neighbors < 2) && (cellArray[i, j] == 1)) { tmpCellArray[i, j] = 0; }
                    if ((neighbors > 3) && (cellArray[i, j] == 1)) { tmpCellArray[i, j] = 0; }
                }
            }
            for (int i = 0; i < i_size; i++)
            {
                for (int j = 0; j < j_size; j++)
                {
                    cellArray[i, j] = tmpCellArray[i, j];
                    if (cellArray[i, j] == 1)
                        butnArray[i, j].Background = new SolidColorBrush(Colors.White); 
                    else 
                        butnArray[i, j].Background = new SolidColorBrush(Colors.Black);
                }
            }
        }*/
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
}
    /*private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = textBox1.Text;
            if (text != "")
            {
                MessageBox.Show(text);
            }
        }*/


