using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurve3Axis : AvfxCurveBase {
        public readonly AvfxEnum<AxisConnect3> AxisConnectType = new( "Axis Connect", "ACT" );
        public readonly AvfxEnum<AxisConnect3> AxisConnectRandomType = new( "Axis Connect Random", "ACTR" );
        public readonly AvfxCurve X;
        public readonly AvfxCurve Y;
        public readonly AvfxCurve Z;
        public readonly AvfxCurve RX;
        public readonly AvfxCurve RY;
        public readonly AvfxCurve RZ;

        private readonly List<AvfxBase> Parsed;
        private readonly List<AvfxCurveBase> Curves;

        public AvfxCurve3Axis( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( name, avfxName, locked ) {
            X = new( "X", "X", type );
            Y = new( "Y", "Y", type );
            Z = new( "Z", "Z", type );
            RX = new( "Random X", "XR", type );
            RY = new( "Random Y", "YR", type );
            RZ = new( "Random Z", "ZR", type );

            Parsed = [
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
            ];

            Curves = [
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
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
