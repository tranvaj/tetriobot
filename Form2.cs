using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tetriobrowser
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.TopMost = true;
            this.Opacity = .4;
            //this.BackColor = Color.Turquoise;

             this.TransparencyKey = BackColor;
            //initial the matrix here
            //this.Height = heightArr * size;
            this.Size = new Size(widthArr * (5*size), heightArr * 5* size);
            this.FormBorderStyle = FormBorderStyle.None;
          
            this.Paint += Draw2DArray;
        }
        public static int heightArr = 40;
        public static int size = Form1.pixelSize;
        public static int widthArr = 10;
        string[,] matrix = new string[widthArr, heightArr];
        Point point;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            point = e.Location;
            base.OnMouseDown(e);
        }
        public void setMatrix(string[,] matrif)
        {
            matrix = matrif;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - point.X;
                this.Top += e.Y - point.Y;
            }
        }

        public void Draw2DArray(object sender, PaintEventArgs e)
        {

            size = Form1.pixelSize;
            //int step = 5; //distance between the rows and columns
            // int size = 32; //the width of the rectangle
            int rowsOrHeight = matrix.GetLength(1);
            int colsOrWidth = matrix.GetLength(0);
            using (Graphics g = this.CreateGraphics())
            {
                Pen blackPen = new Pen(Color.Gray, 1);

                // Create rectangle.
                for(int i = 0; i < colsOrWidth; i++)
                {
                    for(int j = 0; j < rowsOrHeight; j++)
                    {
                        Rectangle rect = new Rectangle(i*size, j*size, size, size);
                        //g.DrawRectangle(blackPen, rect);
                        if(j == 19) g.FillRectangle(new SolidBrush(Color.Blue), rect);

                        if (!(String.IsNullOrEmpty(matrix[i, j])))
                        {
                            
                            switch(matrix[i, j])
                            {
                                case "S":
                                    g.FillRectangle(new SolidBrush(Color.Green), rect);
                                    break;
                                case "O":
                                    g.FillRectangle(new SolidBrush(Color.Yellow), rect);
                                    break;
                                case "Z":
                                    g.FillRectangle(new SolidBrush(Color.Red), rect);
                                    break;
                                case "L":
                                    g.FillRectangle(new SolidBrush(Color.Orange), rect);
                                    break;
                                case "J":
                                    g.FillRectangle(new SolidBrush(Color.Blue), rect);
                                    break;
                                case "T":
                                    g.FillRectangle(new SolidBrush(Color.Pink), rect);
                                    break;
                                case "I":
                                    g.FillRectangle(new SolidBrush(Color.Cyan), rect);
                                    break;
                                default:
                                    g.DrawRectangle(new Pen(Color.White,3), rect);
                                    break;
                                   

                            }
                        }

                    }
                }
                // Draw rectangle to screen.
                //g.DrawRectangle(blackPen, new Rectangle(200,0,size,size));
                //g.FillRectangle(blackPe)
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
