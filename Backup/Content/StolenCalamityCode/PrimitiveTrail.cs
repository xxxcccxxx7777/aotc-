using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace AotC.Content.StolenCalamityCode
{
  public class PrimitiveTrail
  {
    public int DegreeOfBezierCurveCornerSmoothening;
    public PrimitiveTrail.VertexWidthFunction WidthFunction;
    public PrimitiveTrail.VertexColorFunction ColorFunction;
    public BasicEffect BaseEffect;
    public MiscShaderData SpecialShader;
    public PrimitiveTrail.TrailPointRetrievalFunction TrailPointFunction;
    public Vector2 OverridingStickPointStart = Vector2.Zero;
    public Vector2 OverridingStickPointEnd = Vector2.Zero;

    public PrimitiveTrail(
      PrimitiveTrail.VertexWidthFunction widthFunction,
      PrimitiveTrail.VertexColorFunction colorFunction,
      PrimitiveTrail.TrailPointRetrievalFunction pointFunction = null,
      MiscShaderData specialShader = null)
    {
      this.WidthFunction = widthFunction != null && colorFunction != null ? widthFunction : throw new NullReferenceException("In order to create a primitive trail, a non-null " + (widthFunction == null ? "width" : "color") + " function must be specified.");
      this.ColorFunction = colorFunction;
      if (pointFunction == null)
        pointFunction = new PrimitiveTrail.TrailPointRetrievalFunction(this.SmoothBezierPointRetreivalFunction);
      this.TrailPointFunction = pointFunction;
      if (specialShader != null)
        this.SpecialShader = specialShader;
      this.BaseEffect = new BasicEffect(((Game) Main.instance).GraphicsDevice)
      {
        VertexColorEnabled = true,
        TextureEnabled = false
      };
      this.UpdateBaseEffect(out Matrix _, out Matrix _);
    }

    public void UpdateBaseEffect(out Matrix effectProjection, out Matrix effectView)
    {
      CalamityUtils.CalculatePerspectiveMatricies(out effectView, out effectProjection);
      this.BaseEffect.View = effectView;
      this.BaseEffect.Projection = effectProjection;
    }

    public static List<Vector2> RigidPointRetreivalFunction(
      IEnumerable<Vector2> originalPositions,
      Vector2 generalOffset,
      int totalTrailPoints,
      IEnumerable<float> _ = null)
    {
      List<Vector2> list = originalPositions.Where<Vector2>((Func<Vector2, bool>) (originalPosition => Vector2.op_Inequality(originalPosition, Vector2.Zero))).ToList<Vector2>();
      List<Vector2> vector2List1 = new List<Vector2>();
      if (list.Count < 2)
      {
        for (int index1 = 0; index1 < list.Count; ++index1)
        {
          List<Vector2> vector2List2 = list;
          int num = index1;
          List<Vector2> vector2List3 = vector2List2;
          int index2 = num;
          vector2List3[index2] = Vector2.op_Addition(vector2List3[index2], generalOffset);
        }
        return list;
      }
      for (int index3 = 0; index3 < totalTrailPoints; ++index3)
      {
        float num = (float) index3 / ((float) totalTrailPoints - 1f);
        int index4 = (int) ((double) num * (double) (list.Count - 1));
        Vector2 vector2_1 = list[index4];
        Vector2 vector2_2 = list[(index4 + 1) % list.Count];
        vector2List1.Add(Vector2.op_Addition(Vector2.Lerp(vector2_1, vector2_2, (float) ((double) num * (double) (list.Count - 1) % 0.999989986419678)), generalOffset));
      }
      vector2List1.Add(Vector2.op_Addition(((IEnumerable<Vector2>) list).Last<Vector2>(), generalOffset));
      return vector2List1;
    }

    public List<Vector2> SmoothBezierPointRetreivalFunction(
      IEnumerable<Vector2> originalPositions,
      Vector2 generalOffset,
      int totalTrailPoints,
      IEnumerable<float> originalRotations = null)
    {
      List<Vector2> controlPoints = new List<Vector2>();
      for (int index = 0; index < originalPositions.Count<Vector2>(); ++index)
      {
        if (!Vector2.op_Equality(originalPositions.ElementAt<Vector2>(index), Vector2.Zero))
          controlPoints.Add(Vector2.op_Addition(originalPositions.ElementAt<Vector2>(index), generalOffset));
      }
      bool pointCountCondition = this.DegreeOfBezierCurveCornerSmoothening <= 0 || controlPoints.Count < this.DegreeOfBezierCurveCornerSmoothening * 3;
      int totalPoints = this.DegreeOfBezierCurveCornerSmoothening > 0 ? totalTrailPoints * this.DegreeOfBezierCurveCornerSmoothening : totalTrailPoints;
      BezierCurve bezierCurve = new BezierCurve(((IEnumerable<Vector2>) controlPoints).Where<Vector2>((Func<Vector2, int, bool>) ((_, i) => ((this.DegreeOfBezierCurveCornerSmoothening <= 0 || i % this.DegreeOfBezierCurveCornerSmoothening == 0 || i == 0 ? 1 : (i == controlPoints.Count - 1 ? 1 : 0)) | (pointCountCondition ? 1 : 0)) != 0)).ToArray<Vector2>());
      return controlPoints.Count > 1 ? bezierCurve.GetPoints(totalPoints) : controlPoints;
    }

    public static List<Vector2> SmoothCatmullRomPointRetreivalFunction(
      IEnumerable<Vector2> originalPositions,
      Vector2 generalOffset,
      int _,
      IEnumerable<float> originalRotations)
    {
      List<Vector2> vector2List = new List<Vector2>();
      for (int index = 0; index < originalPositions.Count<Vector2>() - 1; ++index)
      {
        Vector2 vector2_1 = originalPositions.ElementAt<Vector2>(index);
        Vector2 vector2_2 = originalPositions.ElementAt<Vector2>(index + 1);
        if (!Vector2.op_Equality(vector2_1, Vector2.Zero) && !Vector2.op_Equality(vector2_2, Vector2.Zero))
        {
          float num1 = MathHelper.WrapAngle(originalRotations.ElementAt<float>(index));
          float num2 = MathHelper.WrapAngle(originalRotations.ElementAt<float>(index + 1));
          int num3 = (int) Math.Round((double) Math.Abs(MathHelper.WrapAngle(num2 - num1)) * 8.0 / 3.14159274101257);
          vector2List.Add(Vector2.op_Addition(vector2_1, generalOffset));
          if (num3 != 0)
          {
            float num4 = Vector2.Distance(vector2_1, vector2_2);
            float num5 = 1f / (float) (num3 + 2);
            Vector2 vector2_3 = Vector2.op_Addition(vector2_1, Vector2.op_Multiply(Utils.ToRotationVector2(num1), num4));
            Vector2 vector2_4 = Vector2.op_Addition(vector2_2, Vector2.op_Multiply(Utils.ToRotationVector2(num2), 0.0f - num4));
            for (float num6 = num5; (double) num6 < 1.0; num6 += num5)
              vector2List.Add(Vector2.op_Addition(Vector2.CatmullRom(vector2_3, vector2_1, vector2_2, vector2_4, num6), generalOffset));
          }
        }
      }
      return vector2List;
    }

    public PrimitiveTrail.VertexPosition2DColor[] GetVerticesFromTrailPoints(
      List<Vector2> trailPoints)
    {
      PrimitiveTrail.VertexPosition2DColor[] source = new PrimitiveTrail.VertexPosition2DColor[trailPoints.Count * 2 - 2];
      Vector2 textureCoordinates1 = new Vector2();
      Vector2 textureCoordinates2 = new Vector2();
      Vector2 vector2_1 = new Vector2();
      for (int index = 0; index < trailPoints.Count - 1; ++index)
      {
        float completionRatio = (float) index / (float) trailPoints.Count;
        float num = this.WidthFunction(completionRatio);
        Color color = this.ColorFunction(completionRatio);
        Vector2 trailPoint = trailPoints[index];
        Vector2 vector2_2 = Utils.SafeNormalize(Vector2.op_Subtraction(trailPoints[index + 1], trailPoints[index]), Vector2.Zero);
        ((Vector2) ref textureCoordinates1).\u002Ector(completionRatio, 0.0f);
        ((Vector2) ref textureCoordinates2).\u002Ector(completionRatio, 1f);
        ((Vector2) ref vector2_1).\u002Ector(0.0f - vector2_2.Y, vector2_2.X);
        Vector2 position1 = Vector2.op_Subtraction(trailPoint, Vector2.op_Multiply(vector2_1, num));
        Vector2 position2 = Vector2.op_Addition(trailPoint, Vector2.op_Multiply(vector2_1, num));
        if (index == 0 && Vector2.op_Inequality(this.OverridingStickPointStart, Vector2.Zero))
        {
          position1 = this.OverridingStickPointStart;
          position2 = this.OverridingStickPointEnd;
        }
        source[index * 2] = new PrimitiveTrail.VertexPosition2DColor(position1, color, textureCoordinates1);
        source[index * 2 + 1] = new PrimitiveTrail.VertexPosition2DColor(position2, color, textureCoordinates2);
      }
      return ((IEnumerable<PrimitiveTrail.VertexPosition2DColor>) source).ToArray<PrimitiveTrail.VertexPosition2DColor>();
    }

    public short[] GetIndicesFromTrailPoints(int pointCount)
    {
      short[] indicesFromTrailPoints = new short[(pointCount - 1) * 6];
      for (int index1 = 0; index1 < pointCount - 2; ++index1)
      {
        int index2 = index1 * 6;
        int num = index1 * 2;
        indicesFromTrailPoints[index2] = (short) num;
        indicesFromTrailPoints[index2 + 1] = (short) (num + 1);
        indicesFromTrailPoints[index2 + 2] = (short) (num + 2);
        indicesFromTrailPoints[index2 + 3] = (short) (num + 2);
        indicesFromTrailPoints[index2 + 4] = (short) (num + 1);
        indicesFromTrailPoints[index2 + 5] = (short) (num + 3);
      }
      return indicesFromTrailPoints;
    }

    public void Draw(
      IEnumerable<Vector2> originalPositions,
      Vector2 generalOffset,
      int totalTrailPoints,
      IEnumerable<float> originalRotations = null)
    {
      ((Game) Main.instance).GraphicsDevice.RasterizerState = RasterizerState.CullNone;
      List<Vector2> trailPoints = this.TrailPointFunction(originalPositions, generalOffset, totalTrailPoints, originalRotations);
      if (trailPoints.Count < 2 || ((IEnumerable<Vector2>) trailPoints).Any<Vector2>((Func<Vector2, bool>) (point => Utils.HasNaNs(point))) || ((IEnumerable<Vector2>) trailPoints).All<Vector2>((Func<Vector2, bool>) (point => Vector2.op_Equality(point, trailPoints[0]))))
        return;
      Matrix effectProjection;
      Matrix effectView;
      this.UpdateBaseEffect(out effectProjection, out effectView);
      PrimitiveTrail.VertexPosition2DColor[] verticesFromTrailPoints = this.GetVerticesFromTrailPoints(trailPoints);
      short[] indicesFromTrailPoints = this.GetIndicesFromTrailPoints(trailPoints.Count);
      if (indicesFromTrailPoints.Length % 6 != 0 || verticesFromTrailPoints.Length % 2 != 0)
        return;
      if (this.SpecialShader != null)
      {
        ((ShaderData) this.SpecialShader).Shader.Parameters["uWorldViewProjection"].SetValue(Matrix.op_Multiply(effectView, effectProjection));
        this.SpecialShader.Apply(new DrawData?());
      }
      else
        ((Effect) this.BaseEffect).CurrentTechnique.Passes[0].Apply();
      ((Game) Main.instance).GraphicsDevice.DrawUserIndexedPrimitives<PrimitiveTrail.VertexPosition2DColor>((PrimitiveType) 0, verticesFromTrailPoints, 0, verticesFromTrailPoints.Length, indicesFromTrailPoints, 0, indicesFromTrailPoints.Length / 3);
      Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }

    public struct VertexPosition2DColor : IVertexType
    {
      public Vector2 Position;
      public Color Color;
      public Vector2 TextureCoordinates;
      private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
      {
        new VertexElement(0, (VertexElementFormat) 1, (VertexElementUsage) 0, 0),
        new VertexElement(8, (VertexElementFormat) 4, (VertexElementUsage) 1, 0),
        new VertexElement(12, (VertexElementFormat) 1, (VertexElementUsage) 2, 0)
      });

      public VertexDeclaration VertexDeclaration => PrimitiveTrail.VertexPosition2DColor._vertexDeclaration;

      public VertexPosition2DColor(Vector2 position, Color color, Vector2 textureCoordinates)
      {
        this.Position = position;
        this.Color = color;
        this.TextureCoordinates = textureCoordinates;
      }
    }

    public delegate float VertexWidthFunction(float completionRatio);

    public delegate Color VertexColorFunction(float completionRatio);

    public delegate List<Vector2> TrailPointRetrievalFunction(
      IEnumerable<Vector2> originalPositions,
      Vector2 generalOffset,
      int totalTrailPoints,
      IEnumerable<float> originalRotations = null);
  }
}
