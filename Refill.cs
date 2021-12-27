using MelonLoader;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using static CustomExperienceModeManager;

namespace RefillingContainers
{
    [RegisterTypeInIl2Cpp]
    class Refill : MonoBehaviour
    {
        public Container m_Container;
        public int m_DaySearched;

        private static System.Random rand = new System.Random();
        private bool m_WasEmpty;

        public Refill(IntPtr intPtr) : base(intPtr) { }

        void Start()
        {
#if DEBUG
            MelonLogger.Msg("Refill GameObject Start {0}", name);
#endif
            m_Container = GetComponent<Container>();

            if (!m_Container)
            {
                MelonLogger.Warning("Container component missing {0}", name);
                return;
            }

            RefillManager.Register(this);
        }

        void OnDestroy()
        {
            RefillManager.Unregister(this);
        }

        internal void OnOpen()
        {
            m_WasEmpty = m_Container.IsInspected() && m_Container.IsEmpty();
        }

        internal void OnClose()
        {
            if (!m_WasEmpty && m_Container.IsEmpty())
            {
                UpdateDaySearched();
            }
        }

        internal void UpdateDaySearched()
        {
            m_DaySearched = GameManager.m_TimeOfDay.GetDayNumber();
        }

        internal void DoRefill()
        {
#if DEBUG
            MelonLogger.Msg(ConsoleColor.DarkGray, "Checking {0}", name);
#endif
            if (!(m_Container.IsInspected() && m_Container.IsEmpty()))
            {
#if DEBUG
                MelonLogger.Msg(ConsoleColor.DarkGray, "Unsearched or not empty");
#endif
                return;
            }

            var currentDay = GameManager.m_TimeOfDay.GetDayNumber();
            var updateDay = m_DaySearched + Settings.options.refillAfterDays;

            if (currentDay >= updateDay)
            {
                m_Container.m_LootTable = m_Container.m_LootTablePrefab;
                m_Container.m_Inspected = false;
                m_Container.m_GearToInstantiate.Clear();

                int roll = rand.Next(0, 100);
                float chanceEmpty = GetModifiedChanceEmpty(m_Container.m_ChanceEmpty);
                if (roll < chanceEmpty)
                {
#if DEBUG
                    MelonLogger.Msg(ConsoleColor.DarkGray, "Bad luck, got {0} needed {1}", roll, chanceEmpty);
#endif
                    return;
                }

                float max = Mathf.Max(0, m_Container.m_MaxRandomItems) * GetDensityModifier();
                float min = Mathf.Min(m_Container.m_MinRandomItems, max);
                int count = rand.Next((int)Mathf.Round(min), (int)Mathf.Round(max) + 1);

#if DEBUG
                MelonLogger.Msg(ConsoleColor.DarkGray, "Filling with {0} item(s)", count);
#endif
                for (var i = 0; i < count; i++)
                {
                    var prefab = m_Container.m_LootTablePrefab.GetRandomGearPrefab();

                    if (prefab)
                    {
#if DEBUG
                        MelonLogger.Msg(ConsoleColor.DarkGray, "Adding {0}", prefab.name);
#endif
                        m_Container.m_GearToInstantiate.Add(prefab.name);
                    }
                }
            }
#if DEBUG
            else
            {
                MelonLogger.Msg(ConsoleColor.DarkGray, "Too early. {0} ({1}->{2})", currentDay, m_DaySearched, updateDay);
            }
#endif
        }

        [HideFromIl2Cpp]
        private float GetModifiedChanceEmpty(float chanceEmpty)
        {
            var diff = 100f - chanceEmpty;

            switch (Settings.options.chanceEmptyModifier)
            {
                case CustomTunableNLMH.None:
                    return 0f;
                case CustomTunableNLMH.Low:
                    return chanceEmpty;
                case CustomTunableNLMH.Medium:
                    return chanceEmpty + diff * 0.3f;
                case CustomTunableNLMH.High:
                    return chanceEmpty + diff * 0.6f;
                default:
                    throw new Exception();
            }
        }

        [HideFromIl2Cpp]
        private float GetDensityModifier()
        {
            switch (Settings.options.containerDensityModifier)
            {
                case CustomTunableLMH.Low:
                    return 0.4f;
                case CustomTunableLMH.Medium:
                    return 0.7f;
                case CustomTunableLMH.High:
                    return 1f;
                default:
                    throw new Exception();
            }
        }
    }
}
