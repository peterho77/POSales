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
    //public static class Now 
    //{
    //    public static DateTime EndOfDay(DateTime date)
    //    {
    //        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
    //    }

    //    public static DateTime StartOfDay(DateTime date)
    //    {
    //        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
    //    }
    //}
    public partial class DailySale : Form
    {
        SqlConnection connection;
        SqlCommand command;
        public string cancel_cashier;
        
        public DailySale()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            LoadCashier();
            
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadCashier()
        {
            cboCashier.Items.Clear();
            cboCashier.Items.Add("Tất cả thu ngân");
            string sql = "Select * from UserAccount where Role like 'Nhân viên thu ngân'";

            //C1
            //cboCashier.DataSource = DataProvider.getTable(sql);
            //cboCashier.DisplayMember = "Name";
            //cboCashier.ValueMember = "Username";

            //C2
            connection.Open();
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                cboCashier.Items.Add(reader["Username"].ToString());
            }
            connection.Close();
        }

        public void LoadSold()
        {
            double total = 0;
            dgvSold.Rows.Clear();
            
            connection.Open();
            string sql;

          
            if (cboCashier.Text == "Tất cả thu ngân")
            {
                sql = "Select c.Id, c.Transno, c.Pcode, p.Description, c.Price, c.Qty, c.Disc, c.Total " +
                    "from Cart as c INNER JOIN Product as p on c.Pcode = p.Pcode " +
                    "where c.Status = 'Sold' and c.Sdate between '" + dtFrom.Value.ToString("MM/dd/yyyy") + "' and '" + dtTo.Value.ToString("MM/dd/yyyy") + "'";
            }
            else
            {
                sql = "Select c.Id, c.Transno, c.Pcode, p.Description, c.Price, c.Qty, c.Disc, c.Total " +
                    "from Cart as c INNER JOIN Product as p on c.Pcode = p.Pcode " +
                    "where c.Status = 'Sold' and c.Sdate between '" + dtFrom.Value.ToString("MM/dd/yyyy") + "' and '" + dtTo.Value.ToString("MM/dd/yyyy") + "' and c.Cashier = '" + cboCashier.Text + "'";
            }
            
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                i++;
                total += double.Parse(reader["Total"].ToString());
                dgvSold.Rows.Add(i, reader["Id"].ToString(), reader["Transno"].ToString(), reader["Pcode"].ToString(), reader["Description"].ToString(), 
                    double.Parse(reader["Price"].ToString()).ToString("#,##0.00"), reader["Qty"].ToString(), 
                    reader["Disc"].ToString(), double.Parse(reader["Total"].ToString()).ToString("#,##0.00"));
            }
            connection.Close();

            lblTotal.Text = total.ToString("#,##0.00");
        }

        private void DailySale_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }

        private void dgvSold_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvSold.Columns[e.ColumnIndex].Name;
            if (colName == "Cancel")
            {
                CancelOrder cancelorder = new CancelOrder(this);
                cancelorder.txtId.Text = dgvSold[1, e.RowIndex].Value.ToString();
                cancelorder.txtPcode.Text = dgvSold[3,e.RowIndex].Value.ToString();
                cancelorder.txtTransno.Text = dgvSold[2, e.RowIndex].Value.ToString();
                cancelorder.txtDescription.Text = dgvSold[4,e.RowIndex].Value.ToString();
                cancelorder.txtPrice.Text = dgvSold[5, e.RowIndex].Value.ToString();
                cancelorder.txtQty.Text = dgvSold[6, e.RowIndex].Value.ToString();
                cancelorder.txtDiscount.Text = dgvSold[7, e.RowIndex].Value.ToString();
                cancelorder.txtTotal.Text = dgvSold[8, e.RowIndex].Value.ToString();
                cancelorder.txtCancel.Text = cancel_cashier;
                cancelorder.txtCancel.Enabled = false;
                cancelorder.numCancel.Value = int.Parse(dgvSold[6, e.RowIndex].Value.ToString());
                cancelorder.numCancel.Enabled = false;
                cancelorder.ShowDialog();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            POSReport report = new POSReport();
            string param = "Từ ngày " + dtFrom.Value.ToString("dd/MM/yyyy") + " đến " + dtTo.Value.ToString("dd/MM/yyyy");
            string sql = "";
            if (cboCashier.Text == "Tất cả thu ngân")
            {
                report.LoadDailyReport("Select c.Id, c.Transno, c.Pcode, p.Description, c.Price, c.Qty, c.Disc, c.Total " +
                    "from Cart as c INNER JOIN Product as p on c.Pcode = p.Pcode " +
                    "where c.Status = 'Sold' and Sdate between '" + dtFrom.Value + "' and '" + dtTo.Value + "'", param, cboCashier.Text);
            }
            else
            {
                report.LoadDailyReport("Select c.Id, c.Transno, c.Pcode, p.Description, c.Price, c.Qty, c.Disc, c.Total " +
                    "from Cart as c INNER JOIN Product as p on c.Pcode = p.Pcode " +
                    "where c.Status = 'Sold' and " +
                    "Sdate between '" + dtFrom.Value + "' and '" + dtTo.Value + "' and c.Cashier = '" + cboCashier.Text + "'", param, cboCashier.Text);
            }
            report.ShowDialog();
        }

        private void DailySale_Load(object sender, EventArgs e)
        {
            LoadSold();
        }

        private void cboCashier_TextChanged(object sender, EventArgs e)
        {
            LoadSold();
        }

        private void dtFrom_ValueChanged_1(object sender, EventArgs e)
        {
            LoadSold();
        }

        private void dtTo_ValueChanged_1(object sender, EventArgs e)
        {
            LoadSold();
        }
    }
}
