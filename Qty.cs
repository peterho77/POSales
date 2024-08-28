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
    public partial class Qty : Form
    {
        SqlConnection connection;
        SqlCommand command;
        private bool searchProduct = false;
        private string Pcode;
        private double Price;
        private string Transno;
        private int Quantity;
        Cashier cashier;
        SearchProduct sp;

        public Qty(Cashier cashier,SearchProduct sp)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cashier = cashier;
            this.sp = sp;
            searchProduct = true;
        }

        public Qty(Cashier cashier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cashier = cashier;
        }

        public void ProductDetails(string pcode, double price, string transno, int quantity)
        {
            this.Pcode = pcode;
            this.Price = price;
            this.Transno = transno;
            this.Quantity = quantity;
        }
        public int cart_qty = 0;
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == 13 && txtQty.Text != string.Empty) {

                if (int.Parse(txtQty.Text) > 0 && int.Parse(txtQty.Text) <= Quantity)
                {
                    string _id = "";
                    
                    bool found = false;

                    connection.Open();

                    string sql = "Select * from Cart where Transno = @Transno and Pcode = @Pcode";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Transno", Transno);
                    command.Parameters.AddWithValue("@Pcode", Pcode);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        found = true;
                        _id = reader["Id"].ToString();
                        cart_qty = int.Parse(reader["Qty"].ToString());
                    }

                    connection.Close();

                    if (found)
                    {
                        connection.Open();

                        string sql1 = "Update Cart set Qty = @Qty where Id = @Id";
                        command = new SqlCommand(sql1, connection);
                        command.Parameters.AddWithValue("@Qty", int.Parse(txtQty.Text));
                        command.Parameters.AddWithValue("@Id", _id);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                    else
                    {
                        connection.Open();

                        string sql2 = "Insert into Cart(Transno,Pcode,Price,Qty,Sdate,Cashier) Values (@Transno,@Pcode,@Price,@Qty,@Sdate,@Cashier)";

                        command = new SqlCommand(sql2, connection);
                        command.Parameters.AddWithValue("@Pcode", Pcode);
                        command.Parameters.AddWithValue("@Transno", Transno);
                        command.Parameters.AddWithValue("@Price", Price);
                        command.Parameters.AddWithValue("@Qty", int.Parse(txtQty.Text));
                        command.Parameters.AddWithValue("@Sdate", DateTime.Now);
                        command.Parameters.AddWithValue("@Cashier", cashier.lblUsername.Text);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }

                    connection.Open();

                    string sql3 = "Update Product set Quantity = @Quantity where Pcode = @Pcode";
                    command = new SqlCommand(sql3, connection);
                    command.Parameters.AddWithValue("@Pcode", Pcode);
                    command.Parameters.AddWithValue("@Quantity", (Quantity + cart_qty - int.Parse(txtQty.Text)));
                    command.ExecuteNonQuery();

                    connection.Close();

                    if (searchProduct)
                    {
                        sp.LoadProduct();
                    }
                    cashier.LoadCart();
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Không thể tiếp tục. Số lượng còn lại là : " + Quantity.ToString());
                    txtQty.Focus();
                    txtQty.Text = string.Empty;
                }
            }
        }
    }
}
