using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;

namespace aws_cognito_desktop_client
{
    public partial class FormLogin : Form
    {
        private string POOL_ID = ConfigurationManager.AppSettings["POOL_id"];
        private string CLIENTAPP_ID = ConfigurationManager.AppSettings["CLIENT_id"];


        public FormLogin()
        {
            InitializeComponent();
        }

        private async void buttonSignIn_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                var cognitoUser = await ChallengeUser(txtUsername.Text, txtPassword.Text);
                Console.WriteLine(cognitoUser.Username);
                MessageBox.Show($@"Hello {cognitoUser.Username}");

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                MessageBox.Show(exp.Message);
            }
        }

        private async Task<CognitoUser> ChallengeUser(string username, string password)
        {
            AmazonCognitoIdentityProviderClient provider =
                new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast2);


            var request = new InitiateAuthRequest
            {
                ClientId = this.CLIENTAPP_ID,
                AuthFlow = AuthFlowType.CUSTOM_AUTH,
            };
            
            request.AuthParameters.Add("USERNAME", username);
            request.AuthParameters.Add("PASSWORD", password);

            var response = await provider.InitiateAuthAsync(request).ConfigureAwait(false);

            if (response.AuthenticationResult != null)
            {
                return createUser(username, provider);
            }
            if (response.ChallengeName != null)
            {
                var selectAccount = new SelectAccount(username,
                    response.Session,
                    response.ChallengeParameters);

                selectAccount.ShowDialog();
                return createUser(username, provider);
            }

            return null;
        }
        private CognitoUser createUser(string username, AmazonCognitoIdentityProviderClient provider)
        {
            var userPool = new CognitoUserPool(this.POOL_ID, this.CLIENTAPP_ID, provider);
            return new CognitoUser(username, this.CLIENTAPP_ID, userPool, provider);
        }
    }
}
