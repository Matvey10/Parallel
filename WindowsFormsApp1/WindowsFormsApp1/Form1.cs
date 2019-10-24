using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
 {
        public partial class Form1 : Form
        {
            private byte[,] cells;
        public delegate void repaintHandler();
        public event repaintHandler Repaint;
            public Form1()
            {
            InitializeComponent();
            Random rand = new Random();
            cells = new byte[100, 100];
            for (int i = 0; i < 100; i++)
                 for (int j = 0; j < 100; j++)
                 {
                    cells[i, j] = (byte)rand.Next(0, 2);
                 }
            Repaint += repaint;
        }
           
            protected override void OnPaint(PaintEventArgs e)
            {
            base.OnPaint(e);
            var cellSize = new Size(10, 10);
                var location = new Point(0, 0);

                for (int i = 0; i < cells.GetLength(0); i++)
                    for (int j = 0; j < cells.GetLength(1); j++)
                    {
                        var rect = new Rectangle(location.X + i * cellSize.Width, location.Y + j * cellSize.Height, cellSize.Width, cellSize.Height);
                        var brush = cells[i, j] == 1 ? Brushes.Red : Brushes.White;
                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(Pens.Black, rect);
                    }
            Repaint?.Invoke();

        }
         public void repaint()
            {
              for (int i = 0; i < 100; i++)
                    for (int j = 0; j < 100; j++)
                    {
                    if (cells[i, j] == 0)
                        cells[i, j] = 1;
                    else
                        cells[i, j] = 0;
                    }
                Invalidate();
        }
    }
 }

