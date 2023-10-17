﻿using DevExpress.XtraCharts;
using HIS.Desktop.Plugins.HisCareSum.ADO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HIS.Desktop.Plugins.HisCareSum
{
    class ChartDhstProcess
    {
        internal static MemoryStream GetChartImage(ChartControl chart, int _idChart)
        {
            MemoryStream result = new MemoryStream();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("chart.Series.count = " + chart.Series.Count);
                if (_idChart == 0)
                {
                    chart.Series[0].Visible = false;
                    chart.Series[1].Visible = true;
                }
                else
                {
                    chart.Series[0].Visible = true;
                    chart.Series[1].Visible = false;
                }
                chart.ExportToImage(result, System.Drawing.Imaging.ImageFormat.Png);
                Inventec.Common.Logging.LogSystem.Debug("GetChartImage. result.Length = " + result.Length);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static MemoryStream GetChartImageAll(ChartControl chart)
        {
            MemoryStream result = new MemoryStream();
            try
            {
                chart.ExportToImage(result, System.Drawing.Imaging.ImageFormat.Png);
                Inventec.Common.Logging.LogSystem.Debug("GetChartImage. result.Length = " + result.Length);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static ChartControl GenerateChartImageAll(List<ChartADO> _chart)
        {
            ChartControl chart = new ChartControl();
            try
            {
                Series series1 = new Series("Series 1", ViewType.Line);
                Series series2 = new Series("Series 2", ViewType.Line);
                // Add both series to the chart.
                //chart.AutoLayout = true;
                chart.Series.AddRange(new Series[] { series1, series2 });
                chart.Series[0].Name = "Nhiệt độ";
                chart.Series[1].Name = "Mạch";
                chart.Series[0].ArgumentDataMember = "DateTime";
                chart.Series[0].ValueDataMembersSerializable = "TEMPERATURE";
                chart.Series[1].ArgumentDataMember = "DateTime";
                chart.Series[1].ValueDataMembersSerializable = "PULSE";
                chart.Series[1].View.Color = Color.Red;
                //chart.Series[0].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                //chart.Series[1].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                //chart.Series[2].LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                // Create two secondary axes, and add them to the chart's Diagram.
                XYDiagram diagram = (XYDiagram)chart.Diagram;
                diagram.AxisY.WholeRange.MaxValue = 41;
                diagram.AxisY.WholeRange.MinValue = 35;
                diagram.AxisY.WholeRange.SetMinMaxValues(35, 41);
                diagram.AxisY.Label.Font = new Font(diagram.AxisY.Label.Font.FontFamily, (float)10, FontStyle.Bold);
                diagram.AxisX.Label.Font = new Font(diagram.AxisY.Label.Font.FontFamily, (float)10, FontStyle.Bold);
                diagram.EnableAxisYScrolling = true;
                diagram.EnableAxisYZooming = true;

                SecondaryAxisY myAxisY = new SecondaryAxisY("my Y-Axis");
                ((XYDiagram)chart.Diagram).SecondaryAxesY.Add(myAxisY);
                // Assign the series2 to the created axes.
                ((LineSeriesView)series2.View).AxisY = myAxisY;
                ((LineSeriesView)series2.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;

                ((LineSeriesView)series1.View).AxisY.Title.Text = "Nhiệt độ";
                ((LineSeriesView)series1.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)series1.View).AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
                ((LineSeriesView)series1.View).AxisY.Title.Alignment = System.Drawing.StringAlignment.Far;
                ((LineSeriesView)series1.View).AxisY.Title.TextColor = Color.Blue;
                ((LineSeriesView)series1.View).AxisY.Label.TextColor = Color.Blue;
                ((LineSeriesView)series1.View).AxisY.Color = Color.Blue;
                ((LineSeriesView)series1.View).AxisX.Alignment = DevExpress.XtraCharts.AxisAlignment.Far;

                myAxisY.Title.Text = "Mạch";
                myAxisY.Title.Visible = true;
                myAxisY.Title.Alignment = System.Drawing.StringAlignment.Far;
                myAxisY.Alignment = DevExpress.XtraCharts.AxisAlignment.Near;
                myAxisY.Title.TextColor = Color.Red;
                myAxisY.Label.Font = new Font(myAxisY.Label.Font.FontFamily, (float)10, FontStyle.Bold);
                myAxisY.Label.TextColor = Color.Red;
                myAxisY.Color = Color.Red;
                myAxisY.WholeRange.MinValue = 40;
                myAxisY.WholeRange.MaxValue = 160;
                myAxisY.WholeRange.SetMinMaxValues(40, 160);

                DataTable dt = CreateData(_chart);
                chart.DataSource = dt;
                chart.Size = new System.Drawing.Size(1308, 440);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return chart;
        }

        internal static ChartControl GenerateChartImage(List<ChartADO> _chart)
        {
            ChartControl chart = new ChartControl();
            try
            {
                //DevExpress.XtraCharts.XYDiagram xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
                DevExpress.XtraCharts.Series series1 = new DevExpress.XtraCharts.Series();
                DevExpress.XtraCharts.LineSeriesView lineSeriesView1 = new DevExpress.XtraCharts.LineSeriesView();
                DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
                DevExpress.XtraCharts.LineSeriesView lineSeriesView2 = new DevExpress.XtraCharts.LineSeriesView();
                //xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
                //xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
                //chart.Diagram = xyDiagram1;
                lineSeriesView1.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                lineSeriesView2.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
                chart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
                chart.Name = "chartControl1";
                series1.ArgumentDataMember = "Date";
                series1.Name = "Series_1";
                series1.ValueDataMembersSerializable = "PULSE";
                series1.View = lineSeriesView1;
                series2.ArgumentDataMember = "Date";
                series2.Name = "Series_2";
                series2.ValueDataMembersSerializable = "TEMPERATURE";
                series2.View = lineSeriesView2;
                chart.SeriesSerializable = new DevExpress.XtraCharts.Series[] {
        series1,
        series2};

                DataTable dt = CreateData(_chart);
                chart.CrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
                chart.Series[0].DataSource = dt;
                chart.Series[1].DataSource = dt;
                Inventec.Common.Logging.LogSystem.Debug("chart.Series.count = " + chart.Series.Count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return chart;
        }

        static DataTable CreateData(List<ChartADO> _chart)
        {
            DataTable dataTable = new DataTable();
            dataTable = new DataTable("Table");
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("DateTime", typeof(string));
            dataTable.Columns.Add("PULSE", typeof(Decimal));
            dataTable.Columns.Add("TEMPERATURE", typeof(Decimal));

            for (int i = 0; i < _chart.Count; i++)
            {
                DataRow row = dataTable.NewRow();

                row["Date"] = _chart[i].Date;
                row["DateTime"] = _chart[i].DateTime;
                if (_chart[i].PULSE != null)
                {
                    row["PULSE"] = Convert.ToDecimal(_chart[i].PULSE);
                }
                else
                {
                    row["PULSE"] = 0;
                }
                if (_chart[i].TEMPERATURE != null)
                {
                    row["TEMPERATURE"] = _chart[i].TEMPERATURE;
                }
                else
                {
                    row["TEMPERATURE"] = 0;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
