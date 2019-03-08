using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaUserMigration
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public JObject FunctionHandler(JObject input, ILambdaContext context)
        {
            WriteLog(nameof(input), input, context);

            var response = input;
            var userName = input["userName"]?.ToString();
            var password = input["request"]["password"]?.ToString();

            try
            {
                var email = new MailAddress(userName ?? throw new InvalidOperationException());

                if (password == "UserMigration")
                {
                    response["response"]["userAttributes"] = JToken.FromObject(new Dictionary<string, object>()
                    {
                        {"username", userName},
                        {"email", email.Address},
                        {"email_verified", true},
                        //{"cognito", JToken.FromObject(new Dictionary<string, object>
                        //    {
                        //        {"mfa_enabled", false}
                        //    })
                        //} 
                    });

                    response["response"]["forceAliasCreation"] = false;
                    response["response"]["finalUserStatus"] = "CONFIRMED";
                    response["response"]["messageAction"] = "SUPPRESS";
                    response["response"]["desiredDeliveryMediums"] = JArray.FromObject(new[] {"EMAIL"} );
                }
                
                WriteLog(nameof(response), response, context);
            }
            catch (Exception exception)
            {
                context.Logger.LogLine(exception.Message);
            }
            
            return response;
        }

        private static void WriteLog<T>(string holderName, T input, ILambdaContext context)
        {
            var stringFormat = JsonConvert.SerializeObject(input);
            context.Logger.LogLine($"{holderName} was: " + stringFormat);
        }
    }
}
