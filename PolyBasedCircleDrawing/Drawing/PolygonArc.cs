using System;
using System.Collections.Immutable;
using System.Drawing;

namespace PolyBasedCircleDrawing.Drawing
{
    public readonly struct PolygonArc
    {
        public readonly ImmutableArray<PointF> Points;
        public readonly PointF Center;
        public readonly Single StartAngle;
        public readonly Single EndAngle;
        public readonly Single InnerRadius;
        public readonly Single OuterRadius;

        public PolygonArc ( PointF center, Single startAngle, Single endAngle, Single innerRadius, Single outerRadius, ImmutableArray<PointF> points )
        {
            this.Center = center;
            this.StartAngle = startAngle;
            this.EndAngle = endAngle;
            this.InnerRadius = innerRadius;
            this.OuterRadius = outerRadius;
            this.Points = points;
        }
    }
}
