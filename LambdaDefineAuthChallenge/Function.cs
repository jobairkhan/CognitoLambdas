using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaDefineAuthChallenge
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
            var requestSession = input["request"]["session"];

            WriteLog(nameof(requestSession), requestSession, context);
            if (requestSession.HasValues)
            {
                var customChallenge = requestSession
                    .First(s => s["challengeName"].ToString() == "CUSTOM_CHALLENGE");

                if (customChallenge.HasValues && customChallenge["challengeResult"].Value<bool>())
                {

                    var selectAccount = requestSession
                        .First(s => s["challengeMetadata"].ToString() == "SELECT_ACCOUNT");
                    if (selectAccount.HasValues && selectAccount["challengeResult"].Value<bool>())
                    {
                        response["response"]["issueTokens"] = true;
                        response["response"]["failAuthentication"] = false;
                    }
                    else
                    {
                        response["response"]["failAuthentication"] = true;
                    }
                }
                else
                {
                    ChallangeContinue(response);
                }
            }
            else
            {
                ChallangeContinue(response);
            }

            WriteLog(nameof(response), response, context);

            return response;
        }

        private static void ChallangeContinue(JObject response)
        {
            response["response"]["challengeName"] = "CUSTOM_CHALLENGE";
            response["response"]["issueTokens"] = false;
            response["response"]["failAuthentication"] = false;
        }

        private static void WriteLog<T>(string holderName, T input, ILambdaContext context)
        {
            var stringFormat = JsonConvert.SerializeObject(input);
            context.Logger.LogLine($"{holderName} was: " + stringFormat);
        }
    }
}
