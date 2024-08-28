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
    public partial class Product : Form
    {
        SqlConnection connection;
        SqlCommand command;

        public Product()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            LoadProduct();
        }

        public void LoadProduct() // Hàm hiển thị sản phẩm lên bảng 
        {
            int i = 0;

            dgvProduct.Rows.Clear();

            connection.Open();

            string sql = "Select p.Pcode, p.Barcode, p.Description, b.Brand, c.Category, p.Price, p.Reorder from Product as p INNER JOIN Brand as b on p.Brand_ID = b.Id INNER JOIN Category as c on c.Id = p.Category_ID where CONCAT(p.Description,b.Brand,c.Category) like '%"+txtSearch.Text+"%'";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
            }

            connection.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e) // Hàm thêm sản phẩm 
        {
            ProductModule pm = new ProductModule(this);
            pm.btnUpdate.Enabled = false;
            pm.ShowDialog();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e) // Hàm chỉnh sửa hay xóa sản phẩm trên bảng
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Bạn có muốn xóa sản phẩm này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Delete from Product where Pcode = '" + dgvProduct[1, e.RowIndex].Value.ToString() + "'";

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Xóa thành công");
                    LoadProduct();
                }
            }
            else if (colName == "Edit")
            {
                ProductModule pm = new ProductModule(this);

                pm.txtPcode.Text = dgvProduct[1, e.RowIndex].Value.ToString();
                pm.txtBarcode.Text = dgvProduct[2, e.RowIndex].Value.ToString();
                pm.txtDescription.Text = dgvProduct[3, e.RowIndex].Value.ToString();
                pm.cboBrand.Text = dgvProduct[4, e.RowIndex].Value.ToString();
                pm.cboCategory.Text = dgvProduct[5, e.RowIndex].Value.ToString();
                pm.txtPrice.Text = dgvProduct[6, e.RowIndex].Value.ToString();
                pm.numUDReOrder.Value = int.Parse(dgvProduct[7, e.RowIndex].Value.ToString());

                pm.txtPcode.Enabled = false;
                pm.btnUpdate.Enabled = true;
                pm.btnSave.Enabled = false;
                pm.ShowDialog();
            }
            LoadProduct();
        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e) // Hàm tìm kiếm sản phẩm trên thanh tìm kiếm
        {
            LoadProduct();
        }
    }
}
