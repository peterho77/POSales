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
    public partial class SearchProduct : Form
    {
        SqlConnection connection;
        SqlCommand command;
        Cashier cashier;

        public SearchProduct(Cashier cashier)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            LoadProduct();
            this.cashier = cashier;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadProduct() //Hàm thực hiện chức năng tìm kiếm sản phẩm thông qua thanh tìm kiếm
        {
            int i = 0;

            dgvProduct.Rows.Clear();

            connection.Open();

            string sql = "Select p.Pcode, p.Barcode, p.Description, b.Brand, c.Category, p.Price, p.Quantity " +
                "from Product as p INNER JOIN Brand as b on p.Brand_ID = b.Id INNER JOIN Category as c on c.Id = p.Category_ID " +
                "where CONCAT(p.Description,b.Brand,c.Category) like '%" + txtSearch.Text + "%'";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), 
                    reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
            }

            connection.Close();
        }

        public int product_qty;
        public string pcode;

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            pcode = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
            product_qty = int.Parse(dgvProduct.Rows[e.RowIndex].Cells[7].Value.ToString());
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (product_qty == 0 && e.ColumnIndex == 8)
            {
                MessageBox.Show("Sản phẩm đã hết hàng");
                return;
            }
            if (colName == "Select")
            {
                Qty qty = new Qty(cashier,this);
                qty.ProductDetails(dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString(), double.Parse(dgvProduct.Rows[e.RowIndex].Cells[6].Value.ToString()), cashier.lblTransno.Text, int.Parse(dgvProduct.Rows[e.RowIndex].Cells[7].Value.ToString()));
                qty.ShowDialog();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    
    }
}
