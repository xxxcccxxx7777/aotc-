using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AotC.Content.StolenCalamityCode
{
  public class BezierCurve
  {
    public Vector2[] ControlPoints;

    public BezierCurve(params Vector2[] controls) => this.ControlPoints = controls;

    public Vector2 Evaluate(float interpolant) => this.PrivateEvaluate(this.ControlPoints, MathHelper.Clamp(interpolant, 0.0f, 1f));

    public List<Vector2> GetPoints(int totalPoints)
    {
      float num = 1f / (float) totalPoints;
      List<Vector2> points = new List<Vector2>();
      for (float interpolant = 0.0f; (double) interpolant <= 1.0; interpolant += num)
        points.Add(this.Evaluate(interpolant));
      return points;
    }

    private Vector2 PrivateEvaluate(Vector2[] points, float T)
    {
      Vector2[] vector2Array;
      for (; points.Length > 2; points = vector2Array)
      {
        vector2Array = new Vector2[points.Length - 1];
        for (int index = 0; index < points.Length - 1; ++index)
          vector2Array[index] = Vector2.Lerp(points[index], points[index + 1], T);
      }
      return Vector2.Lerp(points[0], points[1], T);
    }
  }
}
