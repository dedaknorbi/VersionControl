using MNB.Entities;
using MNB.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace MNB
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        //BindingList<string> Currencies = new BindingList<string>();
        public Form1()
        {
            InitializeComponent();
            dataGridView1.DataSource = Rates;
            //comboBox1.DataSource = Currencies;
            RefreshData();
        }

        private void RefreshData()
        {
            
            Rates.Clear();

            var mnbService = new MNBArfolyamServiceSoapClient();

            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = Convert.ToString(comboBox1.SelectedItem),
                startDate = Convert.ToString(dateTimePicker1.Value),
                endDate = Convert.ToString(dateTimePicker2.Value)
            };


            var response = mnbService.GetExchangeRates(request);

            var result = response.GetExchangeRatesResult;

            var xml = new XmlDocument();
            xml.LoadXml(result);

            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new RateData();
                Rates.Add(rate);
                rate.Date = DateTime.Parse(element.GetAttribute("date"));
                var childElement = (XmlElement)element.ChildNodes[0];
                rate.Currency = childElement.GetAttribute("curr");
                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;


                chartRateData.DataSource = Rates;

                var series = chartRateData.Series[0];
                series.ChartType = SeriesChartType.Line;
                series.XValueMember = "Date";
                series.YValueMembers = "Value";
                series.BorderWidth = 2;

                var legend = chartRateData.Legends[0];
                legend.Enabled = false;

                var chartArea = chartRateData.ChartAreas[0];
                chartArea.AxisX.MajorGrid.Enabled = false;
                chartArea.AxisY.MajorGrid.Enabled = false;
                chartArea.AxisY.IsStartedFromZero = false;
            }
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
