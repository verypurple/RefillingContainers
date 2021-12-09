using ModSettings;

namespace RefillingContainers
{
    internal class RefillingContainersSettings : JsonModSettings
    {
        [Name("Days to refill")]
        [Description("Number of days after which searched or emptied containers will refill with random items.")]
        [Slider(1, 500)]
        public int refillAfterDays = 60;
    }

    internal class Settings
    {
        internal static readonly RefillingContainersSettings options = new RefillingContainersSettings();

        public static void OnLoad()
        {
            options.AddToModSettings("Refilling Containers", MenuType.Both);
        }
    }
}
