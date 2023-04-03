using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content
{
  public class EonBolt : ModProjectile
  {
    private Random rand = new Random();
    internal PrimitiveTrail TrailDrawer;
    public NPC target;
    private Particle Head;
    public float rotation;

    public virtual string Texture => "AotC/Content/TestStar";

    public Player Owner => Main.player[this.Projectile.owner];

    public ref float Hue => ref this.Projectile.ai[0];

    public ref float HomingStrenght => ref this.Projectile.ai[1];

    public virtual void SetStaticDefaults()
    {
      this.DisplayName.SetDefault("Eon Bolt");
      ProjectileID.Sets.TrailCacheLength[this.Projectile.type] = 20;
      ProjectileID.Sets.TrailingMode[this.Projectile.type] = 2;
    }

    public virtual void SetDefaults()
    {
      ((Entity) this.Projectile).width = ((Entity) this.Projectile).height = 30;
      this.Projectile.friendly = true;
      this.Projectile.penetrate = 1;
      this.Projectile.timeLeft = 160;
      this.Projectile.DamageType = DamageClass.Melee;
      this.Projectile.tileCollide = false;
    }

    public virtual void AI()
    {
      this.Projectile.rotation = Utils.ToRotation(((Entity) this.Projectile).velocity) + 1.570796f;
      if (this.Head != null)
      {
        this.Head.Position = Vector2.op_Addition(((Entity) this.Projectile).Center, Vector2.op_Multiply(((Entity) this.Projectile).velocity, 0.5f));
        this.Head.Time = 0;
        this.Head.Scale += (float) (Math.Sin((double) Main.GlobalTimeWrappedHourly * 6.0) * 0.0199999995529652) * this.Projectile.scale;
      }
      if (this.target == null)
      {
        this.target = ((Entity) this.Projectile).Center.ClosestNPCAt(812f);
      }
      else
      {
        if (this.rand.NextInt64(2L) == 0L)
          ++this.Projectile.timeLeft;
        if ((double) ((Entity) this.Projectile).velocity.AngleBetween(Vector2.op_Subtraction(((Entity) this.target).Center, ((Entity) this.Projectile).Center)) < 3.14159274101257)
        {
          float num = ((Entity) this.Projectile).AngleTo(((Entity) this.target).Center);
          ((Entity) this.Projectile).velocity = Vector2.op_Multiply(Vector2.op_Multiply(Utils.ToRotationVector2(Utils.AngleTowards(Utils.ToRotation(((Entity) this.Projectile).velocity), num, this.HomingStrenght)), ((Vector2) ref ((Entity) this.Projectile).velocity).Length()), 0.995f);
        }
      }
      Lighting.AddLight(((Entity) this.Projectile).Center, 0.75f, 1f, 0.24f);
    }

    internal Color ColorFunction(float completionRatio)
    {
      float num1 = MathHelper.Lerp(0.65f, 1f, (float) (Math.Cos((0.0 - (double) Main.GlobalTimeWrappedHourly) * 3.0) * 0.5 + 0.5));
      float num2 = Utils.GetLerpValue(1f, 0.64f, completionRatio, true) * this.Projectile.Opacity;
      return Color.op_Multiply(Color.Lerp(Color.White, CalamityUtils.HsvToRgb((double) Main.GlobalTimeWrappedHourly * (double) byte.MaxValue % (double) byte.MaxValue, 1.0, 1.0), num1), num2);
    }

    internal float WidthFunction(float completionRatio) => MathHelper.Lerp(0.0f, 22f * this.Projectile.scale * this.Projectile.Opacity, (float) Math.Pow(1.0 - (double) completionRatio, 3.0));

    public virtual bool PreDraw(ref Color lightColor)
    {
      if (this.TrailDrawer == null)
        this.TrailDrawer = new PrimitiveTrail(new PrimitiveTrail.VertexWidthFunction(this.WidthFunction), new PrimitiveTrail.VertexColorFunction(this.ColorFunction), specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);
      GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("AotC/Content/ScarletDevilStreak", (AssetRequestMode) 2));
      this.TrailDrawer.Draw((IEnumerable<Vector2>) this.Projectile.oldPos, Vector2.op_Subtraction(Vector2.op_Multiply(((Entity) this.Projectile).Size, 0.5f), Main.screenPosition), 30);
      Texture2D texture2D = ModContent.Request<Texture2D>("AotC/Content/TestStar", (AssetRequestMode) 2).Value;
      Main.EntitySpriteDraw(texture2D, Vector2.op_Subtraction(((Entity) this.Projectile).Center, Main.screenPosition), new Rectangle?(), Color.Lerp(lightColor, Color.White, 0.5f), this.Projectile.rotation + this.rotation, Vector2.op_Division(Utils.Size(texture2D), 2f), this.Projectile.scale, (SpriteEffects) 0, 0);
      this.rotation += 0.2f;
      return false;
    }
  }
}
