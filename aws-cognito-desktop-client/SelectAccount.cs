using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace aws_cognito_desktop_client
{
    public partial class SelectAccount : Form
    {
        private readonly string SESSION;
        private string CLIENTAPP_ID = ConfigurationManager.AppSettings["CLIENT_id"];


        public SelectAccount(string username, string session, Dictionary<string, string> accounts)
        {
            SESSION = session;
            InitializeComponent();
            lblusername.Text = username;

            checkedListBox1.DataSource = new BindingSource(
                accounts.Where(x => x.Key.ToString() != "USERNAME"), null);
            checkedListBox1.DisplayMember = "Value";
            checkedListBox1.ValueMember = "Key";

            checkedListBox1.SelectionMode = SelectionMode.One;
        }

        private async void buttonSelect_ClickAsync(object sender, EventArgs e)
        {
            var success = await VerifyChallenge(
                lblusername.Text,
                SESSION,
                checkedListBox1.SelectedValue as string);
            if (success.AuthenticationResult?.AccessToken != null)
            {
                MessageBox.Show($@"Id token: {Environment.NewLine} {success.AuthenticationResult.IdToken}");
                this.Close();
            }
            else
            {
                MessageBox.Show("Unable to validate");
            }
        }

        private async Task<RespondToAuthChallengeResponse> VerifyChallenge(string username, string session, string accountId)
        {
            var provider = new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast2);

            try
            {
                var challengeRequest = new RespondToAuthChallengeRequest
                {
                    Session = session,
                    ChallengeName = "CUSTOM_CHALLENGE",
                    ClientId = CLIENTAPP_ID,
                    ChallengeResponses = new Dictionary<string, string>()
                    {
                        {"ANSWER", accountId},
                        {"USERNAME", username}
                    }
                };

                return await provider.RespondToAuthChallengeAsync(challengeRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
