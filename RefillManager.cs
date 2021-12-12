using MelonLoader;
using System.Collections.Generic;

namespace RefillingContainers
{
    internal static class RefillManager
    {
        private static List<Refill> subscribers = new List<Refill>();

        internal static void Register(Refill refill)
        {
            if (refill.m_Container.m_LootTablePrefab)
            {
                subscribers.Add(refill);
                refill.DoRefill();
            }
        }

        internal static void Unregister(Refill refill)
        {
            subscribers.Remove(refill);
        }

        internal static void NotifySubscribers()
        {
            if (!Settings.options.modEnabled)
            {
                return;
            }

            foreach (var subscriber in subscribers)
            {
                subscriber.DoRefill();
            }
        }
    }
}
