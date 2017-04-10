using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro;

namespace SupplierSitesFileShuffler
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        protected override void OnStartup(StartupEventArgs e)
        {
            //add Axis theme
            ThemeManager.AddAccent("Axis Theme", new Uri("pack://application:,,,/FileShuffler;component/Resources/AxisTheme.xaml"));


            base.OnStartup(e);
        }
    }
}
