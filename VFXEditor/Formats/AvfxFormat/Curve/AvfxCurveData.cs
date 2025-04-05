using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor;
using VfxEditor.AvfxFormat;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VFXEditor.Formats.AvfxFormat.Curve {
    public enum CurveType {
        Color,
        Angle,
        Base
    }

    public class AvfxCurveData : AvfxCurveBase {
        public readonly CurveType Type;
        public readonly AvfxEnum<CurveBehavior> PreBehavior = new( "Pre Behavior", "BvPr" );
        public readonly AvfxEnum<CurveBehavior> PostBehavior = new( "Post Behavior", "BvPo" );
        public readonly AvfxEnum<RandomType> Random = new( "Random Type", "RanT" );
        public readonly AvfxCurveKeys KeyList;

        public List<AvfxCurveKey> Keys => KeyList.Keys;
        public bool IsColor => Type == CurveType.Color;

        private readonly List<AvfxBase> Parsed;

        public AvfxCurveData( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( name, avfxName, locked ) {
            Type = type;

            KeyList = new( this );

            Parsed = [
                PreBehavior,
                PostBehavior,
                Random,
                KeyList
            ];
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) {
            WriteLeaf( writer, "KeyC", 4, KeyList.Keys.Count );
            if( Type == CurveType.Color ) Random.SetAssigned( false );
            WriteNested( writer, Parsed );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() { } // should not be drawing this

        public void Sort( List<ICommand> commands ) {
            var sorted = new List<AvfxCurveKey>( Keys );
            sorted.Sort( ( x, y ) => x.Time.Value.CompareTo( y.Time.Value ) );
            commands.Add( new ListSetCommand<AvfxCurveKey>( Keys, sorted, Cleanup ) );
        }

        public double ToRadians( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( Math.PI / 180 ) * value;
        }

        public double ToDegrees( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( 180 / Math.PI ) * value;
        }

        public virtual void Cleanup() {
            KeyList.SetAssigned( true );
            if( !IsColor || Keys.Count < 2 ) return;
            if( !Keys.FindFirst( x => x.Time.Value != Keys[0].Time.Value, out var _ ) ) Keys[^1].Time.Value++;
        }

        public void GetDrawLine( out List<double> xs, out List<double> ys ) {
            xs = [];
            ys = [];

            if( Keys.Count > 0 ) {
                xs.Add( Keys[0].DisplayX );
                ys.Add( Keys[0].DisplayY );
            }

            for( var idx = 1; idx < Keys.Count; idx++ ) {
                var p1 = Keys[idx - 1];
                var p2 = Keys[idx];

                if( p1.Type.Value == KeyType.Linear || IsColor ) {
                    // p1 should already be added
                    xs.Add( p2.DisplayX );
                    ys.Add( p2.DisplayY );
                }
                else if( p1.Type.Value == KeyType.Step ) {
                    // p1 should already be added
                    xs.Add( p2.DisplayX );
                    ys.Add( p1.DisplayY );

                    xs.Add( p2.DisplayX );
                    ys.Add( p2.DisplayY );
                }
                else if( p1.Type.Value == KeyType.Spline ) {
                    var p1X = p1.DisplayX;
                    var p1Y = p1.DisplayY;
                    var p2X = p2.DisplayX;
                    var p2Y = p2.DisplayY;

                    var midX = ( p2X - p1X ) / 2;

                    var handle1X = p1X + p1.Converted.X * midX;
                    var handle1Y = p1Y;
                    var handle2X = p2X - p1.Converted.Y * midX;
                    var handle2Y = p2Y;

                    for( var i = 0; i < 100; i++ ) {
                        var t = i / 99.0d;

                        Bezier( p1X, p1Y, p2X, p2Y, handle1X, handle1Y, handle2X, handle2Y, t, out var bezX, out var bezY );
                        xs.Add( bezX );
                        ys.Add( bezY );
                    }
                    xs.Add( p2X );
                    ys.Add( p2Y );
                }
            }
        }

        private static void Bezier( double p1X, double p1Y, double p2X, double p2Y, double handle1X, double handle1Y, double handle2X, double handle2Y, double t, out double x, out double y ) {
            var u = 1 - t;
            var w1 = u * u * u;
            var w2 = 3 * u * u * t;
            var w3 = 3 * u * t * t;
            var w4 = t * t * t;

            x = w1 * p1X + w2 * handle1X + w3 * handle2X + w4 * p2X;
            y = w1 * p1Y + w2 * handle1Y + w3 * handle2Y + w4 * p2Y;
        }
    }
}
