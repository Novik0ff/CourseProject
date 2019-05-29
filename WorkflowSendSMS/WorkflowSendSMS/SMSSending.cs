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
        public string Message { get; set; }
        [XmlArray("PhoneNumbers")]
        [XmlArrayItem("PhoneNumber")]
        public List<string> Numbers { get; set; }
        public bool WriteToFile(string path)
        {
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(SMSSending));
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, this);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidWorkflowException(ex.Message.ToString());
            }
        }
    }
}
