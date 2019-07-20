using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iDEdge.Wizard
{
    public partial class PageSucceed : Form
    {
        public PageSucceed(string 龙警察喝龙井茶)
        {
            InitializeComponent();
            label3.Text = 龙警察喝龙井茶;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Process.Start(label3.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "/select," + label3.Text);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
