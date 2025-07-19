using Microsoft.Xna.Framework.Input;
using osu;

namespace OsuLoader.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch]
public class MainPatch
{
    internal static List<Keys> KeysQueue = new();
    private static bool gameStarted;
    private static Dictionary<string, string> fileMap = new()
    {
        { LoaderMain.OsuExePath, LoaderMain.OsuOriginalPath },
        { "osu!.exe", "osu!-backup.exe" }
    };
    
    [HarmonyPrefix, HarmonyPatch("#=zLEOSHzaRD49csLZzhHBM8rQ=", "#=zCG6rNX1F1WQE")]
    static void GlobalMD5Inject(ref string __0)
    {
        if (fileMap.TryGetValue(__0, out var value))
            __0 = value;
    }

    [HarmonyPrefix, HarmonyPatch("#=zbeqIGRFXmLRQ9xnCC4QgSVE=", "#=z$rg9QkVOa$4N")]
    static void LocalMD5Inject(ref string __0)
    {
        if (fileMap.TryGetValue(__0, out var value))
            __0 = value;
    }
    
    [HarmonyPostfix, HarmonyPatch("#=z_30JNFvlfFotBr3jlgSv$ts=", "#=ziXd1tto=")]
    static void SignInject(ref bool __result)
    {
        __result = true;
    }

    [HarmonyPrefix, HarmonyPatch("#=zdEqWFlrNfRD97Tqz0Q==", "#=z4byN7N7FXptSTnXniA==")]
    static void ModeChangeDirectInject(OsuModes __0)
    {
        if (__0 == OsuModes.Play)
            gameStarted = false;
    }

    [HarmonyPrefix, HarmonyPatch("#=zP$TXuW9OveBuc$n8B_U9kWYQBsfm", "#=zr__5T0o=")]
    static void GameUpdateInject(object __instance)
    {
        var mode = OsuManager.CurrentPlayMode;
        if (!gameStarted && mode == OsuModes.Play)
        {
            string string1;
            object object4;
            try
            {
                var object1 = Reflector.GetField<object>(__instance, "#=z222U6SLVD5dq");
                string1 = Reflector.GetField<string>(object1, "#=zz_ay$wx2bKp4");
                var object2 = Reflector.GetField<object>(__instance, "#=zMPiDHus=");
                var object3 = Reflector.GetField<object>(object2, "#=zt_V_Ps0=");
                object4 = Reflector.GetField<object>(object3, "#=zQ2HInVssZBKO");

            }
            catch (Exception) { return; }
            
            gameStarted = true;
            Console.WriteLine(">>> GAME STARTED! <<<");
            Console.WriteLine("Beatmap: " + string1);
            Console.WriteLine("Mods: " + object4);
            OsuManager.Reset();
        }

        if (gameStarted && mode == OsuModes.Play)
        {
            try
            {
                var object1 = Reflector.GetField<object>(__instance, "#=zMPiDHus=");
                var object2 = Reflector.GetField<object>(object1, "#=zt_V_Ps0=");
                var object3 = Reflector.GetField<object>(object1, "#=zxj80IcITQJht");
                if (object2 is not null && object3 is not null)
                {
                    var score = Reflector.CallMethod<int>(object2, "#=z0s9GuwwQ4jkQ");
                    var accuracy = Reflector.CallMethod<float>(object2, "#=zjFtmtnWYcwRFRgiTSA==") * 100.0f;
                    var combo = Reflector.CallMethod<int>(object3, "#=za_w$axo5Ws_J");
                    var __300 = Reflector.GetField<ushort>(object2, "#=zEL7g0Vo=");
                    var __100 = Reflector.GetField<ushort>(object2, "#=zF1xddhA=");
                    var __50 = Reflector.GetField<ushort>(object2, "#=zFbNF$5g=");
                    var perfect = Reflector.GetField<ushort>(object2, "#=z7qbgB98JI_3S");
                    var good = Reflector.GetField<ushort>(object2, "#=zUKsEF3IdIEIL");
                    var miss = Reflector.GetField<ushort>(object2, "#=zDM8b3TI=");
                    if (OsuManager.Update(score, accuracy, combo, __300, __100, __50, perfect, good, miss))
                        Console.WriteLine(
                            "Score: {0} | Acc: {1:F2}% | Combo: {2} | 300s: {3} | 100s: {4} | 50s: {5} | Perfect: {6} | Good: {7} | Miss: {8}",
                            score, accuracy, combo, __300, __100, __50, perfect, good, miss
                        );
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }

    [HarmonyPrefix, HarmonyPatch("#=zd67Vp1YN0U1ubMsGhQ==", "#=zGbjhAMY=")]
    static void KeyInputInject(List<Keys> __0)
    {
        __0.AddRange(KeysQueue);
        KeysQueue.Clear();
    }
}
