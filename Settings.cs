using ModSettings;
using static CustomExperienceModeManager;

namespace RefillingContainers
{
    internal class RefillingContainersSettings : JsonModSettings
    {
        [Section("General")]

        [Name("Mod enabled")]
        [Description("Please use this setting to temporarily turn off the mod. If you delete the file your progress will be lost.")]
        public bool modEnabled = true;

        [Section("Containers")]

        [Name("Days to refill")]
        [Description("Number of days after which searched or emptied containers will refill with random items.")]
        [Slider(1, 500)]
        public int refillAfterDays = 60;

        [Name("Empty container chance")]
        [Description("The chance of refilled containers being empty.")]
        public CustomTunableNLMH chanceEmptyModifier = CustomTunableNLMH.Medium;

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
