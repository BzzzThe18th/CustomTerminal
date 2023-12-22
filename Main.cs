using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTerminal
{
    public class ModInfo
    {
        public const string name = "Custom Terminal";
        public const string guid = "buzzbb.customterminal";
        public const string version = "1.0.0";
    }

    [BepInPlugin(ModInfo.guid, ModInfo.name, ModInfo.version)]
    public class Main : BaseUnityPlugin
    {
        private Harmony harmonyInstance = new Harmony(ModInfo.guid);
        private Color currentGamerRGB;

        public static Terminal terminalInstance;
        public static RawImage wallpaperInstance;
        public static Image currencyBackground;
        public static Image scrollbarHandle;

        public static Texture2D GetTextureFromFile(string path)
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1000,1000);
            texture.LoadImage(imageData);
            texture.filterMode = FilterMode.Point;
            return texture;
        }
        private static Color ColorTheme()
        {
            return new Color(CustomTerminal.Config.Config.colorThemeR.Value/255, CustomTerminal.Config.Config.colorThemeG.Value/255, CustomTerminal.Config.Config.colorThemeB.Value/255);
        }

        public static void RefreshAll()
        {
            if (terminalInstance == null)
                return;
            Color colorTheme = ColorTheme();
            if (CustomTerminal.Config.Config.useWallpaper.Value)
            {
                /*
                    set to color theme if enabled, white if not
                */
                if (CustomTerminal.Config.Config.wallpaperColorTheme.Value)
                    wallpaperInstance.color = new Color(colorTheme.r, colorTheme.g, colorTheme.b, CustomTerminal.Config.Config.wallpaperAlpha.Value);
                else
                    wallpaperInstance.color = new Color(1, 1, 1, CustomTerminal.Config.Config.wallpaperAlpha.Value);
            }
            /*
                set all the ui colors
            */
            terminalInstance.screenText.caretColor = colorTheme;
            terminalInstance.topRightText.color = colorTheme;
            terminalInstance.inputFieldText.color = colorTheme;
            terminalInstance.scrollBarVertical.image.color = colorTheme;
            scrollbarHandle.color = colorTheme;
            /*
                the alpha on the back image of the credits display needs to maintain it's alpha value for visibility
            */
            currencyBackground.color = new Color(colorTheme.r,colorTheme.g,colorTheme.b,currencyBackground.color.a);

            /*
                set the light color and intensity
            */
            terminalInstance.terminalLight.color = colorTheme;
            terminalInstance.terminalLight.intensity = CustomTerminal.Config.Config.lightIntensity.Value;
            terminalInstance.terminalLight.range = CustomTerminal.Config.Config.lightRange.Value;
        }

        void Awake()
        {
            Logger.LogInfo($"Reached awake point for {ModInfo.name}@{ModInfo.version}, applying patches");
            /*
                this applies all harmony patches, to unpatch, use harmonyInstance.UnpatchSelf()
                this template includes an example patch to learn from
            */
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo("Applied patches");
        }

        void FixedUpdate() {
            if (terminalInstance != null)
            {
                currentGamerRGB = Color.HSVToRGB(Mathf.PingPong(Time.time / 25 * CustomTerminal.Config.Config.rgbSpeed.Value, 1), 1, 1);

                if (CustomTerminal.Config.Config.uiGamerMode.Value)
                {
                    /*
                        set all the ui colors
                    */
                    terminalInstance.screenText.caretColor = currentGamerRGB;
                    terminalInstance.topRightText.color = currentGamerRGB;
                    terminalInstance.inputFieldText.color = currentGamerRGB;
                    terminalInstance.scrollBarVertical.image.color = currentGamerRGB;
                    terminalInstance.scrollBarVertical.GetComponent<Image>().color = currentGamerRGB;
                    /*
                        the alpha on the back image of the credits display needs to maintain it's alpha value for visibility
                    */
                    currencyBackground.color = new Color(currentGamerRGB.r,currentGamerRGB.g,currentGamerRGB.b,currencyBackground.color.a);
                }
                if (CustomTerminal.Config.Config.lightGamerMode.Value)
                {
                    /*
                        set the light color
                    */
                    terminalInstance.terminalLight.color = currentGamerRGB;
                }
                if (CustomTerminal.Config.Config.wallpaperGamerMode.Value && CustomTerminal.Config.Config.useWallpaper.Value)
                {
                    if (wallpaperInstance != null) {
                        /*
                            set the wallpaper image color
                        */
                        wallpaperInstance.color = currentGamerRGB;
                    }
                }
            }
        }
    }
}
