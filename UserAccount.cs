using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSales
{
    public partial class UserAccount : Form
    {
        SqlConnection connection;
        SqlCommand command;
        MainForm main;
        public string username;
        string name;
        string role;
        string status;

        public UserAccount(MainForm main)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.main = main;
            LoadUser();
            LoadUsername();
        }

        public void Clear()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtRePassword.Clear();
            txtFullName.Clear();
            cboRole.Text = "";
        }

        private void btnAccSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text == txtRePassword.Text) {
                    connection.Open();

                    string sql = "Insert into UserAccount(Username,Password,Role,Name) Values (@Username,@Password,@Role,@Name)";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Username", txtUsername.Text);
                    command.Parameters.AddWithValue("@Password", txtPassword.Text);
                    command.Parameters.AddWithValue("@Role", cboRole.Text);
                    command.Parameters.AddWithValue("@Name", txtFullName.Text);
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Tài khoản mới đã được lưu thành công", "POS");
                    LoadUser();
                    Clear();
                }
                else
                {
                    MessageBox.Show("Mật khẩu không khớp", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAccCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void LoadUser()
        {
            int i = 0;

            dgvUser.Rows.Clear();

            connection.Open();

            string sql = "Select * from UserAccount";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvUser.Rows.Add(i, reader[0].ToString(), reader[3].ToString(), reader[4].ToString(), reader[2].ToString());
            }

            connection.Close();
        }

        public void LoadUsername()
        {
            cboUser.Items.Clear();
            cboUser.DataSource = DataProvider.getTable("Select * from UserAccount where Role = 'Admin'");
            cboUser.DisplayMember = "Username";
            cboUser.ValueMember = "Username";
        }

        private void btnPassSave_Click(object sender, EventArgs e)
        {
            if (txtCurrentPass.Text != main._password)
            {
                MessageBox.Show("Mật khẩu hiện tại không khớp!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(txtNewPass.Text != txtRetypePass.Text)
            {
                MessageBox.Show("Xác nhận mật khẩu mới không khớp!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            connection.Open();
            string sql = "Update UserAccount set Password = @Password where Username = @Username";
            command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Password", txtNewPass.Text);
            command.Parameters.AddWithValue("@Username", cboUser.Text);
            command.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Mật khẩu đã được thay đổi thành công!", "Password");
        }

        private void btnPassCancel_Click(object sender, EventArgs e)
        {
            ClearPass();
        }

        public void ClearPass()
        {
            txtCurrentPass.Clear();
            txtNewPass.Clear();
            txtRetypePass.Clear();
        }

        private void dgvUser_SelectionChanged(object sender, EventArgs e)
        {
            int i = dgvUser.CurrentRow.Index;
            username = dgvUser[1, i].Value.ToString();
            name = dgvUser[2, i].Value.ToString();
            role = dgvUser[4, i].Value.ToString();
            status = dgvUser[3, i].Value.ToString();
            if (cboUser.Text == username)
            {
                btnRemove.Enabled = false;
                btnResetPass.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = true;
                btnResetPass.Enabled = true;
            }
            gbUser.Text = "Password for " + username;
            lblAccNote.Text = "To change password for " + username + ", click Reset Password";
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa tài khoản người dùng này không " + username + " ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                connection.Open();
                string sql = "Delete from UserAccount where Username = @Username";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.ExecuteNonQuery();
                MessageBox.Show("Tài khoản đã được xóa thành công");
                LoadUser();
            }
        }

        private void btnResetPass_Click(object sender, EventArgs e)
        {
            ResetPassword reset = new ResetPassword(this);
            reset.ShowDialog();
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            UserProperties property = new UserProperties(this);
            property.txtFullname.Text = name;
            property.cboRole.Text = role;
            property.cboActivate.Text = status;
            property.ShowDialog();
        }

        private void UserAccount_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
