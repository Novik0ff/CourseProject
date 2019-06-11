using System;
using System.IO;
using System.Activities;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace WorkflowSendSMS
{
    public class SMSSending
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        [XmlArray("SMSList")]
        [XmlArrayItem("SMS")]
        public List<SMS> SMSList { get; set; } = new List<SMS>();
        [XmlIgnore]
        public string Message { get; set; }
        [XmlIgnore]
        public List<string> PhoneNumbers { get; set; }
        public void WriteToFile(string path)
        {
            SetSMSList(PhoneNumbers, Message);
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(SMSSending));
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, this);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message.ToString());
            }
        }
        private void SetSMSList(List<string> numbers, string message)
        {
            foreach (var phoneNumber in PhoneNumbers)
            {
                SMSList.Add(new SMS { Message = message, PhoneNumber = phoneNumber });
            }
        }
    }
}
