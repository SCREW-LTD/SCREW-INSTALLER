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
        public static string appName = "SCREW: WS";
        public static string companyName = "SCREW LTD.";
        public static string appLogo = @"https://wallet.screwltd.com/logo.png";
        public static IInstall installer = new UpdatebleInstall("https://remnant.screwltd.com/screw_work_new/Updater.xml");
        public static string installPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public static string workingDerictory = System.IO.Path.Combine("SCREW", "WORKSPACE");
        public static string runName = "SCREW_WORKSPACE.exe";
    }
}
