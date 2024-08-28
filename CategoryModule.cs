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
    public partial class CategoryModule : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Category category;

        public CategoryModule(Category catergory)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            this.category = catergory;
        }      

        private void btnSave_Click(object sender, EventArgs e) // Hàm lưu thông tin loại sản phẩm
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn lưu danh mục này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Insert into Category(Category) Values ('" + txtCategoryName.Text + "')";

                    command = new SqlCommand(sql, connection);
                    //command.Parameters.AddWithValue("@Brand", txtBrandName.Text);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Lưu thành công !", "POS");
                    Clear();

                    category.LoadCategory();
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

        private void btnCancel_Click(object sender, EventArgs e) //Hàm reset textbox
        {
            Clear();
        }

        public void Clear()
        {
            txtCategoryName.Clear();
            txtCategoryName.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)  // Nút thực hiện sự kiện chỉnh sửa thông tin 
        {
            if (MessageBox.Show("Bạn có muốn cập nhật danh mục này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                connection.Open();

                string sql = "Update Category set Category = @Category where Id = " + lblId.Text;

                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Category", txtCategoryName.Text);
                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Danh mục đã được cập nhật thành công");

                Clear();

                this.Dispose();
            }
        }

        private void picClose_Click(object sender, EventArgs e) // Nút đóng 
        {
            this.Dispose();
        }
    }
}
