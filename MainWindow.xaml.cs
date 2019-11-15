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
        private static readonly RegistryKey CORTANA_PARENT_KEY = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Policies").OpenSubKey("Microsoft").OpenSubKey("Windows");
        private static string CORTANA_KEY_NAME = "Windows Search";
        private static string CORTANA_STATE_VALUE_NAME = "AllowCortana";

        private static readonly RegistryKey BING_SEARCH_PARENT_KEY = Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Search");
        private static string[] BING_SEARCH_VALUE_NAMES = { "BingSearchEnabled", "CortanaConsent" };
        private static string BING_SEARCH_CORTANA_VALUE_NAME = "DisableWebSearch";

        private static int IntFromBool(bool b)
        {
            return b ? 1 : 0;
        }

        private static bool CortanaIsEnabled()
        {
            RegistryKey cortanaParentKey = CORTANA_PARENT_KEY.OpenSubKey(CORTANA_KEY_NAME);
            if (cortanaParentKey == null)
                return true;

            var allowCortana = cortanaParentKey.GetValue(CORTANA_STATE_VALUE_NAME);
            if (allowCortana == null)
                return true;

            return (int)allowCortana == 1;
        }

        private static void CortanaSetEnabled(bool enabled)
        {
            RegistryKey cortanaParentKey = CORTANA_PARENT_KEY.OpenSubKey(CORTANA_KEY_NAME, true);
            if (cortanaParentKey == null)
                cortanaParentKey = CORTANA_PARENT_KEY.CreateSubKey(CORTANA_KEY_NAME);

            cortanaParentKey.SetValue(CORTANA_STATE_VALUE_NAME, IntFromBool(enabled), RegistryValueKind.DWord);
        }

        private static bool BingSearchIsEnabled()
        {
            foreach (string valueName in BING_SEARCH_VALUE_NAMES)
            {
                var value = BING_SEARCH_PARENT_KEY.GetValue(valueName);
                if (value == null || (int)value == 1)
                    return true;
            }

            RegistryKey cortanaParentKey = CORTANA_PARENT_KEY.OpenSubKey(CORTANA_KEY_NAME);
            if (cortanaParentKey == null)
                return true;

            var disableWebSearch = cortanaParentKey.GetValue(BING_SEARCH_CORTANA_VALUE_NAME);
            if (disableWebSearch == null || (int)disableWebSearch == 0)
                return true;

            return false;
        }

        private static void BingSearchSetEnabled(bool enabled)
        {
            foreach (string valueName in BING_SEARCH_VALUE_NAMES)
            {
                BING_SEARCH_PARENT_KEY.SetValue(valueName, IntFromBool(enabled), RegistryValueKind.DWord);
            }

            RegistryKey cortanaParentKey = CORTANA_PARENT_KEY.OpenSubKey(CORTANA_KEY_NAME);
            if (cortanaParentKey == null)
                return;

            cortanaParentKey.SetValue(BING_SEARCH_CORTANA_VALUE_NAME, IntFromBool(!enabled), RegistryValueKind.DWord);
        }

        public MainWindow()
        {
            InitializeComponent();
            CortanaStateComboBox.SelectedIndex = IntFromBool(CortanaIsEnabled());
            BingSearchStateComboBox.SelectedIndex = IntFromBool(BingSearchIsEnabled());
        }

        private void RestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            CortanaStateComboBox.SelectedIndex = 1;
            BingSearchStateComboBox.SelectedIndex = 1;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            CortanaSetEnabled(CortanaStateComboBox.SelectedIndex == 1);
            BingSearchSetEnabled(BingSearchStateComboBox.SelectedIndex == 1);
        }
    }
}
