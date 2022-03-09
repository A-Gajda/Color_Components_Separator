using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekt3
{
    public partial class Form1 : Form
    {
        Bitmap image, canvaImage;
        DirectBitmap canva1Direct, canva2Direct, canva3Direct, canvaDirect, reducedCanvaImage;

        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(@"..\..\..\..\images\image11.png");
            chooseProfile.SelectedIndex = 0;
            SetCanvas();
            colorProfileComboBox.SelectedIndex = 0;
            illuminantComboBox.SelectedIndex = 0;
            canva1.Refresh();
            canva2.Refresh();
            canva3.Refresh();
            SetLabSettings();
            redYfield.Maximum = 0.999999M - redXfield.Value;
            redXfield.Maximum = 0.999999M - redYfield.Value;
            greenYfield.Maximum = 0.999999M - greenXfield.Value;
            greenXfield.Maximum = 0.999999M - greenYfield.Value;
            blueYfield.Maximum = 0.999999M - blueXfield.Value;
            blueXfield.Maximum = 0.999999M - blueYfield.Value;
            whiteXfield.Maximum = 0.999999M - whiteYfield.Value;
            whiteYfield.Maximum = 0.999999M - whiteXfield.Value;
            LabSettings.picture = new double[canva1.Width, canva1.Height, 3];
            CalculateXYZ();
        }

        private void SetCanvas()
        {
            originalCanva.Image = new Bitmap(image, originalCanva.Width, originalCanva.Height);
            canvaImage = new Bitmap(image, canva1.Width, canva1.Height);
            canva1Direct = new DirectBitmap(canva1.Width, canva1.Height);
            canva2Direct = new DirectBitmap(canva1.Width, canva1.Height);
            canva3Direct = new DirectBitmap(canva1.Width, canva1.Height);
            canvaDirect = new DirectBitmap(canva1.Width, canva1.Height);
            reducedCanvaImage = new DirectBitmap(image.Width, image.Height);
            for (int i = 0; i < canva1.Width; i++)
            {
                for (int j = 0; j < canva1.Height; j++)
                {
                    canvaDirect.SetPixel(i, j, canvaImage.GetPixel(i, j));
                }
            }
        }

        private void chooseProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chooseProfile.SelectedIndex == 0)
            {
                label1.Text = "Y";
                label2.Text = "Cb";
                label3.Text = "Cr";
                labGroupBox.Enabled = false;
            }
            if (chooseProfile.SelectedIndex == 1)
            {
                label1.Text = "H";
                label2.Text = "S";
                label3.Text = "V";
                labGroupBox.Enabled = false;
            }
            if (chooseProfile.SelectedIndex == 2)
            {
                label1.Text = "L";
                label2.Text = "a";
                label3.Text = "b";
                labGroupBox.Enabled = true;
            }
            canva1.Refresh();
            canva2.Refresh();
            canva3.Refresh();
        }

        private void canva1_Paint(object sender, PaintEventArgs e)
        {
            if (chooseProfile.SelectedIndex == 0)
            {
                for (int i = 0; i < canva1.Width; i++)
                {
                    for (int j = 0; j < canva1.Height; j++)
                    {
                        SetYPixel(i, j);
                    }
                }
            }
            else if (chooseProfile.SelectedIndex == 1)
            {
                for (int i = 0; i < canva1.Width; i++)
                {
                    for (int j = 0; j < canva1.Height; j++)
                    {
                        SetHPixel(i, j);
                    }
                }
            }
            else if (chooseProfile.SelectedIndex == 2)
            {
                for (int i = 0; i < canva1.Width; i++)
                {
                    for (int j = 0; j < canva1.Height; j++)
                    {
                        SetLPixel(i, j);
                    }
                }
            }
            e.Graphics.DrawImage(canva1Direct.Bitmap, 0, 0);
        }

        private void SetYPixel(int x, int y)
        {
            Color c = canvaDirect.GetPixel(x, y);
            int Y = (int)(0.299 * (double)c.R + 0.587 * (double)c.G + 0.114 * (double)c.B);
            Color C = Color.FromArgb(Y, Y, Y);
            canva1Direct.SetPixel(x, y, C);
        }

        private void SetHPixel(int x, int y)
        {
            Color c = canvaDirect.GetPixel(x, y);
            Color C = Color.FromArgb(0, 0, 0);
            double H = 0;
            double R = (double)c.R / 255;
            double G = (double)c.G / 255;
            double B = (double)c.B / 255;
            double M = Math.Max(R, Math.Max(G, B));
            double m = Math.Min(R, Math.Min(G, B));
            double range = M - m;
            if (range == 0)
            {
                canva1Direct.SetPixel(x, y, C);
                return;
            }
            if (M == R) H = ((G - B) / range + 6) % 6;
            if (M == G) H = ((B - R) / range + 2) % 6;
            if (M == B) H = ((R - G) / range + 4) % 6;
            int X = (int)(H * 255 / 6);
            X = Math.Max(0, X);
            X = Math.Min(255, X);
            C = Color.FromArgb(X, X, X);
            canva1Direct.SetPixel(x, y, C);
        }

        private void SetLPixel(int x, int y)
        {
            double L = 116 * Math.Cbrt(LabSettings.picture[x, y, 1] / 100) - 16;
            if (LabSettings.picture[x, y, 1] / 100 <= 0.008856)
                L = 903.3 * LabSettings.picture[x, y, 1] / 100;
            int X = (int)(L * 255 / 150);
            X = Math.Max(0, X);
            X = Math.Min(255, X);
            Color C = Color.FromArgb(X, X, X);
            canva1Direct.SetPixel(x, y, C);
        }

        private void canva2_Paint(object sender, PaintEventArgs e)
        {
            if (chooseProfile.SelectedIndex == 0)
            {
                for (int i = 0; i < canva2.Width; i++)
                {
                    for (int j = 0; j < canva2.Height; j++)
                    {
                        SetCbPixel(i, j);
                    }
                }
            }
            else if (chooseProfile.SelectedIndex == 1)
            {
                for (int i = 0; i < canva2.Width; i++)
                {
                    for (int j = 0; j < canva2.Height; j++)
                    {
                        SetSPixel(i, j);
                    }
                }
            }
            else if (chooseProfile.SelectedIndex == 2)
            {
                for (int i = 0; i < canva2.Width; i++)
                {
                    for (int j = 0; j < canva2.Height; j++)
                    {
                        SetaPixel(i, j);
                    }
                }
            }
            e.Graphics.DrawImage(canva2Direct.Bitmap, 0, 0);
        }

        private void SetCbPixel(int x, int y)
        {
            Color c = canvaDirect.GetPixel(x, y);
            /*double Cb = (-0.169 * (double)c.R - 0.331 * (double)c.G + 0.5 * (double)c.B) + 128;
            int X = (int)Cb;*/
            int Y = (int)(0.299 * (double)c.R + 0.587 * (double)c.G + 0.114 * (double)c.B);
            double Cb = ((double)c.B / 255 - (double)Y / 255) / 1.772 + 0.5;
            int X = (int)(Cb * 255);
            X = Math.Max(0, X);
            X = Math.Min(255, X);
            Color C = Color.FromArgb(127, 255 - X, X);
            canva2Direct.SetPixel(x, y, C);
        }

        private void SetSPixel(int x, int y)
        {
            Color c = canvaDirect.GetPixel(x, y);
            if (Math.Max(c.R, Math.Max(c.G, c.B)) == 0)
            {
                canva2Direct.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                return;
            }
            double S = (double)(Math.Max(c.R, Math.Max(c.G, c.B)) - Math.Min(c.R, Math.Min(c.G, c.B)))
                / (double)Math.Max(c.R, Math.Max(c.G, c.B));
            Color C = Color.FromArgb((int)(S * 255), (int)(S * 255), (int)(S * 255));
            canva2Direct.SetPixel(x, y, C);
        }

        private void SetaPixel(int x, int y)
        {
            double a = 500 * (Math.Cbrt(LabSettings.picture[x, y, 0] / (LabSettings.whiteX / LabSettings.whiteY * 100))
                - Math.Cbrt(LabSettings.picture[x, y, 1] / 100));
            int X = (int)(a + 127);
            X = Math.Max(0, X);
            X = Math.Min(255, X);
            Color C = Color.FromArgb(X, 255 - X, 127);
            canva2Direct.SetPixel(x, y, C);
        }

        private void canva3_Paint(object sender, PaintEventArgs e)
        {
            if (chooseProfile.SelectedIndex == 0)
            {
                for (int i = 0; i < canva3.Width; i++)
                {
                    for (int j = 0; j < canva3.Height; j++)
                    {
                        SetCrPixel(i, j);
                    }
                }
            }
            else if (chooseProfile.SelectedIndex == 1)
            {
                for (int i = 0; i < canva3.Width; i++)
                {
                    for (int j = 0; j < canva3.Height; j++)
                    {
                        SetVPixel(i, j);
                    }
                }
            }
            else if (chooseProfile.SelectedIndex == 2)
            {
                for (int i = 0; i < canva3.Width; i++)
                {
                    for (int j = 0; j < canva3.Height; j++)
                    {
                        SetbPixel(i, j);
                    }
                }
            }
            e.Graphics.DrawImage(canva3Direct.Bitmap, 0, 0);
        }

        private void SetCrPixel(int x, int y)
        {
            Color c = canvaDirect.GetPixel(x, y);
            /*double Cr = (int)(0.5 * (double)c.R - 0.419 * (double)c.G - 0.081 * (double)c.B) + 128;
            int X = (int)Cr;*/
            int Y = (int)(0.299 * (double)c.R + 0.587 * (double)c.G + 0.114 * (double)c.B);
            double Cr = ((double)c.R / 255 - (double)Y / 255) / 1.402 + 0.5;
            int X = (int)(Cr * 255);
            X = Math.Max(0, X);
            X = Math.Min(255, X);
            Color C = Color.FromArgb(X, 255 - X, 127);
            canva3Direct.SetPixel(x, y, C);
        }

        private void SetVPixel(int x, int y)
        {
            Color c = canvaDirect.GetPixel(x, y);
            int S = Math.Max(c.R, Math.Max(c.G, c.B));
            Color C = Color.FromArgb(S, S, S);
            canva3Direct.SetPixel(x, y, C);
        }

        private void SetbPixel(int x, int y)
        {
            double b = 200 * (Math.Cbrt(LabSettings.picture[x, y, 1] / 100)
                - Math.Cbrt(LabSettings.picture[x, y, 2] / ((1 - LabSettings.whiteX - LabSettings.whiteY) / LabSettings.whiteY * 100)));
            int X = (int)(b + 127);
            X = Math.Max(0, X);
            X = Math.Min(255, X);
            Color C = Color.FromArgb(X, 127, 255 - X);
            canva3Direct.SetPixel(x, y, C);
        }

        private double Calculate(double t)
        {
            if (t > 0.008856) return Math.Pow(t, 1 / 3);
            else return t / (3 * 0.008856 * 0.008856) + 4 / 29;
        }

        private void CalculateXYZ()
        {
            for (int i = 0; i < canva1.Width; i++)
            {
                for (int j = 0; j < canva1.Height; j++)
                {
                    double[] XYZ = RBGtoXYZ(canvaDirect.GetPixel(i, j));
                    LabSettings.picture[i, j, 0] = XYZ[0];
                    LabSettings.picture[i, j, 1] = XYZ[1];
                    LabSettings.picture[i, j, 2] = XYZ[2];
                }
            }
            canva1.Refresh();
            canva2.Refresh();
            canva3.Refresh();
        }

        private double[] RBGtoXYZ(Color c)
        {
            double R = (double)c.R / 255;
            double G = (double)c.G / 255;
            double B = (double)c.B / 255;
            R = Math.Pow(R, LabSettings.gamma);
            G = Math.Pow(G, LabSettings.gamma);
            B = Math.Pow(B, LabSettings.gamma);
            /*double[,] a = new double[3, 3]
            {
                { LabSettings.redX / LabSettings.redY, LabSettings.greenX / LabSettings.greenY, LabSettings.blueX / LabSettings.blueY },
                { 1, 1, 1 },
                { (1 - LabSettings.redX - LabSettings.redY) / LabSettings.redY,
                    (1 - LabSettings.greenX - LabSettings.greenY) / LabSettings.greenY,
                    (1 - LabSettings.blueX - LabSettings.blueY) / LabSettings.blueY }
            };*/
            double[,] a = new double[3, 3]
            {
                { LabSettings.redX, LabSettings.greenX, LabSettings.blueX },
                { LabSettings.redY, LabSettings.greenY, LabSettings.blueY },
                { 1 - LabSettings.redX - LabSettings.redY, 1 - LabSettings.greenX - LabSettings.greenY, 
                    1 - LabSettings.blueX - LabSettings.blueY }
            };
            double[,] A = Inverse3x3Matrix(a);
            double[] white = new double[3]
            { LabSettings.whiteX / LabSettings.whiteY, 1, 
                (1 - LabSettings.whiteX - LabSettings.whiteY) / LabSettings.whiteY };
            double[] S = MultiplyMatrix3x3AndVector3(A, white);
            double[,] M = new double[3, 3]
            {
                { S[0] * LabSettings.redX, S[1] * LabSettings.greenX, S[2] * LabSettings.blueX },
                { S[0] * LabSettings.redY, S[1] * LabSettings.greenY, S[2] * LabSettings.blueY },
                { S[0] * (1 - LabSettings.redX - LabSettings.redY),
                    S[1] * (1 - LabSettings.greenX - LabSettings.greenY),
                    S[2] * (1 - LabSettings.blueX - LabSettings.blueY) }
            };
            double[] RGB = new double[3] { c.R, c.G, c.B };
            double[] XYZ = MultiplyMatrix3x3AndVector3(M, RGB);
            return XYZ;
        }

        private void changeImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                string p = Application.ExecutablePath;
                //fileDialog.InitialDirectory = p.Substring(0, p.LastIndexOf(@"\")) + @"\images";
                fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    image = new Bitmap(fileDialog.FileName);
                    originalCanva.Image = new Bitmap(image, originalCanva.Width, originalCanva.Height);
                    canvaImage = new Bitmap(image, canva1.Width, canva1.Height);
                    for (int i = 0; i < canva1.Width; i++)
                    {
                        for (int j = 0; j < canva1.Height; j++)
                        {
                            canvaDirect.SetPixel(i, j, canvaImage.GetPixel(i, j));
                        }
                    }
                    CalculateXYZ();
                    canva1.Refresh();
                    canva2.Refresh();
                    canva3.Refresh();
                }
            }
        }

        private void colorProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (colorProfileComboBox.SelectedIndex == 3) ChangedColorProfile(true);
            else ChangedColorProfile(false);
            if (colorProfileComboBox.SelectedIndex == 0)
            {
                ResetMaximums();
                redXfield.Value = 0.64M;
                redYfield.Value = 0.33M;
                greenXfield.Value = 0.3M;
                greenYfield.Value = 0.6M;
                blueXfield.Value = 0.15M;
                blueYfield.Value = 0.06M;
                gammaField.Value = 2.2M;
                illuminantComboBox.SelectedIndex = 0;
                SetLabSettings();
            }
            if (colorProfileComboBox.SelectedIndex == 1)
            {
                ResetMaximums();
                redXfield.Value = 0.64M;
                redYfield.Value = 0.33M;
                greenXfield.Value = 0.21M;
                greenYfield.Value = 0.71M;
                blueXfield.Value = 0.15M;
                blueYfield.Value = 0.06M;
                gammaField.Value = 2.2M;
                illuminantComboBox.SelectedIndex = 0;
                SetLabSettings();
            }
            if (colorProfileComboBox.SelectedIndex == 2)
            {
                ResetMaximums();
                redXfield.Value = 0.734699M;
                redYfield.Value = 0.2653M;
                greenXfield.Value = 0.1152M;
                greenYfield.Value = 0.8264M;
                blueXfield.Value = 0.1566M;
                blueYfield.Value = 0.0177M;
                gammaField.Value = 1.2M;
                illuminantComboBox.SelectedIndex = 1;
                SetLabSettings();
            }
        }

        private void ResetMaximums()
        {
            redYfield.Maximum = 0.999999M;
            redXfield.Maximum = 0.999999M;
            greenYfield.Maximum = 0.999999M;
            greenXfield.Maximum = 0.999999M;
            blueYfield.Maximum = 0.999999M;
            blueXfield.Maximum = 0.999999M;
            whiteXfield.Maximum = 0.999999M;
            whiteYfield.Maximum = 0.999999M;
        }

        private void ChangedColorProfile(bool b)
        {
            illuminantComboBox.Enabled = b;
            redXfield.Enabled = b;
            redYfield.Enabled = b;
            greenXfield.Enabled = b;
            greenYfield.Enabled = b;
            blueXfield.Enabled = b;
            blueYfield.Enabled = b;
            gammaField.Enabled = b;
            saveLabButton.Enabled = b;
        }

        private void illuminantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (illuminantComboBox.SelectedIndex == 4) ChangedIlluminant(true);
            else ChangedIlluminant(false);
            if (illuminantComboBox.SelectedIndex == 0)
            {
                whiteXfield.Value = 0.312730M;
                whiteYfield.Value = 0.329020M;
                SetLabSettings();
            }
            if (illuminantComboBox.SelectedIndex == 1)
            {
                whiteXfield.Value = 0.345670M;
                whiteYfield.Value = 0.358500M;
                SetLabSettings();
            }
            if (illuminantComboBox.SelectedIndex == 2)
            {
                whiteXfield.Value = 0.447570M;
                whiteYfield.Value = 0.407440M;
                SetLabSettings();
            }
            if (illuminantComboBox.SelectedIndex == 3)
            {
                whiteXfield.Value = 0.348400M;
                whiteYfield.Value = 0.351600M;
                SetLabSettings();
            }
        }

        private void ChangedIlluminant(bool b)
        {
            whiteXfield.Enabled = b;
            whiteYfield.Enabled = b;
        }

        private void SetLabSettings()
        {
            LabSettings.redX = (double)redXfield.Value;
            LabSettings.redY = (double)redYfield.Value;
            LabSettings.greenX = (double)greenXfield.Value;
            LabSettings.greenY = (double)greenYfield.Value;
            LabSettings.blueX = (double)blueXfield.Value;
            LabSettings.blueY = (double)blueYfield.Value;
            LabSettings.whiteX = (double)whiteXfield.Value;
            LabSettings.whiteY = (double)whiteYfield.Value;
            LabSettings.gamma = (double)gammaField.Value;
            canva1.Refresh();
            canva2.Refresh();
            canva3.Refresh();
        }

        private void redXfield_ValueChanged(object sender, EventArgs e)
        {
            redYfield.Maximum = 0.999999M - redXfield.Value;
        }

        private void redYfield_ValueChanged(object sender, EventArgs e)
        {
            redXfield.Maximum = 0.999999M - redYfield.Value;
        }

        private void greenXfield_ValueChanged(object sender, EventArgs e)
        {
            greenYfield.Maximum = 0.999999M - greenXfield.Value;
        }

        private void greenYfield_ValueChanged(object sender, EventArgs e)
        {
            greenXfield.Maximum = 0.999999M - greenYfield.Value;
        }

        private void blueXfield_ValueChanged(object sender, EventArgs e)
        {
            blueYfield.Maximum = 0.999999M - blueXfield.Value;
        }

        private void blueYfield_ValueChanged(object sender, EventArgs e)
        {
            blueXfield.Maximum = 0.999999M - blueYfield.Value;
        }

        private void whiteXfield_ValueChanged(object sender, EventArgs e)
        {
            whiteYfield.Maximum = 0.999999M - whiteXfield.Value;
        }

        private void whiteYfield_ValueChanged(object sender, EventArgs e)
        {
            whiteXfield.Maximum = 0.999999M - whiteYfield.Value;
        }

        private void saveLabButton_Click(object sender, EventArgs e)
        {
            SetLabSettings();
        }


        private double[,] Inverse3x3Matrix(double [,] a)
        {
            double[,] i = new double[a.GetLength(0), a.GetLength(1)];

            i[0, 0] = 1;
            i[1, 1] = 1;
            i[2, 2] = 1;

            double f = a[1, 0] / a[0, 0];

            a[1, 0] = a[1, 0] - a[0, 0] * f;
            a[1, 1] = a[1, 1] - a[0, 1] * f;
            a[1, 2] = a[1, 2] - a[0, 2] * f;

            i[1, 0] = i[1, 0] - i[0, 0] * f;

            double e = a[2, 0] / a[0, 0];

            a[2, 0] = a[2, 0] - a[0, 0] * e;
            a[2, 1] = a[2, 1] - a[0, 1] * e;
            a[2, 2] = a[2, 2] - a[0, 2] * e;

            i[2, 0] = i[2, 0] - i[0, 0] * e;

            double k = a[2, 1] / a[1, 1];

            a[2, 1] = a[2, 1] - a[1, 1] * k;
            a[2, 2] = a[2, 2] - a[1, 2] * k;

            i[2, 0] = i[2, 0] - i[1, 0] * k;
            i[2, 1] = i[2, 1] - i[1, 1] * k;

            double n = a[1, 2] / a[2, 2];

            a[1, 2] = a[1, 2] - a[2, 2] * n;

            i[1, 0] = i[1, 0] - i[2, 0] * n;
            i[1, 1] = i[1, 1] - i[2, 1] * n;
            i[1, 2] = i[1, 2] - i[2, 2] * n;

            double o = a[0, 2] / a[2, 2];

            a[0, 2] = a[0, 2] - a[2, 2] * o;

            i[0, 0] = i[0, 0] - i[2, 0] * o;
            i[0, 1] = i[0, 1] - i[2, 1] * o;
            i[0, 2] = i[0, 2] - i[2, 2] * o;

            double m = a[0, 1] / a[1, 1];

            a[0, 1] = a[0, 1] - a[1, 1] * m;

            i[0, 0] = i[0, 0] - i[1, 0] * m;
            i[0, 1] = i[0, 1] - i[1, 1] * m;
            i[0, 2] = i[0, 2] - i[1, 2] * m;

            i[0, 0] /= a[0, 0];
            i[0, 1] /= a[0, 0];
            i[0, 2] /= a[0, 0];

            i[1, 0] /= a[1, 1];
            i[1, 1] /= a[1, 1];
            i[1, 2] /= a[1, 1];

            i[2, 0] /= a[2, 2];
            i[2, 1] /= a[2, 2];
            i[2, 2] /= a[2, 2];

            return i;
        }

        private double[] MultiplyMatrix3x3AndVector3(double [,] a, double [] v)
        {
            if (a.GetLength(0) != a.GetLength(1) || a.GetLength(1) != v.Length) return null;
            double[] r = new double[v.Length];
            for (int i = 0; i < r.Length; i++)
            {
                for (int j = 0; j < r.Length; j++)
                    r[i] += a[i, j] * v[j];
            }
            return r;
        }

        private void buttonReduceColors_Click(object sender, EventArgs e)
        {
            int K = (int)kField.Value;
            ReduceColors(K);
        }

        private void ReduceColors(int K)
        {
            int[,,] colorCount = new int[256, 256, 256];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color c = image.GetPixel(x, y);
                    colorCount[c.R, c.G, c.B]++;
                }
            }
            Color[] chosenColor = new Color[K];
            for (int i = 0; i < K; i++)
            {
                var idx = (0, 0, 0);
                int max = 0;
                for (int r = 0; r < 256; r++)
                {
                    for (int g = 0; g < 256; g++)
                    {
                        for (int b = 0; b < 256; b++)
                        {
                            if (colorCount[r, g, b] > max)
                            {
                                max = colorCount[r, g, b];
                                idx = (r, g, b);
                            }
                        }
                    }
                }
                chosenColor[i] = Color.FromArgb(idx.Item1, idx.Item2, idx.Item3);
                colorCount[idx.Item1, idx.Item2, idx.Item3] = 0;
            }
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color c = image.GetPixel(x, y);
                    int idxK = 0;
                    int min = int.MaxValue;
                    for (int i = 0; i < K; i++)
                    {
                        int d = (chosenColor[i].R - c.R) * (chosenColor[i].R - c.R) + (chosenColor[i].G - c.G) * (chosenColor[i].G - c.G) +
                            (chosenColor[i].B - c.B) * (chosenColor[i].B - c.B);
                        if (d < min)
                        {
                            min = d;
                            idxK = i;
                        }
                    }
                    image.SetPixel(x, y, chosenColor[idxK]);
                }
            }
            SetCanvas();
            CalculateXYZ();
            canva1.Refresh();
            canva2.Refresh();
            canva3.Refresh();
            originalCanva.Refresh();
        }
    }
}
