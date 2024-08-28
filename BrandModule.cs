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
    public partial class BrandModule : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Brand brand;

        public BrandModule(Brand brand)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            this.brand = brand;
        }

        private void picClose_Click(object sender, EventArgs e) //Hàm thoát chương trình
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e) // Hàm lưu thương hiệu
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn lưu thương hiệu này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Insert into Brand(Brand) Values ('" + txtBrandName.Text + "')";

                    command = new SqlCommand(sql, connection);
                    //command.Parameters.AddWithValue("@Brand", txtBrandName.Text);
                    command.ExecuteNonQuery();

                    MessageBox.Show(" Đã được lưu thành công", "POS");
                    Clear();

                    brand.LoadBrand();
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

        private void btnCancel_Click(object sender, EventArgs e) // Hàm reset textbox 
        {
            Clear();
        }

        public void Clear()
        {
            txtBrandName.Clear();
            txtBrandName.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e) // Hàm chỉnh sửa thương hiệu
        {
            if (MessageBox.Show(" Bạn có muốn cập nhật thương hiệu này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                connection.Open();

                string sql = "Update Brand set Brand = @Brand where Id = " + lblId.Text;

                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Brand", txtBrandName.Text);
                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Thương hiệu đã được cập nhật thành công");

                Clear();

                this.Dispose();
            }
        }
    }
}
