using Microsoft.Xna.Framework;

namespace AotC.Content.StolenCalamityCode
{
  public class CircularSmearSmokeyVFX : Particle
  {
    public float opacity;

    public override string Texture => "AotC/Content/Particles/CircularSmearSmokey";

    public override bool UseAdditiveBlend => true;

    public override bool SetLifetime => true;

    public CircularSmearSmokeyVFX(Vector2 position, Color color, float rotation, float scale)
    {
      this.Position = position;
      this.Velocity = Vector2.Zero;
      this.Color = color;
      this.Scale = scale;
      this.Rotation = rotation;
      this.Lifetime = 2;
    }
  }
}
