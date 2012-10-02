using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace PetitMIDI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string assemblyDirectory = "Lib";

        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromAssemblyFolder);
        }

        private static Assembly LoadFromAssemblyFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(folderPath, assemblyDirectory, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false)
            {
                return null;
            }
            return Assembly.LoadFrom(assemblyPath);
        }
    }
}