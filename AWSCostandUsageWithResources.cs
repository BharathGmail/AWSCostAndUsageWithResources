using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Amazon.CostExplorer.Model;
using Amazon.CostExplorer;

namespace AWSCostAndUsageWithResources
{
    public static class AWSCostandUsageWithResources
    {
        [FunctionName("AWSCostandUsageWithResources")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("requestBody : " + requestBody);
                GetCostAndUsageWithResourcesRequest costAndUsageWithResourcesRequest = JsonConvert.DeserializeObject<GetCostAndUsageWithResourcesRequest>(requestBody);

                log.LogInformation("Build JSON Completed and AWS call initiated..");
                GetCostAndUsageWithResourcesResponse costAndUsageWithResourcesResponse = CallAWSCostAndUsageAPI(costAndUsageWithResourcesRequest);
                log.LogInformation("AWS call Completed..");


                return new OkObjectResult(costAndUsageWithResourcesResponse);
            }
            catch (Exception ex)
            {
                log.LogInformation("Error occured in AWS Cost and Usage " + ex.Message.ToString());
                return new BadRequestObjectResult(ex.Message.ToString());
            }

        }
            private static GetCostAndUsageWithResourcesResponse  CallAWSCostAndUsageAPI(GetCostAndUsageWithResourcesRequest costAndUsageWithResourcesRequest)
            {

                var client = new AmazonCostExplorerClient(
                    awsAccessKeyId: Environment.GetEnvironmentVariable("awsAccessKeyId"),
                    awsSecretAccessKey: Environment.GetEnvironmentVariable("awsSecretAccessKey"),
                    Amazon.RegionEndpoint.USEast1);

                GetCostAndUsageWithResourcesResponse costAndUsageWithResourcesResponse = client.GetCostAndUsageWithResourcesAsync(costAndUsageWithResourcesRequest).Result;

                return costAndUsageWithResourcesResponse ;
            }
           
        
    }
}
