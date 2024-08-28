using System;
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
    public partial class Record : Form
    {
        SqlConnection connection;
        SqlCommand command;
        public Record()
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());
            LoadCriticalItems();
            LoadInventoryList();
            cboTopSell.Items.Add("Số lượng sản phẩm");
            cboTopSell.Items.Add("Tổng tiền");
        }

        public void LoadTopSelling()
        {
            int i = 0;

            dgvTopSelling.Rows.Clear();

            connection.Open();

            if (cboTopSell.Text == "Số lượng sản phẩm")
            {
                string sql = "Select top 10 Pcode, Description, isnull(Sum(Qty),0) as Quantity, isnull(Sum(Total),0) as Total from viewTopSelling where Sdate between '" + dtFromTopSell.Value.ToString() + "' and '" + dtToTopSell.Value.ToString() + "' and Status like 'Sold' group by Pcode,Description order by Sum(Qty) desc";
                command = new SqlCommand(sql, connection);
            }
            else if (cboTopSell.Text == "Tổng tiền")
            {
                string sql = "Select top 10 Pcode, Description, isnull(Sum(Qty),0) as Quantity, isnull(Sum(Total),0) as Total from viewTopSelling where Sdate between '" + dtFromTopSell.Value.ToString() + "' and '" + dtToTopSell.Value.ToString() + "' and Status like 'Sold' group by Pcode,Description order by Sum(Total) desc";
                command = new SqlCommand(sql, connection);
            }          
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                i++;
                dgvTopSelling.Rows.Add(i, reader["Pcode"].ToString(), reader["Description"].ToString(), reader["Quantity"].ToString(), reader["Total"].ToString());
            }
            connection.Close();
        }

        private void btnLoadTopSelling_Click(object sender, EventArgs e)
        {
            if (cboTopSell.Text == "Thống kê theo")
            {
                MessageBox.Show("Vui lọng chọn kiểu thống kê !");
                cboTopSell.Focus();
                return;
            }
            LoadTopSelling();
        }

        public void LoadSoldItems()
        {
            try
            {
                int i = 0;

                dgvSoldItems.Rows.Clear();

                connection.Open();
                string sql = "Select c.Pcode,p.Description,c.Price,Sum(c.Qty) as Qty,Sum(c.Disc) as Disc,Sum(c.Total) as Total from Cart as c INNER JOIN Product as p on p.Pcode = c.Pcode where c.Sdate between '" + dtFromSoldItems.Value.ToString() + "' and '" + dtToSoldItems.Value.ToString() + "' and c.Status like 'Sold' group by c.Pcode,p.Description,c.Price";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvSoldItems.Rows.Add(i, reader["Pcode"].ToString(), reader["Description"].ToString(), reader["Price"].ToString(),reader["Qty"].ToString(), reader["Disc"].ToString(), double.Parse(reader["Total"].ToString()).ToString("#,##0.00"));
                }
                connection.Close();

                connection.Open();
                string sql1 = "Select Isnull(Sum(Total),0) as Sum  from Cart where Status like 'Sold' and Sdate between '" + dtFromTopSell.Value.ToString() + "' and '" + dtToTopSell.Value.ToString() + "'";
                command = new SqlCommand(sql1, connection);
                var reader1 = command.ExecuteReader();
                if (reader1.Read())
                {
                    lblTotal.Text = double.Parse(reader1["Sum"].ToString()).ToString("#,##0.00");
                }
                connection.Close();
            }
            catch(Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadSoldItems_Click(object sender, EventArgs e)
        {
            LoadSoldItems();
        }

        public void LoadCriticalItems()
        {
            try
            {
                int i = 0;

                dgvCriticalItems.Rows.Clear();

                connection.Open();
                string sql = "Select * from viewCriticalItems";
                command= new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvCriticalItems.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(),reader[5].ToString(),reader[6].ToString(),reader[7].ToString());
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadInventoryList()
        {
            try
            {
                int i = 0;

                dgvInventoryList.Rows.Clear();

                connection.Open();
                string sql = "Select * from viewInventoryList";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvInventoryList.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadCancelItem()
        {
            try
            {
                int i = 0;

                dgvCancel.Rows.Clear();

                connection.Open();
                string sql = "Select * from viewCancelItems where Sdate between '" + dtFromCancel.Value.ToString() + "' and '" + dtToCancel.Value.ToString() + "'";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvCancel.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString());
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadCancel_Click(object sender, EventArgs e)
        {
            LoadCancelItem();
        }

        public void LoadInStockHistory()
        {
            try
            {
                int i = 0;

                dgvStockIn.Rows.Clear();

                connection.Open();
                string sql = "Select * from viewInStock where Stock_date between '" + dtFromCancel.Value.ToString() + "' and '" + dtToCancel.Value.ToString() + "' and Status like 'Done'";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvStockIn.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString(), reader[9].ToString());
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadStockIn_Click(object sender, EventArgs e)
        {
            LoadInStockHistory();
        }

        private void btnPrintTopSelling_Click(object sender, EventArgs e)
        {
            POSReport report = new POSReport();
            string param = "Từ ngày : " + dtFromTopSell.Value.ToString("dd/MM/yyyy") + " đến : " + dtToTopSell.Value.ToString("dd/MM/yyyy");
            if (cboTopSell.Text == "Số lượng sản phẩm")
            {
                report.LoadTopSelling("Select top 10 Pcode, Description, isnull(Sum(Qty),0) as Quantity, isnull(Sum(Total),0) as Total from viewTopSelling where Sdate between '" + dtFromTopSell.Value.ToString() + "' and '" + dtToTopSell.Value.ToString() + "' and Status like 'Sold' group by Pcode,Description order by Sum(Qty) desc", param, "Danh sách sản phẩm bán chạy theo số lượng sản phẩm");
            }
            else if (cboTopSell.Text == "Tổng tiền")
            {
                report.LoadTopSelling(" Select top 10 Pcode, Description, isnull(Sum(Qty),0) as Quantity, isnull(Sum(Total),0) as Total from viewTopSelling where Sdate between '" + dtFromTopSell.Value.ToString() + "' and '" + dtToTopSell.Value.ToString() + "' and Status like 'Sold' group by Pcode,Description order by Sum(Total) desc", param, "Danh sách sản phẩm bán chạy theo tổng tiền");
            }
            report.ShowDialog();
        }

        private void btnPrintSoldItems_Click(object sender, EventArgs e)
        {
            POSReport report = new POSReport();
            string param = "Từ ngày : " + dtFromSoldItems.Value.ToString("dd/MM/yyyy") + " đến " + dtToSoldItems.Value.ToString("dd/MM/yyyy");
            report.LoadSoldItems("Select c.Pcode,p.Description,c.Price,Sum(c.Qty) as Qty,Sum(c.Disc) as Disc,Sum(c.Total) as Total from Cart as c INNER JOIN Product as p on p.Pcode = c.Pcode where c.Sdate between '" + dtFromSoldItems.Value.ToString() + "' and '" + dtToSoldItems.Value.ToString() + "' and c.Status like 'Sold' group by c.Pcode,p.Description,c.Price", param);
            report.ShowDialog();
        }

        private void btnPrintInventoryList_Click(object sender, EventArgs e)
        {
            POSReport report = new POSReport();

            report.LoadInventory("Select * from viewInventoryList");
            report.ShowDialog();
        }

        private void btnPrintCancel_Click(object sender, EventArgs e)
        {
            POSReport report = new POSReport();
            string param = "From : " + dtFromCancel.Value.ToString("dd/MM/yyyy") + " to " + dtToCancel.Value.ToString("dd/MM/yyyy");
            report.LoadCancelOrder("Select * from viewCancelItems where Sdate between '" + dtFromCancel.Value.ToString() + "' and '" + dtToCancel.Value.ToString() + "'", param);
            report.ShowDialog();
        }

        private void btnPrintStockIn_Click(object sender, EventArgs e)
        {
            POSReport report = new POSReport();
            string param = "From : " + dtFromStockIn.Value.ToString("dd/MM/yyyy") + " to " + dtToStockIn.Value.ToString("dd/MM/yyyy");
            report.LoadStockInHistory("Select * from viewInStock where Stock_date between '" + dtFromStockIn.Value.ToString() + "' and '" + dtToStockIn.Value.ToString() + "'", param);
            report.ShowDialog();
        }
    }  
}
