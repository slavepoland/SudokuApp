using SudokuApp.Models;
using SudokuApp.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace SudokuApp.Services.Settings
{
    public class SettingsService : BaseViewModel
    {
        private static SettingsService _instance;
        public static SettingsService Instance => _instance ??= new SettingsService();

        public SettingsService()
        {
            // set the default (in advanced scenarios, this could be read from the preferences)
            Theme = Theme.System;            
        }
        public SettingsService(AppTheme appTheme)
        {
            // set the default (in advanced scenarios, this could be read from the preferences)
            if (appTheme == AppTheme.Light)
            {
                Theme = Theme.Light;

            }
            else if (appTheme == AppTheme.Dark)
            {
                Theme = Theme.Dark;
            }
        }

        private Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set
            {
                if (_theme == value) return;
                _theme = value;
                OnPropertyChanged();
            }
        }
    }
}
