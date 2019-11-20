using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace PolyBasedCircleDrawing.Drawing
{
    public static class ArcUtilities
    {
        private const Single PI2 = ( Single ) ( Math.PI * 2d );
        private const Single PIAbove180 = ( Single ) Math.PI / 180f;
        private const Single PIUnder180 = 180f / ( Single ) Math.PI;

        private static Single Deg2Rad ( Single deg ) => PIAbove180 * deg;

        private static Single Rad2Deg ( Single rad ) => PIUnder180 * rad;

        /// <summary>
        /// Returns the incremental step in degrees
        /// </summary>
        /// <param name="r">The circle's radius</param>
        /// <param name="mlen">The maximum length in degrees</param>
        /// <returns></returns>
        private static Single GetStepInDeg ( Single r, Single mlen = 1f ) =>
            /*
             * In degrees
             * arc len = 2·π·r·(Θ/360)
             * arc len = mlen
             * ∴ mlen = 2·π·r·(Θ/360)
             * mlen/(2·π·r) = Θ/360
             * Θ = (360·mlen)/(2·π·r)
             */
            ( mlen * 360f ) / ( PI2 * r );

        /// <summary>
        /// Returns the incremental step in radians
        /// </summary>
        /// <param name="r">The circle's radius</param>
        /// <param name="mlen">The maximum length in radians</param>
        /// <returns></returns>
        private static Single GetStepInRad ( Single r, Single mlen = 0.01f ) =>
             /*
              * In radians
              * arc len = r·Θ
              * arc len = mlen
              * mlen = r·Θ
              * Θ = mlen / r
              */
             mlen / r;

        public static PointF[] GetCircularArcVertices ( Point center, Single startingAngle, Single finalAngle, Single innerRadius, Single outerRadius, Single maxDistance = 1f )
        {
            if ( 0f > startingAngle || startingAngle > 360f )
                throw new ArgumentOutOfRangeException ( nameof ( startingAngle ), "Angles must be values in the interval [0, 360]." );
            if ( 0f > finalAngle || finalAngle > 360f )
                throw new ArgumentOutOfRangeException ( nameof ( finalAngle ), "Angles must be values in the interval [0, 360]." );
            if ( finalAngle < startingAngle )
                throw new ArgumentOutOfRangeException ( "Starting angle should be lower than or equal to the final angle." );
            if ( innerRadius > outerRadius )
                throw new ArgumentOutOfRangeException ( "Inner radius should be smaller than outer radius." );

            startingAngle = Deg2Rad ( startingAngle );
            finalAngle = Deg2Rad ( finalAngle );
            maxDistance = Deg2Rad ( maxDistance );

            var arclen = finalAngle - startingAngle;
            var step = GetStepInRad ( outerRadius, maxDistance );
            var points = new PointF[( Int32 ) Math.Floor ( arclen / step ) *  2];
            var innerIdx = 0;
            var outerIdx = points.Length - 1;
            for ( var rad = startingAngle; rad <= finalAngle; rad += step )
            {
                // cos == x, sin == y
                var cos = ( Single ) Math.Cos ( rad );
                var sin = -( Single ) Math.Sin ( rad );

                points[innerIdx++] = new PointF (
                    center.X + cos * innerRadius,
                    center.Y + sin * innerRadius
                );
                points[outerIdx--] = new PointF (
                    center.X + cos * outerRadius,
                    center.Y + sin * outerRadius
                );
            }

            return points;
        }

        public static PointF[] GetCircularArcVerticesWithFixedStep ( Point center, Single startingAngle, Single finalAngle, Single innerRadius, Single outerRadius, Single step = 5f )
        {
            if ( 0f > startingAngle || startingAngle > 360f )
                throw new ArgumentOutOfRangeException ( nameof ( startingAngle ), "Angles must be values in the interval [0, 360]." );
            if ( 0f > finalAngle || finalAngle > 360f )
                throw new ArgumentOutOfRangeException ( nameof ( finalAngle ), "Angles must be values in the interval [0, 360]." );
            if ( finalAngle < startingAngle )
                throw new ArgumentOutOfRangeException ( "Starting angle should be lower than or equal to the final angle." );
            if ( innerRadius > outerRadius )
                throw new ArgumentOutOfRangeException ( "Inner radius should be smaller than outer radius." );

            startingAngle = Deg2Rad ( startingAngle );
            finalAngle = Deg2Rad ( finalAngle );
            step = Deg2Rad ( step );

            finalAngle = ( Single ) Math.Ceiling ( finalAngle / step ) * step;
            var arclen = finalAngle - startingAngle;
            var innerPoints = new List<PointF>();
            var outerPoints = new List<PointF>();
            for ( var rad = startingAngle; rad <= finalAngle; rad += step )
            {
                // cos == x, sin == y
                var cos = ( Single ) Math.Cos ( rad );
                var sin = -( Single ) Math.Sin ( rad );

                innerPoints.Add ( new PointF (
                    center.X + cos * innerRadius,
                    center.Y + sin * innerRadius
                ) );
                outerPoints.Add ( new PointF (
                    center.X + cos * outerRadius,
                    center.Y + sin * outerRadius
                ) );
            }

            outerPoints.Reverse ( );
            return innerPoints.Concat ( outerPoints ).ToArray ( );
        }
    }
}