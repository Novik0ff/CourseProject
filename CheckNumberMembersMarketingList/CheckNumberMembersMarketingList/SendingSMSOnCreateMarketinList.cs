using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SendingSMSOnCreateMarketinList
{
    public class SendingSMSOnCreateMarketinList : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(null);
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var smsSend = (Entity)context.InputParameters["Target"];

            Guid smsSendId = smsSend.Id;
            var smssendType = smsSend.LogicalName;
            var new_smsSend = service.Retrieve(smssendType, smsSendId, new ColumnSet(true));


            if (smsSend.GetAttributeValue<OptionSetValue>("new_message_status_code").Value == 1)
            {
                try
                {
                    var listMarket = new_smsSend.GetAttributeValue<EntityReference>("new_marketing_list_leads");
                    string fetchXml =
                    $@"<fetch mapping='logical'><entity name='listmember'><filter type='and'>
                    <condition attribute='listid' operator='eq' value='{listMarket.Id}'/>
                </filter></entity></fetch>";
                    EntityCollection contactlist = service.RetrieveMultiple(new FetchExpression(fetchXml));

                    var entityType = contactlist.Entities[0].Attributes["entitytype"];

                }
                catch (Exception)
                {
                    smsSend.GetAttributeValue<OptionSetValue>("new_message_status_code").Value = 0;
                    service.Update(smsSend);
                    throw new InvalidPluginExecutionException(@"Marketing list is empty. Add entries to list or select another list");
                }
            }
        }
    }
}
