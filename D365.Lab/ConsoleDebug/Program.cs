using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.Text;

namespace ConsoleDebug
{
    class Program
    {
        private static IOrganizationService _service;

        static void Main(string[] args)
        {

            var client = new CrmServiceClient(connectionString);
            _service = client.OrganizationWebProxyClient;

            var userid = ((WhoAmIResponse)_service.Execute(new WhoAmIRequest()))?.UserId ?? Guid.Empty;

            if (userid.Equals(Guid.Empty))
            {
                Console.WriteLine("Connected error");
            }
            else
            {
                Console.WriteLine("Connected");
                var program = new PluginValidator.Program("dg", "gdfg");
                program.Main(_service, accountEntity);
            }

            Console.WriteLine("End");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
