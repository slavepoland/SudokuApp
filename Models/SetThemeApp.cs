using SudokuApp.Services.Device.Platform;
using SudokuApp.Services.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Models
{
    public class SetThemeApp
    {
        public SetThemeApp()
        {
            SetTheme();
            SettingsService.Instance.PropertyChanged += OnSettingsPropertyChanged;
        }
       
        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsService.Theme))
            {
                SetTheme();
            }
        }

        public void SetTheme()
        {
            //themeModels = Application.Current.UserAppTheme;
            //if (appTheme == themeModels)
            //{
            //    App.Current.UserAppTheme = SettingsService.Instance.Theme.AppTheme;
            //}
            //else
            //{
            //    App.Current.UserAppTheme = AppTheme.Light;
            //}

            Application.Current.UserAppTheme = SettingsService.Instance?.Theme != null
                         ? SettingsService.Instance.Theme.AppTheme
                         : AppTheme.Unspecified;

            switch (App.Current.UserAppTheme)
            {
                case AppTheme.Light:
                    DeviceService.Instance.SetStatusBarColor(Colors.White, true);
                    break;
                case AppTheme.Dark:
                    DeviceService.Instance.SetStatusBarColor(Colors.Black, false);
                    break;
                case AppTheme.Unspecified when App.Current.RequestedTheme == AppTheme.Light:
                    DeviceService.Instance.SetStatusBarColor(Colors.White, true);
                    break;
                case AppTheme.Unspecified:
                    DeviceService.Instance.SetStatusBarColor(Colors.Black, false);
                    break;
            }
        }
    }
}
