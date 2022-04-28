using Autofac;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Startup;
using FriendOrganizer.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.UI
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup(object sender, StartupEventArgs e)
    {
      var bootstrapper = new Bootstrapper();
      var container = bootstrapper.Bootstrap();

      var mainWindow = container.Resolve<MainWindow>();
      mainWindow.Show();
    }

    private void Application_DispatcherUnhandledException(object sender, 
      System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      MessageBox.Show("Unexpected error occured. Please inform the admin."
        + Environment.NewLine + e.Exception.Message, "Unexpected error");

      e.Handled = true;
    }
  }
}
