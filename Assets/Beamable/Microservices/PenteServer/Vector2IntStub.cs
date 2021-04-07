#if BEAMABLE_MICROSERVICE
// Decompiled with JetBrains decompiler
// Type: UnityEngine.Vector2Int
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C88E78D9-5135-4A2E-BF09-9668E318691F
// Assembly location: /Applications/UnityEditor/2019.4.17f1/Unity.app/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll

using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
  /// <summary>
  ///   <para>Representation of 2D vectors and points using integers.</para>
  /// </summary>
  public struct Vector2Int : IEquatable<Vector2Int>
  {
    private static readonly Vector2Int s_Zero = new Vector2Int(0, 0);
    private static readonly Vector2Int s_One = new Vector2Int(1, 1);
    private static readonly Vector2Int s_Up = new Vector2Int(0, 1);
    private static readonly Vector2Int s_Down = new Vector2Int(0, -1);
    private static readonly Vector2Int s_Left = new Vector2Int(-1, 0);
    private static readonly Vector2Int s_Right = new Vector2Int(1, 0);
    private int m_X;
    private int m_Y;

    /// <summary>
    ///   <para>X component of the vector.</para>
    /// </summary>
    public int x
    {
      get
      {
        return this.m_X;
      }
      set
      {
        this.m_X = value;
      }
    }

    /// <summary>
    ///   <para>Y component of the vector.</para>
    /// </summary>
    public int y
    {
      get
      {
        return this.m_Y;
      }
      set
      {
        this.m_Y = value;
      }
    }

    public Vector2Int(int x, int y)
    {
      this.m_X = x;
      this.m_Y = y;
    }

    /// <summary>
    ///   <para>Set x and y components of an existing Vector2Int.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Set(int x, int y)
    {
      this.m_X = x;
      this.m_Y = y;
    }

    public int this[int index]
    {
      get
      {
        switch (index)
        {
          case 0:
            return this.x;
          case 1:
            return this.y;
          default:
            throw new IndexOutOfRangeException(string.Format("Invalid Vector2Int index addressed: {0}!", (object) index));
        }
      }
      set
      {
        switch (index)
        {
          case 0:
            this.x = value;
            break;
          case 1:
            this.y = value;
            break;
          default:
            throw new IndexOutOfRangeException(string.Format("Invalid Vector2Int index addressed: {0}!", (object) index));
        }
      }
    }

    /// <summary>
    ///   <para>Returns the length of this vector (Read Only).</para>
    /// </summary>
    public float magnitude
    {
      get
      {
        return (float)Math.Sqrt((this.x * this.x + this.y * this.y));
      }
    }

    /// <summary>
    ///   <para>Returns the squared length of this vector (Read Only).</para>
    /// </summary>
    public int sqrMagnitude
    {
      get
      {
        return this.x * this.x + this.y * this.y;
      }
    }

    /// <summary>
    ///   <para>Returns the distance between a and b.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static float Distance(Vector2Int a, Vector2Int b)
    {
      float num1 = (float) (a.x - b.x);
      float num2 = (float) (a.y - b.y);
      return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector2Int Min(Vector2Int lhs, Vector2Int rhs)
    {
      return new Vector2Int(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static Vector2Int Max(Vector2Int lhs, Vector2Int rhs)
    {
      return new Vector2Int(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
    }

    /// <summary>
    ///   <para>Multiplies two vectors component-wise.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static Vector2Int Scale(Vector2Int a, Vector2Int b)
    {
      return new Vector2Int(a.x * b.x, a.y * b.y);
    }

    /// <summary>
    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
    /// </summary>
    /// <param name="scale"></param>
    public void Scale(Vector2Int scale)
    {
      this.x *= scale.x;
      this.y *= scale.y;
    }

    /// <summary>
    ///   <para>Clamps the Vector2Int to the bounds given by min and max.</para>
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void Clamp(Vector2Int min, Vector2Int max)
    {
      this.x = Math.Max(min.x, this.x);
      this.x = Math.Min(max.x, this.x);
      this.y = Math.Max(min.y, this.y);
      this.y = Math.Min(max.y, this.y);
    }

    public static Vector2Int operator -(Vector2Int v)
    {
      return new Vector2Int(-v.x, -v.y);
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
      return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
      return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static Vector2Int operator *(Vector2Int a, Vector2Int b)
    {
      return new Vector2Int(a.x * b.x, a.y * b.y);
    }

    public static Vector2Int operator *(int a, Vector2Int b)
    {
      return new Vector2Int(a * b.x, a * b.y);
    }

    public static Vector2Int operator *(Vector2Int a, int b)
    {
      return new Vector2Int(a.x * b, a.y * b);
    }

    public static Vector2Int operator /(Vector2Int a, int b)
    {
      return new Vector2Int(a.x / b, a.y / b);
    }

    public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
    {
      return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
    {
      return !(lhs == rhs);
    }

    /// <summary>
    ///   <para>Returns true if the objects are equal.</para>
    /// </summary>
    /// <param name="other"></param>
    public override bool Equals(object other)
    {
      if (!(other is Vector2Int))
        return false;
      return this.Equals((Vector2Int) other);
    }

    public bool Equals(Vector2Int other)
    {
      return this.x.Equals(other.x) && this.y.Equals(other.y);
    }

    /// <summary>
    ///   <para>Gets the hash code for the Vector2Int.</para>
    /// </summary>
    /// <returns>
    ///   <para>The hash code of the Vector2Int.</para>
    /// </returns>
    public override int GetHashCode()
    {
      int num1 = this.x;
      int hashCode = num1.GetHashCode();
      num1 = this.y;
      int num2 = num1.GetHashCode() << 2;
      return hashCode ^ num2;
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    public override string ToString()
    {
      return String.Format("({0}, {1})", (object) this.x, (object) this.y);
    }

    /// <summary>
    ///   <para>Shorthand for writing Vector2Int (0, 0).</para>
    /// </summary>
    public static Vector2Int zero
    {
      get
      {
        return Vector2Int.s_Zero;
      }
    }

    /// <summary>
    ///   <para>Shorthand for writing Vector2Int (1, 1).</para>
    /// </summary>
    public static Vector2Int one
    {
      get
      {
        return Vector2Int.s_One;
      }
    }

    /// <summary>
    ///   <para>Shorthand for writing Vector2Int (0, 1).</para>
    /// </summary>
    public static Vector2Int up
    {
      get
      {
        return Vector2Int.s_Up;
      }
    }

    /// <summary>
    ///   <para>Shorthand for writing Vector2Int (0, -1).</para>
    /// </summary>
    public static Vector2Int down
    {
      get
      {
        return Vector2Int.s_Down;
      }
    }

    /// <summary>
    ///   <para>Shorthand for writing Vector2Int (-1, 0).</para>
    /// </summary>
    public static Vector2Int left
    {
      get
      {
        return Vector2Int.s_Left;
      }
    }

    /// <summary>
    ///   <para>Shorthand for writing Vector2Int (1, 0).</para>
    /// </summary>
    public static Vector2Int right
    {
      get
      {
        return Vector2Int.s_Right;
      }
    }
  }
}

#endif