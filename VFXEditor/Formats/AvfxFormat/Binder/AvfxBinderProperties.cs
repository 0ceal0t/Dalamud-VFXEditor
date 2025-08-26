using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Binder;
using VfxEditor.Ui.Interfaces;
using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinderProperties : AvfxOptional {
        public readonly string Name;

        public readonly AvfxEnum<BindPoint> BindPointType = new( "Bind Point Type", "BPT" );
        public readonly AvfxEnum<BindTargetPoint> BindTargetPointType = new( "Bind Target Point Type", "BPTP", value: BindTargetPoint.ByName );
        public readonly AvfxBinderPropertiesName BinderName = new();
        public readonly AvfxInt BindPointId = new( "Bind Point Id", "BPID", value: 3 );
        public readonly AvfxInt GenerateDelay = new( "Generate Delay", "GenD" );
        public readonly AvfxInt CoordUpdateFrame = new( "Coordinate Update Frame", "CoUF", value: -1 );
        public readonly AvfxBool RingEnable = new( "Ring Enabled", "bRng" );
        public readonly AvfxInt RingProgressTime = new( "Ring Progress Time", "RnPT", value: 1 );
        public readonly AvfxFloat RingPositionX = new( "Ring Position X", "RnPX" );
        public readonly AvfxFloat RingPositionY = new( "Ring Position Y", "RnPY" );
        public readonly AvfxFloat RingPositionZ = new( "Ring Position Z", "RnPZ" );
        public readonly AvfxFloat RingRadius = new( "Ring Radius", "RnRd" );
        public readonly AvfxInt BCT = new( "BCT", "BCT" );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos", locked: true );

        private readonly List<AvfxBase> Parsed;

        private readonly UiDisplayList Parameters;
        private readonly List<INamedUiItem> DisplayTabs;

        private static readonly Dictionary<int, string> BinderIds = new() {
            { 0, "(null)" },
            { 1, "Face (j_kao)" },
            { 3, "Base of weapon (relative)" },
            { 4, "Center of weapon" },
            { 5, "Tip of weapon (relative)" },
            { 6, "Right shoulder" },
            { 7, "Left shoulder" },
            { 8, "Right forearm" },
            { 9, "Left forearm" },
            { 10, "Right calves" },
            { 11, "Left calves" },
            { 16, "Front of character" },
            { 25, "Top of head" },
            { 26, "Middle of head" },
            { 27, "Front of head" },
            { 28, "Chest (j_sebo_c)" },
            { 29, "Center of character (n_root)" },
            { 30, "Center of character (n_hara)" },
            { 31, "Waist (j_kosi)" },
            { 32, "Right hand (j_te_r)" },
            { 33, "Left hand (j_te_l)" },
            { 34, "Right foot" },
            { 35, "Left foot" },
            { 42, "Above character (n_root)" },
            { 43, "Head (near right eye)" },
            { 44, "Head (near left eye)" },
            { 71, "Character origin (n_root)" },
            { 77, "Right hand (n_buki_r)" },
            { 78, "Left hand (n_buki_l)" },
            { 107, "n_throw" },
            { 108, "SGE: noulith 3, front / RPR avatar: neck" },
            { 109, "SGE: noulith 3, back / RPR avatar: spine" },
            { 110, "SGE: noulith 2, front / RPR avatar: left hand" },
            { 111, "SGE: noulith 2, back / RPR avatar: face" },
            { 112, "SGE: noulith 4, front / RPR avatar: origin" },
            { 113, "SGE: noulith 4, back" },
        };

        public AvfxBinderProperties( string name, string avfxName ) : base( avfxName ) {
            Name = name;

            Parsed = [
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
                BCT,
                Position
            ];
            BinderName.SetAssigned( false );
            Position.SetAssigned( true );

            DisplayTabs = [
                ( Parameters = new UiDisplayList( "Parameters" ) ),
                Position
            ];

            Parameters.AddRange( [
                BindPointType,
                BindTargetPointType,
                BinderName,
                new UiIntCombo( "Bind Point Id", BindPointId, BinderIds ),
                GenerateDelay,
                CoordUpdateFrame,
                RingEnable,
                RingProgressTime,
                new UiFloat3( "Ring Position", RingPositionX, RingPositionY, RingPositionZ ),
                RingRadius,
                BCT
            ] );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        public override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
        }

        public override void DrawBody() {
            DrawNamedItems( DisplayTabs );
        }

        public override string GetDefaultText() => Name;
    }
}
