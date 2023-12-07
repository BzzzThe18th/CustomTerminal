using BepInEx;
using BepInEx.Configuration;
using System.IO;

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
        public static ConfigEntry<bool> monitorStaysOn, useWallpaper, wallpaperGamerMode;

        public static void LoadConfig()
        {
            ConfigFile cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "CustomTerminal.cfg"), true);

            colorThemeR = cfg.Bind("Colors", "Red", 255f, "Red value between 0 and 255 for terminal color theme");
            colorThemeG = cfg.Bind("Colors", "Green", 255f, "Green value between 0 and 255 for terminal color theme");
            colorThemeB = cfg.Bind("Colors", "Blue", 255f, "Blue value between 0 and 255 for terminal color theme");
            
            uiAlpha = cfg.Bind("UI", "Transparency", 255f, "Alpha value between 0 and 255 for all fully opaque terminal UI elements");
            uiGamerMode = cfg.Bind("UI", "Gamer RGB", true, "Whether or not to use gamer rgb for the ui element colors");

            lightIntensity = cfg.Bind("Lights", "Intensity", 30f, "The intensity of the lights on the terminal that are by defualt only on when in the terminal");
            lightStaysOn = cfg.Bind("Lights", "Keep Lights On", true, "Whether or not to keep the lights on when outside of the terminal");
            lightGamerMode = cfg.Bind("Lights", "Gamer RGB", true, "Whether or not to use gamer rgb lights for the terminal light");

            rgbSpeed = cfg.Bind("Misc", "RGB Speed", 1f, "The speed at which the RGB mode fades through colors");
            monitorStaysOn = cfg.Bind("Misc", "Keep Monitor On", true, "Keeps the monitor on even when the terminal is not in use");
            useWallpaper = cfg.Bind("Misc", "Use Wallpaper", true, "Enables the use of a loaded wallpaper from the mod directory with the name 'wallpaper.png'");
            wallpaperGamerMode = cfg.Bind("Misc", "Wallpaper Gamer RGB", true, "Whether or not to use gamer rgb for the wallpaper color, recommended to keep on true if light is on gamer mode to look like it is effected by light");
        }
    }
}