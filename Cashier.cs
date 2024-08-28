using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using DarrenLee.Media;
using AForge.Video.DirectShow;
using ZXing;
using AForge.Video;
using AForge;

namespace POSales
{
    public partial class Cashier : Form
    {
        SqlConnection connection;
        SqlCommand command;

        public Cashier()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            Reset();
        }
        private void Cashier_Load(object sender, EventArgs e)
        {
            
        }

        public void LoadCart()
        {
            dgvCart.Rows.Clear();
            bool cart = false;
            double total = 0;
            double discount = 0;

            connection.Close();
            connection.Open();

            string sql = "Select c.Id, c.Pcode, p.Description, c.Price, c.Qty, c.Disc, c.Total, c.Transno from Cart as c INNER JOIN Product as p on c.Pcode = p.Pcode where c.Transno = @Transno and c.Status = 'Pending'";           
            command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Transno", lblTransno.Text);
            var reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                i++;
                total += double.Parse(reader["Total"].ToString());
                discount += double.Parse(reader["Disc"].ToString());
                dgvCart.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), double.Parse(reader[6].ToString()).ToString("#,##0.00"), reader[7].ToString());
                cart = true;
            }

            connection.Close();

            lblSaleTotal.Text = total.ToString("#,##0.00");
            lblDiscount.Text = discount.ToString("#,##0.00");

            getCartTotal();

            if (cart)
            {
                btnAddDiscount.Enabled = true;
                btnClearCart.Enabled = true;
                btnSettlePayment.Enabled = true;
            }
            else
            {
                btnAddDiscount.Enabled = false;
                btnClearCart.Enabled = false;
                btnSettlePayment.Enabled = false;
            }
        }

        public void getCartTotal()
        {
            double discount = double.Parse(lblDiscount.Text);
            double sales = double.Parse(lblSaleTotal.Text);
            double vat = sales * 0.12;
            double vatable = sales + vat;

            lblVat.Text = vat.ToString("#,##0.00");
            lblVatable.Text = vatable.ToString("#,##0.00");
            lblDisplayTotal.Text = vatable.ToString("#,##0.00");
        }



        public void Reset()
        {
            lblDate.Text = "0000000000000000000";
            lblDiscount.Text = "0.00";
            lblSaleTotal.Text = "0.00";
            lblVat.Text = "0.00";
            lblVatable.Text = "0.00";
            lblDisplayTotal.Text = "0.00";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        public int check_Transno(int n) // hàm check số chữ số
        {
            int count = 0;
            while (n != 0)
            {
                n /= 10;
                count++;
            }
            return count;
        }

        public void getTranNo() // Hàm cập nhật số giao dịch hóa đơn tự động tăng lên mỗi khi tạo hóa đơn mới hoặc thanh toán hóa đơn xong
        {
            int count;
            string sdate = DateTime.Now.ToString("yyyyMMdd");
            string transno;

            connection.Open();

            string sql = "Select top 1 Transno from Cart where Transno like '" + sdate + "%' ORDER BY Id desc";
            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                transno = reader[0].ToString();
                int check = check_Transno(int.Parse(transno.Substring(8, 5)));
                count = int.Parse(transno.Substring(13 - check, check));
                long n = long.Parse(sdate) * 100000 + count + 1;
                lblTransno.Text = n.ToString();
            }
            else
            {
                transno = sdate + "00001";
                lblTransno.Text = transno;
            }

            connection.Close();
        }

        public int id;
        public double total;
        public double discount;
        private string transno;
        private int cart_qty;
        

        private void dgvCart_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCart.Rows.Count == 0)
            {
                Reset();

                btnAddDiscount.Enabled = false;
                btnClearCart.Enabled = false;
                btnSettlePayment.Enabled = false;
            }
            else
            {
                int i = dgvCart.CurrentRow.Index;

                id = int.Parse(dgvCart[1, i].Value.ToString());
                total = double.Parse(dgvCart[7, i].Value.ToString());
                discount = double.Parse(dgvCart[6, i].Value.ToString());
                transno = dgvCart[8, i].Value.ToString();
                cart_qty = int.Parse(dgvCart[5, i].Value.ToString());

                lblSaleTotal.Text = total.ToString("#,##0.00");
                lblDiscount.Text = discount.ToString("#,##0.00");
                lblTransno.Text = transno;

                getCartTotal();
            }
        }

        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string pcode = dgvCart[2, e.RowIndex].Value.ToString();
            string s = "";
            int i = dgvCart.CurrentRow.Index;
            string sql = "select sum(Quantity) as 'Quantity' from Product where Pcode = '" + pcode + "' GROUP BY Pcode";

            connection.Open();

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                s = reader["Quantity"].ToString();
            }

            connection.Close();

            int product_qty = int.Parse(s);
            int cart_qty = int.Parse(dgvCart.Rows[e.RowIndex].Cells[5].Value.ToString());

            if (e.RowIndex > -1 && e.ColumnIndex == 10)
            {
                if (MessageBox.Show("Remove this item ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql1 = "Delete from Cart where Id = " + id;
                    command = new SqlCommand(sql1, connection);
                    command.ExecuteNonQuery();

                    string sql2 = "Update Product set Quantity = @Qty where Pcode = @Pcode";
                    command = new SqlCommand(sql2, connection);
                    command.Parameters.AddWithValue("@Qty", product_qty + cart_qty);
                    command.Parameters.AddWithValue("@Pcode", pcode);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Tất cả các mục đã được xóa thành công", "Remove Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCart();
                }
            }
            else if (e.RowIndex > -1 && e.ColumnIndex == 9)
            {
                connection.Open();

                if (product_qty > 0)
                {
                    Qty new_qty = new Qty(this);
                    new_qty.cart_qty = cart_qty;
                    new_qty.ProductDetails(dgvCart.Rows[e.RowIndex].Cells[2].Value.ToString(), double.Parse(dgvCart.Rows[e.RowIndex].Cells[4].Value.ToString()), dgvCart.Rows[e.RowIndex].Cells[2].Value.ToString(), product_qty);
                    new_qty.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Sản phẩm đã hết hàng. Số hàng còn lại là " + product_qty);
                    return;
                }

                connection.Close();
            }
        }

        public void txtSearchTrans_TextChanged(object sender, EventArgs e) //Hàm tìm kiếm số hóa đơn chưa hoàn tất thanh toán
        {
            total = 0;
            discount = 0;
            bool cart = false;
            dgvCart.Rows.Clear();

            connection.Open();

            string sql = "Select c.Id, c.Pcode, p.Description, c.Price, c.Qty, c.Disc, c.Total, c.Transno from Cart as c INNER JOIN Product as p on c.Pcode = p.Pcode where c.Transno like '%" + txtSearchTransno.Text + "%' and c.Status = 'Pending'";
            command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Transno", txtSearchTransno.Text);
            var reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                i++;
                total += double.Parse(reader["Total"].ToString());
                discount += double.Parse(reader["Disc"].ToString());
                dgvCart.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), double.Parse(reader[6].ToString()).ToString("#,##0.00"), reader[7].ToString());
                cart = true;
            }

            if (cart)
            {
                lblSaleTotal.Text = total.ToString("#,##0.00");
                lblDiscount.Text = discount.ToString("#,##0.00");
                getCartTotal();
                btnSearchProduct.Enabled = true;
                btnAddDiscount.Enabled = true;
                btnClearCart.Enabled = true;
                btnSettlePayment.Enabled = true;
            }
            else
            {
                btnSearchProduct.Enabled = true;
                btnAddDiscount.Enabled = false;
                btnClearCart.Enabled = false;
                btnSettlePayment.Enabled = false;
            }
            connection.Close();
        }

      

        private void btnAddDiscount_Click_1(object sender, EventArgs e)
        {
            Discount dis = new Discount(this);
            dis.lblId.Text = id.ToString();
            if (discount > 0)
            {
                dis.txtTotalPrice.Text = (double.Parse(lblDiscount.Text) + double.Parse(lblSaleTotal.Text)).ToString();
            }
            else
            {
                dis.txtTotalPrice.Text = lblSaleTotal.Text;
            }

            dis.ShowDialog();
        }

        private void btnNewTransaction_Click_1(object sender, EventArgs e)
        {
            dgvCart.Rows.Clear();
            Reset();
            getTranNo();
            btnSearchProduct.Enabled = true;
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            SearchProduct sp = new SearchProduct(this);
            sp.LoadProduct();
            sp.ShowDialog();
        }

        private void btnSettlePayment_Click(object sender, EventArgs e)
        {
            Settle settle = new Settle(this);
            settle.txtCharge.Text = lblDisplayTotal.Text;
            settle.ShowDialog();
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(" Xóa tất cả các mặt hàng khỏi giỏ hàng ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                connection.Open();

                string sql = "Delete from Cart where Transno like '" + lblTransno.Text + "'";
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Tất cả các mục đã được xóa thành công", "Remove Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
                           
                for (int i = 0; i < dgvCart.Rows.Count; i++)
                {
                    connection.Open();
                    int product_qty = 0;
                    string sql2 = "Select * from Product where Pcode = '" + dgvCart.Rows[i].Cells[2].Value.ToString() + "'";
                    command = new SqlCommand(sql2, connection);
                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        product_qty = int.Parse(reader["Quantity"].ToString());
                    }
                    connection.Close();

                    //update product quantity
                    connection.Open();
                    int new_product_qty = product_qty + int.Parse(dgvCart[5, i].Value.ToString());
                    string sql1 = "Update Product set Quantity = " + new_product_qty.ToString() + " where Pcode like '" + dgvCart.Rows[i].Cells[2].Value.ToString() + "'";
                    command = new SqlCommand(sql1, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                LoadCart();
            }
        }

        private void btnDailySales_Click(object sender, EventArgs e)
        {            
            DailySale dailysale = new DailySale();
            dailysale.cancel_cashier = lblName.Text;
            dailysale.dtFrom.Enabled = false;
            dailysale.dtTo.Enabled = false;
            dailysale.cboCashier.Text = lblUsername.Text;
            dailysale.cboCashier.Enabled = false;
            dailysale.picClose.Enabled = true;
            dailysale.lblTitle.Enabled = true;
            dailysale.ShowDialog();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            ChangePassword change = new ChangePassword(this);
            change.ShowDialog();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
                 "Bạn có chắc chắn muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
             MessageBoxIcon.Question);             
           
            if (dialogResult == DialogResult.Yes)
            {
                DangNhap formDangNhap = new DangNhap();
                this.Hide();
                formDangNhap.ShowDialog();
                this.Show();
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }
    }
}
