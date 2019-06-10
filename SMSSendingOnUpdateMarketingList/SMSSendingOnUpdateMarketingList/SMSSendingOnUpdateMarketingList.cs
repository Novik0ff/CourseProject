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
            var smsSendPage = (Entity)context.InputParameters["Target"];

            if (!(smsSendPage.GetAttributeValue<OptionSetValue>("statuscode").Value == 100000000))
            {
                return;
            }

            Entity PreImage = context.PreEntityImages["PreImage"];

            var listMarket = PreImage.GetAttributeValue<EntityReference>("new_marketing_list_leads");
            string fetchXml =
            $@"<fetch mapping='logical'>
                    <entity name='listmember'>
                        <filter type='and'>
                            <condition attribute='listid' operator='eq' value='{listMarket.Id}'/>
                        </filter>
                    </entity>
                </fetch>";
            EntityCollection listMembers = service.RetrieveMultiple(new FetchExpression(fetchXml));

            if (listMembers.Entities.Count > 0)
            {
                return;
            }

            smsSendPage.GetAttributeValue<OptionSetValue>("statuscode").Value = 1;
            service.Update(smsSendPage);
            throw new InvalidPluginExecutionException(@"Marketing list is empty. Add members to list or select another list");
        }
    }
}
