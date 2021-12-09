using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RefillingContainers
{
    internal static class Patches
    {
        [HarmonyPatch(typeof(Container), "OnContainerSearchComplete")]
        internal static class Container_OnContainerSearchComplete
        {
            internal static void Postfix(Container __instance, bool success, bool playerCancel, float progress)
            {
                var refill = __instance.GetComponent<Refill>();

                if (!refill)
                {
                    refill = __instance.gameObject.AddComponent<Refill>();
                    refill.m_Container = __instance;
                }

                refill.UpdateDaySearched();
            }
        }

        [HarmonyPatch(typeof(Container), "Open")]
        internal static class Container_Open
        {
            internal static void Prefix(Container __instance)
            {
                __instance.GetComponent<Refill>()?.OnOpened();
            }
        }

        [HarmonyPatch(typeof(Container), "Close")]
        internal static class Container_Close
        {
            internal static void Postfix(Container __instance)
            {
                __instance.GetComponent<Refill>()?.OnClosed();
            }
        }

        [HarmonyPatch(typeof(Container), "Serialize")]
        internal static class Container_Serialize
        {
            internal static void Postfix(Container __instance, ref string __result)
            {
                var refill = __instance.GetComponent<Refill>();

                if (refill)
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(__result);
                    dict["m_DaySearched"] = refill.m_DaySearched;

                    __result = JsonConvert.SerializeObject(dict);
                }
            }
        }

        [HarmonyPatch(typeof(Container), "Deserialize")]
        internal static class Container_Deserialize
        {
            internal static void Postfix(Container __instance, string text, List<GearItem> loadedItems)
            {
                var refill = __instance.gameObject.AddComponent<Refill>();
                refill.m_Container = __instance;

                var jo = JObject.Parse(text);

                if (jo.ContainsKey("m_DaySearched"))
                {
                    refill.m_DaySearched = jo.GetValue("m_DaySearched").ToObject<int>();
                }
                else if (__instance.IsInspected())
                {
                    refill.UpdateDaySearched();
                }
            }
        }
    }
}
