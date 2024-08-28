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
    public partial class InStock : Form
    {
        SqlConnection connection;
        SqlCommand command;
        public InStock()
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());           
            LoadSupplier();
            LoadInStock();
            LoadProduct();
            LoadStockHistory();
            GetRefno();
            cboSupplier.Text = "";
        }

        public int check_Refno(int n)
        {
            int count = 0;
            while (n != 0)
            {
                n /= 10;
                count++;
            }
            return count;
        }

        public void GetRefno()  // Hàm tạo mã đơn hàng tự động tăng lên 1
        {
            int count;
            string rn = "DH" + DateTime.Now.Year.ToString();
            string refno = "";

            connection.Open();
            string sql = "Select top 1 Refno from Stock where Refno like '" + refno + "%' ORDER BY Id desc";
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                refno = reader[0].ToString();
                long n = long.Parse(refno.Substring(2,9)) + 1;
                txtReference.Text = "DH" + n.ToString();
            }
            else
            {
                refno = rn + "00001";
                txtReference.Text = refno;
            }
            connection.Close();
        }

        public void LoadSupplier() // Hàm đưa textbox lên combobox Supplier
        {
            cboSupplier.Items.Clear();
            cboSupplier.DataSource = DataProvider.getTable("Select * from Supplier");
            cboSupplier.DisplayMember = "Supplier";
            cboSupplier.ValueMember = "Id";
        }

        public void LoadInStock()   // Hàm hiển thị dữ liệu đơn hàng chưa được kiểm duyệt
        {
            int i = 0;
            dgvStockIn.Rows.Clear();

            connection.Open();

            string sql = "Select * from viewInStock where Status like 'Pending'";
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                i++;
                dgvStockIn.Rows.Add(i, reader["Id"].ToString(), reader["Refno"].ToString(), reader["Pcode"].ToString(), reader["Description"].ToString(), reader["Qty"].ToString(), reader["Stock_date"].ToString(), reader["Stockinby"].ToString(), reader["Supplier"].ToString());
            }
            connection.Close();
        }

        private void cboSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnEntry_Click(object sender, EventArgs e) // Nút xác nhận đã nhập đơn hàng vào kho và cập nhật lại số lượng sản phẩm
        {
            try
            {
                if (dgvStockIn.Rows.Count > 0)
                {
                    if (MessageBox.Show("Do you want to save this product ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        connection.Open();

                        int total_qty = 0;

                        for (int i = 0; i < dgvStockIn.Rows.Count; i++)
                        {
                            total_qty += int.Parse(dgvStockIn.Rows[i].Cells[5].Value.ToString());

                            //update stockin quantity
                            string sql1 = "Update Stock set Qty = " + total_qty + ",status = 'Done' where Id like '" + dgvStockIn.Rows[i].Cells[1].Value.ToString() + "'";
                            command = new SqlCommand(sql1, connection);
                            command.ExecuteNonQuery();

                            //clear table in stock
                            string sql2 = "Update viewInStock set Status = 'Done' where Id like '" + dgvStockIn.Rows[i].Cells[1].Value.ToString() + "'";
                            command = new SqlCommand(sql2, connection);
                            command.ExecuteNonQuery();
                        }                       
                       
                        connection.Close();
                        LoadInStock();
                        Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void Clear() // Hàm xóa thông tin kiểm kê
        {
            txtReference.Clear();
            txtStockInBy.Clear();
            dtStockIn.Value = DateTime.Now;
        }

        private void linkProduct_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)  // Hàm hiện lên bảng sản phẩm và chọn sản phẩm cần nhập đơn
        {
            if (txtContactPerson.Text == "" && txtAddress.Text == "")
            {
                MessageBox.Show("Người liên hệ và địa chỉ không được đề trống");
                cboSupplier.Focus();
                return;
            }
            else
            {
                ProductStockIn stockIn = new ProductStockIn(this);
                stockIn.ShowDialog();
            }           
        }

        private void cboSupplier_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            connection.Open();

            string sql = "Select * from Supplier where Supplier = '" + cboSupplier.Text + "'";
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                lblId.Text = reader["Id"].ToString();
                txtAddress.Text = reader["Address"].ToString();
                txtContactPerson.Text = reader["ContactPerson"].ToString();
            }

            connection.Close();
        }

        private void linkGenerate_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GetRefno();
        }

        public void LoadStockHistory()
        {
            try
            {
                int i = 0;
                dgvStockHistory.Rows.Clear();
                connection.Open();

                string sql = "Select * from viewInStock where Status like 'Done'";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvStockHistory.Rows.Add(i, reader["Id"].ToString(), reader["Refno"].ToString(), reader["Pcode"].ToString(), reader["Description"].ToString(), reader["Qty"].ToString(), reader["Stock_date"].ToString(), reader["Stockinby"].ToString(), reader["Supplier"].ToString());
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadRecord_Click_1(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                dgvStockHistory.Rows.Clear();
                connection.Open();

                string sql = "Select * from viewInStock where Stock_date between '" + dtFrom.Value.ToString("MM/dd/yyyy") + "' and '" + dtTo.Value.ToString("MM/dd/yyyy") + "' and Status like 'Done'";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i++;
                    dgvStockHistory.Rows.Add(i, reader["Id"].ToString(), reader["Refno"].ToString(), reader["Pcode"].ToString(), reader["Description"].ToString(), reader["Qty"].ToString(), reader["Stock_date"].ToString(), reader["Stockinby"].ToString(), reader["Supplier"].ToString());
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvStockIn_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvStockIn.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Bạn có muốn xỏa sản phẩm này khỏi đơn nhập hàng ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Delete from InStock where Id = " + dgvStockIn.Rows[e.RowIndex].Cells[1].Value.ToString();
                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Item has been sucessfully removed");
                    LoadInStock();
                }
            }
            else if (colName == "Adjust")
            {               
                InStock_Qty new_qty = new InStock_Qty(this); 
                new_qty.refno = dgvStockIn.Rows[e.RowIndex].Cells[2].Value.ToString();
                new_qty.pcode = dgvStockIn.Rows[e.RowIndex].Cells[3].Value.ToString();                
                new_qty.ShowDialog();
            }
        }

        public void LoadProduct() // Hàm hiển thị sản phẩm lên bảng 
        {
            int i = 0;

            dgvProduct.Rows.Clear();

            connection.Open();

            string sql = "Select p.Pcode , p.Description, b.Brand, c.Category, p.Price, p.Quantity, p.Reorder from Product as p INNER JOIN Brand as b on p.Brand_ID = b.Id INNER JOIN Category as c on c.Id = p.Category_ID";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
            }

            connection.Close();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (colName == "Add")
            {
                if (MessageBox.Show("Bạn có muốn thêm số lượng sản phẩm trên kệ không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Product_Qty product_qty = new Product_Qty(this);
                    product_qty.pcode = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
                    product_qty.ShowDialog();
                }
            }
        }
    }
}
