using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTool
{
    public partial class PressureForm : Form
    {
        private bool moving = false;
        private Point oldMousePosition;

        public PressureForm()
        {
            InitializeComponent();
            InitUI();
        }

      
        private void PressureForm_Load(object sender, EventArgs e)
        {
            InitPumpType();
        }

        private void InitPumpType()
        {
            cbPumpType.Items.Clear();
            cbPumpType.Items.AddRange(Enum.GetNames(typeof(ProductID)));
            cbPumpType.SelectedIndex = 0;
            //m_LocalPid = (ProductID)Enum.Parse(typeof(ProductID), cbPumpType.Items[cbPumpType.SelectedIndex].ToString(), true);
        }

        private void InitUI()
        {
            lbTitle.ForeColor = Color.FromArgb(3, 116, 214);
            tlpParameter.BackColor = Color.FromArgb(19, 113, 185);
            cbPumpType.BackColor = Color.FromArgb(19, 113, 185);
            tbPumpNo.BackColor = Color.FromArgb(19, 113, 185);
        }

        private void tlpTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                return;
            }
            oldMousePosition = e.Location;
            moving = true; 
        }

        private void tlpTitle_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void tlpTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && moving)
            {
                Point newPosition = new Point(e.Location.X - oldMousePosition.X, e.Location.Y - oldMousePosition.Y);
                this.Location += new Size(newPosition);
            }
        }

    }
}
