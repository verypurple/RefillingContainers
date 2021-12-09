using MelonLoader;
using System;
using UnityEngine;

namespace RefillingContainers
{
    [RegisterTypeInIl2Cpp]
    class Refill : MonoBehaviour
    {
        public Container m_Container;
        public int m_DaySearched;

        private bool m_WasEmpty;
        private System.Random rand = new System.Random();

        public Refill(IntPtr intPtr) : base(intPtr) { }

        public void Start()
        {
            enabled = m_Container.IsInspected() && m_Container.IsEmpty();
        }

        public void Update()
        {
            var currentDay = GameManager.m_TimeOfDay.GetDayNumber();

            if (currentDay >= m_DaySearched + Settings.options.refillAfterDays)
            {
                m_Container.m_LootTable = m_Container.m_LootTablePrefab;
                m_Container.m_Inspected = false;
                m_Container.m_GearToInstantiate.Clear();

                int max = rand.Next(Mathf.Max(0, m_Container.m_MaxRandomItems) + 1);

                for (var i = 0; i < max; i++)
                {
                    var prefab = m_Container.m_LootTablePrefab.GetRandomGearPrefab();

                    if (prefab)
                    {
                        m_Container.m_GearToInstantiate.Add(prefab.name);
                    }
                }

                enabled = false;
            }
        }

        public void OnOpened()
        {
            enabled = false;
            m_WasEmpty = m_Container.IsEmpty();
        }

        public void OnClosed()
        {
            if (!m_WasEmpty && m_Container.IsEmpty())
            {
                UpdateDaySearched();
            }

            enabled = m_Container.IsEmpty();
        }

        public void UpdateDaySearched()
        {
            m_DaySearched = GameManager.m_TimeOfDay.GetDayNumber();
        }
    }
}
