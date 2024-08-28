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
    public partial class UserProperties : Form
    {
        SqlConnection connection;
        SqlCommand command;
        UserAccount user;

        public UserProperties(UserAccount user)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.user = user;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                string sql = "Update UserAccount set Name = @Name, Role = @Role, isActivate = @Activate where Username = @Username";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", txtFullname.Text);
                command.Parameters.AddWithValue("@Role", cboRole.Text);
                command.Parameters.AddWithValue("@Activate", cboActivate.Text);
                command.Parameters.AddWithValue("@Username", user.username);
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Thuộc tính tài khoản đã thay đổi thành công!", "Update");
                this.Dispose();
                user.LoadUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
