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
    public partial class DangKy : Form
    {
        SqlConnection connection;
        SqlCommand command;

        public DangKy()
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());

            // Thêm quyền vào combobox "Quyền truy cập"
            cboRole.Items.Add("Quản lý"); 
            cboRole.Items.Add("Nhân viên thu ngân");
        }

        public bool checkAccount(string ac) //Hàm kiểm tra tài khoản
        {
            return Regex.IsMatch(ac, "^[a-zA-Z0-9]{6,24}$"); //Tài khoản phải có 6 chữ trở lên hoặc nhỏ hơn 24 ký tự
        }

        public bool checkEmail(string em) //Hàm kiểm tra email
        {
            return Regex.IsMatch(em, @"^[a-zA-Z0-9_.]{3,20}@gmail.com(.vn|)$"); //Gmail (.vn|) có thể thêm hoặc không
        }


        ModifyAccount modify = new ModifyAccount();

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            string tentk = txtTaiKhoanDangKy.Text.Trim();
            string matkhau = txtMatKhauDangKy.Text.Trim();
            string xacnhanmatkhau = txtXacNhanMatKhauDangKy.Text.Trim();
            string quyenTruyCap = cboRole.Text;
            string email = txtEmailDangKy.Text.Trim();
            string hoten = txtTenDangKy.Text.Trim();

            // Kiểm tra thông tin đăng ký
            if (!checkAccount(tentk))
            {
                MessageBox.Show("Vui Lòng Nhập Tên Tài Khoản 6 đến 24 ký tự, với các ký tự chữ và số.");
                return;
            }
            if (!checkAccount(matkhau))
            {
                MessageBox.Show("Vui Lòng Nhập Mật Khẩu 6 đến 24 ký tự, với các ký tự chữ và số.");
                return;
            }
            if (xacnhanmatkhau != matkhau)
            {
                MessageBox.Show("Vui Lòng Xác Nhận Mật Khẩu Chính Xác");
                return;
            }
            if (!checkEmail(email))
            {
                MessageBox.Show("Vui Lòng Nhập Đúng Định Dạng Email");
                return;
            }

            try
            {
                using (connection = new SqlConnection(DataProvider.myConnection()))
                {
                    connection.Open();
                    // Kiểm tra xem email đã tồn tại chưa
                    string checkEmailQuery = "SELECT COUNT(*) FROM UserAccount WHERE Email = @Email";
                    using (command = new SqlCommand(checkEmailQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int emailCount = (int)command.ExecuteScalar();

                        if (emailCount > 0)
                        {
                            MessageBox.Show("Email này đã được đăng ký. Vui lòng sử dụng email khác.");
                            return;
                        }
                    }

                    // Thực hiện thêm tài khoản mới
                    string query = "INSERT INTO UserAccount (Username, Password, Role, Name, isActivate, Email) VALUES (@Username, @Password, @Role, @Name, 'Đang hoạt động', @Email)";
                    using (command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", tentk);
                        command.Parameters.AddWithValue("@Password", matkhau);
                        command.Parameters.AddWithValue("@Role", quyenTruyCap);
                        command.Parameters.AddWithValue("@Name", hoten);
                        command.Parameters.AddWithValue("@Email", email);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Tài khoản đã được đăng ký thành công");
                    this.Close();

                    // Mở form đăng nhập
                    DangNhap dangNhapForm = new DangNhap();
                    dangNhapForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng ký: " + ex.Message);
            }
            }
        }
    }
