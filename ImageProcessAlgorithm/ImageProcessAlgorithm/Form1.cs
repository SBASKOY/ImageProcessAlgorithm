using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessAlgorithm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int label7X;
        private VideoCaptureDevice aygıt;
        private FilterInfoCollection aygıtlar;
        Algorithms algorithm = new Algorithms();
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int x = (int)((label7X + (trackBar1.Value)*1.4));
            label7.Location=new Point(x, label7.Location.Y);
            label7.Text = trackBar1.Value.ToString();
        }
        BackgroundWorker bg;
        private void Form1_Load(object sender, EventArgs e)
        {
            label7X = label7.Location.X;
            aygıtlar = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            aygıt = new VideoCaptureDevice();
            timer1.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            bg = new BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.DoWork += new DoWorkEventHandler(DoWork);
            bg.ProgressChanged += new ProgressChangedEventHandler(ProgressChange);
            panel1.Visible = false;
           
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
        private void ProgressChange(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            
            this.Text = e.ProgressPercentage.ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            aygıt = new VideoCaptureDevice(aygıtlar[0].MonikerString);
            aygıt.NewFrame += Video;
            aygıt.Start();
        }
        public void Video(object sender, NewFrameEventArgs video)
        {
            Bitmap resim = (Bitmap)video.Frame.Clone();
            pictureBox1.Image =algorithm.Resize(resim, pictureBox1.Width, pictureBox1.Height);

        }
        int matrixsize;
        int aralıkx, aralıky;
        bool getMatrizSize()
        {
            if (radioButton1.Checked) { matrixsize = 3;aralıkx =-1;aralıky = 1; return true; }
            else if (radioButton2.Checked) { matrixsize = 5; aralıkx = -2; aralıky = 2; return true; }
            else if (radioButton3.Checked) { matrixsize = 7; aralıkx = -3; aralıky = 3; return true; }
            else
            {
                MessageBox.Show("Matrix Boyutu Şeçmelisiniz", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
                
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
            int operation = comboBox1.SelectedIndex;

            progressBar1.Maximum = pictureBox1.Image.Width;
            Bitmap srcimage = pictureBox1.Image as Bitmap;  
            switch (operation)
            {
                case 0:
                    if (getMatrizSize()) pictureBox2.Image = algorithm.meanFiltresi(srcimage, matrixsize,progressBar1);
                    return;
                case 1:
                    if (getMatrizSize())pictureBox2.Image = algorithm.medianFiltresi(matrixsize, srcimage, progressBar1);
                    return;
                case 2:
                        try
                        {
                            int sapma = Convert.ToInt16(textBox1.Text);
                            if (getMatrizSize())
                            {
                                int[] matrix = algorithm.GaussMatrisOluştur(aralıkx, aralıky, sapma, matrixsize);
                                pictureBox2.Image = algorithm.GaussMatris(srcimage,matrix, matrixsize, progressBar1);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Standart Sapma Uygun Değildi", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    return;
                case 3:
                    algorithm.Sobel(pictureBox1, pictureBox2, progressBar1);
                    return;
                case 4:
                    algorithm.RobertCross(pictureBox1, pictureBox2, progressBar1);
                    return;
                case 5:
                    algorithm.Compass(pictureBox1, pictureBox2, progressBar1);
                    return;
                case 6:
                    algorithm.Prewitt(pictureBox1, pictureBox2, progressBar1);
                    return;
               default:
                    MessageBox.Show("Algoritma Seçmediniz", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }else
            {
                MessageBox.Show("Bu İşlemler İçin Resim Yüklemelisiniz", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar1.Value = 0;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox1.Image = pictureBox2.Image;
            }
            else
            {
                MessageBox.Show("Lütfen Bir Resime İşle Uygulayınız", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (aygıt.IsRunning) aygıt.Stop();
            pictureBox2.Image = null;
            if (timer1.Enabled) timer1.Enabled = false;
            OpenFileDialog resimaç = new OpenFileDialog();
            resimaç.Filter = "Tümü|*.*";
            resimaç.RestoreDirectory = true;
            if (resimaç.ShowDialog() == DialogResult.OK)
            {
                Image resim = Image.FromFile(resimaç.FileName);
                pictureBox1.Image = algorithm.Resize(resim, pictureBox1.Width, pictureBox2.Height);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 2)
            {
                panel1.Visible = true;
            }else
            {
                panel1.Visible = false;
            }
            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
            }
            else
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Enabled = false;
            }
            
        }

        int a;
        Bitmap sonresim;
        bool nightvisıon = false;
        bool movedetection = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (nightvisıon)
            {
               algorithm.GeceGörüşü(pictureBox1,pictureBox2,trackBar1);
            }
            else if (movedetection)
            {
                a++;
                if (a % 2 != 0)
                {
                    sonresim = pictureBox1.Image as Bitmap;
                }
                else
                {
                    algorithm.HareketAlgıla(sonresim,pictureBox1,pixsels);
                    Bitmap image = pictureBox1.Image as Bitmap;
                    List<int> x = new List<int>();
                    List<int> y = new List<int>();
                    foreach (Point i in pixsels)
                    {
                        x.Add(i.X);
                        y.Add(i.Y);
                    }

                    if (x.Count > 1 && y.Count > 1)
                    {
                        Graphics g = Graphics.FromImage(image);
                        int xmax = x.Max();
                        int xmin = x.Min();
                        int ymax = y.Max();
                        int ymin = y.Min();
                        g.DrawRectangle(new Pen(Color.Red), xmin, ymin, xmax - xmin, ymax - ymin);
                    }
                    pictureBox2.Image = image;
                }
            }

        }
        List<Point> pixsels = new List<Point>();

        private void button3_Click(object sender, EventArgs e)
        {
            if (aygıt.IsRunning)
            {
                movedetection = true;
                nightvisıon = false;
                timer1.Enabled = true;
            }else
            {
                MessageBox.Show("Lütfen Kamerayı Acınız", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (aygıt.IsRunning) aygıt.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (aygıt.IsRunning)
            {
                movedetection = false;
                nightvisıon = true;
                timer1.Enabled = true;
            }
            else
            {
                MessageBox.Show("Lütfen Kamerayı Acınız", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
