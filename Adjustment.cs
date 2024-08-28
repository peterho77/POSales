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
using System.Collections;

namespace POSales
{
    public partial class Adjustment : Form
    {
        SqlConnection connection;
        SqlCommand command;
        MainForm main;
        public int product_qty;
        public Adjustment(MainForm main)
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());
            LoadStock();
            this.main = main;

        }

        public void LoadStock()
        {
            int i = 0;

            dgvAdjustment.Rows.Clear();

            connection.Open();

            string sql = "Select s.Id, s.Pcode, p.Description, s.Qty, s.Status, s.Sdate, s.Inspector from Product_Status_Report s INNER JOIN Product p on p.Pcode = s.Pcode where CONCAT(p.Description,s.Pcode) like '%" + txtSearch.Text + "%'";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvAdjustment.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
            }

            connection.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadStock();
        }

        public void Clear()
        {
            txtQty.Clear();
            txtPcode.Text = "";
            txtDescription.Text = "";
            cboStatus.Text = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboStatus.Text == "")
            {
                MessageBox.Show("Không được để trống. Vui lòng chọn trạng thái hàng hóa cần báo cáo", "Warning");
                cboStatus.Focus();
                return;
            }
            else if (txtPcode.Text == "" && txtDescription.Text == "")
            {
                MessageBox.Show("Không được để trống mã sản phẩm và tên sản phẩm. Vui lòng nhấn vào thêm sản phẩm để báo cáo", "Warning");
                txtQty.Focus();
                return;
            }
            else if (txtQty.Text == "")
            {
                MessageBox.Show("Không được để trống. Vui lòng nhập số lượng hàng hóa hư hại.", "Warning");
                txtQty.Focus();
                return;
            }           
            else if (int.Parse(txtQty.Text) < 0 || int.Parse(txtQty.Text) > int.Parse(txtProductQty.Text))
            {
                MessageBox.Show("Số lượng hàng hóa cần nhập vượt quá số lượng hiện có là " + txtProductQty.Text + " .Vui lòng nhập lại.", "Warning");
                txtQty.Focus();
                return;
            }

            string sql1 = "Insert into Product_Status_Report(Pcode, Qty, Status, Sdate, Inspector) values (@Pcode, @Qty, @Status, @Sdate, @User)";
            connection.Open();
            command = new SqlCommand(sql1, connection);
            command.Parameters.AddWithValue("@Pcode", txtPcode.Text);
            command.Parameters.AddWithValue("@Qty", txtQty.Text);
            command.Parameters.AddWithValue("@Status", cboStatus.Text);
            command.Parameters.AddWithValue("@Sdate", DateTime.Now);
            command.Parameters.AddWithValue("@User", txtName.Text);
            command.ExecuteNonQuery();
            connection.Close();
            LoadStock();
            Clear();
        }

        private void linkProduct_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProductStockIn stockIn = new ProductStockIn(this);
            stockIn.ShowDialog();
        }

        private void dgvAdjustment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // thao tac nhan nut select
            string colName = dgvAdjustment.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                try
                {
                    if (MessageBox.Show("Bạn có muốn thay đổi thông tin báo cáo trạng thái đơn hàng ko ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        connection.Open();
                        string sql = "Update Product_Status_Report set Qty = @Qty, Status = @Status where Id = @Id";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@Status", cboStatus.Text);
                        command.Parameters.AddWithValue("@Qty", txtQty.Text);
                        command.Parameters.AddWithValue("@Id", dgvAdjustment.Rows[e.RowIndex].Cells[1].Value.ToString());
                        command.ExecuteNonQuery();
                        connection.Close();
                        LoadStock();
                    }
                }
                catch(Exception ex)
                {
                    connection.Close();
                    MessageBox.Show(ex.Message);
                }
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Bạn có xóa báo cáo trạng thái đơn hàng này ko ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();
                    string sql = "Delete from Product_Status_Report where Id = @Id";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Id", dgvAdjustment.Rows[e.RowIndex].Cells[1].Value.ToString());
                    command.ExecuteNonQuery();
                    connection.Close();
                    LoadStock();
                }
            }
        }
    }
}
