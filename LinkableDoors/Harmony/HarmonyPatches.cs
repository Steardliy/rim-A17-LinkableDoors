using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using System.Reflection;

namespace LinkableDoors.NewFolder1
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.steardliy.LinkableDoors");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        [HarmonyPatch(typeof(Building_Door), "Draw", new Type[0])]
        static class Building_Door_Draw_Fix
        {
            public static bool Prefix(Building_Door __instance)
            {

            }
        }
        [HarmonyPatch(typeof(Building_Door), "SpawnSetup", new Type[] { typeof(Map), typeof(bool) })]
        static class Building_Door_SpawnSetup_Fix
        {
            public static void Postfix(Building_Door __instance)
            {

            }
        }
        [HarmonyPatch(typeof(Building_Door), "DeSpawn", new Type[0])]
        static class Building_Door_DeSpawn_Fix
        {
            public static void Postfix(Building_Door __instance)
            {

            }
        }
    }
}
