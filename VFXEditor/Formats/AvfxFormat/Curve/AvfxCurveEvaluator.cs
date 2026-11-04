using System;
using System.Collections.Generic;
using static VfxEditor.Formats.AvfxFormat.Base.Enums;

namespace VfxEditor.Formats.AvfxFormat.Curve {
    /*
     * - The interpolation of the segment between key k and key k+1 is decided by the type of key k+1 (the key the segment ends on)
     * - Linear: straight line. Step: holds the value of key k, then jumps to key k+1 once its frame is reached
     * - Spline: cubic Hermite, where the tangent at each key is computed from its neighbors (Catmull-Rom style):
     *      tangent_j = ( Y_j * ( V_j - V_j-1 ) + X_j * ( V_j+1 - V_j ) ) / ( T_j+1 - T_j-1 )      [per frame]
     *   so X weights the outgoing slope and Y the incoming one. With X = Y = 1 this is a Catmull-Rom spline.
     *   On the first key only X is used ( X * slope of the first segment ), on the last key only Y is used
     * - X and Y are stored by the game as (sbyte)(int)(value * 15), so they only have a precision of 1/15
     * - Pre / post behavior: Const holds the first / last value, Repeat loops the curve, Add loops it while offsetting by (last - first) each loop
     */
    public static class AvfxCurveEvaluator {
        public readonly struct Key( double time, KeyType type, double value, float x, float y ) {
            public readonly double Time = time;
            public readonly KeyType Type = type;
            public readonly double Value = value;
            public readonly double OutWeight = Quantize( x );
            public readonly double InWeight = Quantize( y );
        }

        public static double Quantize( float weight ) => unchecked(( sbyte )( int )( weight * 15f )) / 15d;

        public static double Evaluate( IReadOnlyList<Key> keys, CurveBehavior pre, CurveBehavior post, double frame ) {
            if( keys.Count == 0 ) return 0;
            if( keys.Count == 1 ) return keys[0].Value;

            var first = keys[0];
            var last = keys[^1];
            var length = ( int )( last.Time - first.Time );

            if( frame < first.Time ) {
                if( length <= 0 ) return first.Value;
                var loops = ( ( int )( first.Time - frame ) / length ) + 1;
                return pre switch {
                    CurveBehavior.Repeat => EvaluateInRange( keys, frame + ( loops * length ) ),
                    CurveBehavior.Add => EvaluateInRange( keys, frame + ( loops * length ) ) - ( ( last.Value - first.Value ) * loops ),
                    CurveBehavior.None => last.Value, // the game indexes past the end of the "pre" table, which lands on "post Const"
                    _ => first.Value
                };
            }

            if( frame > last.Time ) {
                if( length <= 0 ) return last.Value;
                var loops = ( ( int )( frame - last.Time ) / length ) + 1;
                return post switch {
                    CurveBehavior.Repeat => EvaluateInRange( keys, frame - ( loops * length ) ),
                    CurveBehavior.Add => EvaluateInRange( keys, frame - ( loops * length ) ) + ( ( last.Value - first.Value ) * loops ),
                    _ => last.Value
                };
            }

            return EvaluateInRange( keys, frame );
        }

        public static double EvaluateInRange( IReadOnlyList<Key> keys, double frame ) => EvaluateSegment( keys, FindSegment( keys, frame ), frame );

        // Index of the key that starts the segment containing this frame
        public static int FindSegment( IReadOnlyList<Key> keys, double frame ) {
            var target = Math.Ceiling( frame );
            var low = 0;
            var high = keys.Count - 1;
            while( low <= high ) {
                var mid = ( low + high ) >> 1;
                if( keys[mid].Time < target ) low = mid + 1;
                else high = mid - 1;
            }
            return Math.Clamp( low - 1, 0, Math.Max( 0, keys.Count - 2 ) );
        }

        public static double EvaluateSegment( IReadOnlyList<Key> keys, int idx, double frame ) {
            var k0 = keys[idx];
            var k1 = keys[idx + 1];
            var duration = k1.Time - k0.Time;
            if( duration <= 0 ) return k1.Value;

            switch( k1.Type ) {
                case KeyType.Linear:
                    return k0.Value + ( ( frame - k0.Time ) / duration * ( k1.Value - k0.Value ) );
                case KeyType.Step:
                    return frame >= k1.Time ? k1.Value : k0.Value;
            }

            // Spline. Tangents are in units of the segment ( 0 -> 1 ), so neighbouring segments are scaled by duration / (span of the neighbors)
            var t = ( frame - k0.Time ) / duration;

            var tangent0 = k0.OutWeight * ( k1.Value - k0.Value );
            if( idx > 0 ) {
                var prev = keys[idx - 1];
                var span = k1.Time - prev.Time;
                tangent0 = span <= 0 ? 0 : ( ( k0.InWeight * ( k0.Value - prev.Value ) ) + tangent0 ) * duration / span;
            }

            var tangent1 = k1.InWeight * ( k1.Value - k0.Value );
            if( idx + 2 < keys.Count ) {
                var next = keys[idx + 2];
                var span = next.Time - k0.Time;
                tangent1 = span <= 0 ? 0 : ( tangent1 + ( k1.OutWeight * ( next.Value - k1.Value ) ) ) * duration / span;
            }

            var t2 = t * t;
            var t3 = t2 * t;
            return ( ( ( 2 * t3 ) - ( 3 * t2 ) + 1 ) * k0.Value ) +
                ( ( t3 - ( 2 * t2 ) + t ) * tangent0 ) +
                ( ( ( -2 * t3 ) + ( 3 * t2 ) ) * k1.Value ) +
                ( ( t3 - t2 ) * tangent1 );
        }
    }
}