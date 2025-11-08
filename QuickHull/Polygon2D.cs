using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics; // INumber<T>, IFloatingPoint<T>

namespace MathPrimitives
{
    /// <summary>
    /// Ring buffer
    /// </summary>
    public class Ring<T> : IEnumerable<T>
    {
        private readonly List<T> _items;

        public Ring() => _items = new List<T>();
        public Ring(IEnumerable<T> items) => _items = items is null ? new() : new(items);

        public int Count => _items.Count;
        public bool IsEmpty => Count == 0;

        // Indexer with wrapping
        public T this[int index]
        {
            get
            {
                if (Count == 0) throw new IndexOutOfRangeException("Ring is empty");
                return _items[WrapIndex(index)];
            }
            set
            {
                if (Count == 0) throw new IndexOutOfRangeException("Ring is empty");
                _items[WrapIndex(index)] = value;
            }
        }

        public void Add(T item) => _items.Add(item);
        public void AddRange(IEnumerable<T> items) => _items.AddRange(items);
        public void InsertAfter(int index, T item)
        {
            if (Count == 0)
            {
                _items.Add(item);
                return;
            }

            int pos = WrapIndex(index) + 1;
            if (pos >= _items.Count)
                _items.Add(item);
            else
                _items.Insert(pos, item);
        }
        public void Clear() => _items.Clear();
        public bool Remove(T item) => _items.Remove(item);
        public void RemoveAt(int index)
        {
            if (Count == 0) return;
            _items.RemoveAt(WrapIndex(index));
        }

        public IEnumerable<T> Cyclic(int start = 0, int count = -1)
        {
            if (Count == 0) yield break;
            int n = Count;
            int total = count < 0 ? n : count;

            int i = WrapIndex(start);
            for (int k = 0; k < total; k++)
            {
                yield return _items[i];
                i++; if (i == n) i = 0;
            }
        }

        /// <summary>
        /// Edges as pairs (A,B)
        /// </summary>
        public IEnumerable<(T A, T B)> Edges()
        {
            int n = Count;
            if (n == 0) yield break;
            if (n == 1) { yield return (_items[0], _items[0]); yield break; }

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                yield return (_items[i], _items[j]);
            }
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected int WrapIndex(int index)
        {
            int n = Count;
            if (n == 0) throw new IndexOutOfRangeException("Ring is empty");
            int r = index % n;
            return r < 0 ? r + n : r;
        }

        public T[] ToArray() => _items.ToArray();
        public IReadOnlyList<T> AsReadOnly() => _items.AsReadOnly();
    }

    /// <summary>
    /// Polygon in 2D
    /// </summary>
    public class Polygon2D : Ring<Vec2>
    {
        public Polygon2D() : base() { }
        public Polygon2D(IEnumerable<Vec2> vertices) : base(vertices) { }

        public double SignedDoubleArea()
        {
            int n = Count;
            if (n == 0) return 0;
            double s = 0;

            foreach (var (a, b) in Edges())
                s += a.X * b.Y - a.Y * b.X;

            return s;
        }

        public double Area()
        {
            var half = double.CreateChecked(0.5);
            var s = SignedDoubleArea();
            return double.Abs(s) * half;
        }

        /// <summary>Периметр, требует плавающей точки (для sqrt).</summary>
        public double Perimeter()
        {
            double sum = 0;
            foreach (var (a, b) in Edges())
                sum += Vec2Math.Distance(a, b);
            return sum;
        }

        /// <summary>Ориентация по знаку удвоенной площади.</summary>
        public bool IsClockwise() => SignedDoubleArea() < 0;

        /// <summary>Axis-aligned bbox.</summary>
        public (Vec2 Min, Vec2 Max) BBox()
        {
            if (Count == 0) return (new Vec2(0, 0), new Vec2(0, 0));

            var minX = this[0].X; var maxX = this[0].X;
            var minY = this[0].Y; var maxY = this[0].Y;

            foreach (var v in this)
            {
                if (v.X < minX) minX = v.X;
                if (v.X > maxX) maxX = v.X;
                if (v.Y < minY) minY = v.Y;
                if (v.Y > maxY) maxY = v.Y;
            }
            return (new Vec2(minX, minY), new Vec2(maxX, maxY));
        }

        /// <summary>Тест "точка внутри" (чётно-нечётный луч), требует плавающей точки.</summary>
        public bool ContainsPoint(in Vec2 p)
        {
            bool inside = false;
            foreach (var (a, b) in Edges())
            {
                // (a.Y > p.Y) != (b.Y > p.Y) &&
                // p.X < (b.X - a.X) * (p.Y - a.Y) / (b.Y - a.Y) + a.X
                bool condY = (a.Y > p.Y) != (b.Y > p.Y);
                if (condY)
                {
                    var xIntersect = (b.X - a.X) * (p.Y - a.Y) / (b.Y - a.Y) + a.X;
                    if (p.X < xIntersect) inside = !inside;
                }
            }
            return inside;
        }
    }
}