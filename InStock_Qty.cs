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
    public partial class InStock_Qty : Form
    {
        SqlConnection connection;
        SqlCommand command;
        InStock new_stock;
        public string pcode;
        public string refno;
        public InStock_Qty(InStock stock)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.new_stock = stock;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                string sql = "Update Stock set Qty = @Qty where Refno = '" + refno + "' and Pcode = '" + pcode + "'";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Qty", numStockQty.Value);
                command.ExecuteNonQuery();              
                connection.Close();
                new_stock.LoadInStock();
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                connection.Close();
            }
        }
    }

}
