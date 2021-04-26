using DALServer;
using MessageServerClient;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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


        Socket listenSocket;
        List<Thread> threads = new List<Thread>();

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        List<Info> infos = new List<Info>();

        public Form1(Form form)
        {
            InitializeComponent();
            unVisableGroup();
            work = new GenericUnitOfWork(new ServerContext(ConfigurationManager.ConnectionStrings["conStr"].ConnectionString));
            dataGridView1.DataSource = work.Repository<Group>().GetAll();
            this.form = form;


            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Сервер завжди сідає на локал хост!!
            IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
            IPAddress iPAddress = iPHostEntry.AddressList[1]; //[0] доступ до першої мережевої карти


            int port = 33333;


            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);

            listenSocket.Bind(iPEndPoint);
            var token = tokenSource.Token;
            Task.Factory.StartNew(() => ListenThread(listenSocket, token), token);
        }

        private void ListenThread(Socket listenSocket, CancellationToken cancellationToken)
        {
            listenSocket.Listen(2);
            while (true)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Socket clientSocket = listenSocket.Accept(); //.Accept() - це блокуюча функцію
                Info info = new Info() { RemoteEndPoint = clientSocket.RemoteEndPoint.ToString(), ClientSocket = clientSocket };

                infos.Add(info);



                Thread receiveThread = new Thread(ReceiveThreadFunction);
                threads.Add(receiveThread);
                receiveThread.IsBackground = true; 
                receiveThread.Start(info);


            }
        }

        private void ReceiveThreadFunction(object sender)
        {
            try
            {
                while (true)
            {
                Info info = sender as Info;
                Socket receiveSocked = info.ClientSocket;
                if (receiveSocked == null) throw new ArgumentException("Receive Socket Exception");
                byte[] receivebyte = new byte[2048];

                Int32 nCount = receiveSocked.Receive(receivebyte);//Receive() -  блокуюча функція - чекає доки  буде повідомлення

                MemoryStream ms = new MemoryStream(receivebyte);
                BinaryFormatter bf = new BinaryFormatter();
                Mess d = (Mess)bf.Deserialize(ms);
                byte[] infooo = d.info;
                string type = "";
                Byte[] mess=new byte[5];
                int id = 0;
                    bool skip = false;
                switch (d.type)
                {
                    case "join":
                        MemoryStream mS = new MemoryStream(infooo);

                        BinaryFormatter bF = new BinaryFormatter();
                        User user = (User)bF.Deserialize(mS);

                        User user1 =work.Repository<User>().FindAll(x => x.Login == user.Login).FirstOrDefault();
                        if (user1 != default)
                        {
                            if (user1.Password == user.Password)
                            {
                                type = "join";

                                id = user1.Id;
                            }
                            else
                            {
                                type = "join";

                                string newText = "wrong password";

                                mess = new byte[newText.Length];
                                mess = Encoding.ASCII.GetBytes(newText);
                            }
                        }
                        else
                        {
                            type = "join";

                            string newText = "user not found";

                            mess = new byte[newText.Length];
                            mess = Encoding.ASCII.GetBytes(newText);
                        }
                        break;
                    case "Tests":
                        type = "Tests";
                        ICollection<Test> tests = new List<Test>();
                        User u = work.Repository<User>().FindById(d.id);
                        foreach(Test test in work.Repository<Test>().GetAll())
                        {
                            foreach(Group group in test.Groups)
                            {
                                foreach (Group groupUser in u.Groups)
                                {
                                    if(group==groupUser&&!tests.Contains(test))
                                    {
                                        tests.Add(test);
                                    }
                                }

                            }
                        }
                        List<TestDLL.Test> tests1 = new List<TestDLL.Test>();
                        foreach(Test test1 in tests)
                        {
                            TestDLL.Test t = new TestDLL.Test();
                            t.Title = test1.Title;
                            t.Time = test1.Time;
                            t.Author = test1.Author;
                            t.Id = test1.Id;
                            foreach(Question question in test1.Questions)
                            {
                                TestDLL.Question question1 = new TestDLL.Question();
                                question1.Difficulty = question.Difficulty;
                                question1.Title = question.Title;                               
                                foreach(Answer answer in question.Answers)
                                {
                                    TestDLL.Answer answer1 = new TestDLL.Answer();
                                    answer1.Description = answer.Description;
                                    answer1.IsRight = answer.IsRight;
                                    question1.Answers.Add(answer1);                                   
                                }
                                t.Questions.Add(question1);
                            }
                            tests1.Add(t);
                        }



                        BinaryFormatter binary = new BinaryFormatter();

                        MemoryStream stream = new MemoryStream();
                        binary.Serialize(stream, tests1);

                        mess = stream.ToArray();
                        break;
                    case "result":
                        MemoryStream mSS = new MemoryStream(infooo);

                        BinaryFormatter bFF = new BinaryFormatter();
                        TestDLL.Result res = (TestDLL.Result)bFF.Deserialize(mSS);
                        Result result = new Result();
                            result.User = work.Repository<User>().FindById(d.id);
                            result.Mark = res.Mark;
                            result.QtyOfRightAnswers = res.QtyOfRightAnswers;
                            result.Date = DateTime.Now;
                            result.Test = work.Repository<Test>().FindAll(x => x.Title == res.NameTest).FirstOrDefault();
                        work.Repository<Result>().Add(result);
                            skip = true;
                        break;
                    case "passes":

                            type = "passes";
                            List<TestDLL.Result> results = new List<TestDLL.Result>();
                            foreach (var it in work.Repository<Result>().GetAll())
                            {
                                if (work.Repository<User>().FindById(d.id) == it.User)
                                {
                                    results.Add(new TestDLL.Result() { NameTest = it.Test.Title, Date = it.Date, Mark = it.Mark,QtyOfRightAnswers=it.QtyOfRightAnswers }) ;
                                }
                            }
                        BinaryFormatter b = new BinaryFormatter();

                        MemoryStream s = new MemoryStream();
                        bf.Serialize(s, results);

                        mess = s.ToArray();
                        break;
                    default:
                        break;
                }

                    if (!skip)
                    {
                        Mess m = new Mess();
                        m.type = type;
                        m.id = id;
                        m.info = mess;


                        MemoryStream memoryStream = new MemoryStream();
                        bf.Serialize(memoryStream, m);

                        byte[] sendData = memoryStream.ToArray();
                        info.ClientSocket.Send(sendData);
                    }
            }
            }
            catch
            {

            }
        }

        private void unVisableGroup()
        {
            groupBox1.Visible = false;
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
        }

        private void asignesTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            assingTestGroupGroupBox.Visible = true;
            comboBox4.Items.Clear();
            foreach (Group group in work.Repository<Group>().GetAll())
                comboBox4.Items.Add(group);
            dataGridView10.DataSource = work.Repository<Test>().GetAll();
        }

        private void showTestOfGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            testByGroupGroupBox.Visible = true;
            comboBox3.Items.Clear();
            foreach (Group group in work.Repository<Group>().GetAll())
               comboBox3.Items.Add(group);
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
            try
            {
                foreach (DataGridViewRow s in dataGridView4.SelectedRows)
                {
                   if( !(comboBox2.SelectedItem as Group).Users.Contains(work.Repository<User>().FindById(s.Cells[0].Value)))
                      (comboBox2.SelectedItem as Group).Users.Add(work.Repository<User>().FindById(s.Cells[0].Value));
                   else
                        MessageBox.Show("user already in group");
                }
                work.Repository<Group>().Update((comboBox2.SelectedItem as Group));
            }
            catch
            {
            }
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
                user.Login = textBox5.Text;
                user.Password = textBox4.Text;
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
            try
            {
                foreach (DataGridViewRow s in dataGridView10.SelectedRows)
                {
                    if(!(comboBox4.SelectedItem as Group).Tests.Contains(work.Repository<Test>().FindById(s.Cells[0].Value)))
                        (comboBox4.SelectedItem as Group).Tests.Add(work.Repository<Test>().FindById(s.Cells[0].Value));
                    else
                    {
                        MessageBox.Show("test alreade asing");
                    }
                }
                work.Repository<Group>().Update(comboBox4.SelectedItem as Group);
                dataGridView9.DataSource = null;
                dataGridView9.DataSource = (comboBox4.SelectedItem as Group).Tests;
            }
            catch
            {
                
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Xml2CSharp.Test));
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
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
            test1.Questions = questions;
            work.Repository<Test>().Add(test1);
            MessageBox.Show("Add");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Close();
        }

        private void resultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            groupBox1.Visible = true;
            dataGridView1.DataSource = work.Repository<Group>().GetAll();
            foreach (var f in work.Repository<User>().GetAll())
                comboBox5.Items.Add(f);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Result> results = new List<Result>();
            foreach(var it in work.Repository<Result>().GetAll())
            {
                if ((comboBox5.SelectedItem as User) == it.User)
                    results.Add(it);
            }
            dataGridView11.DataSource= results;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow s in dataGridView3.SelectedRows)
                {
                    (comboBox1.SelectedItem as Group).Users.Remove(work.Repository<User>().FindById(s.Cells[0].Value));
                    work.Repository<Group>().Update((comboBox1.SelectedItem as Group));
                }
                dataGridView3.DataSource = null;
                dataGridView3.DataSource = (comboBox1.SelectedItem as Group).Users;
            }
            catch
            { 
            }

        }
    }
}
