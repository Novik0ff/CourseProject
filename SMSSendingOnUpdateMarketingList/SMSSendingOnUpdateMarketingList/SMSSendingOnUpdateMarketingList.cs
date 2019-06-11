using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SMSSendingOnUpdateMarketingList
{
    public class SMSSendingOnUpdateMarketingList : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(null);
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            Entity preImage = context.PreEntityImages["PreImage"];
            Entity smsSendPage = (Entity)context.InputParameters["Target"];

            if (!(smsSendPage.GetAttributeValue<OptionSetValue>("statuscode").Value == 100000000))
            {
                return;
            }
            Entity listMarket = service.Retrieve("list", preImage.GetAttributeValue<EntityReference>("new_marketing_list_leads").Id, new ColumnSet("membercount"));

            if (listMarket.GetAttributeValue<int>("membercount") > 0)
            {
                return;
            }
            smsSendPage.GetAttributeValue<OptionSetValue>("statuscode").Value = 1;
            throw new InvalidPluginExecutionException(@"Marketing list is empty. Add members to list or select another marketing list");
        }
    }
}
