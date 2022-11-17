using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderProperties : AvfxOptional {
        public readonly string Name;

        public readonly AvfxEnum<BindPoint> BindPointType = new( "Bind Point Type", "BPT" );
        public readonly AvfxEnum<BindTargetPoint> BindTargetPointType = new( "Bind Target Point Type", "BPTP", defaultValue: BindTargetPoint.ByName );
        public readonly AvfxString BinderName = new( "Name", "Name", showRemoveButton: true );
        public readonly AvfxInt BindPointId = new( "Bind Point Id", "BPID", defaultValue: 3 );
        public readonly AvfxInt GenerateDelay = new( "Generate Delay", "GenD" );
        public readonly AvfxInt CoordUpdateFrame = new( "Coord Update Frame", "CoUF", defaultValue: -1 );
        public readonly AvfxBool RingEnable = new( "Ring Enabled", "bRng" );
        public readonly AvfxInt RingProgressTime = new( "Ring Progress Time", "RnPT", defaultValue: 1 );
        public readonly AvfxFloat RingPositionX = new( "Ring Position X", "RnPX" );
        public readonly AvfxFloat RingPositionY = new( "Ring Position Y", "RnPY" );
        public readonly AvfxFloat RingPositionZ = new( "Ring Position Z", "RnPZ" );
        public readonly AvfxFloat RingRadius = new( "Ring Radius", "RnRd" );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos", locked: true );

        private readonly List<AvfxBase> Parsed;

        private readonly UiDisplayList Display;
        private readonly List<IUiItem> DisplayTabs;

        private static readonly Dictionary<int, string> BinderIds = new() {
            { 0, "Not working" },
            { 1, "Head" },
            { 3, "Left hand weapon" },
            { 4, "Right hand weapon" },
            { 6, "Right shoulder" },
            { 7, "Left shoulder" },
            { 8, "Right forearm" },
            { 9, "Left forearm" },
            { 10, "Right calves" },
            { 11, "Left calves" },
            { 16, "Front of character" },
            { 25, "Head" },
            { 26, "Head" },
            { 27, "Head" },
            { 28, "Cervical" },
            { 29, "Center of the character" },
            { 30, "Center of the character" },
            { 31, "Center of the character" },
            { 32, "Right hand" },
            { 33, "Left hand" },
            { 34, "Right foot" },
            { 35, "Left foot" },
            { 42, "Above character?" },
            { 43, "Head (near right eye)" },
            { 44, "Head (near left eye )" },
            { 77, "Monsters weapon" },
        };

        public AvfxBinderProperties( string name, string avfxName ) : base( avfxName ) {
            Name = name;

            Parsed = new() {
                BindPointType,
                BindTargetPointType,
                BinderName,
                BindPointId,
                GenerateDelay,
                CoordUpdateFrame,
                RingEnable,
                RingProgressTime,
                RingPositionX,
                RingPositionY,
                RingPositionZ,
                RingRadius,
                Position
            };
            BinderName.SetAssigned( false );
            Position.SetAssigned( true );

            DisplayTabs = new() {
                ( Display = new UiDisplayList( "Parameters" ) ),
                Position
            };
            Display.Add( BindPointType );
            Display.Add( BindTargetPointType );
            Display.Add( BinderName );
            Display.Add( new UiIntCombo( "Bind Point Id", BindPointId, BinderIds) );
            Display.Add( GenerateDelay );
            Display.Add( CoordUpdateFrame );
            Display.Add( RingEnable );
            Display.Add( RingProgressTime );
            Display.Add( new UiFloat3( "Ring Position", RingPositionX, RingPositionY, RingPositionZ ) );
            Display.Add( RingRadius );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void DrawUnassigned( string parentId ) {
            AssignedCopyPaste( this, Name );
            DrawAddButtonRecurse( this, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;

            AssignedCopyPaste( this, Name );
            DrawRemoveButton( this, Name, id );

            IUiItem.DrawListTabs( DisplayTabs, id );
        }

        public override string GetDefaultText() => Name;
    }
}
