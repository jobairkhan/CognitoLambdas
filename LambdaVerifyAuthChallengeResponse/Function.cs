using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaVerifyAuthChallengeResponse
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


            WriteLog("userName", input["UserName"], context);
            WriteLog("email", input["request"]["userAttributes"]["email"], context);
            var challengeAnswer = input["request"]["challengeAnswer"].ToString();
            var privateChallengeParams = input["request"]["privateChallengeParameters"];
            if (privateChallengeParams.HasValues)
            {
                var answerCorrect = privateChallengeParams
                    .ToObject<Dictionary<string, string>>()
                    .Any(a=> a.Key == challengeAnswer);
                response["response"]["answerCorrect"] = answerCorrect;
            }
            
            WriteLog(nameof(response), response, context);

            return response;
        }

        private static void WriteLog<T>(string holderName, T input, ILambdaContext context)
        {
            var stringFormat = JsonConvert.SerializeObject(input);
            context.Logger.LogLine($"{holderName} was: " + stringFormat);
        }

    }
}
