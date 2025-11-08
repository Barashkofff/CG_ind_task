using System;
using System.Numerics; // INumber, IFloatingPoint

namespace MathPrimitives
{
    public readonly struct Vec2
    {
        public double X { get; }
        public double Y { get; }

        public Vec2(double x, double y)
        {
            X = x; Y = y;
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public void Deconstruct(out double x, out double y) { x = X; y = Y; }

        // Constants
        public static Vec2 Zero => new(0, 0);
        public static Vec2 One => new(1, 1);
        public static Vec2 UnitX => new(1, 0);
        public static Vec2 UnitY => new(0, 1);

        // Vec operations
        public static Vec2 operator +(in Vec2 a, in Vec2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(in Vec2 a, in Vec2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(in Vec2 v) => new(-v.X, -v.Y);

        // Scalar operations
        public static Vec2 operator *(in Vec2 v, double s) => new(v.X * s, v.Y * s);
        public static Vec2 operator *(double s, in Vec2 v) => new(v.X * s, v.Y * s);
        public static Vec2 operator /(in Vec2 v, double s) => new(v.X / s, v.Y / s);

        // Equality
        public static bool operator ==(in Vec2 a, in Vec2 b) =>
            a.X == b.X && a.Y == b.Y;
        public static bool operator !=(in Vec2 a, in Vec2 b) => !(a == b);

        public override bool Equals(object? obj) =>
            obj is Vec2 o && this == o;


        public Vec2 WithX(double x) => new(x, Y);
        public Vec2 WithY(double y) => new(X, y);

        public override string ToString() => $"({X}, {Y})";
    }

    public static class Vec2Math
    {
        public static double Dot(this in Vec2 a, in Vec2 b)
            => a.X * b.X + a.Y * b.Y;

        public static double Cross(in Vec2 a, in Vec2 b)
            => a.X * b.Y - a.Y * b.X;

        public static double LengthSquared(in Vec2 v)
            => Dot(v, v);

        public static double DistanceSquared(this in Vec2 a, in Vec2 b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        public static Vec2 Lerp(in Vec2 a, in Vec2 b, double t)
        {
            // a + t*(b - a)
            return new Vec2(a.X + t * (b.X - a.X),
                               a.Y + t * (b.Y - a.Y));
        }

        // Approximately equal with given epsilon
        public static bool Approximately(this in Vec2 a, in Vec2 b, double eps)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            dx = dx >= 0 ? dx : -dx;
            dy = dy >= 0 ? dy : -dy;
            return (dx <= eps) && (dy <= eps);
        }

        public static double Length(this in Vec2 v)
            => Math.Sqrt(LengthSquared(v));

        public static double Distance(this in Vec2 a, in Vec2 b)
            => Math.Sqrt(DistanceSquared(a, b));

        public static Vec2 Normalize(this in Vec2 v, double epsilon = default)
        {
            if (epsilon == default) epsilon = double.CreateChecked(1e-12);
            var len2 = LengthSquared(v);
            if (len2 <= epsilon * epsilon) return Vec2.Zero;
            var invLen = 1 / Math.Sqrt(len2);
            return new Vec2(v.X * invLen, v.Y * invLen);
        }

        // Project a onto b
        public static Vec2 Project(in Vec2 a, in Vec2 b, double epsilon = default)
        {
            var denom = Dot(b, b);
            if (epsilon == default) epsilon = double.CreateChecked(1e-12);
            if (denom <= epsilon) return Vec2.Zero;
            var k = Dot(a, b) / denom;
            return new Vec2(b.X * k, b.Y * k);
        }

        // Reflect v relative to n
        public static Vec2 Reflect(in Vec2 v, in Vec2 n)
        {
            var nn = Dot(n, n);
            if (nn == 0) return v;
            var k = Dot(v, n) / nn;
            return new Vec2(v.X - (n.X * (k + k)),
                               v.Y - (n.Y * (k + k)));
        }

        // Clamp vector magnitude to maxLen
        public static Vec2 ClampMagnitude(in Vec2 v, double maxLen)
        {
            var len2 = LengthSquared(v);
            var max2 = maxLen * maxLen;
            if (len2 <= max2) return v;
            var scale = maxLen / Math.Sqrt(len2);
            return new Vec2(v.X * scale, v.Y * scale);
        }

        // Rotate 90 degrees CCW
        public static Vec2 PerpCCW(this in Vec2 v)
            => new(-v.Y, v.X);

        // Rotate 90 degrees CW
        public static Vec2 PerpCW(this in Vec2 v)
            => new(v.Y, -v.X);
    }
}