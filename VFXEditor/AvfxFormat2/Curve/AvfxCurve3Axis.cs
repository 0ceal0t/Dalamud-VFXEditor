using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxCurve3Axis : AvfxAssignable {
        public readonly string Name;
        public readonly bool Locked;

        public readonly AvfxEnum<AxisConnect> AxisConnectType = new( "Axis Connect", "ACT" );
        public readonly AvfxEnum<RandomType> AxisConnectRandomType = new( "Axis Connect Random", "ACTR" );
        public readonly AvfxCurve X = new( "X", "X" );
        public readonly AvfxCurve Y = new( "Y", "Y" );
        public readonly AvfxCurve Z = new( "Z", "Z" );
        public readonly AvfxCurve RX = new( "Random X", "XR" );
        public readonly AvfxCurve RY = new( "Random Y", "YR" );
        public readonly AvfxCurve RZ = new( "Random Z", "ZR" );

        private readonly List<AvfxBase> Parsed;
        private readonly List<AvfxCurve> Curves;

        public AvfxCurve3Axis( string name, string avfxName, bool locked = false ) : base( avfxName ) {
            Name = name;
            Locked = locked;

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

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void DrawUnassigned( string parentId ) => DrawAddButtonRecurse( this, Name, parentId );

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked && DrawRemoveButton( this, Name, id ) ) return;

            AvfxCurve.DrawUnassignedCurves( Curves, id );

            AxisConnectType.Draw( id );
            AxisConnectRandomType.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            AvfxCurve.DrawAssignedCurves( Curves, id );
        }

        public override string GetDefaultText() => Name;
    }
}
