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
    public partial class ProductStockIn : Form
    {
        SqlConnection connection;
        SqlCommand command;
        InStock stockin;
        Adjustment adjust;
        string parent_form = "";

        public ProductStockIn(InStock stockin)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            parent_form = "instock";
            this.stockin = stockin;
            LoadProduct();
        }

        public ProductStockIn(Adjustment adjust)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            parent_form = "adjustment";
            this.adjust = adjust;
            LoadProduct();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadProduct()
        {
            int i = 0;

            dgvProduct.Rows.Clear();

            connection.Open();

            string sql = "Select Pcode, Description, Quantity from Product where Description like '%" + txtSearch.Text + "%'";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString());
            }

            connection.Close();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;

            if (colName == "Select")
            {
                if(parent_form == "instock")
                {
                    if (stockin.txtStockInBy.Text == string.Empty)
                    {
                        MessageBox.Show("Vui lòng nhập tên người kiểm kê hàng hóa", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Dispose();
                        stockin.txtStockInBy.Focus();
                        return;
                    }

                    if (MessageBox.Show("Bạn có muốn thêm mục này không !", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            connection.Open();

                            string sql = "Insert into Stock(Refno,Pcode,Stock_date,Stockinby,Supplier_ID) Values (@Refno,@Pcode,@Sdate,@StockInBy,@Supplier_ID)";

                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@Pcode", dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString());
                            command.Parameters.AddWithValue("@Refno", stockin.txtReference.Text);
                            command.Parameters.AddWithValue("@Sdate", stockin.dtStockIn.Value);
                            command.Parameters.AddWithValue("@StockInBy", stockin.txtStockInBy.Text);
                            command.Parameters.AddWithValue("@Supplier_ID", int.Parse(stockin.lblId.Text));
                            command.ExecuteNonQuery();

                            stockin.LoadInStock();
                        }
                        catch
                        {
                            MessageBox.Show("Sản phẩm này đã được thêm vào trong kho nhập hàng. Vui lòng chọn sản phẩm khác");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                else if (parent_form == "adjustment")
                {
                    adjust.txtPcode.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
                    adjust.txtDescription.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
                    adjust.txtProductQty.Text = dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString();
                    adjust.product_qty = int.Parse(dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString());
                }
            }
        }

        

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {
            LoadProduct();
        }
    }
}
