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
    public partial class ProductModule : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Product product;

        public ProductModule(Product product)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            LoadBrand();
            LoadCategory();

            this.product = product;
        }

        public void LoadCategory()   //Hàm thêm loại sản phẩm vào combobox
        {
            cboCategory.Items.Clear();
            string query = "Select * from Category";
            cboCategory.DataSource = DataProvider.getTable(query);
            cboCategory.DisplayMember = "Category";
            cboCategory.ValueMember = "Id";
        }

        public void LoadBrand() //Hàm thêm thương hiệu vào combobox
        {
            cboBrand.Items.Clear();
            string query = "Select * from Brand";
            cboBrand.DataSource = DataProvider.getTable(query);
            cboBrand.DisplayMember = "Brand";
            cboBrand.ValueMember = "Id";
        }

        public void Clear() // Hàm reset thông tin sản phẩm
        {
            txtPcode.Clear();
            txtBarcode.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
            cboBrand.SelectedIndex = 0;
            cboCategory.SelectedIndex = 0;
            numUDReOrder.Value = 1;

            txtPcode.Enabled = true;
            txtPcode.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }

        private void picClose_Click(object sender, EventArgs e) //Hàm đóng cửa sổ
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e) //Hàm lưu thông tin sản phẩm
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn lưu sản phẩm này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Insert into Product(Pcode,Barcode,Description,Brand_ID,Category_ID,Price,Reorder) Values (@Pcode,@Barcode,@Description,@Brand_ID,@Category_ID,@Price,@Reorder)";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Pcode", txtPcode.Text);
                    command.Parameters.AddWithValue("@Barcode", txtBarcode.Text);
                    command.Parameters.AddWithValue("@Description", txtDescription.Text);
                    command.Parameters.AddWithValue("@Brand_ID", cboBrand.SelectedValue);
                    command.Parameters.AddWithValue("@Category_ID", cboCategory.SelectedValue);
                    command.Parameters.AddWithValue("@Price", double.Parse(txtPrice.Text));
                    command.Parameters.AddWithValue("@Reorder", numUDReOrder.Value);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Lưu thành công", "POS");
                    Clear();
                    product.LoadProduct();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)  // Hàm chỉnh sửa thông tin sản phẩm
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn cập nhật sản phẩm này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Update Product set Barcode = @Barcode, Description = @Description, Brand_ID = @Brand_ID, Category_ID = @Category_ID, Price = @Price, Reorder = @Reorder where Pcode = @Pcode";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Pcode", txtPcode.Text);
                    command.Parameters.AddWithValue("@Barcode", txtBarcode.Text);
                    command.Parameters.AddWithValue("@Description", txtDescription.Text);
                    command.Parameters.AddWithValue("@Brand_ID", cboBrand.SelectedValue);
                    command.Parameters.AddWithValue("@Category_ID", cboCategory.SelectedValue);
                    command.Parameters.AddWithValue("@Price", double.Parse(txtPrice.Text));
                    command.Parameters.AddWithValue("@Reorder", numUDReOrder.Value);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Cập nhật sản phẩm thành công");

                    Clear();

                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
