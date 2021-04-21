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
    public partial class Form1 : Form
    {
        GenericUnitOfWork work;
        public Form1()
        {
            InitializeComponent();
            unVisableGroup();
            work = new GenericUnitOfWork(new ServerContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
        }
        private void unVisableGroup()
        {
            addUserGroupgroupBox.Visible = false;
            assingTestGroupGroupBox.Visible = false;
            loadTestGroupBox.Visible = false;
            showGroupGroupBox.Visible = false;
            showTestGroupBox.Visible = false;
            showUserGroupBox.Visible = false;
            testByGroupGroupBox.Visible = false;
            testGroupBox.Visible = false;
           
            updateGroupGroupBox.Visible = false;
            updateUserGroupBox.Visible = false;
            userByGruopGroupBox.Visible = false;
        }
        private void showGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            showGroupGroupBox.Visible = true;
            dataGridView1.DataSource = work.Repository<Group>().GetAll();
            //dataGridView1.Columns.Add(new DataGridViewColumn() { Name = "qty of test" });

        }

        private void updateGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            updateGroupGroupBox.Visible = true;
            dataGridView2.DataSource = work.Repository<Group>().GetAll();

        }

        private void userInGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            addUserGroupgroupBox.Visible = true;
            comboBox2.Items.Clear();
            foreach (Group group in work.Repository<Group>().GetAll())
                comboBox2.Items.Add(group);
            dataGridView4.DataSource= work.Repository<User>().GetAll();

        }

        private void showUserGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            userByGruopGroupBox.Visible = true;
            comboBox1.Items.Clear();
            foreach (Group group in work.Repository<Group>().GetAll())
                comboBox1.Items.Add(group);

        }

        private void showUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            showUserGroupBox.Visible = true;
            dataGridView5.DataSource = work.Repository<User>().GetAll();
        }

        private void updateUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            updateUserGroupBox.Visible = true;
            dataGridView6.DataSource = work.Repository<User>().GetAll();
        }

        private void showTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            showTestGroupBox.Visible = true;
            dataGridView7.DataSource = work.Repository<Test>().GetAll();
        }

        private void loadTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            loadTestGroupBox.Visible = true;
            dataGridView10.DataSource = work.Repository<Test>().GetAll();
            comboBox4.Items.Clear();
            foreach (Group group in work.Repository<Group>().GetAll())
                comboBox4.Items.Add(group);
        }

        private void asignesTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            assingTestGroupGroupBox.Visible = true;
            comboBox3.Items.Clear();
            foreach (Group group in work.Repository<Group>().GetAll())
                comboBox3.Items.Add(group);
        }

        private void showTestOfGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            testByGroupGroupBox.Visible = true;
        }

        private void addGroupbButton_Click(object sender, EventArgs e)
        {
            work.Repository<Group>().Add(new Group { NameGroup = textBox1.Text });
            dataGridView2.DataSource = work.Repository<Group>().GetAll();
        }

        private void removeGroupButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow s in dataGridView2.SelectedRows)
            {
                work.Repository<Group>().Remove(work.Repository<Group>().FindById(s.Cells[0].Value));
            }
            dataGridView2.DataSource = work.Repository<Group>().GetAll();
        }

        private void addUserToGroup_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow s in dataGridView4.SelectedRows)
            {
                (comboBox2.SelectedItem as Group).Users.Add(work.Repository<User>().FindById(s.Cells[0].Value));
            }
            work.Repository<Group>().Update((comboBox2.SelectedItem as Group));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView3.DataSource = (comboBox1.SelectedItem as Group).Users;
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView6_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridView6.SelectedRows.Count>0)
            {
                textBox2.Text = dataGridView6.SelectedRows[0].Cells[1].Value.ToString();
                textBox3.Text= dataGridView6.SelectedRows[0].Cells[2].Value.ToString();
                textBox4.Text= dataGridView6.SelectedRows[0].Cells[4].Value.ToString();
                textBox5.Text = dataGridView6.SelectedRows[0].Cells[3].Value.ToString();
                checkBox1.Checked = (bool)dataGridView6.SelectedRows[0].Cells[5].Value;
            }
        }

        private void addUserButton_Click(object sender, EventArgs e)
        {
            if (work.Repository<User>().FindAll(x => x.Login == textBox4.Text).FirstOrDefault() == null)
            {
                work.Repository<User>().Add(new User { FirstName = textBox2.Text, LastName = textBox3.Text, IsAdmin = checkBox1.Checked, Login = textBox5.Text, Password = textBox4.Text });
            }else

                MessageBox.Show("Login is allready used");
            dataGridView6.DataSource = work.Repository<User>().GetAll();
        }

        private void updateUserButton2_Click(object sender, EventArgs e)
        {
            if (dataGridView6.SelectedRows.Count > 0)
            {
                User user = work.Repository<User>().FindById((int)dataGridView6.SelectedRows[0].Cells[0].Value);
                user.FirstName = textBox2.Text;
                user.LastName = textBox3.Text;
                user.IsAdmin = checkBox1.Checked;
                user.Login = textBox4.Text;
                user.Password = textBox5.Text;
                work.Repository<User>().Update(user);
            }
            else
            {
                MessageBox.Show("select item to update");
            }
            dataGridView6.DataSource = work.Repository<User>().GetAll();
        }

        private void clearUserFormButton_Click(object sender, EventArgs e)
        {
            textBox2.Text="";
            textBox3.Text="";
            checkBox1.Checked=false;
            textBox4.Text="";
            textBox5.Text="";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView6.SelectedRows.Count > 0)
            {
                User user = work.Repository<User>().FindById((int)dataGridView6.SelectedRows[0].Cells[0].Value);
                work.Repository<User>().Remove(user);
            }
            else
            {
                MessageBox.Show("select item to delete");
            }
            dataGridView6.DataSource = work.Repository<User>().GetAll();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView8.DataSource = (comboBox3.SelectedItem as Group).Tests;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView9.DataSource = (comboBox4.SelectedItem as Group).Tests;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow s in dataGridView10.SelectedRows)
            {
                (comboBox4.SelectedItem as Group).Tests.Add(work.Repository<Test>().FindById(s.Cells[0].Value));
            }
            dataGridView9.DataSource = (comboBox4.SelectedItem as Group).Tests;
        }
    }
}
