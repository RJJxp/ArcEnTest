using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// drawing part
using System.Drawing.Drawing2D;


using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using System.Threading;

namespace ArcEnTest
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // 创建表
            DataTable pTable = new DataTable();
            // 创建列
            pTable.Columns.Add("Width");
            pTable.Columns.Add("Area");
            pTable.Columns.Add("Cost");
            pTable.Columns.Add("Value");
            // 为列添加
            int countTimes = Form3.Wmax - Form3.Wmin + 1;
            for (int i = 0; i < countTimes; i++)
            { 
                DataRow row = pTable.NewRow();
                row[0] = Form3.Wmin + i;
                row[1] = Form3.myArea[i];
                row[2] = Form3.myCost[i];
                row[3] = Form3.myValue[i];
                pTable.Rows.Add(row);
            }
            dataGridView1.DataSource = pTable;


            // combobox
            comboBox1.Items.Add("Area");
            comboBox1.Items.Add("Cost");
            comboBox1.Items.Add("Value");

            
        }

        double[] drawData;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            Graphics g;
            g = e.Graphics;
            g.Clear(Color.White);
            if (comboBox1.SelectedItem == null)
                return;

            int w = pictureBox1.Width;
            int h = pictureBox1.Height;

            Pen myPen = new Pen(Color.Blue, 1);
            myPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            System.Drawing.Point oriPoint = new System.Drawing.Point((int)(0.05 * w), (int)(0.9 * h));
            
            System.Drawing.Point p1 = new System.Drawing.Point((int)(0.05 * w), (int)(0.1 * h));
            g.DrawLine(myPen, oriPoint, p1);
            
            p1.X = (int)(0.95 * w);p1.Y = (int)(0.9 * h);
            g.DrawLine(myPen, oriPoint,p1);

            int drawH = (int)(h * 0.8);
            int drawW = (int)(w * 0.9);
            int count = Form3.Wmax - Form3.Wmin + 1;

            // get the max and min of the drawData
            double drawData_max = -1;
            double drawData_min = -1;
            for(int i=0;i<count;i++)
            {
                if(i == 0)
                {
                    drawData_max = drawData[i];
                    drawData_min = drawData[i];
                }
                else
                {
                    if(drawData[i] > drawData_max)
                    {
                         drawData_max = drawData[i];
                    }
                    if(drawData[i] < drawData_min)
                    {
                        drawData_min = drawData[i];
                    }   
                }
            }

            double drawData_range = drawData_max - drawData_min;
            double drawScale = drawH * 0.8 / drawData_range;
            double startH = oriPoint.Y - drawH * 0.1;
            double startW = oriPoint.X;
            float[] yyy = new float[count];
            for(int i = 0 ;i<count;i++)
            {
                yyy[i] = (float)(startH - (drawData[i] - drawData_min) * drawScale);
            }
            float[] xxx = new float[count];
            for(int i = 0;i<count;i++)
            {
                xxx[i] = (float)(startW + drawW*(i+1) / (count + 1));
            }
            // drawLines
            Pen drawPen = new Pen(Color.Red,2);
            for (int i = 0; i < count - 1; i++)
            { 
                PointF pp1 = new PointF(xxx[i],yyy[i]);
                PointF pp2 = new PointF(xxx[i+1],yyy[i+1]);
                g.DrawLine(drawPen,pp1,pp2);
            }


            // drawText
            if(comboBox1.SelectedIndex == 2)
            {
                int ValueMinIndex = -1;
                float ValueMin = -1;
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        ValueMin = (float)Form3.myValue[0];
                        ValueMinIndex = 0;
                    }
                    else
                    {
                        if ((float)Form3.myValue[i] < ValueMin)
                        {
                            ValueMin = (float)Form3.myValue[i];
                            ValueMinIndex = i;
                        }
                    }
                }
                drawPen.Color = Color.Green;
                drawPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                PointF ppp1 = new PointF(xxx[ValueMinIndex],yyy[ValueMinIndex]);
                PointF ppp2 = new PointF(xxx[ValueMinIndex],oriPoint.Y);
                g.DrawLine(drawPen,ppp1,ppp2);

                string str = (Form3.Wmin + ValueMinIndex).ToString();
                Font font = new Font("宋体", 10f); 
                Brush brush = Brushes.Red;
                PointF point = new PointF(xxx[ValueMinIndex]-2, oriPoint.Y+5); 
                System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
                g.DrawString(str, font, brush, point, sf);

                for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
                {
                    //MessageBox.Show(i.ToString());
                    //MessageBox.Show(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    if ((ValueMinIndex+Form3.Wmin).ToString() == 
                        dataGridView1.Rows[i].Cells[0].Value.ToString())
                    {
                        dataGridView1.CurrentCell = dataGridView1[0,i];
                    }
                }
            }
            

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                drawData = Form3.myArea;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                drawData = Form3.myCost;
            }
            else
            {
                drawData = Form3.myValue;
            }
            pictureBox1.Refresh();
        }
    }
}
