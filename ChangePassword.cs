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
    public partial class ChangePassword : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Cashier cashier;

        public ChangePassword(Cashier cashier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cashier = cashier;
            lblUsername.Text = cashier.lblUsername.Text;
        }

        private void picClose_Click(object sender, EventArgs e)  // Nút đóng form thay đổi MK
        {
            this.Dispose();
        }

        private void btnNext_Click(object sender, EventArgs e) // Nút thực hiện chức năng xác nhận MK cũ và hiện thay đổi MK mới
        {
            try
            {
                string current_pass = DataProvider.getPassword(lblUsername.Text);
                if (current_pass != txtCurrentPass.Text)
                {
                    MessageBox.Show("Mật khẩu sai , Vui lòng thử lại");
                }
                else
                {
                    txtNewPass.Visible = true;
                    txtCurrentPass.Visible = true;
                    btnSave.Visible = true;
                    btnNext.Visible = false;
                    txtCurrentPass.Visible = false;
                    txtConfirmPass.Visible = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)  //Hàm lưu mật khẩu mới của tài khoản
        {
            try
            {
                if(txtConfirmPass.Text != txtNewPass.Text)
                {
                    MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không khớp , Vui lòng thử lại");
                }
                else
                {
                    connection.Open();
                    string sql = "Update UserAccount set Password = '" + txtNewPass.Text + "' where Username = '" + lblUsername.Text + "'";
                    command = new SqlCommand(sql,connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Mật khẩu đã được cập nhật thành công");
                    this.Dispose();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
