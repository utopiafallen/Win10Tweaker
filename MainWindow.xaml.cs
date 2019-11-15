using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;

namespace Win10Tweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int IntFromBool(bool b)
        {
            return b ? 1 : 0;
        }

        private static bool CortanaIsEnabled()
        {
            RegistryKey windowsKey = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Policies").OpenSubKey("Microsoft").OpenSubKey("Windows");

            RegistryKey windowsSearchKey = windowsKey.OpenSubKey("Windows Search");
            if (windowsSearchKey == null)
                return true;

            var allowCortana = windowsSearchKey.GetValue("AllowCortana");
            if (allowCortana == null)
                return true;

            return (int)allowCortana == 1;
        }

        private static bool BingSearchIsEnabled()
        {
            RegistryKey searchKey = Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Search");

            var bingSearchEnabled = searchKey.GetValue("BingSearchEnabled");
            var cortanaConsent = searchKey.GetValue("CortanaConsent");

            if (bingSearchEnabled == null || (int)bingSearchEnabled == 1)
                return true;

            if (cortanaConsent == null || (int)cortanaConsent == 1)
                return true;

            return false;
        }

        public MainWindow()
        {
            InitializeComponent();
            CortanaStateComboBox.SelectedIndex = IntFromBool(CortanaIsEnabled());
            BingSearchStateComboBox.SelectedIndex = IntFromBool(BingSearchIsEnabled());
        }
    }
}
