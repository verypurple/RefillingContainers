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

        private System.Random rand = new System.Random();

        public Refill(IntPtr intPtr) : base(intPtr) { }

        public void Update()
        {
            var currentDay = GameManager.m_TimeOfDay.GetDayNumber();

            if (currentDay >= m_DaySearched + Settings.options.refillAfterDays)
            {
                m_Container.m_LootTable = m_Container.m_LootTablePrefab;
                m_Container.m_Inspected = false;
                m_Container.m_GearToInstantiate.Clear();

                int empty = rand.Next(0, 100);
                if (empty < m_Container.m_ChanceEmpty * GetEmptyChanceModifier())
                {
                    enabled = false;
                    return;
                }

                float max = Mathf.Max(0, m_Container.m_MaxRandomItems) * GetDensityModifier();
                float min = Mathf.Min(m_Container.m_MinRandomItems, max);
                int count = rand.Next((int)Mathf.Round(min), (int)Mathf.Round(max) + 1);

                for (var i = 0; i < count; i++)
                {
                    var prefab = m_Container.m_LootTablePrefab.GetRandomGearPrefab();
                    m_Container.m_GearToInstantiate.Add(prefab.name);
                }

                enabled = false;
            }
        }

        public void OnOpened()
        {
            var anim = gameObject.GetComponent<ObjectAnim>();

            if (anim && anim.IsAnimating())
            {
                return;
            }

            enabled = false;
        }

        public void OnClosed()
        {
            if (m_Container.IsEmpty())
            {
                UpdateDaySearched();
            }

            enabled = m_Container.IsEmpty();
        }

        public void MaybeEnable()
        {
            enabled = m_Container.IsInspected() && m_Container.IsEmpty();
        }

        public void UpdateDaySearched()
        {
            m_DaySearched = GameManager.m_TimeOfDay.GetDayNumber();
        }

        [HideFromIl2Cpp]
        private float GetEmptyChanceModifier()
        {
            return 1 - (int)Settings.options.chanceEmptyModifier * 0.25f;
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
                    throw new ArgumentException();
            }
        }
    }
}
