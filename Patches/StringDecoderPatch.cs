using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using Console = System.Console;

namespace OsuLoader.Patches;

[HarmonyPatch()]
public class StringDecoderPatch
{
    private static Dictionary<int, string> decodedStrings = new();
    private static HashSet<int> traceCodes = [255189429];
    
    [HarmonyPostfix, HarmonyPatch("#=z34g3Q7K28MWhcUVJVAsN309ULQnG", "#=z6TB7efI=")]
    static void Postfix(int __0, ref string __result)
    {
        if (!decodedStrings.ContainsKey(__0))
        {
            decodedStrings[__0] = __result;
            // Console.WriteLine($"{__0} = {__result}");
            // if (traceCodes.Contains(__0))
            //     Console.WriteLine($"Trace: {new StackTrace()}");
        }
    }
}