
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;


namespace POSales
{
    public partial class POSReport : Form
    {
        SqlConnection connection;
        SqlCommand command;
        string store;
        string address;

        public POSReport()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            LoadStore();
        }

        private void POSReport_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
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

        public void LoadDailyReport(string sql, string param, string cashier)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource report1;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rpSoldReport.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                connection.Open();
                da.SelectCommand = new SqlCommand(sql, connection);
                da.Fill(ds.Tables["dtSoldReport"]);
               

                Microsoft.Reporting.WinForms.ReportParameter pDate = new Microsoft.Reporting.WinForms.ReportParameter("pDate", param);
                Microsoft.Reporting.WinForms.ReportParameter pCashier = new Microsoft.Reporting.WinForms.ReportParameter("pCashier", cashier);
                Microsoft.Reporting.WinForms.ReportParameter pHeader = new Microsoft.Reporting.WinForms.ReportParameter("pHeader", "Daily Sale Report");
                Microsoft.Reporting.WinForms.ReportParameter pStore = new Microsoft.Reporting.WinForms.ReportParameter("pStore", store);
                Microsoft.Reporting.WinForms.ReportParameter pAddress = new Microsoft.Reporting.WinForms.ReportParameter("pAddress", address);


                reportViewer1.LocalReport.SetParameters(pDate);
                reportViewer1.LocalReport.SetParameters(pCashier);
                reportViewer1.LocalReport.SetParameters(pHeader);
                reportViewer1.LocalReport.SetParameters(pStore);
                reportViewer1.LocalReport.SetParameters(pAddress);

                report1 = new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", ds.Tables["dtSoldReport"]);
                reportViewer1.LocalReport.DataSources.Add(report1);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadTopSelling(string sql, string param, string header)
        {
            try
            {
                ReportDataSource report1;
                this.reportViewer1.LocalReport.ReportPath = @"C:\Users\peter\Downloads\Supermarket\POSales\rpTopSelling.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                connection.Open();
                da.SelectCommand = new SqlCommand(sql, connection);
                da.Fill(ds.Tables["dtTopSelling"]);

                ReportParameter pDate = new ReportParameter("pDate", param);
                ReportParameter pHeader = new ReportParameter("pHeader", header);

                reportViewer1.LocalReport.SetParameters(pDate);
                reportViewer1.LocalReport.SetParameters(pHeader);

                report1 = new ReportDataSource("DataSet1", ds.Tables["dtTopSelling"]);
                reportViewer1.LocalReport.DataSources.Add(report1);
                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
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

        public void LoadSoldItems(string sql, string param)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource report1;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rpSoldItems.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                connection.Open();
                da.SelectCommand = new SqlCommand(sql, connection);
                da.Fill(ds.Tables["dtSoldItems"]);

                Microsoft.Reporting.WinForms.ReportParameter pDate = new Microsoft.Reporting.WinForms.ReportParameter("pDate", param);

                reportViewer1.LocalReport.SetParameters(pDate);

                report1 = new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", ds.Tables["dtSoldItems"]);
                reportViewer1.LocalReport.DataSources.Add(report1);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadInventory(string sql)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource report1;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rpInventory.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                connection.Open();
                da.SelectCommand = new SqlCommand(sql, connection);
                da.Fill(ds.Tables["dtInventory"]);

                report1 = new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", ds.Tables["dtInventory"]);
                reportViewer1.LocalReport.DataSources.Add(report1);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadCancelOrder(string sql, string param)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource report1;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rpCancelOrder.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                connection.Open();
                da.SelectCommand = new SqlCommand(sql, connection);
                da.Fill(ds.Tables["dtCancelOrder"]);

                Microsoft.Reporting.WinForms.ReportParameter pDate = new Microsoft.Reporting.WinForms.ReportParameter("pDate", param);

                reportViewer1.LocalReport.SetParameters(pDate);

                report1 = new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", ds.Tables["dtCancelOrder"]);
                reportViewer1.LocalReport.DataSources.Add(report1);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadStockInHistory(string sql, string param)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource report1;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rpStockInHistory.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                connection.Open();
                da.SelectCommand = new SqlCommand(sql, connection);
                da.Fill(ds.Tables["dtStockInHistory"]);

                Microsoft.Reporting.WinForms.ReportParameter pDate = new Microsoft.Reporting.WinForms.ReportParameter("pDate", param);

                reportViewer1.LocalReport.SetParameters(pDate);

                report1 = new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", ds.Tables["dtStockInHistory"]);
                reportViewer1.LocalReport.DataSources.Add(report1);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;

                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void POSReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
