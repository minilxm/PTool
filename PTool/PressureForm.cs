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
        public PressureForm()
        {
            InitializeComponent();
            InitUI();
        }

      
        private void PressureForm_Load(object sender, EventArgs e)
        {

        }

        private void InitUI()
        {
            lbTitle.ForeColor = Color.FromArgb(3, 116, 214);
            tlpParameter.BackColor = Color.FromArgb(19, 113, 185);
        }

    }
}
