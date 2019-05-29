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

            Guid smsSendId = workflowContext.PrimaryEntityId;
            var smssendType = workflowContext.PrimaryEntityName;
            var smsSend = service.Retrieve(smssendType, smsSendId, new ColumnSet(true));


            if (!(smsSend.GetAttributeValue<OptionSetValue>("statuscode").Value == 100000000))
            {
                return;
            }

            ListMembers listMembers = new ListMembers();
            listMembers.SetListMembers(smsSend.GetAttributeValue<EntityReference>("new_marketing_list_leads"), service);

            var entityType = listMembers.Members.Entities[0].GetAttributeValue<string>("entitytype");

            PhoneNumbers phoneNumbers = new PhoneNumbers();
            phoneNumbers.SetPhoneNumberAttribute(entityType);
            phoneNumbers.SetListPhoneNumbers(listMembers, entityType, service);

            SMSSending smsSending = new SMSSending
            {
                DateTime = DateTime.Now,
                Name = smsSend.GetAttributeValue<string>("new_name"),
                Message = smsSend.GetAttributeValue<string>("new_message"),
                Numbers = phoneNumbers.ListPhoneNumbers
            };

            if (smsSending.WriteToFile(@"\\CRM-TRAIN\Shared\Novikov.xml"))
            {
                smsSend.GetAttributeValue<OptionSetValue>("statuscode").Value = 100000001;
                service.Update(smsSend);
            }
        }
    }
}
