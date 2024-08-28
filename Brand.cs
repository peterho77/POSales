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
    public partial class Brand : Form
    {
        SqlConnection connection;
        SqlCommand command;

        public Brand()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            //connection.Open();

            LoadBrand();
        }

        public void LoadBrand() // Hàm hiện lên các thương hiệu đã thêm
        {
            int i = 0;

            dgvBrand.Rows.Clear();

            connection.Open();

            string sql = "Select * from Brand";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                i++;
                dgvBrand.Rows.Add(i, reader["Id"].ToString(),reader["Brand"].ToString()); 
            }

            connection.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e) //Sự kiện thêm thương hiệu
        {
            BrandModule bm = new BrandModule(this);
            bm.btnUpdate.Enabled = false;
            bm.ShowDialog();
        }

        private void dgvBrand_CellContentClick(object sender, DataGridViewCellEventArgs e) // Hàm thao tác xóa hay chỉnh sửa thương hiệu trên bảng
        {
            if (e.ColumnIndex == 4 && e.RowIndex > -1)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa thương hiệu này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Delete from Brand where Id = " + dgvBrand[1, e.RowIndex].Value.ToString();

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Thương hiệu đã được xóa thành công !");
                }
                LoadBrand();
            }
            else if (e.ColumnIndex == 3 && e.RowIndex > -1)
            {
                BrandModule bm = new BrandModule(this);
                bm.lblId.Text = dgvBrand.Rows[e.RowIndex].Cells[1].Value.ToString();
                bm.txtBrandName.Text = dgvBrand.Rows[e.RowIndex].Cells[2].Value.ToString();

                bm.btnSave.Enabled = false;
                bm.btnUpdate.Enabled = true;
                bm.ShowDialog();
            }
            LoadBrand();
        }
    }
}
