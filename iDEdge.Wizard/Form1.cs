using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace iDEdge.Wizard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            forms = new List<Form>()
            {
                new PageWelcome(this),
                new PageLocal(),

                new PageLocalMaking(),
            };
            foreach (Form f in forms)
            {
                f.TopLevel = false;
                f.Parent = panel2;
                f.Dock = DockStyle.Fill;
            }
            UpdateForms();
        }

        public int plantform = -2;

        int curr = 0;
        List<Form> forms;

        private void UpdateForms()
        {
            for (int i = 0; i < forms.Count; i++)
            {
                if (i == curr)
                    forms[i].Show();
                else
                    forms[i].Hide();
            }
            button1.Enabled = true;
            button2.Enabled = true;
            if (curr == 0)
            {
                button1.Enabled = false;
            }
            if (curr == forms.Count - 1)
            {
                button2.Enabled = false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            curr = 0;
            UpdateForms();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            switch (curr)
            {
                case 0:
                    if (plantform < 0)
                    {
                        MessageBox.Show("请选择音乐平台");
                        return;
                    }
                    curr = plantform;
                    break;
                case 1:
                    PageLocal pg = (PageLocal)forms[1];
                    if (pg.textBox1.Text == "")
                    {
                        MessageBox.Show("必须选择音乐文件");
                        return;
                    }
                    if (!File.Exists(pg.textBox1.Text))
                    {
                        MessageBox.Show("文件不存在");
                        return;
                    }
                    curr = 2;
                    new Thread(() =>
                    {
                        int r;
                        Form f;
                        if (pg.textBox2.Text.Trim() == "")
                            r = Core.Local(pg.textBox1.Text);
                        else
                            r = Core.Local(pg.textBox1.Text, pg.textBox2.Text);
                        if (r == 0)
                            f = new PageSucceed(pg.textBox1.Text + ".mkv");
                        else
                            f = new PageFailed();
                        forms.Add(f);
                        f.TopLevel = false;
                        panel2.Invoke(new Action(() =>
                        {
                            f.Parent = panel2;
                            f.Dock = DockStyle.Fill;
                            curr = forms.Count - 1;
                            UpdateForms();
                        })); // *龙门粗口*
                    }).Start();
                    break;
            }
            UpdateForms();
        }
    }
}
