using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;

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
    }
}
