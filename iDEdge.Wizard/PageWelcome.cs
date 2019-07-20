using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iDEdge.Wizard
{
    public partial class PageWelcome : Form
    {
        public PageWelcome(Form1 form1)
        {
            InitializeComponent();
            f1 = form1;
        }

        Form1 f1;

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            f1.plantform = 1;
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            f1.plantform = 2;
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            f1.plantform = -1;

        }
    }
}
