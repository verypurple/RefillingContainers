using MelonLoader;

namespace RefillingContainers
{
    public class Implementation : MelonMod
    {
        public override void OnApplicationStart()
        {
            Settings.OnLoad();
        }
    }
}
