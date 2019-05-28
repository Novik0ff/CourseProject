using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Activities;
using System.Collections.Generic;

namespace WorkflowSendSMS
{
    class PhoneNumbers
    {
        public string Attribute { get; set; }
        public List<string> ListPhoneNumbers { get; set; }
        public void SetPhoneNumberAttribute(string entityType)
        {
            Attribute = "mobilephone";
            if (entityType.Equals("account"))
            {
                Attribute = "telephone1";
            }
        }
        public void SetListPhoneNumbers(ListMembers listMembers, string entityType, IOrganizationService service)
        {
            try
            {
                ListPhoneNumbers = new List<string>();
                string fetchXmlMembers = $@"<fetch mapping='logical'><entity name='{entityType}'><filter type='or'>";
                foreach (var item in listMembers.Members.Entities)
                {
                    var entityId = item.GetAttributeValue<EntityReference>("entityid").Id;
                    fetchXmlMembers += $"<condition attribute='{entityType}id' operator='eq' value='{entityId}'/>";
                }
                fetchXmlMembers += "</filter></entity></fetch>";

                foreach (var item in service.RetrieveMultiple(new FetchExpression(fetchXmlMembers)).Entities)
                {
                    ListPhoneNumbers.Add(item.GetAttributeValue<string>(Attribute));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message.ToString());
            }
        }
    }
}
