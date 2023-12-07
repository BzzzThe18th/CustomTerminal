using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTerminal.Patches
{
    [HarmonyPatch(typeof(Terminal), "Start", MethodType.Normal)]
    internal class TerminalStartPatch
    {
        internal static void Postfix(Terminal __instance)
        {
            Main.terminalInstance = __instance;
            Config.Config.LoadConfig();

            if (Config.Config.useWallpaper.Value)
            {
                /*
                    set up custom wallpaper
                */
                GameObject wallpaper = new GameObject("TerminalBackground");
                wallpaper.transform.SetParent(__instance.topRightText.transform.parent, false);
                wallpaper.transform.localPosition = new Vector3(30,15,0);
                wallpaper.transform.localScale = new Vector3(5,5,5);
                wallpaper.transform.SetSiblingIndex(2);
                Main.wallpaperInstance = wallpaper.AddComponent<RawImage>();
                Main.wallpaperInstance.texture = Main.GetTextureFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wallpaper.png"));
            }
            /*
                convert epic 256 bit rgb to cringe unity rgb
            */
            Color colorTheme = new Color(Config.Config.colorThemeR.Value/255, Config.Config.colorThemeG.Value/255, Config.Config.colorThemeB.Value/255);

            if (!Config.Config.uiGamerMode.Value)
            {
                /*
                    set all the ui colors
                */
                __instance.screenText.caretColor = colorTheme;
                __instance.topRightText.color = colorTheme;
                __instance.inputFieldText.color = colorTheme;
                __instance.scrollBarVertical.image.color = colorTheme;
                __instance.scrollBarVertical.GetComponent<Image>().color = colorTheme;
                /*
                    the alpha on the back image of the credits display needs to maintain it's alpha value for visibility
                */
                float imageAlpha = __instance.topRightText.transform.parent.GetChild(6).GetComponent<Image>().color.a;
                __instance.topRightText.transform.parent.GetChild(5).GetComponent<Image>().color = new Color(colorTheme.r,colorTheme.g,colorTheme.b,imageAlpha);
            }

            if (!Config.Config.lightGamerMode.Value)
            {
                /*
                    set the light color and intensity
                */
                __instance.terminalLight.color = colorTheme;
                __instance.terminalLight.intensity = Config.Config.lightIntensity.Value;
            }
        }
    }
    [HarmonyPatch(typeof(Terminal), "SetTerminalNoLongerInUse", MethodType.Normal)]
    internal class TerminalDeactivePatch
    {
        /*
            I will use a prefix bool here to run our code instead of the original code
        */
        internal static bool Prefix(Terminal __instance) {
            __instance.placeableObject.inUse = false;
            __instance.terminalLight.enabled = Config.Config.lightStaysOn.Value;
            return false;
        }
    }
    [HarmonyPatch(typeof(Terminal), "SetTerminalInUseClientRpc", MethodType.Normal)]
    internal class TerminalInUseClientRPC
    {
        /*
            I'll just use a postfix here because I don't feel like re-writing the original code, some delay but whatever
        */
        internal static void Postfix(Terminal __instance, bool inUse) {
            __instance.StopCoroutine("waitUntilFrameEndToSetActive");
            __instance.terminalLight.enabled = Config.Config.lightStaysOn.Value ? true : inUse;
        }
    }
    
    [HarmonyPatch(typeof(Terminal), "QuitTerminal", MethodType.Normal)]
    internal class TerminalQuitPatch
    {
        /*
            postfix into ienumerator because I cannot figure out how to properly patch an ienumerator for the life of me
        */
        internal static void Postfix(Terminal __instance)
        {
            if (Config.Config.monitorStaysOn.Value)
                __instance.StartCoroutine(waitUntilFrameEndTwiceToSetActive(__instance));
        }

        static IEnumerator waitUntilFrameEndTwiceToSetActive(Terminal terminal)
        {
            /*
                wait two frames since original waits one
            */
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            terminal.terminalUIScreen.gameObject.SetActive(true);
            yield break;
        }
    }
}