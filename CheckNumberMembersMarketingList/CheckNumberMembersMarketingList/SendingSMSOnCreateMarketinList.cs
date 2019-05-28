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
            var SMSSendPage = (Entity)context.InputParameters["Target"];

            Guid SMSSendId = SMSSendPage.Id;
            var SMSSendType = SMSSendPage.LogicalName;
            var SMSSendCRM = service.Retrieve(SMSSendType, SMSSendId, new ColumnSet(true));

            if (SMSSendPage.GetAttributeValue<OptionSetValue>("new_message_status_code").Value == 1)
            {
                try
                {
                    var listMarket = SMSSendCRM.GetAttributeValue<EntityReference>("new_marketing_list_leads");
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
                    SMSSendPage.GetAttributeValue<OptionSetValue>("new_message_status_code").Value = 0;
                    service.Update(SMSSendPage);
                    throw new InvalidPluginExecutionException(@"Marketing list is empty. Add entries to list or select another list");
                }
            }
        }
    }
}
