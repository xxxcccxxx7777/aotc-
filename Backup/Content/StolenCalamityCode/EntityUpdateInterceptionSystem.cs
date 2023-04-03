using Terraria;
using Terraria.ModLoader;

namespace AotC.Content.StolenCalamityCode
{
  public class EntityUpdateInterceptionSystem : ModSystem
  {
    public virtual void PostUpdateEverything()
    {
      if (Main.dedServ)
        return;
      GeneralParticleHandler.Update();
      GeneralParticleHandler.DrawAllParticles(Main.spriteBatch);
    }
  }
}
