using System.Collections.Generic;

namespace RefillingContainers
{
    static class RefillManager
    {
        static List<Refill> subscribers = new List<Refill>();

        internal static void Subscribe(Refill refill)
        {
            subscribers.Add(refill);

            refill.DoRefill();
        }

        internal static void Unsubscribe(Refill refill)
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
                if (subscriber.CanRefill())
                {
                    subscriber.DoRefill();
                }
            }
        }
    }
}
