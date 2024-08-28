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
    public partial class CancelOrder : Form
    {
        SqlConnection connection;
        SqlCommand command;
        DailySale dailysale;

        public CancelOrder(DailySale dailysale)
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());
            this.dailysale = dailysale;
        }

        public void LoadInventory()
        {
            cboInventory.SelectedIndex = 0;
            cboInventory.Items.Add("Có");
            cboInventory.Items.Add("Không");
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboInventory.Text != string.Empty && numCancel.Value > 0 && txtReasons.Text != string.Empty)
                {
                    if (int.Parse(txtQty.Text) >= numCancel.Value)
                    {
                        Void cancel = new Void(this, this.dailysale);
                        cancel.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Số lượng hàng hóa cần hủy không được lớn hơn số sản phẩm trong hóa đơn");
                        numCancel.Focus();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void loadSoldList()
        {
            dailysale.LoadSold();
        }

        private void cboInventory_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
