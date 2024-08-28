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
    public partial class Supplier : Form
    {
        SqlConnection connection;
        SqlCommand command;

        public Supplier()
        {
            InitializeComponent();

            connection = new SqlConnection(DataProvider.myConnection());

            LoadSupplier();
        }

        public void LoadSupplier()
        {
            int i = 0;

            dgvSupplier.Rows.Clear();

            connection.Open();

            string sql = "Select * from Supplier";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvSupplier.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
            }

            connection.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SupplierModule sm = new SupplierModule(this);
            sm.btnSave.Enabled = true;
            sm.btnUpdate.Enabled = false;
            sm.ShowDialog();
        }

        private void dgvSupplier_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvSupplier.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Bạn có muốn xóa nhà cung cấp này không ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    connection.Open();

                    string sql = "Delete from Supplier where Id = '" + dgvSupplier[1, e.RowIndex].Value.ToString() + "'";

                    command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Nhà cung cấp đã được xóa thành công");
                }
            }
            else if (colName == "Edit")
            {
                SupplierModule sm = new SupplierModule(this);

                sm.txtSupplierName.Text = dgvSupplier[2, e.RowIndex].Value.ToString();
                sm.txtAddress.Text = dgvSupplier[3, e.RowIndex].Value.ToString();
                sm.txtContactPerson.Text = dgvSupplier[4, e.RowIndex].Value.ToString();
                sm.txtPhone.Text = dgvSupplier[5, e.RowIndex].Value.ToString();
                sm.txtEmail.Text = dgvSupplier[6, e.RowIndex].Value.ToString();
                sm.txtFax.Text = dgvSupplier[7, e.RowIndex].Value.ToString();
                sm.lblId.Text = dgvSupplier[1, e.RowIndex].Value.ToString();

                sm.btnUpdate.Enabled = true;
                sm.btnSave.Enabled = false;
                sm.ShowDialog();
            }
            LoadSupplier();
        }

        

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void Supplier_Load(object sender, EventArgs e)
        {

        }
    }
}
