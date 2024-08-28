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
    public partial class Settle : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Cashier cashier;

        public Settle(Cashier cashier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.cashier = cashier;
        }

        private void btnClear_Click(object sender, EventArgs e)  // Nút clear số tiền thực hiện thanh toán
        {
            txtCash.Clear();
        }

        private void btnOne_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnOne.Text;
        }

        private void btnTwo_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnTwo.Text;
        }

        private void btnThree_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnThree.Text;
        }

        private void btnFour_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnFour.Text;
        }

        private void btnFive_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnFive.Text;
        }

        private void btnSix_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnSix.Text;
        }

        private void btnSeven_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnSeven.Text;
        }

        private void btnEight_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnEight.Text;
        }

        private void btnNine_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnNine.Text;
        }

        private void btnZero_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnZero.Text;
        }

        private void btnDZero_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnDZero.Text;
        }

        private void txtCash_TextChanged(object sender, EventArgs e) //Hàm tự động tính tiền thừa khi thanh toán hóa đơn
        {
            try
            {
                double charge = double.Parse(txtCharge.Text);
                double cash = double.Parse(txtCash.Text);
                double change = cash - charge;
                txtChange.Text = change.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                txtChange.Text = "0.00";
            }
        }

        private void Settle_KeyDown(object sender, KeyEventArgs e) // Hàm cho phép form thực hiện nút Enter để thanh toán và nút Escape để thoát
        {
            if (e.KeyCode == Keys.Escape) this.Dispose();
            else if (e.KeyCode == Keys.Enter) btnEnter.PerformClick();
        }

        private void btnEnter_Click(object sender, EventArgs e) // Nút Enter thực hiện chức năng thanh toán hóa đơn
        {
            try
            {
                if ((double.Parse(txtChange.Text) < 0) || (txtCash.Text == ""))
                {
                    MessageBox.Show("Số tiền không đủ, vui lòng nhập đúng số tiền");
                }
                else
                {
                    connection.Open();

                    for (int i = 0; i < cashier.dgvCart.Rows.Count; i++)
                    { 
                        string sql = "Update Cart set Status = 'Sold' where Id = " + cashier.dgvCart.Rows[i].Cells[1].Value.ToString();
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();                        
                    }

                    connection.Close();

                    Receipt receipt = new Receipt(cashier);
                    receipt.LoadReceipt(txtCash.Text, txtChange.Text);
                    receipt.ShowDialog();

                    MessageBox.Show("Đã lưu thanh toán thành công!", "Payment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Dispose();
                    cashier.getTranNo();
                    cashier.LoadCart();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
