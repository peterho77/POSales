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
    public partial class DangNhap : Form
    {

        SqlConnection connection;
        SqlCommand command;
        public DangNhap()
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());
            LoadQuyenDangNhap();
        }

        private void LoadQuyenDangNhap() //Hàm thêm vào quyền truy cập
        {
            cmbchonquyen.Items.Clear();
            cmbchonquyen.Items.Add("Quản lý");
            cmbchonquyen.Items.Add("Nhân viên thu ngân");
            cmbchonquyen.SelectedIndex = 0;
        }


        private void btnDangNhap_Click(object sender, EventArgs e)  //Sự kiện nhấn nút đăng nhập 
        {
            string tentk = txtUsername.Text.Trim();
            string matkhau = txtPassword.Text.Trim();
            string quyen = cmbchonquyen.Text;

            if (string.IsNullOrEmpty(tentk))
            {
                MessageBox.Show("Vui Lòng Nhập Tên Tài Khoản !");
                return;
            }
            if (string.IsNullOrEmpty(matkhau))
            {
                MessageBox.Show("Vui Lòng Nhập Mật Khẩu !");
                return;
            }
            if (string.IsNullOrEmpty(quyen))
            {
                MessageBox.Show("Vui Lòng Chọn Quyền Truy Cập !");
                return;
            }

            try
            {
                using (connection = new SqlConnection(DataProvider.myConnection()))
                {
                    connection.Open();

                    // Kiểm tra tài khoản và mật khẩu
                    string sql = "SELECT * FROM UserAccount WHERE Username = @Username AND Password = @Password";
                    using (command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", tentk);
                        command.Parameters.AddWithValue("@Password", matkhau);

                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Tài khoản hoặc mật khẩu của bạn không chính xác");
                                txtPassword.Focus();
                                txtPassword.SelectAll();
                                return;
                            }
                        }
                    }

                    // Kiểm tra quyền truy cập
                    string sql1 = "SELECT * FROM UserAccount WHERE Username = @Username AND Password = @Password AND Role = @Role";
                    using (command = new SqlCommand(sql1, connection))
                    {
                        command.Parameters.AddWithValue("@Username", tentk);
                        command.Parameters.AddWithValue("@Password", matkhau);
                        command.Parameters.AddWithValue("@Role", quyen);

                        using (var reader1 = command.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                if (quyen == "Nhân viên thu ngân")
                                {
                                    Cashier cashier = new Cashier();
                                    cashier.lblName.Text = reader1["Name"].ToString() + " | " + reader1["Role"].ToString();
                                    cashier.lblUsername.Text = reader1["Username"].ToString();
                                    cashier.btnAddDiscount.Enabled = false;
                                    cashier.btnSearchProduct.Enabled = false;
                                    cashier.btnSettlePayment.Enabled = false;
                                    cashier.ShowDialog();
                                }
                                else if (quyen == "Quản lý")
                                {
                                    MainForm mainForm = new MainForm();
                                    mainForm.lblUsername.Text = reader1["Username"].ToString();
                                    mainForm._fullname = reader1["Name"].ToString();
                                    mainForm.ShowDialog();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Quyền truy cập không hợp lệ !");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)  //Sự kiện hiển thị mật khẩu dưới dạng kí tự
        {
            if (checkBox1.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }

        private void btnThoatdangnhap_Click(object sender, EventArgs e) //Sự kiện thoát đăng nhập
        {
            Application.Exit();
        }

        private void linkDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)  // Hàm nhấn click vào đăng ký
        {
            DangKy dangKy = new DangKy();
            dangKy.ShowDialog();
            this.Dispose();
        }
    }
}

