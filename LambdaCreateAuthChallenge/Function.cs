using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaCreateAuthChallenge
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
            
            response["response"]["publicChallengeParameters"] = JToken.FromObject(
                new Dictionary<string, string>{
                    {"10", "Demo UK"},
                    {"20", "Demo Singapore"}
                });;

            response["response"]["privateChallengeParameters"] = JToken.FromObject(20);;

            response["response"]["challengeMetadata"] = "SELECT_ACCOUNT";

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
