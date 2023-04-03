using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace AotC.Content.StolenCalamityCode
{
  public abstract class BaseLaserbeamProjectile : ModProjectile
  {
    public float RotationalSpeed
    {
      get => this.Projectile.ai[0];
      set => this.Projectile.ai[0] = value;
    }

    public float Time
    {
      get => this.Projectile.localAI[0];
      set => this.Projectile.localAI[0] = value;
    }

    public float LaserLength
    {
      get => this.Projectile.localAI[1];
      set => this.Projectile.localAI[1] = value;
    }

    public abstract float Lifetime { get; }

    public abstract float MaxScale { get; }

    public abstract float MaxLaserLength { get; }

    public abstract Texture2D LaserBeginTexture { get; }

    public abstract Texture2D LaserMiddleTexture { get; }

    public abstract Texture2D LaserEndTexture { get; }

    public virtual float ScaleExpandRate => 4f;

    public virtual Color LightCastColor => Color.White;

    public virtual Color LaserOverlayColor => Color.op_Multiply(Color.White, 0.9f);

    public virtual void Behavior()
    {
      this.AttachToSomething();
      ((Entity) this.Projectile).velocity = Utils.SafeNormalize(((Entity) this.Projectile).velocity, Vector2.op_UnaryNegation(Vector2.UnitY));
      ++this.Time;
      if ((double) this.Time >= (double) this.Lifetime)
      {
        this.Projectile.Kill();
      }
      else
      {
        this.DetermineScale();
        this.UpdateLaserMotion();
        this.LaserLength = MathHelper.Lerp(this.LaserLength, this.DetermineLaserLength(), 0.9f);
        if (!Color.op_Inequality(this.LightCastColor, Color.Transparent))
          return;
        Color lightCastColor = this.LightCastColor;
        DelegateMethods.v3_1 = ((Color) lightCastColor).ToVector3();
        
        Utils.PlotTileLine(((Entity) this.Projectile).Center, Vector2.op_Addition(((Entity) this.Projectile).Center, Vector2.op_Multiply(((Entity) this.Projectile).velocity, this.LaserLength)), (float) ((Entity) this.Projectile).width * this.Projectile.scale, BaseLaserbeamProjectile. __CastLight ?? (BaseLaserbeamProjectile. __CastLight = new Utils.TileActionAttempt((object) null, __methodptr(CastLight))));
      }
    }

    public virtual void UpdateLaserMotion()
    {
      float num = Utils.ToRotation(((Entity) this.Projectile).velocity) + this.RotationalSpeed;
      this.Projectile.rotation = num - 1.570796f;
      ((Entity) this.Projectile).velocity = Utils.ToRotationVector2(num);
    }

    public virtual void DetermineScale()
    {
      this.Projectile.scale = (float) Math.Sin((double) this.Time / (double) this.Lifetime * 3.14159274101257) * this.ScaleExpandRate * this.MaxScale;
      if ((double) this.Projectile.scale <= (double) this.MaxScale)
        return;
      this.Projectile.scale = this.MaxScale;
    }

    public virtual void AttachToSomething()
    {
    }

    public virtual float DetermineLaserLength() => this.MaxLaserLength;

    public virtual void ExtraBehavior()
    {
    }

    public float DetermineLaserLength_CollideWithTiles(int samplePointCount)
    {
      float[] source = new float[samplePointCount];
      Collision.LaserScan(((Entity) this.Projectile).Center, ((Entity) this.Projectile).velocity, this.Projectile.scale, this.MaxLaserLength, source);
      return ((IEnumerable<float>) source).Average();
    }

    protected internal void DrawBeamWithColor(
      Color beamColor,
      float scale,
      int startFrame = 0,
      int middleFrame = 0,
      int endFrame = 0)
    {
      Rectangle rectangle1 = Utils.Frame(this.LaserBeginTexture, 1, Main.projFrames[this.Projectile.type], 0, startFrame, 0, 0);
      Rectangle rectangle2 = Utils.Frame(this.LaserMiddleTexture, 1, Main.projFrames[this.Projectile.type], 0, middleFrame, 0, 0);
      Rectangle rectangle3 = Utils.Frame(this.LaserEndTexture, 1, Main.projFrames[this.Projectile.type], 0, endFrame, 0, 0);
      Main.EntitySpriteDraw(this.LaserBeginTexture, Vector2.op_Subtraction(((Entity) this.Projectile).Center, Main.screenPosition), new Rectangle?(rectangle1), beamColor, this.Projectile.rotation, Vector2.op_Division(Utils.Size(this.LaserBeginTexture), 2f), scale, (SpriteEffects) 0, 0);
      float num1 = this.LaserLength - (float) (rectangle1.Height / 2 + rectangle3.Height) * scale;
      Vector2 vector2 = Vector2.op_Addition(((Entity) this.Projectile).Center, Vector2.op_Division(Vector2.op_Multiply(Vector2.op_Multiply(((Entity) this.Projectile).velocity, scale), (float) rectangle1.Height), 2f));
      if ((double) num1 > 0.0)
      {
        float num2 = (float) rectangle2.Height * scale;
        float num3 = 0.0f;
        while ((double) num3 + 1.0 < (double) num1)
        {
          Main.EntitySpriteDraw(this.LaserMiddleTexture, Vector2.op_Subtraction(vector2, Main.screenPosition), new Rectangle?(rectangle2), beamColor, this.Projectile.rotation, Vector2.op_Multiply((float) this.LaserMiddleTexture.Width * 0.5f, Vector2.UnitX), scale, (SpriteEffects) 0, 0);
          num3 += num2;
          vector2 = Vector2.op_Addition(vector2, Vector2.op_Multiply(((Entity) this.Projectile).velocity, num2));
        }
      }
      if ((double) Math.Abs(this.LaserLength - this.DetermineLaserLength()) >= 30.0)
        return;
      Main.EntitySpriteDraw(this.LaserEndTexture, Vector2.op_Subtraction(vector2, Main.screenPosition), new Rectangle?(rectangle3), beamColor, this.Projectile.rotation, Utils.Top(Utils.Frame(this.LaserEndTexture, 1, 1, 0, 0, 0, 0)), scale, (SpriteEffects) 0, 0);
    }

    public virtual void AI()
    {
      ProjectileID.Sets.DrawScreenCheckFluff[this.Projectile.type] = 10000;
      this.Behavior();
      this.ExtraBehavior();
    }

    public virtual void CutTiles()
    {
      DelegateMethods.tilecut_0 = (TileCuttingContext) 1;
      Vector2 center = ((Entity) this.Projectile).Center;
      Vector2 vector2_1 = Vector2.op_Addition(((Entity) this.Projectile).Center, Vector2.op_Multiply(((Entity) this.Projectile).velocity, this.LaserLength));
      Vector2 size = ((Entity) this.Projectile).Size;
      Vector2 vector2_2 = vector2_1;
      Vector2 vector2_3 = size;
      double num = (double) ((Vector2)  vector2_3).Length() * (double) this.Projectile.scale;
      
      Utils.TileActionAttempt tileActionAttempt = BaseLaserbeamProjectile. __CutTiles ?? (BaseLaserbeamProjectile. __CutTiles = new Utils.TileActionAttempt((object) null, __methodptr(CutTiles)));
      Utils.PlotTileLine(center, vector2_2, (float) num, tileActionAttempt);
    }

    public virtual bool PreDraw(ref Color lightColor)
    {
      if (Vector2.op_Equality(((Entity) this.Projectile).velocity, Vector2.Zero))
        return false;
      this.DrawBeamWithColor(this.LaserOverlayColor, this.Projectile.scale);
      return false;
    }

    public virtual bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
      Rectangle rectangle = projHitbox;
      if (((Rectangle)  rectangle).Intersects(targetHitbox))
        return new bool?(true);
      float num = 0.0f;
      Vector2 vector2_1 = Utils.TopLeft(targetHitbox);
      Vector2 vector2_2 = Utils.Size(targetHitbox);
      Vector2 center = ((Entity) this.Projectile).Center;
      Vector2 vector2_3 = Vector2.op_Addition(((Entity) this.Projectile).Center, Vector2.op_Multiply(Utils.ToRotationVector2(this.Projectile.ai[0]), 908f));
      Vector2 size = ((Entity) this.Projectile).Size;
      Vector2 vector2_4 = vector2_2;
      Vector2 vector2_5 = center;
      Vector2 vector2_6 = vector2_3;
      double scale = (double) this.Projectile.scale;
      ref float local = ref num;
      return new bool?(Collision.CheckAABBvLineCollision(vector2_1, vector2_4, vector2_5, vector2_6, (float) scale, ref local));
    }

    public virtual bool ShouldUpdatePosition() => false;
  }
}
