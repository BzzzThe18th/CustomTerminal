using BepInEx;
using BepInEx.Configuration;
using System.IO;

namespace CustomTerminal.Config
{
    internal class Config
    {
        public static ConfigFile cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "CustomTerminal.cfg"), true);

        public static ConfigEntry<float> colorThemeR, colorThemeG, colorThemeB;
        public static ConfigEntry<float> uiAlpha;
        public static ConfigEntry<bool> uiGamerMode;
        public static ConfigEntry<float> lightIntensity, lightRange;
        public static ConfigEntry<bool> lightStaysOn, lightGamerMode;
        public static ConfigEntry<bool> useWallpaper, wallpaperGamerMode, wallpaperColorTheme;
        public static ConfigEntry<float> wallpaperAlpha;
        public static ConfigEntry<float> rgbSpeed;
        public static ConfigEntry<bool> monitorStaysOn;

        public static void LoadConfig()
        {
            colorThemeR = cfg.Bind("Colors", "Red", 255f, "Red value between 0 and 255 for terminal color theme");
            colorThemeG = cfg.Bind("Colors", "Green", 255f, "Green value between 0 and 255 for terminal color theme");
            colorThemeB = cfg.Bind("Colors", "Blue", 255f, "Blue value between 0 and 255 for terminal color theme");
            
            uiAlpha = cfg.Bind("UI", "Transparency", 255f, "Alpha value between 0 and 255 for all fully opaque terminal UI elements");
            uiGamerMode = cfg.Bind("UI", "Gamer RGB", true, "Whether or not to use gamer rgb for the ui element colors");

            lightIntensity = cfg.Bind("Lights", "Intensity", 30f, "The intensity of the lights on the terminal that are by defualt only on when in the terminal");
            lightRange = cfg.Bind("Lights", "Range", 10f, "The range of the terminal light, setting to high intensity and range will result in a beam of light shining out of the ship doors");
            lightStaysOn = cfg.Bind("Lights", "Keep Lights On", true, "Whether or not to keep the lights on when outside of the terminal");
            lightGamerMode = cfg.Bind("Lights", "Gamer RGB", true, "Whether or not to use gamer rgb lights for the terminal light");

            useWallpaper = cfg.Bind("Wallpaper", "Enabled", true, "Enables the use of a loaded wallpaper from the mod directory with the name 'wallpaper.png'");
            wallpaperGamerMode = cfg.Bind("Wallpaper", "Gamer RGB", true, "Whether or not to use gamer rgb for the wallpaper color, recommended to keep on true if light is on gamer mode to look like it is effected by light");
            wallpaperColorTheme = cfg.Bind("Wallpaper", "Use Color Theme", false, "Whether or not to use custom color theme color for wallpaper color, if this and gamer rgb are turned off, wallpaper will be normal color");
            wallpaperAlpha = cfg.Bind("Wallpaper", "Transparency", 255f, "Alpha value between 0 and 255 for transparency of the wallpaper");

            rgbSpeed = cfg.Bind("Misc", "RGB Speed", 1f, "The speed at which the RGB mode fades through colors");
            monitorStaysOn = cfg.Bind("Misc", "Keep Monitor On", true, "Keeps the monitor on even when the terminal is not in use");
        }
    }
}