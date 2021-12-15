using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace RefillingContainers
{
    internal static class Patches
    {
        [HarmonyPatch(typeof(Container), "Awake")]
        internal static class Container_Awake
        {
            internal static void Postfix(Container __instance)
            {
                var refill = __instance.gameObject.AddComponent<Refill>();
                refill.m_Container = __instance;
            }
        }

        [HarmonyPatch(typeof(Container), "Open")]
        internal static class Container_Open
        {
            internal static void Prefix(Container __instance)
            {
                __instance.GetComponent<Refill>().OnOpen();
            }
        }

        [HarmonyPatch(typeof(Container), "Close")]
        internal static class Container_Close
        {
            internal static void Postfix(Container __instance)
            {
                __instance.GetComponent<Refill>().OnClose();
            }
        }

        [HarmonyPatch(typeof(Container), "Serialize")]
        internal static class Container_Serialize
        {
            internal static void Postfix(Container __instance, ref string __result)
            {
                var refill = __instance.GetComponent<Refill>();

                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(__result);
                dict[nameof(Refill.m_DaySearched)] = refill.m_DaySearched;

                __result = JsonConvert.SerializeObject(dict);
            }
        }

        [HarmonyPatch(typeof(Container), "Deserialize")]
        internal static class Container_Deserialize
        {
            internal static void Postfix(Container __instance, string text, List<GearItem> loadedItems)
            {
                var refill = __instance.GetComponent<Refill>();

                var jo = JObject.Parse(text);

                if (jo.ContainsKey(nameof(Refill.m_DaySearched)))
                {
                    refill.m_DaySearched = jo.GetValue(nameof(Refill.m_DaySearched)).ToObject<int>();

                    if (Settings.options.modEnabled)
                    {
                        refill.DoRefill();
                    }
                }
                else if (__instance.IsInspected())
                {
                    refill.UpdateDaySearched();
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDay), "DoEndOfDayAnalytics")]
        internal static class TimeOfDay_DoEndOfDayAnalytics
        {
            internal static void Postfix(TimeOfDay __instance)
            {
#if DEBUG
                MelonLogger.Msg("Doing scheduled refill on day {0}", GameManager.m_TimeOfDay.GetDayNumber());
#endif
                RefillManager.RefillAll();
            }
        }
    }
}
