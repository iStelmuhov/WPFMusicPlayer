using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MaterialDesignColors;
using Newtonsoft.Json;

namespace WPFMusicPlayer.Classes
{

    
    public class ProgramSettings
    {
        public bool SaveLoginPassword { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }


        public String PrimaryColor { get; set; }

        public String AccentColor { get; set; }

        public bool IsDark { get; set; }

        public ProgramSettings()
        {
            SaveLoginPassword = false;
            Login = string.Empty;
            Password = string.Empty;

            PrimaryColor = "blue";

            AccentColor ="red";

            IsDark = false;
        }

        public static Swatch FindSwatchByName(string name)
        {

            return new SwatchesProvider().Swatches.FirstOrDefault(
                swatch => swatch.Name == name);
        }
    }
}