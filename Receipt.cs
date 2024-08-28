using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSales
{
    public partial class Receipt : Form
    {

        SqlConnection connection;
        SqlCommand command;
        Cashier cashier;
        string store;
        string address;

        public Receipt(Cashier cashier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cashier = cashier;
            LoadStore();
        }
       
        public void LoadStore()
        {
            connection.Open();
            string sql = "Select * from Store";
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                store = reader["Store"].ToString();
                address = reader["Address"].ToString();
            }
            connection.Close();
        }
        public void LoadReceipt(string pcash, string pchange)
        {
            ReportDataSource report;
            try
            {
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rpReceipt.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();
                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();

                connection.Open();
                da.SelectCommand = new SqlCommand("Select c.Id,c.Transno,c.Pcode,c.Price,c.Qty,c.Disc,c.Total,c.Sdate,c.Status,p.Description from Cart as c INNER JOIN Product as p on p.Pcode = c.Pcode and c.Transno like '" + cashier.lblTransno.Text + "'", connection);
                da.Fill(ds.Tables["dtReceipt"]);


                ReportParameter pVatable = new ReportParameter("pVatable", cashier.lblVatable.Text);
                ReportParameter pVat = new ReportParameter("pVat", cashier.lblVat.Text);
                ReportParameter pDiscount = new ReportParameter("pDiscount", cashier.lblDiscount.Text);
                ReportParameter pTotal = new ReportParameter("pTotal", cashier.lblDisplayTotal.Text);
                ReportParameter pCash = new ReportParameter("pCash", pcash);
                ReportParameter pChange = new ReportParameter("pChange", pchange);
                ReportParameter pStore = new ReportParameter("pStore", store);
                ReportParameter pAddress = new ReportParameter("pAddress", address);
                ReportParameter pTransaction = new ReportParameter("pTransaction", "Hóa đơn: " + cashier.lblTransno.Text);
                ReportParameter pCashier = new ReportParameter("pCashier", cashier.lblUsername.Text);

                reportViewer1.LocalReport.SetParameters(pVatable);
                reportViewer1.LocalReport.SetParameters(pVat);
                reportViewer1.LocalReport.SetParameters(pDiscount);
                reportViewer1.LocalReport.SetParameters(pTotal);
                reportViewer1.LocalReport.SetParameters(pCash);
                reportViewer1.LocalReport.SetParameters(pChange);
                reportViewer1.LocalReport.SetParameters(pStore);
                reportViewer1.LocalReport.SetParameters(pAddress);
                reportViewer1.LocalReport.SetParameters(pTransaction);
                reportViewer1.LocalReport.SetParameters(pCashier);

                report = new ReportDataSource("DataSet1", ds.Tables["dtReceipt"]);
                reportViewer1.LocalReport.DataSources.Add(report);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }
        private void Receipt_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
