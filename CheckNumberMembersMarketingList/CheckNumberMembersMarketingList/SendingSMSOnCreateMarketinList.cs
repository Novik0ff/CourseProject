using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace SendingSMSOnCreateMarketinList
{
    public class SendingSMSOnCreateMarketinList : IPlugin
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

            Guid smsSendId = smsSendPage.Id;
            var smsSendType = smsSendPage.LogicalName;
            var smsSendCRM = service.Retrieve(smsSendType, smsSendId, new ColumnSet(true));

            try
            {
                var listMarket = smsSendCRM.GetAttributeValue<EntityReference>("new_marketing_list_leads");
                string fetchXml =
                $@"<fetch mapping='logical'>
                        <entity name='listmember'>
                            <filter type='and'>
                                <condition attribute='listid' operator='eq' value='{listMarket.Id}'/>
                            </filter>
                        </entity>
                    </fetch>";
                EntityCollection listMembers = service.RetrieveMultiple(new FetchExpression(fetchXml));
                var entityType = listMembers.Entities[0].Attributes["entitytype"];
            }
            catch (Exception)
            {
                smsSendPage.GetAttributeValue<OptionSetValue>("statuscode").Value = 1;
                service.Update(smsSendPage);
                throw new InvalidPluginExecutionException(@"Marketing list is empty. Add entries to list or select another list");
            }
        }
    }
}
