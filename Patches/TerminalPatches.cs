using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using CommandLib;
using System;

namespace CustomTerminal.Patches
{
    [HarmonyPatch(typeof(Terminal), "Start", MethodType.Normal)]
    internal class TerminalStartPatch
    {
        internal static void Postfix(Terminal __instance)
        {
            Config.Config.LoadConfig();
            /*
                set up references
            */
            Main.currencyBackground = __instance.topRightText.transform.parent.GetChild(5).GetComponent<Image>();
            Main.scrollbarHandle = __instance.scrollBarVertical.GetComponent<Image>();
            try {
                GameObject wallpaper = new GameObject("TerminalBackground");
                wallpaper.transform.SetParent(__instance.topRightText.transform.parent, false);
                wallpaper.transform.localPosition = new Vector3(30,15,0);
                wallpaper.transform.localScale = new Vector3(5,5,5);
                wallpaper.transform.SetSiblingIndex(2);
                Main.wallpaperInstance = wallpaper.AddComponent<RawImage>();
                Main.wallpaperInstance.texture = Main.GetTextureFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wallpaper.png"));
                wallpaper.SetActive(false); 
            }
            catch (Exception e) {
                Debug.Log($"Failed to set up wallpaper: {e}");
            }
            /*
                refresh all the junk when terminal starts
            */
            Main.terminalInstance = __instance;
            Main.RefreshAll();

            /*
                register terminal commands with commandlib
            */
            Commands.RegisterCommand("refresh", "Refreshed configuration.\n", "ct_Refresh", __instance);
            Commands.RegisterCommand("uirgb", "Toggled the gamer RGB on UI elements.\n", "ct_ToggleUIRGB", __instance);
            Commands.RegisterCommand("lightrgb", "Toggled the gamer RGB on terminal lights.\n", "ct_ToggleLightRGB", __instance);
            Commands.RegisterCommand("wallpaperrgb", "Toggled the gamer RGB on your wallpaper.\n", "ct_ToggleWallpaperRGB", __instance);
            Commands.RegisterCommand("wallpapercolor", "Toggled the use of custom color theme for wallpaper.\n", "ct_ToggleWallpaperColor", __instance);
            Commands.RegisterCommand("wallpaper", "Toggled the use of wallpaper.\n", "ct_ToggleWallpaper", __instance);
            Commands.RegisterCommand("monitorstay", "Toggled monitor staying on when exiting the terminal.\n", "ct_ToggleUIStay", __instance);
            Commands.RegisterCommand("lightstay", "Toggled lights staying on when exiting the terminal.\n", "ct_ToggleLightStay", __instance);
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

    [HarmonyPatch(typeof(Terminal), "RunTerminalEvents", MethodType.Normal)]
    internal class TerminalRunEventPatch
    {
        /*
            I'm going to pass in my node event to the end here after registering it with my shitty library
        */
        internal static void Postfix(Terminal __instance, TerminalNode node)
        {
            if (string.IsNullOrWhiteSpace(node.terminalEvent))
            {
                return;
            }
            switch (node.terminalEvent)
            {
                case "ct_Refresh":
                    Config.Config.LoadConfig();
                    break;
                case "ct_ToggleUIRGB":
                    Config.Config.uiGamerMode.Value = !Config.Config.uiGamerMode.Value;
                    break;
                case "ct_ToggleLightRGB":
                    Config.Config.lightGamerMode.Value = !Config.Config.lightGamerMode.Value;
                    break;
                case "ct_ToggleWallpaper":
                    if (Main.wallpaperInstance == null)
                    {
                        Config.Config.useWallpaper.Value = true;
                    }
                    else
                    {
                        if (Config.Config.useWallpaper.Value)
                        {
                            Config.Config.useWallpaper.Value = false;
                            Main.wallpaperInstance.gameObject.SetActive(false);
                        }
                        else
                        {
                            Config.Config.useWallpaper.Value = true;
                            Main.wallpaperInstance.gameObject.SetActive(true);
                        }
                    }
                    break;
                case "ct_ToggleWallpaperRGB":
                    Config.Config.wallpaperGamerMode.Value = !Config.Config.wallpaperGamerMode.Value;
                    break;
                case "ct_ToggleWallpaperColor":
                    Config.Config.wallpaperColorTheme.Value = !Config.Config.wallpaperColorTheme.Value;
                    break;
                case "ct_ToggleUIStay":
                    Config.Config.monitorStaysOn.Value = !Config.Config.monitorStaysOn.Value;
                    break;
                case "ct_ToggleLightStay":
                    Config.Config.lightStaysOn.Value = !Config.Config.lightStaysOn.Value;
                    break;
            }
            //TODO: rework config for saving immediately here
            Main.RefreshAll();
        }
    }
}