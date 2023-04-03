using AotC.Content.Sounds;
using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content
{
  public class Beam : BaseLaserbeamProjectile
  {
    public bool initialized;
    public float ear;
    public bool PlayedSound;
    public CalamityUtils.CurveSegment fat = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.PolyOut, 0.0f, 0.15f, 1f, 4);
    public CalamityUtils.CurveSegment retract = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.SineIn, 0.75f, 1f, -1f);

    public virtual string Texture => "AotC/Assets/Textures/Beam";

    public ref float dir => ref this.Projectile.ai[0];

    public Player Owner => Main.player[this.Projectile.owner];

    public override float Lifetime => 35f;

    public override float MaxScale => 3f;

    public override float MaxLaserLength => 80f;

    public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("Aotc/Assets/Textures/BeamBegin", (AssetRequestMode) 1).Value;

    public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("Aotc/Assets/Textures/BeamMiddle", (AssetRequestMode) 1).Value;

    public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("Aotc/Assets/Textures/BeamEnd", (AssetRequestMode) 1).Value;

    public virtual void SetStaticDefaults()
    {
      this.DisplayName.SetDefault(nameof (Beam));
      ProjectileID.Sets.TrailCacheLength[this.Projectile.type] = 20;
      ProjectileID.Sets.TrailingMode[this.Projectile.type] = 2;
    }

    public virtual void SetDefaults()
    {
      ((Entity) this.Projectile).width = 6;
      ((Entity) this.Projectile).height = 6;
      this.Projectile.friendly = true;
      this.Projectile.penetrate = -1;
      this.Projectile.timeLeft = 35;
      this.Projectile.DamageType = DamageClass.Melee;
      this.Projectile.tileCollide = false;
      this.Projectile.hide = true;
    }

    public virtual bool PreAI()
    {
      if (!this.initialized)
      {
        this.initialized = true;
        if (this.Projectile.owner == Main.myPlayer)
        {
          ((Entity) this.Projectile).direction = (double) Main.MouseWorld.X > (double) ((Entity) this.Owner).Center.X ? 1 : -1;
          this.Projectile.netUpdate = true;
          ((Entity) this.Projectile).direction = (double) Main.MouseWorld.X > (double) ((Entity) this.Owner).Center.X ? 1 : -1;
        }
        this.Projectile.rotation = this.dir - 1.570796f;
        ((Entity) this.Projectile).Center = Vector2.op_Addition(Vector2.op_Multiply(Utils.ToRotationVector2(this.dir), 370f), ((Entity) this.Owner).Center);
      }
      this.Owner.heldProj = ((Entity) this.Projectile).whoAmI;
      this.Projectile.netUpdate = true;
      if (!this.PlayedSound)
      {
        SoundEngine.PlaySound(ref AotCAudio.ChaosBlasterFire, new Vector2?(((Entity) this.Projectile).position));
        this.PlayedSound = true;
      }
      return true;
    }

    public override void UpdateLaserMotion()
    {
    }

    public override void DetermineScale()
    {
      try
      {
        this.Projectile.scale = (float) (2.0 * (double) CalamityUtils.PiecewiseAnimation((float) ((double) this.Time / (double) this.Lifetime), new CalamityUtils.CurveSegment[2]
        {
          this.fat,
          this.retract
        }));
        if ((double) this.Projectile.scale <= (double) this.MaxScale)
          return;
        this.Projectile.scale = this.MaxScale;
      }
      catch
      {
        Main.NewText("if you see this it means my code sucks", byte.MaxValue, byte.MaxValue, byte.MaxValue);
      }
    }

    public override bool PreDraw(ref Color lightColor)
    {
      float scale = this.Projectile.scale;
      Rectangle rectangle1 = Utils.Frame(this.LaserBeginTexture, 1, Main.projFrames[this.Projectile.type], 0, 0, 0, 0);
      Rectangle rectangle2 = Utils.Frame(this.LaserMiddleTexture, 1, Main.projFrames[this.Projectile.type], 0, 0, 0, 0);
      Rectangle rectangle3 = Utils.Frame(this.LaserEndTexture, 1, Main.projFrames[this.Projectile.type], 0, 0, 0, 0);
      Main.EntitySpriteDraw(this.LaserBeginTexture, Vector2.op_Subtraction(((Entity) this.Projectile).Center, Main.screenPosition), new Rectangle?(rectangle1), lightColor, this.Projectile.rotation, Vector2.op_Division(Utils.Size(this.LaserBeginTexture), 2f), scale, (SpriteEffects) 0, 0);
      float num1 = this.LaserLength - (float) (rectangle1.Height / 2 + rectangle3.Height) * scale;
      Vector2 center = ((Entity) this.Projectile).Center;
      if ((double) num1 > 0.0)
      {
        float num2 = (float) rectangle2.Height * scale;
        for (float num3 = 0.0f; (double) num3 + 1.0 < (double) num1; num3 += num2)
          Main.EntitySpriteDraw(this.LaserMiddleTexture, Vector2.op_Subtraction(center, Main.screenPosition), new Rectangle?(rectangle2), lightColor, this.Projectile.rotation, Vector2.op_Multiply((float) this.LaserMiddleTexture.Width * 0.5f, Vector2.UnitX), scale, (SpriteEffects) 0, 0);
      }
      if ((double) Math.Abs(this.LaserLength - this.DetermineLaserLength()) < 30.0)
        Main.EntitySpriteDraw(this.LaserEndTexture, Vector2.op_Subtraction(center, Main.screenPosition), new Rectangle?(rectangle3), lightColor, this.Projectile.rotation, Utils.Top(Utils.Frame(this.LaserEndTexture, 1, 1, 0, 0, 0, 0)), scale, (SpriteEffects) 0, 0);
      int direction = ((Entity) this.Projectile).direction;
      Main.EntitySpriteDraw(ModContent.Request<Texture2D>("AotC/Assets/Textures/BeamWave", (AssetRequestMode) 2).Value, Vector2.op_Subtraction(center, Main.screenPosition), new Rectangle?(), lightColor, this.Projectile.rotation, new Vector2(8f, (float) (0.0 - (double) this.ear % 14.0)), this.Projectile.scale, (SpriteEffects) 0, 0);
      ++this.ear;
      return false;
    }
  }
}
