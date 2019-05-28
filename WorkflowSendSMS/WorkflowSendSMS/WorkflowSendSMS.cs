using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;


namespace WorkflowSendSMS
{
    public class WorkflowSendSMS : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            Guid SMSSendId = workflowContext.PrimaryEntityId;
            var SMSsendType = workflowContext.PrimaryEntityName;
            var SMSSend = service.Retrieve(SMSsendType, SMSSendId, new ColumnSet(true));

            if (SMSSend.GetAttributeValue<OptionSetValue>("new_message_status_code").Value == 1)
            {
                ListMembers listMembers = new ListMembers();
                listMembers.SetListMembers(SMSSend.GetAttributeValue<EntityReference>("new_marketing_list_leads"), service);

                var entityType = listMembers.Members.Entities[0].GetAttributeValue<string>("entitytype");       

                PhoneNumbers phoneNumbers = new PhoneNumbers();
                phoneNumbers.SetPhoneNumberAttribute(entityType) ;
                phoneNumbers.SetListPhoneNumbers(listMembers, entityType, service);

                WriteToFile writeToFile = new WriteToFile
                {
                    Name = SMSSend.GetAttributeValue<string>("new_name"),
                    Message = SMSSend.GetAttributeValue<string>("new_message"),
                    Numbers = phoneNumbers.ListPhoneNumbers
                };

                if (writeToFile.FileWriter(@"\\CRM-TRAIN\Shared\Novikov.xml"))
                {
                    SMSSend.GetAttributeValue<OptionSetValue>("new_message_status_code").Value = 2;
                    service.Update(SMSSend);
                }
            }
        }
    }
}
