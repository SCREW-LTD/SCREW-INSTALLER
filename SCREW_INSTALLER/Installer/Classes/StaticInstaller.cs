using SCREW.Installer.Interfaces;
using System;
using System.Net;
using System.Xml;

namespace SCREW_INSTALLER.Installer.Classes
{
    public class StaticInstaller : IInstall
    {
        private string url, ver;

        public StaticInstaller(string url, string ver)
        {
            this.url = url;
            this.ver = ver;
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
