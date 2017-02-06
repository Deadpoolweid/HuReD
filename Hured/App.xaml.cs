using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //ThemeManager.ChangeAppStyle(this,
            //                        ThemeManager.GetAccent("Amber"),
            //                        ThemeManager.GetAppTheme("BaseDark"));

            try
            {
                var allTypes = typeof(App).Assembly.GetTypes();
                var filteredTypes = allTypes.Where(d =>
                    typeof(MetroWindow).IsAssignableFrom(d)
                    && typeof(MetroWindow) != d
                    && !d.IsAbstract).ToList();

                foreach (var type in filteredTypes)
                {
                    var defaultStyle = Resources.MergedDictionaries.FirstOrDefault(
                        q => q.Source.OriginalString == "ResourcesDictionary.xaml")?[typeof(MetroWindow)];
                    Resources.Add(type, defaultStyle);
                }

                base.OnStartup(e);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                MessageBox.Show(errorMessage);
                //Display or log the error based on your application.
            }


        }
    }
}
