using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Zhorin_Filters
{
    
    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imagePath;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "image files|*.jpeg;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = dialog.FileName;
                image = new Bitmap(imagePath);
                this.pictureBox1.Image = image;
                this.pictureBox1.Refresh();
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            InvertFilter filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
           /* Bitmap resultImg = filter.processImg(image);
            this.pictureBox1.Image = resultImg;
            this.pictureBox1.Refresh();*/
            this.Cursor = Cursors.Default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImg = ((Filters)e.Argument).processImg(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImg;

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();

            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
    }
    abstract class Filters
    {
        protected abstract Color calculate(Bitmap sourseImg, int x, int y);
        public Bitmap processImg(Bitmap sourseImg, BackgroundWorker worker)
        {
            Bitmap ResultImg = new Bitmap(sourseImg.Width, sourseImg.Height);
            for (int i = 0; i < sourseImg.Width; i++)
            {
                worker.ReportProgress((int)((float)i / ResultImg.Width * 100));
                if(worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourseImg.Height; j++)
                {
                    ResultImg.SetPixel(i, j, calculate(sourseImg, i,j));
                }
            }
            return ResultImg;
        }
        int clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
    class InvertFilter : Filters
    {
        protected override Color calculate(Bitmap sourseImg, int x, int y)
        {
            Color sourseColor = sourseImg.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourseColor.R, 255 - sourseColor.G, 255 - sourseColor.B);
            return resultColor;
        }
    }
}

