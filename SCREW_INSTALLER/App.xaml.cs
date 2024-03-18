using SCREW.Installer.Classes;
using SCREW.Installer.Interfaces;
using SCREW_INSTALLER.Installer.Classes;
using System;
using System.Configuration;
using System.Windows;

namespace SCREW_INSTALLER
{
    public partial class App : Application
    {
        public static string appName = "APP_NAME";
        public static string companyName = "ORG";
        public static string appLogo = @"URL";
        public static IInstall installer = new UpdatebleInstall("URL");
        public static string installPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string workingDerictory = System.IO.Path.Combine("ORG", "APP");
        public static string runName = "run.exe";
    }
}
