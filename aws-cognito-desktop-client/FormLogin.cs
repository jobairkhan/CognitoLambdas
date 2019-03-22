using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aws_cognito_desktop_client
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void buttonSignIn_Click(object sender, EventArgs e)
        {
            try
            {
                //CognitoHelper cognitoHelper = new CognitoHelper();
                //cognitoUser = await cognitoHelper.ChallengeUser(txtusername.Text, txtpassword.Text);
                //Console.WriteLine(cognitoUser.Username);
                //MessageBox.Show($@"Hello {cognitoUser.Username}");

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                MessageBox.Show(exp.Message);
            }
        }
    }
}
