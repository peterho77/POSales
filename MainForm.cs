using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace POSales
{
    public partial class MainForm : Form
    {
        SqlConnection connection;
        SqlCommand command;
        public string _password;
        public string _fullname;
        public MainForm()
        {
            InitializeComponent();
            customizeDesign();

            connection = new SqlConnection(DataProvider.myConnection());
            btnDashboard.PerformClick();
            //connection.Open();
        }

        private Form activeForm = null;
        public void openChildForm(Form childform)
        {
            if (activeForm != null) {
                activeForm.Close();
            }
            activeForm = childform;
            childform.TopLevel = false;
            childform.FormBorderStyle = FormBorderStyle.None;
            childform.Dock = DockStyle.Fill;
            lblTitle.Text = childform.Text;
            panMain.Controls.Add(childform);
            panMain.Controls.Add(new TextBox());
            panMain.Tag = childform;
            childform.BringToFront();
            childform.Show();
        }

        #region panelSlide
        private void customizeDesign()
        {
            panSubSetting.Visible = false;
            panSubProduct.Visible = false;
            panSubRecord.Visible = false;
            panSubInStock.Visible = false;
        }

        private void hideSubMenu()
        {
            if (panSubInStock.Visible)
            {
                panSubInStock.Visible = false;
            }

            if (panSubSetting.Visible)
            {
                panSubSetting.Visible = false;
            }

            if (panSubProduct.Visible)
            {
                panSubProduct.Visible = false;
            }

            if (panSubRecord.Visible)
            {
                panSubRecord.Visible = false;
            }
        }

        private void showSubMenu(Panel submenu)
        {
            if (submenu.Visible == false)
            {
                hideSubMenu();
                submenu.Visible = true;
            }
            else
                submenu.Visible = false;
        }

        #endregion panelSlide
               

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnInStock_Click_1(object sender, EventArgs e)
        {
            showSubMenu(panSubInStock);
        }

        private void btnProduct_Click_1(object sender, EventArgs e)
        {
            showSubMenu(panSubProduct);
        }

        private void btnSupplier_Click_1(object sender, EventArgs e)
        {
            openChildForm(new Supplier());
            hideSubMenu();
        }

        private void btnRecord_Click_1(object sender, EventArgs e)
        {
            showSubMenu(panSubRecord);
        }

        private void btnSetting_Click_1(object sender, EventArgs e)
        {
            showSubMenu(panSubSetting);
        }

        private void btnProductList_Click(object sender, EventArgs e)
        {
            openChildForm(new Product());
            hideSubMenu();
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            openChildForm(new Category());
            hideSubMenu();
        }

        private void btnBrand_Click(object sender, EventArgs e)
        {
            openChildForm(new Brand());
            hideSubMenu();
        }

        private void btnStockEntry_Click(object sender, EventArgs e)
        {
            openChildForm(new InStock());
            hideSubMenu();
        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {
            Adjustment adjustment = new Adjustment(this);
            adjustment.txtName.Text = _fullname;
            adjustment.txtName.Enabled = false;
            adjustment.txtPcode.Enabled = false;
            adjustment.txtDescription.Enabled = false;
            adjustment.txtProductQty.Enabled = false;
            openChildForm(adjustment);
            hideSubMenu();
        }

        private void btnSaleHistory_Click(object sender, EventArgs e)
        {
            DailySale dailySale = new DailySale();
            dailySale.cancel_cashier = lblUsername.Text;
            openChildForm(dailySale);
            hideSubMenu();
        }

        private void btnPOSRecord_Click(object sender, EventArgs e)
        {
            openChildForm(new Record());
            hideSubMenu();
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            UserAccount account = new UserAccount(this);
            account.ShowDialog();
            hideSubMenu();
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            Store store = new Store();
            store.ShowDialog();
            
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
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

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.loadDashboard();
            openChildForm(dashboard);
        }

        public void Notification()
        {
            int i = 0;
            connection.Open();
            command = new SqlCommand("Select * from viewCriticalItems", connection);
            var reader = command.ExecuteReader();
            while(reader.Read())
            {
                i++;
                Alert alert = new Alert();
                alert.lblPcode.Text = reader["Pcode"].ToString();
                alert.btnReorder.Enabled = true;
                alert.BringToFront();
                alert.showAlert(i + ". " + reader["Description"].ToString() + " - " + reader["Quantity"].ToString());
            }
            connection.Close();
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {
            btnDashboard.PerformClick();
            Notification(); 
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            openChildForm(new Barcode());
            hideSubMenu();
        }
    }
}
