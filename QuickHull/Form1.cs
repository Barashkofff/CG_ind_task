using MathPrimitives;

namespace QuickHull
{
    public partial class QuickHullForm : Form
    {
        private Bitmap _curBitmap;
        private List<Vec2> _points = new();
        private Polygon2D _hullPoints = new();
        private bool _isDrawingHull = false;

        private Color _outerPoints = Color.DarkGreen;
        private Color _innerPoints = Color.DarkOrange;

        private const float VertexRadius = 5.0f;

        public QuickHullForm()
        {
            InitializeComponent();
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            ResetState();
            _points.Clear();

            pb.Invalidate();
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            _isDrawingHull = true;

            pb.Invalidate();
        }

        private void pb_Paint(object sender, PaintEventArgs e)
        {
            if (_curBitmap != null)
                _curBitmap.Dispose();

            _curBitmap = new Bitmap(pb.Width, pb.Height);

            using (Graphics g = Graphics.FromImage(_curBitmap))
            {
                g.Clear(Color.White);

                if (_isDrawingHull && _points.Count >= 3)
                {
                    QuickHull();
                    RasterizePolygon(_curBitmap, _hullPoints, Color.LightBlue);
                }

                for (int i = 0; i < _points.Count; i++)
                {
                    var point = _points[i];
                    DrawVertex(g, new Point((int)point.X, (int)point.Y), _outerPoints, i.ToString());
                }

                for (int i = 0; i < _hullPoints.Count; i++)
                {
                    var point = _hullPoints[i];
                    DrawVertex(g, new Point((int)point.X, (int)point.Y), _innerPoints, "");
                }
            }

            e.Graphics.DrawImageUnscaled(_curBitmap, 0, 0);
        }

        private void pb_MouseClick(object sender, MouseEventArgs e)
        {
            _points.Add(new Vec2(e.X, e.Y));
            ResetState();

            pb.Invalidate();
        }

        private void ResetState()
        {
            _isDrawingHull = false;
            _hullPoints.Clear();
        }

        #region Draw

        private void DrawVertex(Graphics g, Point p, Color color, string label)
        {
            const float r = VertexRadius;
            using (var b = new SolidBrush(color))
            using (var pen = new Pen(Color.Black, 1))
            using (var font = new Font("Segoe UI", 9f))
            {
                g.FillEllipse(b, p.X - r, p.Y - r, 2 * r, 2 * r);
                g.DrawEllipse(pen, p.X - r, p.Y - r, 2 * r, 2 * r);
                g.DrawString(label, font, Brushes.Black, p.X + 7, p.Y - 14);
            }
        }

        private void RasterizePolygon(Bitmap bmp, Polygon2D polygon, Color fillColor)
        {
            if (polygon == null || polygon.Count < 3)
                return;

            var (min, max) = polygon.BBox();
            int minX = (int)Math.Floor(min.X);
            int maxX = (int)Math.Ceiling(max.X);
            int minY = (int)Math.Floor(min.Y);
            int maxY = (int)Math.Ceiling(max.Y);

            using (var fast = new FastBitmap.FastBitmap(bmp))
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        Vec2 p = new Vec2(x + 0.5, y + 0.5); 

                        if (polygon.ContainsPoint(p))
                        {
                            fast[x, y] = fillColor;
                        }
                    }
                }
            }
        }

        #endregion

        #region QuickHull

        private void QuickHull()
        {
            FindInitialPoints(out int p0Ind, out int p1Ind);
            Vec2 p0 = _points[p0Ind];
            Vec2 p1 = _points[p1Ind];

            _hullPoints.Add(p0);
            _hullPoints.Add(p1);

            List<Vec2> s1 = new();
            List<Vec2> s2 = new();

            foreach (var point in _points) 
            {
                if (point == p0 || point == p1)
                    continue;

                Vec2 lineRightPerp = (p1 - p0).PerpCW();
                if ((point - p0).Dot(lineRightPerp) > 0)
                    s1.Add(point);
                else
                    s2.Add(point);
            }

            FindHull(s1, p0Ind, p1Ind);
            FindHull(s2, p1Ind, p0Ind);
        }

        private void FindInitialPoints(out int p0Ind, out int p1Ind)
        {
            int minXInd = 0;
            int maxXInd = 0;
            double minX = Double.MaxValue;
            double maxX = Double.MinValue;

            for (int i = 0; i < _points.Count; i++)
            {
                double x = _points[i].X;
                if (x < minX)
                {
                    minX = x;
                    minXInd = i;
                }

                if (x > maxX)
                {
                    maxX = x;
                    maxXInd = i;
                }
            }

            p0Ind = minXInd;
            p1Ind = maxXInd;
        }

        private void FindHull(List<Vec2> points, int p0Ind, int p1Ind)
        {
            if (points.Count == 0)
                return;

            Vec2 p0 = _points[p0Ind];
            Vec2 p1 = _points[p1Ind];
            Vec2 lineRightPerp = (p1 - p0).PerpCW();
            int farPointInd = 0;
            double maxDist = Double.MinValue;
            for (int i = 0; i < points.Count; i++)
            {
                var pt = points[i];
                if (pt == p0 || pt == p1)
                    continue;

                double curDist = (pt - p0).Dot(lineRightPerp);
                if (maxDist >= curDist)
                    continue;

                farPointInd = i;
                maxDist = curDist;
            }
            Vec2 farPoint = points[farPointInd];

            InsertBetween(_hullPoints, p0, p1, farPoint);

            List<Vec2> s1 = new();
            List<Vec2> s2 = new();

            foreach (var point in points)
            {
                if (point == p0 || point == p1)
                    throw new Exception("Invalid point");

                if (point == farPoint)
                    continue;

                Vec2 p0ToFarRightPerp = (farPoint - p0).PerpCW();
                Vec2 farToP1RightPerp = (p1 - farPoint).PerpCW();

                if ((point - farPoint).Dot(p0ToFarRightPerp) > 0)
                    s1.Add(point);

                if ((point - farPoint).Dot(farToP1RightPerp) > 0)
                    s2.Add(point);
            }

            int farGlobal = _points.FindIndex(v => v == farPoint);
            FindHull(s1, p0Ind, farGlobal);
            FindHull(s2, farGlobal, p1Ind);
        }

        public static void InsertBetween(Polygon2D polygon, Vec2 a, Vec2 b, Vec2 newPoint)
        {
            if (polygon == null)
                throw new ArgumentNullException(nameof(polygon));

            int n = polygon.Count;
            if (n < 2)
                throw new InvalidOperationException("Polygon must have at least two points.");

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                var p1 = polygon[i];
                var p2 = polygon[j];

                if ((p1 == a && p2 == b) || (p1 == b && p2 == a))
                {
                    polygon.InsertAfter(i, newPoint);
                    return;
                }
            }

            throw new InvalidOperationException("Given points are not adjacent in this polygon.");
        }

        #endregion
    }
}