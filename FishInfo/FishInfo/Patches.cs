using Harmony;
using StardewValley;
using StardewValley.Menus;
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
        public class CollectionsPagePatches
        {
            public static void Postfix(CollectionsPage __instance, ref string __result, int index)
            {
                if (!Game1.objectInformation.TryGetValue(index, out string itemData)) // Break if it's not an item - dodges weird edge cases
                {
                    return;
                }

                string[] split = itemData.Split(new char[] { '/' });

                if (!split[3].Contains("Fish")) { return; } //break if it isn't a fish

                FishData loadedData;
                if (ModEntry.FishInfo.TryGetValue(index, out loadedData))
                {
                    __result += ModEntry.FishInfo[index];
                }
            }
        }

    }
}
