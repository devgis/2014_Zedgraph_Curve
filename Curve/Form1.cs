using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Threading;

namespace Curve
{
    public partial class Form1 : Form
    {
        int xMax = 150;//x轴总共显示的点数
        int xSpace = 10;//x空出的点数
        int yMax = 50;
        int yMin = -50;
        int RefreshTime = 500; //刷新时间

        public Form1()
        {
            InitializeComponent();
        }
        public delegate void doDarw();
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = RefreshTime;
            DrawLine();
            timer1.Start();

            System.Windows.Forms.Timer t2 = new System.Windows.Forms.Timer();
            t2.Interval = 1;
            t2.Tick += new EventHandler(t2_Tick);
            t2.Start();
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = "参数1";
            dataGridView1.Rows[0].Cells[1].Value = "Parm1";
            dataGridView1.Rows[0].Cells[4].Value = double.MaxValue;
            dataGridView1.Rows[0].Cells[5].Value = double.MinValue;
        }

        void t2_Tick(object sender, EventArgs e)
        {
            if (bDraw)
            {
                DrawCross();
            }
        }
        LineItem myCurve;
        GraphPane myPane;
        private void DrawLine()
        {
            PointPairList list = new PointPairList();
            myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "二次曲线测试";
            myPane.Title.FontSpec.Size = 10;
            myPane.XAxis.Scale.BaseTic = 50;
            myPane.YAxis.Title.Text = "工程值";
            myPane.YAxis.Title.FontSpec.Size = 8;
            myPane.XAxis.Title.Text = "时间";
            myPane.XAxis.Title.FontSpec.Size = 8;
            myPane.XAxis.Type = AxisType.DateAsOrdinal;
            myPane.XAxis.CrossAuto = true;
            //myPane.XAxis.Scale.Max = 50;
            myPane.XAxis.Scale.Max = xMax;
            myPane.XAxis.Scale.FontSpec.Size = 5;
            myPane.YAxis.Scale.Max = yMax;
            myPane.YAxis.Scale.Min = yMin;
            myPane.YAxis.Scale.FontSpec.Size = 5;

            myPane.Legend.Border.Color = Color.DarkGray; //去掉图例边框
            myPane.Legend.FontSpec.Size = 8; //图例字体大小

            myPane.Legend.Position = ZedGraph.LegendPos.InsideTopLeft;
            myPane.Legend.Location = new ZedGraph.Location(50, 50, CoordType.AxisXYScale);
            
            //myPane.XAxis.Scale.Min = 500;
            myPane.XAxis.Scale.Format = "hh:mm:ss";
            myCurve = myPane.AddCurve("参数1", list, Color.Red, SymbolType.None);
            
            myCurve.Line.IsAntiAlias = true;
            myCurve.Line.Width = 1.5F;
            myCurve.Symbol.Fill = new Fill(Color.White);
            myCurve.Symbol.Size = 5;
        }
        int i = 0;
        int iPos = 0;
        Random rd = new Random();
        private void timer1_Tick(object sender, EventArgs e)
        {
            double x = (double)new XDate(DateTime.Now);
            double y = Math.Sin(i * Math.PI / 15.0) * 16.0;

            dataGridView1.Rows[0].Cells[2].Value = y*rd.NextDouble();
            dataGridView1.Rows[0].Cells[3].Value = y;

            if (y < Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value))
            {
                dataGridView1.Rows[0].Cells[4].Value = y;
            }
            if (y> Convert.ToDouble(dataGridView1.Rows[0].Cells[5].Value))
            {
                dataGridView1.Rows[0].Cells[5].Value = y;
            }
            //if (myCurve.Points.Count > 100)
            //{
            //    myCurve.RemovePoint(0);
            //}
            myCurve.AddPoint(x, y);

            //double x1=0, y1=0;
            myPane.ReverseTransform(new PointF((float)myPane.XAxis.Scale.Min,0f), out x, out y);
            //XDate xd = new XDate(x1);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            //myPane.GetImage(true).Save(); //调用保存接口
            i++;
            
            //radTrackBar1.Value = 0.5f * i;
            radTrackBar1.Maximum = i;
            //radTrackBar1.Value = i;
            radTrackBar1.Ranges.Maximum =i;
            this.radTrackBar1.Ranges[0].End = i;
            try
            {
                
                //radTrackBar1.Ranges.Maximum = 0.5f * i;
                //radTrackBar1.Ranges.Maximum = 0.5f * i;
                if (i > (xMax - xSpace))
                {
                    this.radTrackBar1.Ranges[0].Start = i - xMax + xSpace;
                }
                else
                {
                    this.radTrackBar1.Ranges[0].Start = 0;
                }
                
                
            }
            catch
            { }
            if (iPos < xMax - xSpace)
            {
                iPos++;
            }
            else
            {
                myPane.XAxis.Scale.Min++;
                myPane.XAxis.Scale.Max++;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("暂停(&S)"))
            {
                button1.Text = "开始(&S)";
                toolTip1.SetToolTip(button1, "开始监视");
                timer1.Stop();
            }
            else
            {
                button1.Text = "暂停(&S)";
                toolTip1.SetToolTip(button1,"暂停并锁定监视");
                timer1.Start();
            }
        }

        private void zedGraphControl1_MouseMove(object sender, MouseEventArgs e)
        {
            #region[曲线显示值例子]
            PointF mousePt = new PointF(e.X, e.Y);
            GraphPane mypane = ((ZedGraphControl)sender).MasterPane.FindChartRect(mousePt);
            //StockPointList stockPointList = new StockPointList(stockChart.stockList);


            if (point_x != MousePosition.X || point_y != MousePosition.Y)
            {

                if (mypane != null)
                {

                    point_x = MousePosition.X;
                    point_y = MousePosition.Y;
                    double x, y;
                    //StockPt mouseStock;
                    mypane.ReverseTransform(mousePt, out x, out y);
                    //mouseStock = stockPointList.GetAt((int)x - 1);
                    //toolTip1.SetToolTip(zedGraphControl1, "Open=" + mouseStock.Open.ToString("f2") + " \n High=" + mouseStock.High.ToString("f2") + "\n Low=" + mouseStock.Low.ToString("f2") + "\n Close=" + mouseStock.Close.ToString("f2"));



                }
                zedGraphControl1.Refresh();
                currentPoint = new Point(e.X, e.Y);

                DrawCross();
            }


            #endregion

            //// Save the mouse location  
            //PointF mousePt = new PointF(e.X, e.Y);
            //dx = e.X;
            //string InfoString = string.Empty;

            //// Find the Chart rect that contains the current mouse location  
            //GraphPane pane = ((ZedGraphControl)sender).MasterPane.FindChartRect(mousePt);

            //// If pane is non-null, we have a valid location.  Otherwise, the mouse is not  
            //// within any chart rect.  
            //if (pane != null)
            //{
            //    double x, y;
            //    // Convert the mouse location to X, and Y scale values  
            //    pane.ReverseTransform(mousePt, out x, out y);
            //    // 获取横纵坐标信息  
            //    //x=myPane.XAxis.
            //    //XDate xd = new XDate(x);
            //    //DateTime date = xd.DateTime;
            //    //InfoString = xd.ToString();
            //}
        }


        private void DrowShuXian(double x)
        {
            //zedGraphControl1.getd
        }

        //Pen pPen;
        //SolidBrush mysbrush = new SolidBrush(Color.Red);
        public float point_x = 0.0F;
        public float point_y = 0.0F;
        private Point currentPoint = new Point(0, 0);    
        private void zedGraphControl1_Paint(object sender, PaintEventArgs e)
        {
            if (bDraw)
            {
                DrawCross();
            }
            //Graphics g = e.Graphics;
            //pPen = new Pen(mysbrush);
            //g.DrawLine(pPen, new Point(100,100), new Point(100, 0));
            //g.DrawLine(pPen, new Point(dx, zedGraphControl1.Height), new Point(dx, 0));
        }

        bool bDraw = false;
        private void zedGraphControl1_MouseEnter(object sender, EventArgs e)
        {
            bDraw = true;
        }

        private void zedGraphControl1_MouseLeave(object sender, EventArgs e)
        {
            bDraw = false;
        }

        //绘制用于方便查看刻度的十字线
        private void DrawCross()
        {
            Graphics g = zedGraphControl1.CreateGraphics();
            g.DrawLine(Pens.Green, 0, currentPoint.Y, this.Width, currentPoint.Y); //绘制横线  
            g.DrawLine(Pens.Green, currentPoint.X, 0, currentPoint.X, this.Height); //会制纵线 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text.Equals("打开(&O)"))
            {
                button2.Text = "关闭(&C)";
                toolTip1.SetToolTip(button2, "关闭测点表");
                dataGridView1.Visible = true;
            }
            else
            {
                button2.Text = "打开(&O)";
                toolTip1.SetToolTip(button2, "打开测点表");
                dataGridView1.Visible = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                myPane.YAxis.Scale.Max = yMax;
                myPane.YAxis.Scale.Min = yMin;
            }
            else
            {
                myPane.YAxis.Scale.Max = 100;
                myPane.YAxis.Scale.Min = 0;
            }
        }

    }
}
