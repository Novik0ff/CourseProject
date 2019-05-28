using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace WorkflowSendSMS
{
    class ListMembers
    {
        public EntityCollection Members { get; set; }
        public void SetListMembers(EntityReference listMarket, IOrganizationService service)
        {
            try
            {
                string fetchXml =
                $@"<fetch mapping='logical'>
                    <entity name='listmember'>
                        <filter type='and'>
                            <condition attribute='listid' operator='eq' value='{listMarket.Id}'/>
                        </filter>
                    </entity>
                </fetch>";
                Members = service.RetrieveMultiple(new FetchExpression(fetchXml));
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message.ToString());
            }
        }
    }
}
