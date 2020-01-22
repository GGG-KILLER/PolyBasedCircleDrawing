using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace PolyBasedCircleDrawing.Drawing
{
    public static class ArcUtilities
    {
        private const Double PI2 = Math.PI * 2;
        private const Double PIAbove180 = Math.PI / 180;
        private const Double PIUnder180 = 180 / Math.PI;

        private static Double Deg2Rad ( Double deg ) => PIAbove180 * deg;

        private static Double Rad2Deg ( Double rad ) => PIUnder180 * rad;

        /// <summary>
        /// Returns the incremental step in degrees
        /// </summary>
        /// <param name="r">The circle's radius</param>
        /// <param name="mlen">The maximum length in degrees</param>
        /// <returns></returns>
        private static Double GetStepInDeg ( Double r, Double mlen = 1f ) =>
             /*
              * In degrees
              * arc len = 2·π·r·(Θ/360)
              * arc len = mlen
              * ∴ mlen = 2·π·r·(Θ/360)
              * mlen/(2·π·r) = Θ/360
              * Θ = (360·mlen)/(2·π·r)
              */
             mlen * 360 / ( PI2 * r );

        /// <summary>
        /// Returns the incremental step in radians
        /// </summary>
        /// <param name="r">The circle's radius</param>
        /// <param name="mlen">The maximum length in radians</param>
        /// <returns></returns>
        private static Double GetStepInRad ( Double r, Double mlen = 0.01f ) =>
             /*
              * In radians
              * arc len = r·Θ
              * arc len = mlen
              * mlen = r·Θ
              * Θ = mlen / r
              */
             mlen / r;

        public static PointF[] GetCircularArcVertices ( Point center, Double startingAngle, Double finalAngle, Double innerRadius, Double outerRadius, Double maxDistance = 1d )
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

            var outerStep = GetStepInRad ( outerRadius, maxDistance );
            var outerPoints = new List<PointF> ( );
            for ( var outerRad = startingAngle; outerRad <= finalAngle; outerRad += outerStep )
            {
                // cos == x, sin == y
                var cos = Math.Cos ( outerRad );
                var sin = -Math.Sin ( outerRad );

                outerPoints.Add ( new PointF (
                    ( Single ) ( center.X + cos * outerRadius ),
                    ( Single ) ( center.Y + sin * outerRadius )
                ) );
            }

            var innerStep = GetStepInRad ( innerRadius, maxDistance );
            var innerPoints = new List<PointF> ( );
            for ( var innerRad = startingAngle; innerRad <= finalAngle; innerRad += innerStep )
            {
                var cos = Math.Cos ( innerRad );
                var sin = -Math.Sin ( innerRad );

                innerPoints.Add ( new PointF (
                    ( Single ) ( center.X + cos * innerRadius ),
                    ( Single ) ( center.Y + sin * innerRadius )
                ) );
            }

            outerPoints.Reverse ( );
            return innerPoints.Concat ( outerPoints ).ToArray ( );
        }

        public static PointF[] GetCircularArcVerticesWithFixedStep ( Point center, Double startingAngle, Double finalAngle, Double innerRadius, Double outerRadius, Double step = 5f )
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

            finalAngle = Math.Ceiling ( finalAngle / step ) * step;
            var arclen = finalAngle - startingAngle;
            var innerPoints = new List<PointF> ( );
            var outerPoints = new List<PointF> ( );
            for ( var rad = startingAngle; rad <= finalAngle; rad += step )
            {
                // cos == x, sin == y
                var cos = Math.Cos ( rad );
                var sin = -Math.Sin ( rad );

                innerPoints.Add ( new PointF (
                    ( Single ) ( center.X + cos * innerRadius ),
                    ( Single ) ( center.Y + sin * innerRadius )
                ) );
                outerPoints.Add ( new PointF (
                    ( Single ) ( center.X + cos * outerRadius ),
                    ( Single ) ( center.Y + sin * outerRadius )
                ) );
            }

            outerPoints.Reverse ( );
            return innerPoints.Concat ( outerPoints ).ToArray ( );
        }
    }
}