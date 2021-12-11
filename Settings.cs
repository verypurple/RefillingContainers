using ModSettings;
using static CustomExperienceModeManager;

namespace RefillingContainers
{
    internal class RefillingContainersSettings : JsonModSettings
    {
        [Name("Days to refill")]
        [Description("Number of days after which searched or emptied containers will refill with random items.")]
        [Slider(1, 500)]
        public int refillAfterDays = 60;

        [Name("Reduce empty container chance")]
        [Description("Reduces the chance of refilled containers being empty.")]
        public CustomTunableNLMHV chanceEmptyModifier = CustomTunableNLMHV.None;

        [Name("Container density modifier")]
        [Description("Modifies the amount of items you find in refilled containers.")]
        public CustomTunableLMH containerDensityModifier = CustomTunableLMH.High;
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
