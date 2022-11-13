using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderProperties : AvfxAssignable {
        public readonly string Name;

        public readonly AvfxEnum<BindPoint> BindPointType = new( "Bind Point Type", "BPT" );
        public readonly AvfxEnum<BindTargetPoint> BindTargetPointType = new( "Bind Target Point Type", "BPTP" );
        public readonly AvfxString BinderName = new( "Name", "Name", showRemoveButton: true );
        public readonly AvfxInt BindPointId = new( "Bind Point Id", "BPID" );
        public readonly AvfxInt GenerateDelay = new( "Generate Delay", "GenD" );
        public readonly AvfxInt CoordUpdateFrame = new( "Coord Update Frame", "CoUF" );
        public readonly AvfxBool RingEnable = new( "Ring Enabled", "bRng" );
        public readonly AvfxInt RingProgressTime = new( "Ring Progress Time", "RnPT" );
        public readonly AvfxFloat RingPositionX = new( "Ring Position X", "RnPX" );
        public readonly AvfxFloat RingPositionY = new( "Ring Position Y", "RnPY" );
        public readonly AvfxFloat RingPositionZ = new( "Ring Position Z", "RnPZ" );
        public readonly AvfxFloat RingRadius = new( "Ring Radius", "RnRd" );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos", locked: true );

        private readonly List<AvfxBase> Parsed;

        private readonly UiParameters Display;
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
            BindTargetPointType.SetValue( BindTargetPoint.ByName );
            BindPointId.SetValue( 3 );
            CoordUpdateFrame.SetValue( -1 );
            RingProgressTime.SetValue( 1 );
            Position.SetAssigned( true );

            DisplayTabs = new() {
                ( Display = new UiParameters( "Parameters" ) ),
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

        public override void DrawUnassigned( string parentId ) => DrawAddButtonRecurse( this, Name, parentId );

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRemoveButton( this, Name, id );

            IUiItem.DrawListTabs( DisplayTabs, id );
        }

        public override string GetDefaultText() => Name;
    }
}
