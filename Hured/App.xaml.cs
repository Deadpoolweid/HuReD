using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //ThemeManager.ChangeAppStyle(this,
            //                        ThemeManager.GetAccent("Amber"),
            //                        ThemeManager.GetAppTheme("BaseDark"));

            var allTypes = typeof(App).Assembly.GetTypes();
            var filteredTypes = allTypes.Where(d =>
                typeof(MetroWindow).IsAssignableFrom(d)
                && typeof(MetroWindow) != d
                && !d.IsAbstract).ToList();

            ResourceDictionary testDictionary = new ResourceDictionary
            {
                {"SomeString", "SomeValue"}
            };

            foreach (var type in filteredTypes)
            {
                var defaultStyle = this.Resources.MergedDictionaries.FirstOrDefault(
                    q => q.Source.OriginalString == "ResourcesDictionary.xaml")[typeof(MetroWindow)];
                this.Resources.Add(type, defaultStyle);
            }

            base.OnStartup(e);
        }
    }
}
