using System.Windows.Forms;
using ZedGraph;

namespace candles
{
    using System;
    using System.Drawing;
    using System.IO;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DEFINE DATA SOURCE
                string path = @"C:\dataset\DAT_ASCII_EURUSD_M1_2017.csv";
                string[] strs = File.ReadAllLines(path);
                int limit = 60; //1 hour

            //define bounds for autoscale

                //start / stop time  + delta
                DateTime min_myDateTime = (DateTime.ParseExact(strs[0].Split(';')[0].Trim('\"'), "yyyyMMdd HHmmss", null));
                DateTime max_myDateTime = (DateTime.ParseExact(strs[limit-1].Split(';')[0].Trim('\"'), "yyyyMMdd HHmmss", null));
     
                double DateTime_delta = (max_myDateTime - min_myDateTime).TotalSeconds *0.1;
                min_myDateTime = (XDate)(min_myDateTime.AddSeconds(-DateTime_delta));
                max_myDateTime = (XDate)(max_myDateTime.AddSeconds(DateTime_delta));

            //high / low price + delta
                 double min_LOW = Convert.ToDouble(strs[0].Split(';')[3].Trim('\"'));
                 double max_HIGH = Convert.ToDouble(strs[0].Split(';')[2].Trim('\"'));

                 double price_delta = max_HIGH - min_LOW;
                 min_LOW -= price_delta;
                 max_HIGH += price_delta;

     
            //LOAD DATA

            double myDateTime, HIGH, OPEN, CLOSE, LOW;
            PointPairList HighLowList = new PointPairList();

            PointPairList OpenCloseList_green = new PointPairList();
            PointPairList OpenCloseList_red = new PointPairList();

            for (int i = 0; i < limit; i++)
                {
                    myDateTime = (double)new XDate(DateTime.ParseExact(strs[i].Split(';')[0].Trim('\"'), "yyyyMMdd HHmmss", null));
                    HIGH = Convert.ToDouble(strs[i].Split(';')[2].Trim('\"'));
                    OPEN = Convert.ToDouble(strs[i].Split(';')[1].Trim('\"'));
                    CLOSE = Convert.ToDouble(strs[i].Split(';')[4].Trim('\"'));
                    LOW = Convert.ToDouble(strs[i].Split(';')[3].Trim('\"'));

                    HighLowList.Add(myDateTime, HIGH, LOW);
                  //  OpenCloseList.Add(myDateTime, OPEN, CLOSE);

                if (CLOSE > OPEN)
                // green-bullish (rise)

                {
                    OpenCloseList_green.Add(myDateTime, OPEN, CLOSE);
                }
                else

                // red - bearish (fall)
                {
                    OpenCloseList_red.Add(myDateTime, OPEN, CLOSE);
                }
            }

            GraphPane myPane = zedGraphControl1.GraphPane;

            //axis titles
            myPane.Title.Text = "Stock Chart";
            myPane.XAxis.Title.Text = "";
            myPane.YAxis.Title.Text = "EUR USD";

            //titles font
            myPane.Title.FontSpec.Family = "Arial";
            myPane.Title.FontSpec.IsItalic = true;
            myPane.Title.FontSpec.Size = 18;


            ErrorBarItem curve_green = myPane.AddErrorBar("O/C rise Price", OpenCloseList_green, Color.Green);
            curve_green.Bar.PenWidth = 5;
            curve_green.Bar.Symbol.IsVisible = false;

            ErrorBarItem curve_red = myPane.AddErrorBar("O/C fall Price", OpenCloseList_red, Color.Red);
            curve_red.Bar.PenWidth = 5;
            curve_red.Bar.Symbol.IsVisible = false;


            ErrorBarItem myCurve = myPane.AddErrorBar("H/L Range", HighLowList, Color.Blue);
            myCurve.Bar.PenWidth = 1;
            myCurve.Bar.Symbol.IsVisible = false;

            //window bounds
            myPane.YAxis.Scale.Min = min_LOW;
            myPane.YAxis.Scale.Max = max_HIGH;
            myPane.XAxis.Scale.Max = (XDate)(max_myDateTime);
            myPane.XAxis.Scale.Min = (XDate)(min_myDateTime);


            //XAxis type 
            myPane.XAxis.Type = AxisType.Date;
            myPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            myPane.XAxis.Scale.MinorUnit = DateUnit.Minute;

            //labels angle
            myPane.XAxis.Scale.FontSpec.Angle = 65;
            myPane.XAxis.Scale.FontSpec.IsBold = true;
            myPane.XAxis.Scale.FontSpec.Size = 12;
            myPane.XAxis.Scale.Format = "dd HH:mm";

            // Display the Y axis grid 
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.Scale.MinorStep = 0.1;

            //refreh chart
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
    }
}
