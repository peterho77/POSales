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
    public partial class Category : Form
    {
        SqlConnection connection;
        SqlCommand command;

        public Category()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            //connection.Open();

            LoadCategory();
        }

        public void LoadCategory()
        {
            int i = 0;

            dgvCategory.Rows.Clear();

            connection.Open();

            string sql = "Select * from Category";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvCategory.Rows.Add(i, reader["Id"].ToString(), reader["Category"].ToString());
            }

            connection.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CategoryModule cm = new CategoryModule(this);
            cm.btnUpdate.Enabled = false;
            cm.ShowDialog();
        }

        private void dgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == 4)
            {
                if (MessageBox.Show("Bạn có muốn xóa thương hiệu này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Delete from Category where Id = " + dgvCategory[1, e.RowIndex].Value.ToString();

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Thương hiệu đã được xóa thành công");
                }
                LoadCategory();
            }
            else if (e.RowIndex > -1 && e.ColumnIndex == 3)
            {
                CategoryModule cm = new CategoryModule(this);
                cm.lblId.Text = dgvCategory[1, e.RowIndex].Value.ToString();
                cm.txtCategoryName.Text = dgvCategory[2, e.RowIndex].Value.ToString();

                cm.btnSave.Enabled = false;
                cm.btnUpdate.Enabled = true;
                cm.ShowDialog();
            }
            LoadCategory();
        }
    }
}
