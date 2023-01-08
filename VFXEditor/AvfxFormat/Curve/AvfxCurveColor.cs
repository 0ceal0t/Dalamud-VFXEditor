using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveColor : AvfxOptional {
        public readonly string Name;
        public readonly bool Locked;

        public readonly AvfxCurve RGB = new( "RGB", "RGB", type:CurveType.Color );
        public readonly AvfxCurve A = new( "A", "A" );
        public readonly AvfxCurve SclR = new( "Scale R", "SclR" );
        public readonly AvfxCurve SclG = new( "Scale G", "SclG" );
        public readonly AvfxCurve SclB = new( "Scale B", "SclB" );
        public readonly AvfxCurve SclA = new( "Scale A", "SclA" );
        public readonly AvfxCurve Bri = new( "Bright", "Bri" );
        public readonly AvfxCurve RanR = new( "Random R", "RanR" );
        public readonly AvfxCurve RanG = new( "Random G", "RanG" );
        public readonly AvfxCurve RanB = new( "Random B", "RanB" );
        public readonly AvfxCurve RanA = new( "Random A", "RanA" );
        public readonly AvfxCurve RBri = new( "Random Bright", "RBri" );

        private readonly List<AvfxCurve> Curves;

        public AvfxCurveColor( string name, string avfxName = "Col", bool locked = false ) : base( avfxName ) {
            Name = name;
            Locked = locked;

            Curves = new() {
                RGB,
                A,
                SclR,
                SclG,
                SclB,
                SclA,
                Bri,
                RanR,
                RanG,
                RanB,
                RanA,
                RBri
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Curves, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Curves, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Curves );

        public override void DrawUnassigned( string parentId ) {
            AssignedCopyPaste( this, Name );
            DrawAddButtonRecurse( this, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            AssignedCopyPaste( this, Name );
            if( !Locked && DrawRemoveButton( this, Name, id ) ) return;

            AvfxCurve.DrawUnassignedCurves( Curves, id );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            AvfxCurve.DrawAssignedCurves( Curves, id );
        }

        public override string GetDefaultText() => Name;
    }
}
