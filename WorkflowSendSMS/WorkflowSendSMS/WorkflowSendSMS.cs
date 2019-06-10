using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;


namespace WorkflowSendSMS
{
    public class WorkflowSendSMS : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            ListMembers listMembers = new ListMembers();
            listMembers.SetListMembers(InNewMarketingList.Get(context), service);

            string entityType = listMembers.Members.Entities[0].GetAttributeValue<string>("entitytype");
            PhoneNumbers phoneNumbers = new PhoneNumbers();
            phoneNumbers.SetPhoneNumberAttribute(entityType);
            phoneNumbers.SetListPhoneNumbers(listMembers, entityType, service);

            SMSSending smsSending = new SMSSending
            {
                DateTime = DateTime.Now,
                Name = InNewName.Get(context),
                Message = InNewMessage.Get(context),
                Numbers = phoneNumbers.ListPhoneNumbers
            };
            smsSending.WriteToFile(@"\\CRM-TRAIN\Shared\Novikov.xml");
        }

        [RequiredArgument]
        [Input("new_name input")]
        [AttributeTarget("new_smsseltning", "new_name")]
        public InArgument<string> InNewName { get; set; }
        [RequiredArgument]
        [Input("new_message input")]
        [AttributeTarget("new_smsseltning", "new_message")]
        public InArgument<string> InNewMessage { get; set; }
        [RequiredArgument]
        [Input("new_marketing_list_leads input")]
        [ReferenceTarget("list")]
        public InArgument<EntityReference> InNewMarketingList { get; set; }
    }
}
