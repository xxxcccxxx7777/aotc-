using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace AotC.Content.StolenCalamityCode
{
  public class CalamityShaders
  {
    public static Effect AstralFogShader;
    public static Effect LightShader;
    public static Effect SCalMouseShader;
    public static Effect TentacleShader;
    public static Effect TeleportDisplacementShader;
    public static Effect LightDistortionShader;
    public static Effect PhaseslayerRipShader;
    public static Effect FadedUVMapStreakShader;
    public static Effect FlameStreakShader;
    public static Effect FadingSolidTrailShader;
    public static Effect ScarletDevilShader;
    public static Effect BordernadoFireShader;
    public static Effect PrismCrystalShader;
    public static Effect ImpFlameTrailShader;
    public static Effect SCalShieldShader;
    public static Effect RancorMagicCircleShader;
    public static Effect BasicTintShader;
    public static Effect CircularBarShader;
    public static Effect CircularBarSpriteShader;
    public static Effect DoGDisintegrationShader;
    public static Effect ArtAttackTrailShader;
    public static Effect CircularAoETelegraph;
    public static Effect IntersectionClipShader;
    public static Effect LocalLinearTransformationShader;
    public static Effect BasicPrimitiveShader;
    public static Effect ArtemisLaserShader;
    public static Effect BaseFusableParticleEdgeShader;
    public static Effect AdditiveFusableParticleEdgeShader;
    public static Effect DoGPortalShader;
    public static Effect FluidShaders;

    public static void LoadShaders()
    {
      if (Main.dedServ)
        return;
      CalamityShaders.AstralFogShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/CustomShader", (AssetRequestMode) 1).Value;
      CalamityShaders.LightShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/LightBurstShader", (AssetRequestMode) 1).Value;
      CalamityShaders.TentacleShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/TentacleShader", (AssetRequestMode) 1).Value;
      CalamityShaders.TeleportDisplacementShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/TeleportDisplacementShader", (AssetRequestMode) 1).Value;
      CalamityShaders.SCalMouseShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/SCalMouseShader", (AssetRequestMode) 1).Value;
      CalamityShaders.LightDistortionShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/DistortionShader", (AssetRequestMode) 1).Value;
      CalamityShaders.PhaseslayerRipShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/PhaseslayerRipShader", (AssetRequestMode) 1).Value;
      CalamityShaders.ScarletDevilShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ScarletDevilStreak", (AssetRequestMode) 1).Value;
      CalamityShaders.BordernadoFireShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/BordernadoFire", (AssetRequestMode) 1).Value;
      CalamityShaders.PrismCrystalShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/PrismCrystalStreak", (AssetRequestMode) 1).Value;
      CalamityShaders.FadedUVMapStreakShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/FadedUVMapStreak", (AssetRequestMode) 1).Value;
      CalamityShaders.FlameStreakShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/Flame", (AssetRequestMode) 1).Value;
      CalamityShaders.FadingSolidTrailShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/FadingSolidTrail", (AssetRequestMode) 1).Value;
      CalamityShaders.ImpFlameTrailShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ImpFlameTrail", (AssetRequestMode) 1).Value;
      CalamityShaders.SCalShieldShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/SupremeShieldShader", (AssetRequestMode) 1).Value;
      CalamityShaders.RancorMagicCircleShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/RancorMagicCircle", (AssetRequestMode) 1).Value;
      CalamityShaders.BasicTintShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/BasicTint", (AssetRequestMode) 1).Value;
      CalamityShaders.CircularBarShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/CircularBarShader", (AssetRequestMode) 1).Value;
      CalamityShaders.CircularBarSpriteShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/CircularBarSpriteShader", (AssetRequestMode) 1).Value;
      CalamityShaders.DoGDisintegrationShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/DoGDisintegration", (AssetRequestMode) 1).Value;
      CalamityShaders.ArtAttackTrailShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ArtAttackTrail", (AssetRequestMode) 1).Value;
      CalamityShaders.CircularAoETelegraph = AotC.AotC.Instance.Assets.Request<Effect>("Effects/CircularAoETelegraph", (AssetRequestMode) 1).Value;
      CalamityShaders.IntersectionClipShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/IntersectionClipShader", (AssetRequestMode) 1).Value;
      CalamityShaders.LocalLinearTransformationShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/LocalLinearTransformationShader", (AssetRequestMode) 1).Value;
      CalamityShaders.BasicPrimitiveShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/BasicPrimitiveShader", (AssetRequestMode) 1).Value;
      CalamityShaders.ArtemisLaserShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ArtemisLaserShader", (AssetRequestMode) 1).Value;
      CalamityShaders.BaseFusableParticleEdgeShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ParticleFusion/BaseFusableParticleEdgeShader", (AssetRequestMode) 1).Value;
      CalamityShaders.AdditiveFusableParticleEdgeShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ParticleFusion/AdditiveFusableParticleEdgeShader", (AssetRequestMode) 1).Value;
      CalamityShaders.DoGPortalShader = AotC.AotC.Instance.Assets.Request<Effect>("Effects/ScreenShaders/DoGPortalShader", (AssetRequestMode) 1).Value;
      CalamityShaders.FluidShaders = AotC.AotC.Instance.Assets.Request<Effect>("Effects/FluidShaders", (AssetRequestMode) 1).Value;
      ((EffectManager<Filter>) Filters.Scene)["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(CalamityShaders.LightShader), "BurstPass"), (EffectPriority) 4);
      ((GameEffect) ((EffectManager<Filter>) Filters.Scene)["CalamityMod:LightBurst"]).Load();
      GameShaders.Misc["CalamityMod:FireMouse"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.SCalMouseShader), "DyePass");
      GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.TentacleShader), "BurstPass");
      GameShaders.Misc["CalamityMod:TeleportDisplacement"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.TeleportDisplacementShader), "GlitchPass");
      GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.PhaseslayerRipShader), "TrailPass");
      GameShaders.Misc["CalamityMod:TrailStreak"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.FadedUVMapStreakShader), "TrailPass");
      GameShaders.Misc["CalamityMod:Flame"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.FlameStreakShader), "TrailPass");
      GameShaders.Misc["CalamityMod:FadingSolidTrail"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.FadingSolidTrailShader), "TrailPass");
      GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.ScarletDevilShader), "TrailPass");
      GameShaders.Misc["CalamityMod:Bordernado"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.BordernadoFireShader), "TrailPass");
      GameShaders.Misc["CalamityMod:PrismaticStreak"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.PrismCrystalShader), "TrailPass");
      GameShaders.Misc["CalamityMod:ImpFlameTrail"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.ImpFlameTrailShader), "TrailPass");
      GameShaders.Misc["CalamityMod:SupremeShield"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.SCalShieldShader), "ShieldPass");
      GameShaders.Misc["CalamityMod:RancorMagicCircle"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.RancorMagicCircleShader), "ShieldPass");
      GameShaders.Misc["CalamityMod:BasicTint"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.BasicTintShader), "TintPass");
      GameShaders.Misc["CalamityMod:CircularBarShader"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.CircularBarShader), "Pass0");
      GameShaders.Misc["CalamityMod:CircularBarSpriteShader"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.CircularBarSpriteShader), "Pass0");
      GameShaders.Misc["CalamityMod:DoGDisintegration"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.DoGDisintegrationShader), "DisintegrationPass");
      GameShaders.Misc["CalamityMod:ArtAttack"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.ArtAttackTrailShader), "TrailPass");
      GameShaders.Misc["CalamityMod:CircularAoETelegraph"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.CircularAoETelegraph), "TelegraphPass");
      GameShaders.Misc["CalamityMod:IntersectionClip"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.IntersectionClipShader), "ClipPass");
      GameShaders.Misc["CalamityMod:LinearTransformation"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.LocalLinearTransformationShader), "TransformationPass");
      GameShaders.Misc["CalamityMod:PrimitiveDrawer"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.BasicPrimitiveShader), "TrailPass");
      GameShaders.Misc["CalamityMod:ArtemisLaser"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.ArtemisLaserShader), "TrailPass");
      GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.BaseFusableParticleEdgeShader), "ParticlePass");
      GameShaders.Misc["CalamityMod:AdditiveFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.AdditiveFusableParticleEdgeShader), "ParticlePass");
      GameShaders.Misc["CalamityMod:DoGPortal"] = new MiscShaderData(new Ref<Effect>(CalamityShaders.DoGPortalShader), "ScreenPass");
      Ref<Effect> ref1 = new Ref<Effect>(AotC.AotC.Instance.Assets.Request<Effect>("Effects/SpreadTelegraph", (AssetRequestMode) 1).Value);
      ((EffectManager<Filter>) Filters.Scene)["SpreadTelegraph"] = new Filter(new ScreenShaderData(ref1, "TelegraphPass"), (EffectPriority) 3);
      ((GameEffect) ((EffectManager<Filter>) Filters.Scene)["SpreadTelegraph"]).Load();
      Ref<Effect> ref2 = new Ref<Effect>(AotC.AotC.Instance.Assets.Request<Effect>("Effects/PixelatedSightLine", (AssetRequestMode) 1).Value);
      ((EffectManager<Filter>) Filters.Scene)["PixelatedSightLine"] = new Filter(new ScreenShaderData(ref2, "SightLinePass"), (EffectPriority) 3);
      ((GameEffect) ((EffectManager<Filter>) Filters.Scene)["PixelatedSightLine"]).Load();
    }
  }
}
