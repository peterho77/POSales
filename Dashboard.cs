using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSales
{
    public partial class Dashboard : Form
    {
        SqlConnection connection;
        SqlCommand command;
        public static string sdate = DateTime.Now.ToShortDateString();

        public Dashboard()
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());
            panelRecord.Parent = picBackground;
        }

        public void loadDashboard()
        {
            string sdate = DateTime.Now.ToShortDateString();
            lblTotalDailySale.Text = DataProvider.ExtractData("Select isnull(Sum(Total),0) as Total from Cart where Status like 'Sold' and Sdate between '" + sdate + "' and '" + sdate + "'").ToString("#,##0.00");
            lblTotalProduct.Text = DataProvider.ExtractData("Select count(*) from Product").ToString();
            lblStockonHand.Text = DataProvider.ExtractData("Select Sum(p.Quantity) + Sum(s.Qty) from Product p FULL JOIN Stock s on p.Pcode = s.Pcode\r\n").ToString();
            lblCriticalItems.Text = DataProvider.ExtractData("Select count(*) from viewCriticalItems").ToString();
        }
        private void Dashboard_Load_1(object sender, EventArgs e)
        {
            loadDashboard();
        }
    }
}
