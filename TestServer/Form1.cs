using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public Form1()
        {
            InitializeComponent();
            unVisableGroup();
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
            uerFormGroupBox.Visible = false;
            updateGroupGroupBox.Visible = false;
            updateUserGroupBox.Visible = false;
            userByGruopGroupBox.Visible = false;
        }
        private void showGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unVisableGroup();
            showGroupGroupBox.Visible = true;
        }
    }
}
