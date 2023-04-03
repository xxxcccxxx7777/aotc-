using AotC.Content.Items.Weapons;
using AotC.Content.Sounds;
using AotC.Content.StolenCalamityCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content
{
  internal class ArkoftheCosmosSwungBlade : ModProjectile
  {
    private Random rand = new Random();
    public float dir;
    public float rng;
    public float rng2;
    private bool initialized;
    private Vector2 direction = Vector2.Zero;
    private Particle smear;
    private float SwingWidth = 2.356194f;
    public const float MaxThrowTime = 140f;
    public float ThrowReach;
    public const float SnapWindowStart = 0.2f;
    public const float SnapWindowEnd = 0.75f;
    public CalamityUtils.CurveSegment anticipation = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.ExpOut, 0.0f, 0.0f, 0.15f);
    public CalamityUtils.CurveSegment thrust = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);
    public CalamityUtils.CurveSegment hold = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.Linear, 0.5f, 1f, 0.2f);
    public CalamityUtils.CurveSegment startup = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.SineIn, 0.0f, 0.0f, 0.25f);
    public CalamityUtils.CurveSegment swing = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.SineOut, 0.1f, 0.25f, 0.75f);
    public CalamityUtils.CurveSegment shoot = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.PolyIn, 0.0f, 1f, -0.2f, 3);
    public CalamityUtils.CurveSegment remain = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.Linear, 0.2f, 0.8f, 0.0f);
    public CalamityUtils.CurveSegment retract = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.SineIn, 0.75f, 1f, -1f);
    public CalamityUtils.CurveSegment sizeCurve = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.SineBump, 0.0f, 0.0f, 1f);
    public CalamityUtils.CurveSegment fat = new CalamityUtils.CurveSegment(CalamityUtils.EasingType.PolyOut, 0.0f, 0.15f, 1f, 4);

    public virtual string Texture => "AotC/Content/Items/Weapons/ArkoftheCosmos";

    public ref float swingType => ref this.Projectile.ai[0];

    public ref float Charge => ref this.Projectile.ai[1];

    public Player Owner => Main.player[this.Projectile.owner];

    public float MaxSwingTime => this.SwirlSwing ? 55f : 35f;

    public bool SwirlSwing => (double) this.swingType == 2.0;

    public int SwingDirection
    {
      get
      {
        float num = this.swingType;
        if ((double) num == 2.0)
          return -1 * Math.Sign(this.direction.X);
        if ((double) num == 3.0)
          return 0;
        return (double) num == 1.0 ? -1 * Math.Sign(this.direction.X) : Math.Sign(this.direction.X);
      }
    }

    public Vector2 DistanceFromPlayer => Vector2.op_Division(this.direction, 30f);

    public float SwingTimer => this.MaxSwingTime - (float) this.Projectile.timeLeft;

    public float SwingCompletion => this.SwingTimer / this.MaxSwingTime;

    public ref float HasFired => ref this.Projectile.localAI[0];

    private bool OwnerCanShoot => this.Owner.channel && !this.Owner.noItems && !this.Owner.CCed && this.Owner.HeldItem.type == ModContent.ItemType<ArkoftheCosmos>();

    public bool Thrown => (double) this.swingType == 4.0;

    public float ThrowTimer => 140f - (float) this.Projectile.timeLeft;

    public float ThrowCompletion => this.ThrowTimer / 140f;

    public float SnapEndTime => 35f;

    public float SnapEndCompletion => (this.SnapEndTime - (float) this.Projectile.timeLeft) / this.SnapEndTime;

    public ref float ChanceMissed => ref this.Projectile.localAI[1];

    internal float SwingRatio() => CalamityUtils.PiecewiseAnimation(this.SwingCompletion, new CalamityUtils.CurveSegment[3]
    {
      this.anticipation,
      this.thrust,
      this.hold
    });

    internal float SwirlRatio() => CalamityUtils.PiecewiseAnimation(this.SwingCompletion, new CalamityUtils.CurveSegment[2]
    {
      this.startup,
      this.swing
    });

    internal float ThrowRatio() => CalamityUtils.PiecewiseAnimation(this.ThrowCompletion, new CalamityUtils.CurveSegment[3]
    {
      this.shoot,
      this.remain,
      this.retract
    });

    internal float ThrowScaleRatio() => CalamityUtils.PiecewiseAnimation(this.ThrowCompletion, new CalamityUtils.CurveSegment[1]
    {
      this.sizeCurve
    });

    internal float StabRatio() => CalamityUtils.PiecewiseAnimation(this.SwingCompletion, new CalamityUtils.CurveSegment[2]
    {
      this.fat,
      this.retract
    });

    public virtual void SetStaticDefaults()
    {
      this.DisplayName.SetDefault("Ark of the Cosmos");
      ProjectileID.Sets.TrailCacheLength[this.Projectile.type] = 10;
      ProjectileID.Sets.TrailingMode[this.Projectile.type] = 2;
    }

    public virtual void SetDefaults()
    {
      this.Projectile.DamageType = DamageClass.Melee;
      ((Entity) this.Projectile).width = ((Entity) this.Projectile).height = 60;
      ((Entity) this.Projectile).width = ((Entity) this.Projectile).height = 60;
      this.Projectile.tileCollide = false;
      this.Projectile.friendly = true;
      this.Projectile.penetrate = -1;
      this.Projectile.extraUpdates = 1;
      this.Projectile.usesLocalNPCImmunity = true;
      this.Projectile.localNPCHitCooldown = this.Thrown ? 10 : (int) this.MaxSwingTime;
    }

    public virtual bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
      float num1 = 172f * this.Projectile.scale;
      if (this.Thrown)
      {
        bool flag1 = Collision.CheckAABBvAABBCollision(Utils.TopLeft(targetHitbox), Utils.Size(targetHitbox), Vector2.op_Subtraction(((Entity) this.Projectile).Center, Vector2.op_Division(Vector2.op_Multiply(Vector2.One, num1), 2f)), Vector2.op_Multiply(Vector2.One, num1));
        if ((double) this.swingType == 4.0)
          return new bool?(flag1);
        Vector2 vector2 = Vector2.SmoothStep(((Entity) this.Owner).Center, ((Entity) this.Projectile).Center, MathHelper.Clamp(this.SnapEndCompletion + 0.25f, 0.0f, 1f));
        bool flag2 = Collision.CheckAABBvLineCollision(Utils.TopLeft(targetHitbox), Utils.Size(targetHitbox), vector2, Vector2.op_Addition(vector2, Vector2.op_Multiply(this.direction, num1)));
        return new bool?(flag1 | flag2);
      }
      float num2 = 0.0f;
      Vector2 distanceFromPlayer = this.DistanceFromPlayer;
      Vector2 vector2_1 = Vector2.op_Multiply(((Vector2) distanceFromPlayer).Length(), Utils.ToRotationVector2(this.Projectile.rotation));
      return new bool?(Collision.CheckAABBvLineCollision(Utils.TopLeft(targetHitbox), Utils.Size(targetHitbox), Vector2.op_Addition(((Entity) this.Owner).Center, vector2_1), Vector2.op_Addition(Vector2.op_Addition(((Entity) this.Owner).Center, vector2_1), Vector2.op_Multiply(Utils.ToRotationVector2(this.Projectile.rotation), num1)), 24f, ref num2));
    }

    public virtual void AI()
    {
      if (!this.initialized)
      {
        this.Projectile.timeLeft = this.Thrown ? 140 : (int) this.MaxSwingTime;
        this.rng = (float) this.rand.Next(-2, 2);
        this.rng2 = (float) this.rand.Next(-2, 2);
        this.direction = ((Entity) this.Projectile).velocity;
        Vector2 direction = this.direction;
        ((Vector2)  direction).Normalize();
        ((Entity) this.Projectile).velocity = this.direction;
        this.Projectile.rotation = Utils.ToRotation(this.direction);
        this.initialized = true;
        this.dir = (double) Utils.ToRotation(((Entity) this.Owner).DirectionTo(Main.MouseWorld)) <= -1.570796251297 || (double) Utils.ToRotation(((Entity) this.Owner).DirectionTo(Main.MouseWorld)) >= 1.5707963 ? -1f : 1f;
        if (this.SwirlSwing)
        {
          this.Projectile.localNPCHitCooldown = (int) ((double) this.Projectile.localNPCHitCooldown / 4.0);
          SoundEngine.PlaySound(ref AotCAudio.Slash, new Vector2?(((Entity) this.Projectile).position));
        }
        this.Projectile.netUpdate = true;
        this.Projectile.netSpam = 0;
        if ((double) this.swingType == 3.0)
        {
          this.Projectile.damage *= 2;
        }
        else
        {
          float num = this.swingType;
          if ((double) num == 0.0 || (double) num == 1.0)
            SoundEngine.PlaySound(ref SoundID.DD2_MonkStaffSwing, new Vector2?(((Entity) this.Projectile).position));
        }
      }
      if (!this.Thrown)
      {
        if ((double) this.swingType != 3.0)
          ((Entity) this.Projectile).Center = Vector2.op_Addition(((Entity) this.Owner).Center, this.DistanceFromPlayer);
        if (!this.SwirlSwing)
        {
          this.Projectile.rotation = Utils.ToRotation(((Entity) this.Projectile).velocity) + MathHelper.Lerp(this.SwingWidth / 2f * (float) this.SwingDirection, (float) ((0.0 - (double) this.SwingWidth) / 2.0) * (float) this.SwingDirection, this.SwingRatio());
          if ((double) this.swingType != 3.0 && ((Entity) this.Owner).whoAmI == Main.myPlayer && (this.Projectile.timeLeft == 23 + (int) this.rng || this.Projectile.timeLeft == 19 + (int) this.rng2))
          {
            float num1 = this.Projectile.rotation - 0.9032079f * this.dir;
            if ((double) this.swingType == 1.0)
            {
              Projectile.NewProjectileDirect(((Entity) this.Projectile).GetSource_FromThis((string) null), Vector2.op_Addition(((Entity) this.Owner).Center, Vector2.op_Multiply(Utils.ToRotationVector2(num1), 150f)), Vector2.op_Multiply(Utils.ToRotationVector2(num1), 20f), ModContent.ProjectileType<EonBolt>(), (int) ((double) ArkoftheCosmos.SwirlBoltDamageMultiplier / (double) ArkoftheCosmos.SwirlBoltAmount * (double) this.Projectile.damage), 0.0f, ((Entity) this.Owner).whoAmI, 0.55f, 0.1570796f).timeLeft = 100;
            }
            else
            {
              float num2 = this.Projectile.rotation - -0.9032079f * this.dir;
              Projectile.NewProjectileDirect(((Entity) this.Projectile).GetSource_FromThis((string) null), Vector2.op_Addition(((Entity) this.Owner).Center, Vector2.op_Multiply(Utils.ToRotationVector2(num2), 150f)), Vector2.op_Multiply(Utils.ToRotationVector2(num2), 20f), ModContent.ProjectileType<EonBolt>(), (int) ((double) ArkoftheCosmos.SwirlBoltDamageMultiplier / (double) ArkoftheCosmos.SwirlBoltAmount * (double) this.Projectile.damage), 0.0f, ((Entity) this.Owner).whoAmI, 0.55f, 0.1570796f).timeLeft = 100;
            }
          }
        }
        else
        {
          float num3 = 2.356194f * (float) this.SwingDirection;
          float num4 = -7.461283f * (float) this.SwingDirection;
          this.Projectile.rotation = Utils.ToRotation(((Entity) this.Projectile).velocity) + MathHelper.Lerp(num3, num4, this.SwirlRatio());
          if (((Entity) this.Owner).whoAmI == Main.myPlayer && (double) (this.Projectile.timeLeft - 1) % Math.Ceiling((double) this.MaxSwingTime / (double) ArkoftheCosmos.SwirlBoltAmount) == 0.0)
          {
            float num5 = this.Projectile.rotation - 0.9032079f * (float) ((Entity) this.Owner).direction;
            Projectile.NewProjectileDirect(((Entity) this.Projectile).GetSource_FromThis((string) null), Vector2.op_Addition(((Entity) this.Owner).Center, Vector2.op_Multiply(Utils.ToRotationVector2(num5), 10f)), Vector2.op_Multiply(Utils.ToRotationVector2(num5), 20f), ModContent.ProjectileType<EonBolt>(), (int) ((double) ArkoftheCosmos.SwirlBoltDamageMultiplier / (double) ArkoftheCosmos.SwirlBoltAmount * (double) this.Projectile.damage), 0.0f, ((Entity) this.Owner).whoAmI, 0.55f, 0.1570796f).timeLeft = 100;
          }
        }
        this.Projectile.scale = (double) this.swingType != 3.0 ? (float) (1.20000004768372 + Math.Sin((double) this.SwingRatio() * 3.14159274101257) * 0.600000023841858 + (double) this.Charge / 10.0 * 0.200000002980232) : (float) (1.20000004768372 + Math.Sin((double) this.ThrowScaleRatio() * 3.14159274101257) * 0.600000023841858 + (double) this.Charge / 10.0 * 0.200000002980232);
      }
      this.Owner.heldProj = ((Entity) this.Projectile).whoAmI;
      ((Entity) this.Owner).direction = Math.Sign(((Entity) this.Projectile).velocity.X);
      this.Owner.itemRotation = this.Projectile.rotation;
      if (((Entity) this.Owner).direction != 1)
        this.Owner.itemRotation -= 3.141593f;
      this.Owner.itemRotation = MathHelper.WrapAngle(this.Owner.itemRotation);
    }

    public virtual bool PreDraw(ref Color lightColor)
    {
      this.DrawSingleSwungScissorBlade(lightColor);
      return false;
    }

    public void DrawSingleSwungScissorBlade(Color lightColor)
    {
      Texture2D texture2D1 = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/ArkoftheCosmos", (AssetRequestMode) 2).Value;
      Texture2D texture2D2 = ModContent.Request<Texture2D>("AotC/Content/Items/Weapons/ArkoftheCosmosGlow", (AssetRequestMode) 2).Value;
      bool flag = ((Entity) this.Owner).direction < 0;
      SpriteEffects spriteEffects = flag ? (SpriteEffects) 1 : (SpriteEffects) 0;
      float num1 = ((Entity) this.Owner).direction < 0 ? 1.570796f : 0.0f;
      float rotation = this.Projectile.rotation;
      float num2 = 0.7853982f;
      float num3 = this.Projectile.rotation + num2 + num1;
      Vector2 vector2_1 = new Vector2();
      ((Vector2)  vector2_1)(flag ? (float) texture2D1.Width : 0.0f, (float) texture2D1.Height);
      Vector2 vector2_2 = (double) this.swingType != 3.0 ? Vector2.op_Subtraction(Vector2.op_Addition(((Entity) this.Owner).Center, Vector2.op_Multiply(Utils.ToRotationVector2(rotation), 10f)), Main.screenPosition) : Vector2.op_Subtraction(Vector2.op_Addition(((Entity) this.Owner).Center, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Division(Utils.ToRotationVector2(rotation), 2f), this.StabRatio()), 100f)), Main.screenPosition);
      if ((double) this.SwingTimer > (double) ProjectileID.Sets.TrailCacheLength[this.Projectile.type] && ((double) this.swingType == 1.0 || (double) this.swingType == 0.0))
      {
        for (int index = 1; index < this.Projectile.oldRot.Length; ++index)
        {
          Color rgb = Main.hslToRgb((float) ((double) index / (double) this.Projectile.oldRot.Length * 0.100000001490116), 1f, (float) (0.600000023841858 + ((double) this.Charge > 0.0 ? 0.300000011920929 : 0.0)), byte.MaxValue);
          float num4 = this.Projectile.oldRot[index] + num2 + num1;
          Main.spriteBatch.Draw(texture2D2, vector2_2, new Rectangle?(), Color.op_Multiply(rgb, 0.05f), num4, vector2_1, this.Projectile.scale - (float) (0.200000002980232 * ((double) index / (double) this.Projectile.oldRot.Length)), spriteEffects, 0.0f);
        }
      }
      Main.EntitySpriteDraw(texture2D1, vector2_2, new Rectangle?(), lightColor, num3, vector2_1, this.Projectile.scale * 1.2f, spriteEffects, 0);
      Main.EntitySpriteDraw(texture2D2, vector2_2, new Rectangle?(), Color.Lerp(lightColor, Color.White, 0.75f), num3, vector2_1, this.Projectile.scale * 1.2f, spriteEffects, 0);
      if ((double) this.SwingCompletion > 0.5 && ((double) this.swingType == 1.0 || (double) this.swingType == 0.0))
      {
        Texture2D texture2D3 = ModContent.Request<Texture2D>("AotC/Content/Particles/TrientCircularSmear", (AssetRequestMode) 2).Value;
        Main.spriteBatch.End();
        Main.spriteBatch.Begin((SpriteSortMode) 1, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
        float num5 = (float) Math.Sin((double) this.SwingCompletion * 3.14159274101257);
        float num6 = (float) (0.392699092626572 * (double) this.SwingCompletion - 0.392699092626572 + ((double) this.swingType == 2.0 ? 0.785398185253143 : 0.0)) * (float) this.SwingDirection;
        Color rgb = Main.hslToRgb((double) this.swingType == 0.0 ? 0.15f : 0.3f, 1f, 0.6f, byte.MaxValue);
        Main.EntitySpriteDraw(texture2D3, Vector2.op_Subtraction(((Entity) this.Owner).Center, Main.screenPosition), new Rectangle?(), Color.op_Multiply(Color.op_Multiply(rgb, 0.5f), num5), Utils.ToRotation(((Entity) this.Projectile).velocity) + 3.141593f + num6, Vector2.op_Division(Utils.Size(texture2D3), 2f), this.Projectile.scale * 3.4f, (SpriteEffects) 0, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin((SpriteSortMode) 0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
      }
      if (!this.SwirlSwing)
        return;
      Texture2D texture2D4 = ModContent.Request<Texture2D>("AotC/Content/Particles/CircularSmearSmokey", (AssetRequestMode) 2).Value;
      Main.spriteBatch.End();
      Main.spriteBatch.Begin((SpriteSortMode) 1, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
      Color color = Color.op_Multiply(Color.Red, MathHelper.Clamp((float) Math.Sin(((double) this.SwirlRatio() - 0.200000002980232) * 3.14159274101257), 0.0f, 1f) * 0.8f);
      float num7 = (float) ((double) this.Projectile.rotation + 0.785398185253143 + (((Entity) this.Owner).direction < 0 ? 3.14159274101257 : 0.0));
      Main.EntitySpriteDraw(texture2D4, Vector2.op_Subtraction(((Entity) this.Owner).Center, Main.screenPosition), new Rectangle?(), color, num7, Vector2.op_Division(Utils.Size(texture2D4), 2f), this.Projectile.scale * 2f, (SpriteEffects) 0, 0);
      Main.spriteBatch.End();
      Main.spriteBatch.Begin((SpriteSortMode) 0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
    }

    public void DoParticleEffects(bool swirlSwing)
    {
      if (!swirlSwing)
        return;
      this.Projectile.scale = (float) (1.60000002384186 + Math.Sin((double) this.SwirlRatio() * 3.14159274101257) * 1.0 + (double) this.Charge / 10.0 * 0.0500000007450581);
      Color color = Color.op_Multiply(Color.Purple, MathHelper.Clamp((float) Math.Sin(((double) this.SwirlRatio() - 0.200000002980232) * 3.14159274101257), 0.0f, 1f) * 0.8f);
      if (this.smear == null)
      {
        this.smear = (Particle) new CircularSmearSmokeyVFX(((Entity) this.Owner).Center, color, this.Projectile.rotation, this.Projectile.scale * 3.4f);
        GeneralParticleHandler.SpawnParticle(this.smear);
      }
      else
      {
        this.smear.Rotation = (float) ((double) this.Projectile.rotation + 0.785398185253143 + (((Entity) this.Owner).direction < 0 ? 3.14159274101257 : 0.0));
        this.smear.Time = 0;
        this.smear.Position = ((Entity) this.Owner).Center;
        this.smear.Scale = MathHelper.Lerp(2.6f, 3.5f, (float) (((double) this.Projectile.scale - 1.60000002384186) / 1.0));
        this.smear.Color = color;
      }
      if (Utils.NextBool(Main.rand))
      {
        float num = this.Projectile.scale * 78f;
        Vector2 vector2 = Utils.NextVector2Circular(Main.rand, num, num);
        Vector2.op_Multiply(Vector2.op_Multiply(Utils.SafeNormalize(Utils.RotatedBy(vector2, 1.57079637050629 * (double) ((Entity) this.Owner).direction, new Vector2()), Vector2.Zero), 2f), (float) (1.0 + (double) ((Vector2)  vector2).Length() / 15.0));
      }
      double num1 = (double) MathHelper.Clamp(MathHelper.Clamp((float) Math.Sin(((double) this.SwirlRatio() - 0.200000002980232) * 3.14159274101257), 0.0f, 1f) * 2f, 0.0f, 1f);
      float num2 = MathHelper.Clamp(MathHelper.Clamp((float) Math.Sin(((double) this.SwirlRatio() - 0.200000002980232) * 3.14159274101257), 0.0f, 1f), 0.0f, 1f);
      if (!Utils.NextBool(Main.rand))
        return;
      for (float num3 = 0.0f; (double) num3 <= 1.0; num3 += 0.5f)
      {
        Vector2.op_Addition(Vector2.op_Addition(((Entity) this.Owner).Center, Vector2.op_Multiply(Vector2.op_Multiply(Utils.ToRotationVector2(this.Projectile.rotation), (float) (30.0 + 50.0 * (double) num3)), this.Projectile.scale)), Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Multiply(Utils.RotatedBy(Utils.ToRotationVector2(this.Projectile.rotation), -1.57079637050629, new Vector2()), 30f), num2), Utils.NextFloat(Main.rand)));
        Vector2.op_Addition(Vector2.op_Multiply(Vector2.op_Multiply(Utils.RotatedBy(Utils.ToRotationVector2(this.Projectile.rotation), -1.57079637050629 * (double) ((Entity) this.Owner).direction, new Vector2()), 20f), num2), ((Entity) this.Owner).velocity);
        Main.rand.Next(3);
      }
    }

    public virtual void SendExtraAI(BinaryWriter writer)
    {
      writer.Write(this.initialized);
      Utils.WriteVector2(writer, this.direction);
      writer.Write(this.ChanceMissed);
      writer.Write(this.ThrowReach);
    }

    public virtual void ReceiveExtraAI(BinaryReader reader)
    {
      this.initialized = reader.ReadBoolean();
      this.direction = Utils.ReadVector2(reader);
      this.ChanceMissed = reader.ReadSingle();
      this.ThrowReach = reader.ReadSingle();
    }
  }
}
