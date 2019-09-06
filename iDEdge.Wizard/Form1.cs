using iDEdge.Netease;
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
            linkLabel1.Text = Core.ver;
            forms = new List<Form>()
            {
                new PageWelcome(this),
                new PageLocal(),
                new PageNease(),
                new PageNeaseRes(),
                new PageMaking(),
            };
            foreach (Form f in forms)
            {
                f.TopLevel = false;
                f.Parent = panel2;
                f.Dock = DockStyle.Fill;
            }
            SetPage(0, false, true);
        }

        public int plantform = -2;

        int curr = 0;
        List<Form> forms;

        private void SetPage(int p, bool btn1, bool btn2)
        {
            curr = p;
            for (int i = 0; i < forms.Count; i++)
            {
                if (i == curr)
                    forms[i].Show();
                else
                    forms[i].Hide();
            }
            button1.Enabled = btn1;
            button2.Enabled = btn2;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (curr == 5)
            {
                button1.Text = "上一步";
                forms[5].Dispose();
                forms.RemoveAt(5);
            }
            if (curr == 3)
                SetPage(2, true, true);
            else
                SetPage(0, false, true);
        }

        string id = "";
        private async void Button2_Click(object sender, EventArgs e)
        {
            switch (curr)
            {
                case 0:
                    if (plantform < 0)
                    {
                        MessageBox.Show("请选择音乐平台");
                        return;
                    }
                    SetPage(plantform, true, true);
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
                    SetPage(4, false, false);
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
                            button1.Text = "重新开始";
                            SetPage(5, true, false);
                        }));// *龙门粗口*
                    }).Start();
                    break;
                case 2:
                    PageNease p = (PageNease)forms[2];
                    if (p.textBox1.Text == "")
                    {
                        MessageBox.Show("不能为空");
                        return;
                    }
                    PageNeaseRes pnr = (PageNeaseRes)forms[3];
                    id = p.textBox1.Text;
                    if (id.IndexOf("http") > -1)
                    {
                        id = Nease.Url2Id(id);
                    }
                    else
                    {
                        id = await Nease.Name2IdAsync(id);
                    }
                    var jobj = await Nease.Id2JObjAsync(id);
                    pnr.name.Text = Nease.JObj2Name(jobj);
                    pnr.pictureBox1.LoadAsync(Nease.JObj2Pic(jobj));
                    pnr.singer.Text = Nease.JObj2Singer(jobj);
                    pnr.album.Text = Nease.JObj2Album(jobj);
                    pnr.textBox1.Text = "";
                    SetPage(3, true, true);
                    break;
                case 3:
                    pnr = (PageNeaseRes)forms[3];
                    if (pnr.textBox1.Text == "")
                    {
                        MessageBox.Show("不能为空");
                        return;
                    }
                    SetPage(4, false, false);
                    new Thread(() =>
                    {
                        int r;
                        Form f;
                        r = Nease.Make(id, pnr.textBox1.Text);
                        if (r == 0)
                            f = new PageSucceed(pnr.textBox1.Text);
                        else
                            f = new PageFailed();
                        forms.Add(f);
                        f.TopLevel = false;
                        panel2.Invoke(new Action(() =>
                        {
                            f.Parent = panel2;
                            f.Dock = DockStyle.Fill;
                            button1.Text = "重新开始";
                            SetPage(5, true, false);
                        }));// *龙门粗口*
                    }).Start();
                    break;
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new AboutBox1().Show();
        }
    }
}
