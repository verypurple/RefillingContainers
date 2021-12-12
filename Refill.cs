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
            if (!m_Container.IsInspected() || !m_Container.IsEmpty())
            {
                return;
            }

            var currentDay = GameManager.m_TimeOfDay.GetDayNumber();

            if (currentDay >= m_DaySearched + Settings.options.refillAfterDays)
            {
                m_Container.m_LootTable = m_Container.m_LootTablePrefab;
                m_Container.m_Inspected = false;
                m_Container.m_GearToInstantiate.Clear();

                int empty = rand.Next(0, 100);
                if (empty < GetChanceEmptyModifier(m_Container.m_ChanceEmpty))
                {
                    return;
                }

                float max = Mathf.Max(0, m_Container.m_MaxRandomItems) * GetDensityModifier();
                float min = Mathf.Min(m_Container.m_MinRandomItems, max);
                int count = rand.Next((int)Mathf.Round(min), (int)Mathf.Round(max) + 1);

                for (var i = 0; i < count; i++)
                {
                    var prefab = m_Container.m_LootTablePrefab.GetRandomGearPrefab();

                    if (prefab)
                    {
                        m_Container.m_GearToInstantiate.Add(prefab.name);
                    }
                }
            }
        }

        [HideFromIl2Cpp]
        private float GetChanceEmptyModifier(float chanceEmpty)
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
