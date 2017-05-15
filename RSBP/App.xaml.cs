using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RSBP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Threading.Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = (string)FindResource("AppTitle");
            bool isCreatedNew = false;

            try
            {
                _mutex = new System.Threading.Mutex(true, mutexName, out isCreatedNew);

                if (isCreatedNew)
                {
                    LanguageChange(CultureInfo.CurrentCulture);
                }
                else
                {
                    MessageBox.Show((string)FindResource("AppCheckDuplicate"), (string)FindResource("AppInfo"), MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, (string)FindResource("AppInfo"), MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }

        public static void LanguageChange(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Format("./Localization/StringResource.{0}.xaml", cultureInfo.ToString()), UriKind.Relative)
            });
        }
    }
}
