using Microsoft.ReportingServices.Diagnostics.Internal;
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
using Zen.Barcode;
using System.Drawing.Imaging;
namespace POSales
{
    public partial class Barcode : Form
    {
        SqlConnection connection;
        SqlCommand command;
        string fname;

        public Barcode()
        {
            InitializeComponent();
            connection = new SqlConnection(DataProvider.myConnection());
            LoadProduct();
            
        }

        public void LoadProduct() // Hàm hiển thị sản phẩm lên bảng 
        {
            int i = 0;

            dgvBarcode.Rows.Clear();

            connection.Open();

            string sql = "Select p.Pcode, p.Barcode, p.Description, b.Brand, c.Category, p.Price, p.Quantity from Product as p INNER JOIN Brand as b on p.Brand_ID = b.Id INNER JOIN Category as c on c.Id = p.Category_ID where CONCAT(p.Description,b.Brand,c.Category) like '%" + txtSearch.Text + "%'";

            command = new SqlCommand(sql, connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                i++;
                dgvBarcode.Rows.Add(i, reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString());
            }

            connection.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void dgvBarcode_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvBarcode.Columns[e.ColumnIndex].Name;
            if (colName == "Select")
            {
                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                picBarcode.Image = barcode.Draw(dgvBarcode.Rows[e.RowIndex].Cells[2].Value.ToString(),90,3);
                fname = dgvBarcode.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = fname;
            saveFile.Filter = "Image File(*.jpg,*.png)|*.png,*.jpg";
            ImageFormat image = ImageFormat.Png;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string ftype = System.IO.Path.GetExtension  (saveFile.FileName).ToLower();
                switch(ftype)
                {
                    case ".jpg":
                        image = ImageFormat.Jpeg; break;
                    case ".png":
                        image = ImageFormat.Png;break;
                }
                picBarcode.Image.Save(saveFile.FileName, image);
            }
        }
    }
}
