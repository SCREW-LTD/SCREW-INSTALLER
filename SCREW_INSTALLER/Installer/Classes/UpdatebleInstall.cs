using SCREW.Installer.Interfaces;
using System;
using System.Net;
using System.Xml;

namespace SCREW.Installer.Classes
{
    public class UpdatebleInstall : IInstall
    {
        private string url, ver;
        public UpdatebleInstall(string schema)
        {
            try
            {
                WebClient client = new WebClient();
                string xmlContent = client.DownloadString(schema);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                XmlNodeList itemNodes = xmlDoc.SelectNodes("//item");

                foreach (XmlNode itemNode in itemNodes)
                {
                    string fileUrl = itemNode.SelectSingleNode("url")?.InnerText;
                    string verUrl = itemNode.SelectSingleNode("version")?.InnerText;
                    if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(verUrl))
                    {
                        url = fileUrl;
                        ver = verUrl;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Error: url node not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public string GetUrl()
        {
            return this.url;
        }

        public string GetVersion()
        {
            return this.ver;
        }
    }
}
