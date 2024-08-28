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
    public partial class Discount : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Cashier cashier;

        public Discount(Cashier cashier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cashier = cashier;
            txtDiscount.Focus();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Discount_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e) // Hàm thêm giảm giá vào hóa đơn
        {
            try
            {
                double disc = double.Parse(txtTotalPrice.Text) * double.Parse(txtDiscount.Text) * 0.01;
                txtDiscountAmount.Text = disc.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                txtDiscountAmount.Text = "0.00";
            }
        }

        private void ConConfirm_Click(object sender, EventArgs e) // Hàm xác nhận thêm giảm giá
        {
            try
            {
                if (MessageBox.Show("Bạn có muốn thêm giảm giá không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Update Cart set Disc_percent = @Disc_percent where Transno = @Transno ";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Disc_percent", double.Parse(txtDiscount.Text) * 0.01);
                    command.Parameters.AddWithValue("@Transno", cashier.lblTransno.Text);
                    command.ExecuteNonQuery();

                    cashier.LoadCart();
                    this.Dispose();
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
    }
}
