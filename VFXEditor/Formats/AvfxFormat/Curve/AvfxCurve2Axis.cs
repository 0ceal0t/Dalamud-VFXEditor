using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurve2Axis : AvfxCurveBase {
        public readonly AvfxEnum<AxisConnect> AxisConnectType = new( "Axis Connect", "ACT" );
        public readonly AvfxEnum<RandomType> AxisConnectRandomType = new( "Axis Connect Random", "ACTR" );
        public readonly AvfxCurve X;
        public readonly AvfxCurve Y;
        public readonly AvfxCurve RX;
        public readonly AvfxCurve RY;

        private readonly List<AvfxBase> Parsed;
        private readonly List<AvfxCurveBase> Curves;

        public AvfxCurve2Axis( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( name, avfxName, locked ) {
            X = new( "X", "X", type );
            Y = new( "Y", "Y", type );
            RX = new( "Random X", "XR", type );
            RY = new( "Random Y", "YR", type );

            Parsed = [
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                RX,
                RY
            ];

            Curves = [
                X,
                Y,
                RX,
                RY
            ];
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        protected override void DrawBody() {
            DrawUnassignedCurves( Curves );
            AxisConnectType.Draw();
            AxisConnectRandomType.Draw();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawAssignedCurves( Curves );
        }
    }
}
