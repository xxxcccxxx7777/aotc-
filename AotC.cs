
using AotC.Content.StolenCalamityCode;
using Terraria;
using Terraria.ModLoader;

namespace AotC
{
  public class AotC : Mod
  {
    internal Mod Calamity;
    public const string ASSET_PATH = "AotC/Assets/";
    internal static AotC.AotC Instance;

    public virtual void Load()
    {
      this.Calamity = (Mod) null;
      Terraria.ModLoader.ModLoader.TryGetMod("CalamityMod", ref this.Calamity);
      AotC.AotC.Instance = this;
      if (Main.dedServ)
        return;
      GeneralParticleHandler.Load();
      CalamityShaders.LoadShaders();
    }
  }
}
