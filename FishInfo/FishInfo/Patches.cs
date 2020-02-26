using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace FishInfo
{
    public class Patches
    {
        public static void DoPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("speshkitty.fishinfo.harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }


        [HarmonyPatch(typeof(CollectionsPage))]
        [HarmonyPatch("createDescription")]
        public class CollectionsPage_CreateDescription
        {
            public static void Postfix(CollectionsPage __instance, ref string __result, int index)
            {
                if (!Game1.objectInformation.TryGetValue(index, out string itemData)) // Break if it's not an item - dodges weird edge cases
                {
                    return;
                }

                string[] split = itemData.Split(new char[] { '/' });

                if (!split[3].Contains("Fish")) { return; } //break if it isn't a fish
                
                if (ModEntry.FishInfo.TryGetValue(index, out FishData loadedData))
                {
                    __result += loadedData;
                }
                
            }
        }
        
    }
}
