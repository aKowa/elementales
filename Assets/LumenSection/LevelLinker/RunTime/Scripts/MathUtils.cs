// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>
using UnityEngine;
using static UnityEngine.Mathf;



namespace LumenSection.LevelLinker
{
public class MathUtils
{
  private static Vector2 RoundPosition(Vector3 position, Vector2 snap, Vector2 offset)
  {
    position = position - (Vector3)offset;
    return new Vector3(!Approximately(snap.x, 0f) ? snap.x * Round(position.x / snap.x) : position.x, !Approximately(snap.y, 0f) ? snap.y * Round(position.y / snap.y) : position.y, position.z) + new Vector3(offset.x, offset.y, 0f);
  }

  public static Vector2 RoundPosition(Vector3 position, Vector2 snap, Vector2 scale, Vector2 offset, Vector2 size)
  {
    int rx = (int)Floor(size.x / scale.x);
    int ry = (int)Floor(size.y / scale.y);
    return RoundPosition(position, snap, new Vector2(rx % 2 == 0 ? 0f : scale.x / 2f, ry % 2 == 0 ? 0f : scale.y / 2f) + offset);
  }

  public static bool IsOnSegment2D(Vector2 p1, Vector2 p2, Vector2 pos, float thickness, out float segmentLength, out float positionOnSegment)
  {
    float a        = p2.y - p1.y;
    float b        = p1.x - p2.x;
    float abLength = Sqrt((a * a) + (b * b));
    a /= abLength;
    b /= abLength;
    float c    = (p1.y * p2.x - p1.x * p2.y) / abLength;
    float dist = Abs(a * pos.x + b * pos.y + c) / (a * a + b * b);
    if (dist < thickness) // The higher this constant, the thicker the line
    {
      float k;
      if (Abs(a) > 0.001f)
      {
        k = (1f / (a * a + b * b) * (-a * b * pos.x + a * a * pos.y - b * c) - p1.y) / a;
      }
      else
      {
        k = (p1.x - 1f / (a * a + b * b) * (b * b * pos.x - a * b * pos.y - a * c)) / b;
      }

      segmentLength     = abLength;
      positionOnSegment = k;

      if (k >= 0f && k <= abLength)
      {
        return true;
      }
    }

    segmentLength     = abLength;
    positionOnSegment = -1f;

    return false;
  }

  public static bool PointCircleIntersection(Vector2 point, Vector2 center, float radius)
  {
    float sqDist = (point - center).sqrMagnitude;
    return sqDist <= radius * radius;
  }
}
}
