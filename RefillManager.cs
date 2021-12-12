using System.Collections.Generic;

namespace RefillingContainers
{
    internal static class RefillManager
    {
        private static List<Refill> handlers = new List<Refill>();

        internal static void Register(Refill refill)
        {
            if (refill.m_Container.m_LootTablePrefab)
            {
                handlers.Add(refill);
                refill.DoRefill();
            }
        }

        internal static void Unregister(Refill refill)
        {
            handlers.Remove(refill);
        }

        internal static void RefillAll()
        {
            if (!Settings.options.modEnabled)
            {
                return;
            }

            foreach (var handler in handlers)
            {
                handler.DoRefill();
            }
        }
    }
}
