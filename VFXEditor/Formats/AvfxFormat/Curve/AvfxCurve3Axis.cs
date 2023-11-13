using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurve3Axis : AvfxOptional {
        public readonly string Name;
        public readonly bool Locked;

        public readonly AvfxEnum<AxisConnect> AxisConnectType = new( "Axis Connect", "ACT" );
        public readonly AvfxEnum<RandomType> AxisConnectRandomType = new( "Axis Connect Random", "ACTR" );
        public readonly AvfxCurve X;
        public readonly AvfxCurve Y;
        public readonly AvfxCurve Z;
        public readonly AvfxCurve RX;
        public readonly AvfxCurve RY;
        public readonly AvfxCurve RZ;

        private readonly List<AvfxBase> Parsed;
        private readonly List<AvfxCurve> Curves;

        public AvfxCurve3Axis( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( avfxName ) {
            Name = name;
            Locked = locked;
            X = new( "X", "X", type );
            Y = new( "Y", "Y", type );
            Z = new( "Z", "Z", type );
            RX = new( "Random X", "XR", type );
            RY = new( "Random Y", "YR", type );
            RZ = new( "Random Z", "ZR", type );

            Parsed = new() {
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
            };

            Curves = new() {
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void DrawUnassigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( Name );
            DrawAssignButton( Name, true );
        }

        public override void DrawAssigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( Name );
            if( !Locked && DrawUnassignButton( Name ) ) return;

            AvfxCurve.DrawUnassignedCurves( Curves );

            AxisConnectType.Draw();
            AxisConnectRandomType.Draw();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            AvfxCurve.DrawAssignedCurves( Curves );
        }

        public override string GetDefaultText() => Name;
    }
}
