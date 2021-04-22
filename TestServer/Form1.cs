using DALServer;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TestServer
{
    public partial class Form1 : Form
    {
        GenericUnitOfWork work;
        Xml2CSharp.Test test;
        Form form;
        public Form1(Form form)
        {
            InitializeComponent();
            unVisableGroup();
            work = new GenericUnitOfWork(new ServerContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            dataGridView1.DataSource = work.Repository<Group>().GetAll();
            this.form = form;
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
            work.Repository<Group>().Add(new Group { NameGroup = textBox1.Text, Tests = new List<Test>(),Users=new List<User>() });
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
                work.Repository<User>().Add(new User { FirstName = textBox2.Text, LastName = textBox3.Text, IsAdmin = checkBox1.Checked, Login = textBox5.Text, Password = textBox4.Text,Groups=new List<Group>() });
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

        private void button5_Click(object sender, EventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Xml2CSharp.Test));
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                test = (Xml2CSharp.Test)formatter.Deserialize(fs);
            }
            textBox8.Text = test.TestName;
            textBox9.Text = test.Author;
            textBox7.Text = test.Qty_questions;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox8.Text = "";
            textBox9.Text = "";
            textBox7.Text = "";
            test = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Test test1 = new Test();
            test1.Author = textBox9.Text;
            test1.Title = textBox8.Text;
            test1.Time = new TimeSpan((int)numericUpDown1.Value, (int)numericUpDown2.Value, 0);
            List<Question> questions = new List<Question>();
            foreach(var quest in test.Question)
            {
                Question question = new Question();
                question.Title = quest.Description;
                question.Difficulty = Convert.ToInt32(quest.Difficulty);
                List<Answer> answers = new List<Answer>();
                foreach(var answ in quest.Answer)
                {
                    Answer answer = new Answer();
                    answer.Description = answ.Description;
                    if (answ.IsRight == "True")
                    {
                        answer.IsRight = true;
                    }
                    else
                        answer.IsRight = false;
                    answers.Add(answer);
                }
                question.Answers = answers;
                questions.Add(question);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Close();
        }

    }
}
