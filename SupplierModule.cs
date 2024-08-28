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
    public partial class SupplierModule : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Supplier supplier;


        public SupplierModule(Supplier supplier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.supplier = supplier;
        }

        public void Clear()  // Hàm reset textbox
        {
            txtSupplierName.Clear();
            txtAddress.Clear();
            txtContactPerson.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtFax.Clear();

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtSupplierName.Focus();
        }

        private void picClose_Click(object sender, EventArgs e) // Hàm đóng chương trình
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e) // Nút thực hiện sư kiện mở form lưu thông tin
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn lưu nhà cung cấp này không  ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Insert into Supplier(Supplier,Address,ContactPerson,Phone,Email,Fax) Values (@Supplier,@Address,@ContactPerson,@Phone,@Email,@Fax)";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Supplier", txtSupplierName.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@ContactPerson", txtContactPerson.Text);
                    command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.Parameters.AddWithValue("@Fax", txtFax.Text);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Lưu thành công", "POS");
                    Clear();
                    supplier.LoadSupplier();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)  // Nút thực hiện sự kiện mở form cập nhật thông tinS
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn cập nhật nhà cung cấp này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Update Supplier set Supplier = @Supplier, Address = @Address, ContactPerson = @ContactPerson, Phone = @Phone, Email = @Email, Fax = @Fax where Id = @Id";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Id", lblId.Text);
                    command.Parameters.AddWithValue("@Supplier", txtSupplierName.Text);
                    command.Parameters.AddWithValue("@Address", txtAddress.Text);
                    command.Parameters.AddWithValue("@ContactPerson", txtContactPerson.Text);
                    command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.Parameters.AddWithValue("@Fax", txtFax.Text);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Nhà cung cấp đã được cập nhật thành công");

                    Clear();

                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
