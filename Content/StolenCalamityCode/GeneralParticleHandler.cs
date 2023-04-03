using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace AotC.Content.StolenCalamityCode
{
  public static class GeneralParticleHandler
  {
    private static List<Particle> particles;
    private static List<Particle> particlesToKill;
    internal static Dictionary<Type, int> particleTypes;
    internal static Dictionary<int, Texture2D> particleTextures;
    private static List<Particle> particleInstances;
    private static List<Particle> batchedAlphaBlendParticles;
    private static List<Particle> batchedNonPremultipliedParticles;
    private static List<Particle> batchedAdditiveBlendParticles;
    private static string noteToEveryone = "This particle system was inspired by spirit mod's own particle system, with permission granted by Yuyutsu. Love you spirit mod! -Iban";

    public static void LoadModParticleInstances(Mod mod)
    {
      Type c = typeof (Particle);
      foreach (Type loadableType in AssemblyManager.GetLoadableTypes(mod.Code))
      {
        if (loadableType.IsSubclassOf(c) && !loadableType.IsAbstract && loadableType != c)
        {
          int count = GeneralParticleHandler.particleTypes.Count;
          GeneralParticleHandler.particleTypes[loadableType] = count;
          Particle uninitializedObject = (Particle) FormatterServices.GetUninitializedObject(loadableType);
          GeneralParticleHandler.particleInstances.Add(uninitializedObject);
          string str = loadableType.Namespace.Replace('.', '/') + "/" + ((MemberInfo) loadableType).Name;
          if (uninitializedObject.Texture != "")
            str = uninitializedObject.Texture;
          GeneralParticleHandler.particleTextures[count] = ModContent.Request<Texture2D>(str, (AssetRequestMode) 1).Value;
        }
      }
    }

    internal static void Load()
    {
      GeneralParticleHandler.particles = new List<Particle>();
      GeneralParticleHandler.particlesToKill = new List<Particle>();
      GeneralParticleHandler.particleTypes = new Dictionary<Type, int>();
      GeneralParticleHandler.particleTextures = new Dictionary<int, Texture2D>();
      GeneralParticleHandler.particleInstances = new List<Particle>();
      GeneralParticleHandler.batchedAlphaBlendParticles = new List<Particle>();
      GeneralParticleHandler.batchedNonPremultipliedParticles = new List<Particle>();
      GeneralParticleHandler.batchedAdditiveBlendParticles = new List<Particle>();
      GeneralParticleHandler.LoadModParticleInstances((Mod) AotC.AotC.Instance);
    }

    internal static void Unload()
    {
      GeneralParticleHandler.particles = (List<Particle>) null;
      GeneralParticleHandler.particlesToKill = (List<Particle>) null;
      GeneralParticleHandler.particleTypes = (Dictionary<Type, int>) null;
      GeneralParticleHandler.particleTextures = (Dictionary<int, Texture2D>) null;
      GeneralParticleHandler.particleInstances = (List<Particle>) null;
      GeneralParticleHandler.batchedAlphaBlendParticles = (List<Particle>) null;
      GeneralParticleHandler.batchedNonPremultipliedParticles = (List<Particle>) null;
      GeneralParticleHandler.batchedAdditiveBlendParticles = (List<Particle>) null;
    }

    public static void SpawnParticle(Particle particle)
    {
      if (Main.gamePaused || Main.dedServ || GeneralParticleHandler.particles == null)
        return;
      GeneralParticleHandler.particles.Add(particle);
      particle.Type = GeneralParticleHandler.particleTypes[particle.GetType()];
    }

    public static void Update()
    {
      foreach (Particle particle1 in GeneralParticleHandler.particles)
      {
        if (particle1 != null)
        {
          Particle particle2 = particle1;
          particle2.Position = Vector2.op_Addition(particle2.Position, particle1.Velocity);
          ++particle1.Time;
          particle1.Update();
        }
      }
      GeneralParticleHandler.particles.RemoveAll((Predicate<Particle>) (particle => particle.Time >= particle.Lifetime && particle.SetLifetime || GeneralParticleHandler.particlesToKill.Contains(particle)));
      GeneralParticleHandler.particlesToKill.Clear();
    }

    public static void RemoveParticle(Particle particle) => GeneralParticleHandler.particlesToKill.Add(particle);

    public static void DrawAllParticles(SpriteBatch sb)
    {
      foreach (Particle particle in GeneralParticleHandler.particles)
      {
        if (particle != null)
        {
          if (particle.UseAdditiveBlend)
            GeneralParticleHandler.batchedAdditiveBlendParticles.Add(particle);
          else if (particle.UseHalfTransparency)
            GeneralParticleHandler.batchedNonPremultipliedParticles.Add(particle);
          else
            GeneralParticleHandler.batchedAlphaBlendParticles.Add(particle);
        }
      }
      if (GeneralParticleHandler.batchedAlphaBlendParticles.Count > 0)
      {
        sb.Begin((SpriteSortMode) 0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
        foreach (Particle alphaBlendParticle in GeneralParticleHandler.batchedAlphaBlendParticles)
        {
          if (alphaBlendParticle.UseCustomDraw)
          {
            alphaBlendParticle.CustomDraw(sb);
          }
          else
          {
            Rectangle rectangle = Utils.Frame(GeneralParticleHandler.particleTextures[alphaBlendParticle.Type], 1, alphaBlendParticle.FrameVariants, 0, alphaBlendParticle.Variant, 0, 0);
            sb.Draw(GeneralParticleHandler.particleTextures[alphaBlendParticle.Type], Vector2.op_Subtraction(alphaBlendParticle.Position, Main.screenPosition), new Rectangle?(rectangle), alphaBlendParticle.Color, alphaBlendParticle.Rotation, Vector2.op_Multiply(Utils.Size(rectangle), 0.5f), alphaBlendParticle.Scale, (SpriteEffects) 0, 0.0f);
            Main.NewText((object) GeneralParticleHandler.particleTextures[alphaBlendParticle.Type], new Color?());
          }
        }
        sb.End();
      }
      if (GeneralParticleHandler.batchedNonPremultipliedParticles.Count > 0)
      {
        sb.Begin((SpriteSortMode) 0, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
        foreach (Particle premultipliedParticle in GeneralParticleHandler.batchedNonPremultipliedParticles)
        {
          if (premultipliedParticle.UseCustomDraw)
          {
            premultipliedParticle.CustomDraw(sb);
          }
          else
          {
            Rectangle rectangle = Utils.Frame(GeneralParticleHandler.particleTextures[premultipliedParticle.Type], 1, premultipliedParticle.FrameVariants, 0, premultipliedParticle.Variant, 0, 0);
            sb.Draw(GeneralParticleHandler.particleTextures[premultipliedParticle.Type], Vector2.op_Subtraction(premultipliedParticle.Position, Main.screenPosition), new Rectangle?(rectangle), premultipliedParticle.Color, premultipliedParticle.Rotation, Vector2.op_Multiply(Utils.Size(rectangle), 0.5f), premultipliedParticle.Scale, (SpriteEffects) 0, 0.0f);
          }
        }
        sb.End();
      }
      if (GeneralParticleHandler.batchedAdditiveBlendParticles.Count > 0)
      {
        sb.Begin((SpriteSortMode) 0, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, (Effect) null, Main.GameViewMatrix.TransformationMatrix);
        foreach (Particle additiveBlendParticle in GeneralParticleHandler.batchedAdditiveBlendParticles)
        {
          if (additiveBlendParticle.UseCustomDraw)
          {
            additiveBlendParticle.CustomDraw(sb);
          }
          else
          {
            Rectangle rectangle = Utils.Frame(GeneralParticleHandler.particleTextures[additiveBlendParticle.Type], 1, additiveBlendParticle.FrameVariants, 0, additiveBlendParticle.Variant, 0, 0);
            sb.Draw(GeneralParticleHandler.particleTextures[additiveBlendParticle.Type], Vector2.op_Subtraction(additiveBlendParticle.Position, Main.screenPosition), new Rectangle?(rectangle), additiveBlendParticle.Color, additiveBlendParticle.Rotation, Vector2.op_Multiply(Utils.Size(rectangle), 0.5f), additiveBlendParticle.Scale * 7.162539E+07f, (SpriteEffects) 0, 0.0f);
          }
        }
        sb.End();
      }
      GeneralParticleHandler.batchedAlphaBlendParticles.Clear();
      GeneralParticleHandler.batchedNonPremultipliedParticles.Clear();
      GeneralParticleHandler.batchedAdditiveBlendParticles.Clear();
    }

    public static Texture2D GetTexture(int type) => GeneralParticleHandler.particleTextures[type];
  }
}
