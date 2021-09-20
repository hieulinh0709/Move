using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.ServiceModel;

namespace PluginValidator
{
    public class Program : IPlugin
    {
        private readonly string _unsecureString;
        private readonly string _secureString;
        public Program(string unsecureString, string secureString)
        {
            if (String.IsNullOrWhiteSpace(unsecureString) ||
                String.IsNullOrWhiteSpace(secureString))
            {
                throw new InvalidPluginExecutionException("Unsecure and secure strings are required for this plugin to execute.");
            }

            _unsecureString = unsecureString;
            _secureString = secureString;


        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.
            if (!(serviceProvider.GetService(typeof(IPluginExecutionContext)) is IPluginExecutionContext context)) return;

            // Obtain the organization service reference which you will need for  
            // web service calls.
            if (!(serviceProvider.GetService(typeof(IOrganizationServiceFactory)) is IOrganizationServiceFactory factory)) return;
            var service = factory.CreateOrganizationService(context.UserId);

            if (context.Depth > 1) return;

            // The InputParameters collection contains all the data passed in the message request.
            // Obtain the target entity from the input parameters.
            if (!(context.InputParameters["Target"] is Entity entity)) return;

            Main(service, entity, tracingService);
        }

        /// <summary>
        /// Plug-in business logic
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <param name="tracingService"></param>
        public void Main(IOrganizationService service, Entity entity, ITracingService tracingService = null)
        {
            //tracingService.Trace("Main - Plug-in business logic");
            try
            {
                EntityConfiguration checkEntity = new EntityConfiguration();

                checkEntity.GetEntityConfig(service, entity, _unsecureString);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.", ex);
            }
            catch (Exception ex)
            {
                tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                throw;
            }

        }

    }
}
