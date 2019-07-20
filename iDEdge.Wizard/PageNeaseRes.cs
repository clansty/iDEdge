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
    public partial class PageNeaseRes : Form
    {
        public PageNeaseRes()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = name.Text + ".mkv";
            saveFileDialog1.ShowDialog();
        }

        private void SaveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = saveFileDialog1.FileName;
        }
    }
}
