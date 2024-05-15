using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuApp.Services.Vibrations
{
    static class TapVibrations
    {
        public static void PlayVibrations()
        {
            if (Preferences.Default.Get("UserVibrations", "") == "True")
                Vibration.Vibrate(250); 
        }
    }
}
