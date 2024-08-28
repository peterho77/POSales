using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSales
{
    public partial class Alert : Form
    {
        private Random random;
        private int tempIndex;
        MainForm mainForm;
        public Alert()
        {
            InitializeComponent();
            random = new Random();

            Color color = SelectThemeColor();
            panel1.BackColor = color;
        }

        public enum Action
        {
            wait,
            start,
            close,
        }

        private Alert.Action action;
        private int x, y;

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (this.action)
            {
                case Action.wait:
                    timer1.Interval = 15000;
                    action = Action.close;
                    break;
                case Action.start:
                    timer1.Interval = 1;
                    this.BringToFront();
                    this.Opacity += 0.1;
                    if (this.x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        
                        if (this.Opacity == 1.0)
                        {
                           
                            action = Action.wait;
                        }
                    }
                    break;
                case Action.close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;
                    this.Left -= 3;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;  
            }
        }

        public void showAlert(string message)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;
            for (int i = 1; i < 10; i++)
            {
                fname = "alert " + i.ToString();
                Alert frm = (Alert)Application.OpenForms[fname];
                
                if (frm == null)
                {
                    this.Name = fname;
                    this.x = Screen.PrimaryScreen.WorkingArea.Width - this.Width + i;
                    this.y = Screen.PrimaryScreen.WorkingArea.Height - this.Height * i - 5 * i;
                    this.Location = new Point(this.x, this.y);
                }              
            }
            this.x = Screen.PrimaryScreen.WorkingArea.Width - base.Width - 5;
            this.lblMessage.Text = message;

            this.Show();
            this.action = Action.start;
            this.timer1.Interval = 1;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            action = Action.close;
        }

        private Color SelectThemeColor()
        {
            int index = random.Next(ThemeColor.colorslist.Count);
            while (tempIndex == index)
            {
                index = random.Next(ThemeColor.colorslist.Count);
            }
            tempIndex = index;
            string color = ThemeColor.colorslist[index];
            return ColorTranslator.FromHtml(color);
        }
    }
}
