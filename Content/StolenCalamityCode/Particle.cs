using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AotC.Content.StolenCalamityCode
{
  public class Particle
  {
    public int ID;
    public int Type;
    public int Time;
    public int Lifetime;
    public Vector2 RelativeOffset;
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Origin;
    public Color Color;
    public float Rotation;
    public float Scale;
    public int Variant;

    public virtual bool Important => false;

    public virtual bool SetLifetime => false;

    public float LifetimeCompletion => this.Lifetime == 0 ? 0.0f : (float) this.Time / (float) this.Lifetime;

    public virtual int FrameVariants => 1;

    public virtual string Texture => "";

    public virtual bool UseCustomDraw => false;

    public virtual bool UseAdditiveBlend => false;

    public virtual bool UseHalfTransparency => false;

    public virtual void CustomDraw(SpriteBatch spriteBatch)
    {
    }

    public virtual void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition)
    {
    }

    public virtual void Update()
    {
    }

    public void Kill() => GeneralParticleHandler.RemoveParticle(this);
  }
}
