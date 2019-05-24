using System;
using System.Xml;
using System.Activities;
using System.Collections.Generic;

namespace WorkflowSendSMS
{
    public class WriteToFile
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public List<string> Numbers { get; set; }
        public bool FileWriter(string path)
        {
            try
            {
                XmlWriter xmlWriter = XmlWriter.Create(path);

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("SendingSMS");

                xmlWriter.WriteStartElement("DateTime");
                xmlWriter.WriteString(DateTime.Now.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Name");
                xmlWriter.WriteString($"{Name}");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Message");
                xmlWriter.WriteString($"{Message}");
                xmlWriter.WriteEndElement();

                if (Numbers.Count > 0)
                {
                    xmlWriter.WriteStartElement("PhoneNumbers");
                    foreach (var item in Numbers)
                    {
                        xmlWriter.WriteStartElement("PhoneNumber");
                        xmlWriter.WriteString($"{item}");
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();

                return true;
            }
            catch (Exception ex )
            {
                throw new InvalidWorkflowException(ex.Message.ToString());
            }
        }
    }
}
