using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Web;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowSendSMS
{
    public class WorkflowSendSMS : CodeActivity
    {

        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            Guid smsSendId = workflowContext.PrimaryEntityId;
            var smssendType = workflowContext.PrimaryEntityName;

            var smsSend = service.Retrieve(smssendType, smsSendId, new ColumnSet(true));

            if (smsSend.GetAttributeValue<OptionSetValue>("new_message_status_code").Value == 1)
            {
                var listMarket = smsSend.GetAttributeValue<EntityReference>("new_marketing_list_leads");
                string fetchXml =
                $@"<fetch mapping='logical'><entity name='listmember'><filter type='and'>
                    <condition attribute='listid' operator='eq' value='{listMarket.Id}'/>
                </filter></entity></fetch>";
                EntityCollection contactlist = service.RetrieveMultiple(new FetchExpression(fetchXml));

                var entityType = contactlist.Entities[0].Attributes["entitytype"];
                string phoneNumber = "mobilephone";
                if (entityType.Equals("account"))
                {
                    phoneNumber = "telephone1";
                }

                string fetchXmlMembers = $@"<fetch mapping='logical'><entity name='{entityType}'><filter type='or'>";
                foreach (var item in contactlist.Entities)
                {
                    var entityId = item.GetAttributeValue<EntityReference>("entityid").Id;
                    fetchXmlMembers += $"<condition attribute='{entityType}id' operator='eq' value='{entityId}'/>";
                }
                fetchXmlMembers += "</filter></entity></fetch>";


                var listMembers = service.RetrieveMultiple(new FetchExpression(fetchXmlMembers));
                List<string> numbers = new List<string>();
                foreach (var item in listMembers.Entities)
                {
                    numbers.Add(item.GetAttributeValue<string>(phoneNumber));
                }

                WriteToFile writeToFile = new WriteToFile
                {
                    Name = smsSend.GetAttributeValue<string>("new_name"),
                    Message = smsSend.GetAttributeValue<string>("new_message"),
                    Numbers = numbers
                };

                if (writeToFile.FileWriter(@"\\CRM-TRAIN\Shared\Novikov.xml"))
                {
                    smsSend.GetAttributeValue<OptionSetValue>("new_message_status_code").Value = 2;
                    service.Update(smsSend);
                }
            }
        }
    }
}
