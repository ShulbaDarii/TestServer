using DALServer;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestServer
{
    public partial class FormStart : Form
    {
        GenericUnitOfWork work;
        public FormStart()
        {
            InitializeComponent();
            work = new GenericUnitOfWork(new ServerContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            work.Repository<User>().GetAll();
            textBox1.Text = "admin";
            textBox2.Text = "password";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (work.Repository<User>().FindAll(x => x.Login == textBox1.Text).FirstOrDefault() != default)
            {
                User us = work.Repository<User>().FindAll(x => x.Login == textBox1.Text).FirstOrDefault();
                if (us.IsAdmin)
                {
                    if (us.Password == textBox2.Text)
                    {
                        Form1 form = new Form1(this);
                        form.Show();
                        Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("wrong password");
                    }
                }
                else
                {
                    MessageBox.Show("You are not admin");
                }
            }
            else
            {
                MessageBox.Show("login not found");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
