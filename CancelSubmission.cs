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
    public partial class Void : Form
    {
        SqlConnection connection;
        SqlCommand command;
        CancelOrder cancelOrder;
        DailySale daily;

        public Void(CancelOrder cancelOrder,DailySale daily)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cancelOrder = cancelOrder;
            this.daily = daily;
        }

        

        public void SaveCancelOrder(string user)
        {
            try
            {
                txtUsername.Text = cancelOrder.txtCancel.Text;
                txtUsername.Enabled = false;
                connection.Open();
                string sql = "Insert into Cancel(Transno,Pcode,Price,Qty,Total,Sdate,CancelledBy,Reason,Action) values (@Transno,@Pcode,@Price,@Qty,@Total,@Sdate,@CancelledBy,@Reason,@Action)";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Transno", cancelOrder.txtTransno.Text);
                command.Parameters.AddWithValue("@Pcode", cancelOrder.txtPcode.Text);
                command.Parameters.AddWithValue("@Price", cancelOrder.txtPrice.Text);
                command.Parameters.AddWithValue("@Qty", cancelOrder.numCancel.Value);
                command.Parameters.AddWithValue("@Total", cancelOrder.txtTotal.Text);
                command.Parameters.AddWithValue("@Sdate", DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
                command.Parameters.AddWithValue("@CancelledBy", cancelOrder.txtCancel.Text);
                command.Parameters.AddWithValue("@Reason", cancelOrder.txtReasons.Text);
                command.Parameters.AddWithValue("@Action", cancelOrder.cboInventory.Text);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnVoid_Click_1(object sender, EventArgs e)
        {
            bool found = false;
            try
            {
                if (txtUsername.Text == cancelOrder.txtCancel.Text)
                {
                    MessageBox.Show("Yêu cầu nhập xác nhận tài khoản quản lý để hùy hóa đơn!", "Warning");
                    return;
                }

                string user;
                connection.Open();
                string sql1 = "Select * from UserAccount where Username = @Username and Password = @Password and Role = 'Quản lý'";
                command = new SqlCommand(sql1, connection);
                command.Parameters.AddWithValue("@Username", txtUsername.Text);
                command.Parameters.AddWithValue("@Password", txtPassword.Text);
                var reader = command.ExecuteReader();
               
                if (reader.Read())
                {
                    user = reader["Username"].ToString();
                    SaveCancelOrder(user);
                    found = true;
                }
                connection.Close();

                if (cancelOrder.cboInventory.Text == "Có")
                {
                    int product_qty = 0;
                    connection.Open();
                    string sql7 = "Select Quantity from Product where Pcode = @Pcode";
                    command = new SqlCommand(sql7, connection);
                    command.Parameters.AddWithValue("@Pcode", cancelOrder.txtPcode.Text);
                    var reader7 = command.ExecuteReader();
                    if (reader7.Read())
                    {
                        product_qty = int.Parse(reader7[0].ToString());
                    }
                    connection.Close();

                    connection.Open();
                    string sql2 = "Update Product set Quantity = @Qty where Pcode = @Pcode";
                    command = new SqlCommand(sql2, connection);
                    command.Parameters.AddWithValue("@Qty", product_qty + cancelOrder.numCancel.Value);
                    command.Parameters.AddWithValue("@Pcode", cancelOrder.txtPcode.Text);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                connection.Open();
                string sql3 = "Delete from Cart where Id = @Id";
                command = new SqlCommand(sql3, connection);
                command.Parameters.AddWithValue("@Id", cancelOrder.txtId.Text);
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Hủy đơn hàng thành công!", "Cancel Order");
                this.Dispose();
                cancelOrder.loadSoldList();
                daily.LoadSold();
                cancelOrder.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
