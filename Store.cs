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
    public partial class Store : Form
    {
        SqlConnection connection;
        SqlCommand command;
        bool store_infor = false;

        public Store()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
        }

        public void LoadStore()
        {
            try
            {
                connection.Open();
                string sql = "Select * from Store";
                command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    store_infor = true;
                    txtStorename.Text = reader["Store"].ToString();
                    txtAddress.Text = reader["Address"].ToString();
                }
                else
                {
                    store_infor = false;
                    txtStorename.Clear();
                    txtAddress.Clear();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn lưu chi tiết cửa hàng không ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if(store_infor)
                    {
                        connection.Open();
                        string sql = "Update Store set Store = @Store, Address = @Address";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@Store", txtStorename.Text);
                        command.Parameters.AddWithValue("@Address", txtAddress.Text);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    else
                    {
                        connection.Open();
                        string sql = "Insert into Store(Store,Address) values (@Store, @Address)";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@Store", txtStorename.Text);
                        command.Parameters.AddWithValue("@Address", txtAddress.Text);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    MessageBox.Show("Chi tiết cửa hàng đã được lưu thành công.", "Save Record",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Store_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
