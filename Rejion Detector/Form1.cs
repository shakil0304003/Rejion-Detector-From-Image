using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rejion_Detector
{
    public partial class Form1 : Form
    {
        private string _sourcePath = "";
        private Bitmap btp;
        private Bitmap destinationBtp;
        private Color color;
        private int _width = 0, _height = 0;
        private Color _destinationColor = Color.Black;
        private int _maxDif = 150;
        private int _initDif = 150;
        private int _region;
        private int _minSize = 0;
        private List<Color> colors = new List<Color>();
        bool[][] _flag;
        List<List<Point>> _allPoints = new List<List<Point>>();

        public Form1()
        {
            colors.Add(Color.Black);
            colors.Add(Color.Yellow);
            colors.Add(Color.Green);
            colors.Add(Color.Red);
            colors.Add(Color.Blue);
            colors.Add(Color.Chocolate);
            colors.Add(Color.DarkBlue);
            colors.Add(Color.DarkGreen);
            colors.Add(Color.DarkOrange);
            colors.Add(Color.DarkRed);
            colors.Add(Color.DarkViolet);
            colors.Add(Color.ForestGreen);
            colors.Add(Color.Gold);
            colors.Add(Color.LightPink);
            colors.Add(Color.LightSteelBlue);
            colors.Add(Color.Pink);
            colors.Add(Color.Purple);
            colors.Add(Color.Silver);
            colors.Add(Color.PaleVioletRed);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            _sourcePath = openFileDialog1.FileName;
            pictureBox1.Image = new Bitmap(_sourcePath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btp = new Bitmap(pictureBox1.Image);
            _minSize = Convert.ToInt16(textBox2.Text);
            _width = btp.Width;
            _height = btp.Height;
            color = ColorTranslator.FromHtml(textBox1.Text);
            Convert11();
            pictureBox1.Image = destinationBtp;
        }

        private void Convert11()
        {
            _region = 0;

            _flag = new bool[_height][];
            destinationBtp = new Bitmap(_width, _height);

            for (int i = 0; i < _height; i++)
            {
                _flag[i] = new bool[_width];
                for (int j = 0; j < _width; j++)
                {
                    _flag[i][j] = false;
                    destinationBtp.SetPixel(j, i, Color.White);
                }
            }

            _allPoints = new List<List<Point>>();

            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    if (_flag[i][j] == false)
                    {
                        Color currentColor = btp.GetPixel(j, i);
                        int diff = Math.Abs(currentColor.R - color.R) + Math.Abs(currentColor.G - color.G) + Math.Abs(currentColor.B - color.B);

                        if (diff <= _initDif)
                        {
                            BFS(j, i);
                        }
                    }
            MessageBox.Show(_region.ToString());

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Select");

            for (int i = 0; i < _allPoints.Count; i++)
            {
                comboBox1.Items.Add((i + 1).ToString());
            }
        }


        private void BFS(int x, int y)
        {
            List<Point> points = new List<Point>();

            _destinationColor = colors[_region % colors.Count];

            points.Add(new Point(x, y));
            _flag[y][x] = true;

            int current = 0;

            while (current < points.Count)
            {
                x = points[current].X;
                y = points[current].Y;
                Color cc = color; //btp.GetPixel(x, y);

                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (x + j >= 0 && x + j < _width && y + i >= 0 && y + i < _height && _flag[y + i][x + j] == false)
                        {
                            Color currentColor = btp.GetPixel(x + j, y + i);

                            if (!(currentColor.R == Color.White.R && currentColor.G == Color.White.G && currentColor.B == Color.White.B))
                            {
                                int diff = Math.Abs(currentColor.R - cc.R) + Math.Abs(currentColor.G - cc.G) + Math.Abs(currentColor.B - cc.B);
                                if (diff <= _maxDif)
                                {
                                    _flag[y + i][x + j] = true;
                                    points.Add(new Point(x + j, y + i));
                                }
                            }
                        }

                current++;
            }

            if (current >= _minSize)
            {
                for (int i = 0; i < current; i++)
                {
                    destinationBtp.SetPixel(points[i].X, points[i].Y, _destinationColor);
                }

                _allPoints.Add(points);
                _region++;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select = comboBox1.SelectedIndex - 1;

            if (select >= 0)
            {
                for (int i = 0; i < _height; i++)
                {
                    for (int j = 0; j < _width; j++)
                    {
                        btp.SetPixel(j, i, Color.White);
                    }
                }

                List<Point> points = new List<Point>();

                for (int i = 0; i < _allPoints[select].Count; i++)
                {
                    if (checkBox1.Checked)
                        btp.SetPixel(_allPoints[select][i].X, _allPoints[select][i].Y, Color.Red);

                    int count = 0;

                    for (int i1 = -1; i1 <= 1; i1++)
                        for (int j1 = -1; j1 <= 1; j1++)
                            if (_allPoints[select][i].X + j1 >= 0 && _allPoints[select][i].X + j1 < _width && _allPoints[select][i].Y + i1 >= 0 && _allPoints[select][i].Y + i1 < _height)
                            {
                                if (_allPoints[select].Contains(new Point(_allPoints[select][i].X + j1, _allPoints[select][i].Y + i1)))
                                    count++;
                            }

                    if (count < 9)
                        points.Add(new Point(_allPoints[select][i].X, _allPoints[select][i].Y));
                }

                listBox1.Items.Clear();

                for (int i = 0; i < points.Count; i++)
                {
                    if (!checkBox1.Checked)
                        btp.SetPixel(points[i].X, points[i].Y, Color.Red);
                    listBox1.Items.Add(points[i].X + "," + points[i].Y);
                }

                lblPoints.Text = listBox1.Items.Count.ToString();
                pictureBox1.Image = btp;
            }
            else
            {
                pictureBox1.Image = destinationBtp;
            }
        }
    }
}
