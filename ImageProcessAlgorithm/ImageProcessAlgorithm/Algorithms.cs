using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ImageProcessAlgorithm
{
    class Algorithms
    {
        //Image Resize and Image Crop
        public  Bitmap Resize(Image image, int width, int height)
        {
            Rectangle kare = new Rectangle(0, 0, width, height);
            Bitmap orantılanmışresim = new Bitmap(width, height);

            orantılanmışresim.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            Graphics g = Graphics.FromImage(orantılanmışresim);
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                g.DrawImage(image, kare, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }


            return orantılanmışresim;
        }
        public  Bitmap Crop(Bitmap src, Rectangle kare)
        {

            Bitmap resim = new Bitmap(kare.Width, kare.Height);

            using (Graphics g = Graphics.FromImage(resim))
            {
                g.DrawImage(src, new Rectangle(0, 0, resim.Width, resim.Height),
                                 kare,
                                 GraphicsUnit.Pixel);
            }
            return resim;
        }
        //Smooting Algorithm
        public Bitmap meanFiltresi(Bitmap GirisResmi, int SablonBoyutu, ProgressBar bg)
        {
            Color OkunanRenk;
            Bitmap CikisResmi;
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int x, y, i, j, toplamR, toplamG, toplamB, ortalamaR, ortalamaG, ortalamaB;
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    toplamR = 0;
                    toplamG = 0;
                    toplamB = 0;
                    for (i = -((SablonBoyutu - 1) / 2); i <= (SablonBoyutu - 1) / 2; i++)
                    {
                        for (j = -((SablonBoyutu - 1) / 2); j <= (SablonBoyutu - 1) / 2; j++)
                        {
                            OkunanRenk = GirisResmi.GetPixel(x + i, y + j);
                            toplamR = toplamR + OkunanRenk.R;
                            toplamG = toplamG + OkunanRenk.G;
                            toplamB = toplamB + OkunanRenk.B;
                            
                        }
                    }
                    ortalamaR = toplamR / (SablonBoyutu * SablonBoyutu);
                    ortalamaG = toplamG / (SablonBoyutu * SablonBoyutu);
                    ortalamaB = toplamB / (SablonBoyutu * SablonBoyutu);
                    CikisResmi.SetPixel(x, y, Color.FromArgb(ortalamaR, ortalamaG, ortalamaB));
                    bg.Value=x;
                }
            }
           return CikisResmi;
        }
        public Bitmap medianFiltresi(int SablonBoyutu, Bitmap GirisResmi, ProgressBar bg)
        {
            Color OkunanRenk;
            Bitmap CikisResmi;
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int ElemanSayisi = SablonBoyutu * SablonBoyutu;
            int[] R = new int[ElemanSayisi];
            int[] G = new int[ElemanSayisi];
            int[] B = new int[ElemanSayisi];
            int[] Gri = new int[ElemanSayisi];
            int x, y, i, j;
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    int k = 0;
                    for (i = -((SablonBoyutu - 1) / 2); i <= (SablonBoyutu - 1) / 2; i++)
                    {
                        for (j = -((SablonBoyutu - 1) / 2); j <= (SablonBoyutu - 1) / 2; j++)
                        {
                            OkunanRenk = GirisResmi.GetPixel(x + i, y + j);
                            R[k] = OkunanRenk.R;
                            G[k] = OkunanRenk.G;
                            B[k] = OkunanRenk.B;
                            Gri[k] = Convert.ToInt16(R[k] * 0.299 + G[k] * 0.587 + B[k] * 0.114);
                            k++;
                        }
                    }
                    int GeciciSayi = 0;
                    for (i = 0; i < ElemanSayisi; i++)
                    {
                        for (j = i + 1; j < ElemanSayisi; j++)
                        {
                            if (Gri[j] < Gri[i])
                            {
                                GeciciSayi = Gri[i];
                                Gri[i] = Gri[j];
                                Gri[j] = GeciciSayi;
                                GeciciSayi = R[i];
                                R[i] = R[j];
                                R[j] = GeciciSayi;
                                GeciciSayi = G[i];
                                G[i] = G[j];
                                G[j] = GeciciSayi;
                                GeciciSayi = B[i];
                                B[i] = B[j];
                                B[j] = GeciciSayi;
                            }
                        }
                    }
                    bg.Value = x;
                    CikisResmi.SetPixel(x, y, Color.FromArgb(R[(ElemanSayisi - 1) / 2], G[(ElemanSayisi - 1) /
                   2], B[(ElemanSayisi - 1) / 2]));
                }
            }
            return CikisResmi;
        }
        public Bitmap GaussFilter(Bitmap GirisResmi, ProgressBar bg)
        {
            Color OkunanRenk;
            Bitmap CikisResmi;
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int SablonBoyutu = 5;
            int ElemanSayisi = SablonBoyutu * SablonBoyutu;
            int x, y, i, j, toplamR, toplamG, toplamB, ortalamaR, ortalamaG, ortalamaB;
            int[] Matris = { 1, 4, 7, 4, 1, 4, 20, 33, 20, 4, 7, 33, 55, 33, 7, 4, 20, 33, 20, 4, 1, 4, 7, 4, 1 };
            int MatrisToplami = 1 + 4 + 7 + 4 + 1 + 4 + 20 + 33 + 20 + 4 + 7 + 33 + 55 + 33 + 7 + 4 + 20 +
           33 + 20 + 4 + 1 + 4 + 7 + 4 + 1;
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    toplamR = 0;
                    toplamG = 0;
                    toplamB = 0;
                    int k = 0;
                    for (i = -((SablonBoyutu - 1) / 2); i <= (SablonBoyutu - 1) / 2; i++)
                    {
                        for (j = -((SablonBoyutu - 1) / 2); j <= (SablonBoyutu - 1) / 2; j++)
                        {
                            OkunanRenk = GirisResmi.GetPixel(x + i, y + j);
                            toplamR = toplamR + OkunanRenk.R * Matris[k];
                            toplamG = toplamG + OkunanRenk.G * Matris[k];
                            toplamB = toplamB + OkunanRenk.B * Matris[k];
                            ortalamaR = toplamR / MatrisToplami;
                            ortalamaG = toplamG / MatrisToplami;
                            ortalamaB = toplamB / MatrisToplami;
                            CikisResmi.SetPixel(x, y, Color.FromArgb(ortalamaR, ortalamaG, ortalamaB));
                            bg.Value = x;
                            k++;
                        }
                    }
                }
            }
            return CikisResmi;
        }

        public int[] GaussMatrisOluştur(int aralık, int sonaralık, int sapma, int matrisBoyutu)
        {
            int[] matris = new int[matrisBoyutu * matrisBoyutu];
            int k = 0;

            double ilkdeger = 1 / (Math.Sqrt(2 * Math.PI) * sapma * sapma);
            double ikincideger = Math.Exp(-(((int)Math.Pow(aralık,2)+(int)Math.Pow(aralık,2)) / (2 * ((int)Math.Pow(sapma,2)))));
            double ölçek = ilkdeger * ikincideger;

            for (int y = aralık; y <= sonaralık; y++)
            {
                for (int x = aralık; x <= sonaralık; x++)
                {
                    double deger = Math.Exp(-(((int)Math.Pow(x,2) + (int)Math.Pow(y,2)) / (2 * (int)Math.Pow(sapma,2))));

                    matris[k] = Convert.ToInt32(ilkdeger * deger * (1 / ölçek));
                    k++;
                }
            }
            return matris;
        }
        public Bitmap GaussMatris(Bitmap GirisResmi, int[] Matris, int SablonBoyutu,ProgressBar bg)
        {
            Color OkunanRenk;
            Bitmap CikisResmi;
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int ElemanSayisi = SablonBoyutu * SablonBoyutu;
            int x, y, i, j;
            int toplamR, toplamG, toplamB;
            int ortalamaR, ortalamaG, ortalamaB;

            int MatrisToplami = Matris.Sum();
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    toplamR = 0;
                    toplamG = 0;
                    toplamB = 0;
                    int k = 0;
                    for (i = -((SablonBoyutu - 1) / 2); i <= (SablonBoyutu - 1) / 2; i++)
                    {
                        for (j = -((SablonBoyutu - 1) / 2); j <= (SablonBoyutu - 1) / 2; j++)
                        {
                            OkunanRenk = GirisResmi.GetPixel(x + i, y + j);
                            toplamR = toplamR + OkunanRenk.R * Matris[k];
                            toplamG = toplamG + OkunanRenk.G * Matris[k];
                            toplamB = toplamB + OkunanRenk.B * Matris[k];
                            ortalamaR = toplamR / MatrisToplami;
                            ortalamaG = toplamG / MatrisToplami;
                            ortalamaB = toplamB / MatrisToplami;
                            CikisResmi.SetPixel(x, y, Color.FromArgb(ortalamaR, ortalamaG, ortalamaB));
                            k++;
                            bg.Value = x;
                        }
                    }
                }
            }
            return CikisResmi;
        }


        public void ResmiNetleştir(PictureBox pictureBox1, PictureBox pictureBox2)

        {
            Color OkunanRenk;
            Bitmap GirisResmi, CikisResmi;
            GirisResmi = new Bitmap(pictureBox1.Image);
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int SablonBoyutu = 3;
            int ElemanSayisi = SablonBoyutu * SablonBoyutu;
            int x, y, i, j, toplamR, toplamG, toplamB;
            int R, G, B;
            int[] Matris = { 0, -2, 0, -2, 11, -2, 0, -2, 0 };
            int MatrisToplami = 0 + -2 + 0 + -2 + 11 + -2 + 0 + -2 + 0;
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    toplamR = 0;
                    toplamG = 0;
                    toplamB = 0;
                    int k = 0;
                    for (i = -((SablonBoyutu - 1) / 2); i <= (SablonBoyutu - 1) / 2; i++)
                    {
                        for (j = -((SablonBoyutu - 1) / 2); j <= (SablonBoyutu - 1) / 2; j++)
                        {
                            OkunanRenk = GirisResmi.GetPixel(x + i, y + j);
                            toplamR = toplamR + OkunanRenk.R * Matris[k];
                            toplamG = toplamG + OkunanRenk.G * Matris[k];
                            toplamB = toplamB + OkunanRenk.B * Matris[k];
                            R = toplamR / MatrisToplami;
                            G = toplamG / MatrisToplami;
                            B = toplamB / MatrisToplami;
                            if (R > 255) R = 255;
                            if (G > 255) G = 255;
                            if (B > 255) B = 255;
                            if (R < 0) R = 0;
                            if (G < 0) G = 0;
                            if (B < 0) B = 0;
                            CikisResmi.SetPixel(x, y, Color.FromArgb(R, G, B));
                            k++;
                        }
                    }
                }
            }
            pictureBox2.Image = CikisResmi;
        }
        //Edge Detection Algorithm
        public void RobertCross(PictureBox pictureBox1,PictureBox pictureBox2, ProgressBar bg)
        {
            Bitmap GirisResmi, CikisResmi;
            GirisResmi = new Bitmap(pictureBox1.Image);
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int x, y;
            Color Renk;
            int P1, P2, P3, P4;
            for (x = 0; x < ResimGenisligi - 1; x++)
            {
                for (y = 0; y < ResimYuksekligi - 1; y++)
                {
                    Renk = GirisResmi.GetPixel(x, y);
                    P1 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y);
                    P2 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y + 1);
                    P3 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y + 1);
                    P4 = (Renk.R + Renk.G + Renk.B) / 3;
                    int Gx = Math.Abs(P1 - P4);
                    int Gy = Math.Abs(P2 - P3);
                    int RobertCrossDegeri = 0;
                    RobertCrossDegeri = Gx;
                    RobertCrossDegeri = Gy;
                    RobertCrossDegeri = Gx + Gy;

                    if (RobertCrossDegeri > 255) RobertCrossDegeri = 255;

                    CikisResmi.SetPixel(x, y, Color.FromArgb(RobertCrossDegeri, RobertCrossDegeri,
                   RobertCrossDegeri));
                    bg.Value = x;
                }
            }
            pictureBox2.Image = CikisResmi;
        }
        public void Prewitt(PictureBox pictureBox1, PictureBox pictureBox2, ProgressBar bg)
        {
            Bitmap GirisResmi, CikisResmi;
            GirisResmi = new Bitmap(pictureBox1.Image);
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int SablonBoyutu = 3;
            int ElemanSayisi = SablonBoyutu * SablonBoyutu;
            int x, y;
            Color Renk;
            int P1, P2, P3, P4, P5, P6, P7, P8, P9;
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    Renk = GirisResmi.GetPixel(x - 1, y - 1);
                    P1 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y - 1);
                    P2 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y - 1);
                    P3 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x - 1, y);
                    P4 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y);
                    P5 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y);
                    P6 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x - 1, y + 1);
                    P7 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y + 1);
                    P8 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y + 1);
                    P9 = (Renk.R + Renk.G + Renk.B) / 3;
                    int Gx = Math.Abs(-P1 + P3 - P4 + P6 - P7 + P9);
                    int Gy = Math.Abs(P1 + P2 + P3 - P7 - P8 - P9);
                    int PrewittDegeri = 0;
                    PrewittDegeri = Gx;
                    PrewittDegeri = Gy;
                    PrewittDegeri = Gx + Gy;

                    if (PrewittDegeri > 255) PrewittDegeri = 255;

                    CikisResmi.SetPixel(x, y, Color.FromArgb(PrewittDegeri, PrewittDegeri, PrewittDegeri));
                    bg.Value = x;
                }
            }
            pictureBox2.Image = CikisResmi;
        }
        public void Sobel(PictureBox pictureBox1, PictureBox pictureBox2, ProgressBar bg)
        {
            Bitmap GirisResmi, CikisResmi;
            GirisResmi = new Bitmap(pictureBox1.Image);
            int ResimGenisligi = GirisResmi.Width;
            int ResimYuksekligi = GirisResmi.Height;
            CikisResmi = new Bitmap(ResimGenisligi, ResimYuksekligi);
            int SablonBoyutu = 3;
            int ElemanSayisi = SablonBoyutu * SablonBoyutu;
            int x, y;
            Color Renk;
            int P1, P2, P3, P4, P5, P6, P7, P8, P9;
            for (x = (SablonBoyutu - 1) / 2; x < ResimGenisligi - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ResimYuksekligi - (SablonBoyutu - 1) / 2; y++)
                {
                    Renk = GirisResmi.GetPixel(x - 1, y - 1);
                    P1 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y - 1);
                    P2 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y - 1);
                    P3 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x - 1, y);
                    P4 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y);
                    P5 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y);
                    P6 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x - 1, y + 1);
                    P7 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x, y + 1);
                    P8 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = GirisResmi.GetPixel(x + 1, y + 1);
                    P9 = (Renk.R + Renk.G + Renk.B) / 3;
                    int Gx = Math.Abs(-P1 + P3 - 2 * P4 + 2 * P6 - P7 + P9);
                    int Gy = Math.Abs(P1 + 2 * P2 + P3 - P7 - 2 * P8 - P9);
                    int SobelDegeri = 0;

                    //SobelDegeri = Gx + Gy;
                    SobelDegeri = Convert.ToInt16(Math.Sqrt(Gx * Gx + Gy * Gy));


                    if (SobelDegeri > 100)
                        SobelDegeri = 255;
                    else
                        SobelDegeri = 0;
                    CikisResmi.SetPixel(x, y, Color.FromArgb(SobelDegeri, SobelDegeri, SobelDegeri));
                    bg.Value = x;
                }
            }
            pictureBox2.Image = CikisResmi;
        }
        public void Compass(PictureBox pictureBox1, PictureBox pictureBox2, ProgressBar bg)
        {
            List<int[,]> matris = new List<int[,]>()
             {
                 new int[,]{{-1,-1,-1},{+1,-2,+1 },{1,1,1}},
                 new int[,]{{-1,-1,1},{-1,-2,+1 },{1,1,1}},
                 new int[,]{{-1,1,1},{-1,-2,+1 },{-1,1,1}},
                 new int[,]{{1,1,1},{-1,-2,+1 },{-1,-1,1}},
                 new int[,]{{1,1,1},{1,-2,1 },{-1,-1,-1}},
                 new int[,]{{1,1,1},{1,-2,-1 },{1,-1,-1}},
                 new int[,]{{1,1,-1},{1,-2,-1 },{1,1,-1}},
                 new int[,]{{+1,-1,-1},{1,-2,-1},{1,1,1}},
             };
            var ilkresim = new Bitmap(pictureBox1.Image);
            var yeniresim = new Bitmap(ilkresim.Width, ilkresim.Height);
            int x, y;
            Color Renk;

            int P1, P2, P3, P4, P5, P6, P7, P8, P9;
            int SablonBoyutu = 3;
            for (x = (SablonBoyutu - 1) / 2; x < ilkresim.Width - (SablonBoyutu - 1) / 2; x++)
            {
                for (y = (SablonBoyutu - 1) / 2; y < ilkresim.Height - (SablonBoyutu - 1) / 2; y++)
                {
                    Renk = ilkresim.GetPixel(x - 1, y - 1);
                    P1 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x, y - 1);
                    P2 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x + 1, y - 1);
                    P3 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x - 1, y);
                    P4 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x, y);
                    P5 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x + 1, y);
                    P6 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x - 1, y + 1);
                    P7 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x, y + 1);
                    P8 = (Renk.R + Renk.G + Renk.B) / 3;
                    Renk = ilkresim.GetPixel(x + 1, y + 1);
                    P9 = (Renk.R + Renk.G + Renk.B) / 3;                    int[,] Matris1 = matris[0]; int[,] Matris2 = matris[1]; int[,] Matris3 = matris[2]; int[,] Matris4 = matris[3];                    int[,] Matris5 = matris[4]; int[,] Matris6 = matris[5]; int[,] Matris7 = matris[6]; int[,] Matris8 = matris[7];                    int G0 = Math.Abs(
                        Matris1[0, 0] * P1 + Matris1[0, 1] * P2 + Matris1[0, 2] * P3 +
                        Matris1[1, 0] * P4 + Matris1[1, 1] * P5 + Matris1[1, 2] * P6 +
                        Matris1[2, 0] * P7 + Matris1[2, 1] * P8 + Matris1[2, 2] * P9
                        );
                    int G1 = Math.Abs(
                      Matris2[0, 0] * P1 + Matris2[0, 1] * P2 + Matris2[0, 2] * P3 +
                      Matris2[1, 0] * P4 + Matris2[1, 1] * P5 + Matris2[1, 2] * P6 +
                      Matris2[2, 0] * P7 + Matris2[2, 1] * P8 + Matris2[2, 2] * P9
                      );
                    int G2 = Math.Abs(
                      Matris3[0, 0] * P1 + Matris3[0, 1] * P2 + Matris3[0, 2] * P3 +
                      Matris3[1, 0] * P4 + Matris3[1, 1] * P5 + Matris3[1, 2] * P6 +
                      Matris3[2, 0] * P7 + Matris3[2, 1] * P8 + Matris3[2, 2] * P9
                      );
                    int G3 = Math.Abs(
                      Matris4[0, 0] * P1 + Matris4[0, 1] * P2 + Matris4[0, 2] * P3 +
                      Matris4[1, 0] * P4 + Matris4[1, 1] * P5 + Matris4[1, 2] * P6 +
                      Matris4[2, 0] * P7 + Matris4[2, 1] * P8 + Matris4[2, 2] * P9
                      );
                    int G4 = Math.Abs(
                      Matris5[0, 0] * P1 + Matris5[0, 1] * P2 + Matris5[0, 2] * P3 +
                      Matris5[1, 0] * P4 + Matris5[1, 1] * P5 + Matris5[1, 2] * P6 +
                      Matris5[2, 0] * P7 + Matris5[2, 1] * P8 + Matris5[2, 2] * P9
                      );
                    int G5 = Math.Abs(
                      Matris6[0, 0] * P1 + Matris6[0, 1] * P2 + Matris6[0, 2] * P3 +
                      Matris6[1, 0] * P4 + Matris6[1, 1] * P5 + Matris6[1, 2] * P6 +
                      Matris6[2, 0] * P7 + Matris6[2, 1] * P8 + Matris6[2, 2] * P9
                      );
                    int G6 = Math.Abs(
                      Matris7[0, 0] * P1 + Matris7[0, 1] * P2 + Matris7[0, 2] * P3 +
                      Matris7[1, 0] * P4 + Matris7[1, 1] * P5 + Matris7[1, 2] * P6 +
                      Matris7[2, 0] * P7 + Matris7[2, 1] * P8 + Matris7[2, 2] * P9
                      );
                    int G7 = Math.Abs(
                      Matris8[0, 0] * P1 + Matris8[0, 1] * P2 + Matris8[0, 2] * P3 +
                      Matris8[1, 0] * P4 + Matris8[1, 1] * P5 + Matris8[1, 2] * P6 +
                      Matris8[2, 0] * P7 + Matris8[2, 1] * P8 + Matris8[2, 2] * P9
                      );

                    int CrossDegeri = 0;                    int[] TümAçılar = new int[] { G0, G1, G2, G2, G4, G5, G6, G7 };                    CrossDegeri = TümAçılar.Max();                    if (CrossDegeri > 255) CrossDegeri = 255;

                    Color Sonrenk = Color.FromArgb(CrossDegeri, CrossDegeri, CrossDegeri);

                    yeniresim.SetPixel(x, y, Sonrenk);
                    bg.Value = x;

                }
            }
            pictureBox2.Image = yeniresim;
        }
        //MOVE DETECTİON AND NIGHT VİSION
        public void HareketAlgıla(Bitmap lastframe,PictureBox pictureBox1,List<Point> pixsels)
        {
            Bitmap ilkresim = new Bitmap(pictureBox1.Image);
            Bitmap ikiresimarasıfark = new Bitmap(ilkresim.Width, ilkresim.Height);
            pixsels.Clear();
            int deger = ilkresim.Height / 10;
            int a = 0;
            int b = 0;
            for (int i = 0; i < ilkresim.Width; i += 5)
            {
                b++;
                for (int j = 0; j < ilkresim.Height; j += 5)
                {
                    a++;
                    Color renk1 = ilkresim.GetPixel(i, j);
                    Color renk2 = lastframe.GetPixel(i, j);
                    int yeniR = Math.Abs(renk1.R - renk2.R);
                    int yeniG = Math.Abs(renk1.G - renk2.G);
                    int yeniB = Math.Abs(renk1.B - renk2.B);
                    if (yeniR > 50 && yeniG > 50 && yeniB > 50)
                    {
                        pixsels.Add(new Point(i, j));
                    }
                }
            }
        }
        public void GeceGörüşü(PictureBox pictureBox1, PictureBox pictureBox2, TrackBar trackBar1)
        {
            Bitmap image = pictureBox1.Image as Bitmap;
            Bitmap newimage = new Bitmap(image.Width, image.Height);
            for (var i = 0; i < newimage.Width; i++)
            {
                for (var j = 0; j < newimage.Height; j++)
                {
                    Color okunanrenk = image.GetPixel(i, j);
                    int newR = okunanrenk.R + trackBar1.Value;
                    if (newR > 255) { newR = 255; };

                    int newG = okunanrenk.G + trackBar1.Value;
                    if (newG > 255) { newG = 255; };

                    int newB = okunanrenk.B + trackBar1.Value;
                    if (newB > 255) { newB = 255; };

                    newimage.SetPixel(i, j, Color.FromArgb(newR, newG, newB));

                }
            }
            pictureBox2.Image = newimage;
        }
    }
}
