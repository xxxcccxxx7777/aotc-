using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AotC.Content.StolenCalamityCode
{
  public static class CalamityUtils
  {
    internal static readonly FieldInfo UImageFieldMisc = typeof (MiscShaderData).GetField("_uImage1", (BindingFlags) 36);
    private static readonly CalamityUtils.EasingFunction[] EasingTypeToFunction = new CalamityUtils.EasingFunction[14]
    {
      new CalamityUtils.EasingFunction(CalamityUtils.LinearEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.SineInEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.SineOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.SineInOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.SineBumpEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.PolyInEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.PolyOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.PolyInOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.ExpInEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.ExpOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.ExpInOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.CircInEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.CircOutEasing),
      new CalamityUtils.EasingFunction(CalamityUtils.CircInOutEasing)
    };

    public static MiscShaderData SetShaderTexture(
      this MiscShaderData shader,
      Asset<Texture2D> texture)
    {
      CalamityUtils.UImageFieldMisc.SetValue((object) shader, (object) texture);
      return shader;
    }

    public static float LinearEasing(float amount, int degree) => amount;

    public static float SineInEasing(float amount, int degree) => 1f - (float) Math.Cos((double) amount * 3.14159274101257 / 2.0);

    public static float SineOutEasing(float amount, int degree) => (float) Math.Sin((double) amount * 3.14159274101257 / 2.0);

    public static float SineInOutEasing(float amount, int degree) => (float) ((0.0 - (Math.Cos((double) amount * 3.14159274101257) - 1.0)) / 2.0);

    public static float SineBumpEasing(float amount, int degree) => (float) Math.Sin((double) amount * 3.14159274101257);

    public static float PolyInEasing(float amount, int degree) => (float) Math.Pow((double) amount, (double) degree);

    public static float PolyOutEasing(float amount, int degree) => 1f - (float) Math.Pow(1.0 - (double) amount, (double) degree);

    public static float PolyInOutEasing(float amount, int degree) => (double) amount >= 0.5 ? (float) (1.0 - Math.Pow(-2.0 * (double) amount + 2.0, (double) degree) / 2.0) : (float) Math.Pow(2.0, (double) (degree - 1)) * (float) Math.Pow((double) amount, (double) degree);

    public static float ExpInEasing(float amount, int degree) => (double) amount != 0.0 ? (float) Math.Pow(2.0, 10.0 * (double) amount - 10.0) : 0.0f;

    public static float ExpOutEasing(float amount, int degree) => (double) amount != 1.0 ? 1f - (float) Math.Pow(2.0, -10.0 * (double) amount) : 1f;

    public static float ExpInOutEasing(float amount, int degree)
    {
      if ((double) amount == 0.0)
        return 0.0f;
      if ((double) amount == 1.0)
        return 1f;
      return (double) amount >= 0.5 ? (float) ((2.0 - Math.Pow(2.0, -20.0 * (double) amount - 10.0)) / 2.0) : (float) Math.Pow(2.0, 20.0 * (double) amount - 10.0) / 2f;
    }

    public static float CircInEasing(float amount, int degree) => 1f - (float) Math.Sqrt(1.0 - Math.Pow((double) amount, 2.0));

    public static float CircOutEasing(float amount, int degree) => (float) Math.Sqrt(1.0 - Math.Pow((double) amount - 1.0, 2.0));

    public static float CircInOutEasing(float amount, int degree) => (double) amount >= 0.5 ? (float) ((Math.Sqrt(1.0 - Math.Pow(-2.0 * (double) amount - 2.0, 2.0)) + 1.0) / 2.0) : (float) ((1.0 - Math.Sqrt(1.0 - Math.Pow(2.0 * (double) amount, 2.0))) / 2.0);

    public static float PiecewiseAnimation(float progress, CalamityUtils.CurveSegment[] segments)
    {
      if (segments.Length == 0)
        return 0.0f;
      if ((double) segments[0].originX != 0.0)
        segments[0].originX = 0.0f;
      progress = MathHelper.Clamp(progress, 0.0f, 1f);
      float num1 = 0.0f;
      for (int index = 0; index <= segments.Length - 1; ++index)
      {
        CalamityUtils.CurveSegment segment = segments[index];
        float originX = segment.originX;
        float num2 = 1f;
        if ((double) progress >= (double) segment.originX)
        {
          if (index < segments.Length - 1)
          {
            if ((double) segments[index + 1].originX > (double) progress)
              num2 = segments[index + 1].originX;
            else
              continue;
          }
          float num3 = num2 - originX;
          float amount = (progress - segment.originX) / num3;
          float originY = segment.originY;
          num1 = segment.easing == null ? originY + CalamityUtils.LinearEasing(amount, segment.degree) * segment.displacement : originY + segment.easing(amount, segment.degree) * segment.displacement;
          break;
        }
      }
      return num1;
    }

    public static NPC ClosestNPCAt(
      this Vector2 origin,
      float maxDistanceToCheck,
      bool ignoreTiles = true,
      bool bossPriority = false)
    {
      NPC npc = (NPC) null;
      float num1 = maxDistanceToCheck;
      if (bossPriority)
      {
        bool flag1 = false;
        for (int index = 0; index < Main.npc.Length; ++index)
        {
          if ((!flag1 || Main.npc[index].boss || Main.npc[index].type == 114) && Main.npc[index].CanBeChasedBy((object) null, false))
          {
            float num2 = (float) (((Entity) Main.npc[index]).width / 2 + ((Entity) Main.npc[index]).height / 2);
            bool flag2 = true;
            if ((double) num2 < (double) num1 && !ignoreTiles)
              flag2 = Collision.CanHit(origin, 1, 1, ((Entity) Main.npc[index]).Center, 1, 1);
            if ((double) Vector2.Distance(origin, ((Entity) Main.npc[index]).Center) < (double) num1 + (double) num2 & flag2)
            {
              if (Main.npc[index].boss || Main.npc[index].type == 114)
                flag1 = true;
              num1 = Vector2.Distance(origin, ((Entity) Main.npc[index]).Center);
              npc = Main.npc[index];
            }
          }
        }
      }
      else
      {
        for (int index = 0; index < Main.npc.Length; ++index)
        {
          if (Main.npc[index].CanBeChasedBy((object) null, false))
          {
            float num3 = (float) (((Entity) Main.npc[index]).width / 2 + ((Entity) Main.npc[index]).height / 2);
            bool flag = true;
            if ((double) num3 < (double) num1 && !ignoreTiles)
              flag = Collision.CanHit(origin, 1, 1, ((Entity) Main.npc[index]).Center, 1, 1);
            if ((double) Vector2.Distance(origin, ((Entity) Main.npc[index]).Center) < (double) num1 + (double) num3 & flag)
            {
              num1 = Vector2.Distance(origin, ((Entity) Main.npc[index]).Center);
              npc = Main.npc[index];
            }
          }
        }
      }
      return npc;
    }

    public static float AngleBetween(this Vector2 v1, Vector2 v2) => (float) Math.Acos((double) Vector2.Dot(Utils.SafeNormalize(v1, Vector2.Zero), Utils.SafeNormalize(v2, Vector2.Zero)));

    public static void CalculatePerspectiveMatricies(
      out Matrix viewMatrix,
      out Matrix projectionMatrix)
    {
      Vector2 zoom = Main.GameViewMatrix.Zoom;
      Matrix scale = Matrix.CreateScale(zoom.X, zoom.Y, 1f);
      Viewport viewport1 = ((Game) Main.instance).GraphicsDevice.Viewport;
      int width = ((Viewport) viewport1).Width;
      Viewport viewport2 = ((Game) Main.instance).GraphicsDevice.Viewport;
      int height = ((Viewport) viewport2).Height;
      viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
      viewMatrix = Matrix.op_Multiply(viewMatrix, Matrix.CreateTranslation(0.0f, (float) -height, 0.0f));
      viewMatrix = Matrix.op_Multiply(viewMatrix, Matrix.CreateRotationZ(3.141593f));
      if ((double) Main.LocalPlayer.gravDir == -1.0)
        viewMatrix = Matrix.op_Multiply(viewMatrix, Matrix.op_Multiply(Matrix.CreateScale(1f, -1f, 1f), Matrix.CreateTranslation(0.0f, (float) height, 0.0f)));
      viewMatrix = Matrix.op_Multiply(viewMatrix, scale);
      projectionMatrix = Matrix.op_Multiply(Matrix.CreateOrthographicOffCenter(0.0f, (float) width * zoom.X, 0.0f, (float) height * zoom.Y, 0.0f, 1f), scale);
    }

    public static Color HsvToRgb(double h, double s, double v)
    {
      int num1 = (int) Math.Floor(h / 60.0) % 6;
      double num2 = h / 60.0 - Math.Floor(h / 60.0);
      double num3 = v * (1.0 - s);
      double num4 = v * (1.0 - num2 * s);
      double num5 = v * (1.0 - (1.0 - num2) * s);
      Color rgb;
      switch (num1)
      {
        case 0:
          rgb = CalamityUtils.GetRgb(v, num5, num3);
          break;
        case 1:
          rgb = CalamityUtils.GetRgb(num4, v, num3);
          break;
        case 2:
          rgb = CalamityUtils.GetRgb(num3, v, num5);
          break;
        case 3:
          rgb = CalamityUtils.GetRgb(num3, num4, v);
          break;
        case 4:
          rgb = CalamityUtils.GetRgb(num5, num3, v);
          break;
        case 5:
          rgb = CalamityUtils.GetRgb(v, num3, num4);
          break;
        default:
          // ISSUE: explicit constructor call
          ((Color) rgb) (1f, 0.0f, 0.0f, 0.0f);
          break;
      }
      return rgb;
    }

    public static Color GetRgb(double r, double g, double b) => new Color((int) (byte) (r * (double) byte.MaxValue), (int) (byte) (g * (double) byte.MaxValue), (int) (byte) (b * (double) byte.MaxValue), (int) byte.MaxValue);

    public enum EasingType
    {
      Linear,
      SineIn,
      SineOut,
      SineInOut,
      SineBump,
      PolyIn,
      PolyOut,
      PolyInOut,
      ExpIn,
      ExpOut,
      ExpInOut,
      CircIn,
      CircOut,
      CircInOut,
    }

    public delegate float EasingFunction(float amount, int degree);

    public struct CurveSegment
    {
      public CalamityUtils.EasingFunction easing;
      public float originX;
      public float originY;
      public float displacement;
      public int degree;

      public CurveSegment(
        CalamityUtils.EasingType MODE,
        float ORGX,
        float ORGY,
        float DISP,
        int DEG = 1)
      {
        this.easing = CalamityUtils.EasingTypeToFunction[(int) MODE];
        this.originX = ORGX;
        this.originY = ORGY;
        this.displacement = DISP;
        this.degree = DEG;
      }

      public CurveSegment(
        CalamityUtils.EasingFunction MODE,
        float ORGX,
        float ORGY,
        float DISP,
        int DEG = 1)
      {
        this.easing = MODE;
        this.originX = ORGX;
        this.originY = ORGY;
        this.displacement = DISP;
        this.degree = DEG;
      }
    }
  }
}
