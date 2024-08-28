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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace POSales
{
    public partial class ResetPassword : Form
    {
        SqlConnection connection;
        SqlCommand command;
        UserAccount useraccount;

        public ResetPassword(UserAccount userAccount)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.useraccount = userAccount;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtNewPass.Text == txtConfirmPass.Text)
            {
                if (MessageBox.Show(" Bạn có chắc chắn đặt lại mật khẩu không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();
                    string sql = "Update UserAccount set Password = @Password where Username = @Username";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Password", txtNewPass.Text);
                    command.Parameters.AddWithValue("@Username", useraccount.username);
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Mật khẩu đã được đặt lại thành công!", "Reset Password");
                    this.Dispose();
                }
            }
            else
            {
                MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không khớp !");
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
