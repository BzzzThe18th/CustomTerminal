using BepInEx;
using BepInEx.Configuration;
using System.IO;
using UnityEngine.Rendering.HighDefinition;

namespace CustomTerminal.Config
{
    internal class Config
    {
        public static ConfigEntry<float> colorThemeR, colorThemeG, colorThemeB;
        public static ConfigEntry<float> uiAlpha;
        public static ConfigEntry<bool> uiGamerMode;
        public static ConfigEntry<float> lightIntensity;
        public static ConfigEntry<bool> lightStaysOn, lightGamerMode;
        public static ConfigEntry<float> rgbSpeed;

        public static void LoadConfig()
        {
            ConfigFile cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "CustomTerminal.cfg"), true);

            colorThemeR = cfg.Bind("Colors", "Red", 255f, "Red value between 0 and 255 for terminal color theme");
            colorThemeG = cfg.Bind("Colors", "Green", 255f, "Green value between 0 and 255 for terminal color theme");
            colorThemeB = cfg.Bind("Colors", "Blue", 255f, "Blue value between 0 and 255 for terminal color theme");
            
            uiAlpha = cfg.Bind("UI", "Transparency", 255f, "Alpha value between 0 and 255 for all fully opaque terminal UI elements");
            uiGamerMode = cfg.Bind("UI", "Gamer RGB", false, "Whether or not to use gamer rgb for the ui element colors");

            lightIntensity = cfg.Bind("Lights", "Intensity", 15f, "The intensity of the lights on the terminal that are by defualt only on when in the terminal");
            lightStaysOn = cfg.Bind("Lights", "Keep Lights On", false, "Whether or not to keep the lights on when outside of the terminal");
            lightGamerMode = cfg.Bind("Lights", "Gamer RGB", false, "Whether or not to use gamer rgb lights for the terminal light");

            rgbSpeed = cfg.Bind("Misc", "RGB Speed", 1f, "The speed at which the RGB mode fades through colors");
            cfg.Save();
        }
    }
}