using System.Collections.Generic;
using System.IO;
using VfxEditor.DirectX;
using VfxEditor.Formats.AvfxFormat.Curve;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public enum CurveType {
        Color,
        Angle,
        Base
    }

    public partial class AvfxCurve : AvfxCurveBase {
        public readonly int RenderId = Renderer.NewId;

        private static int EDITOR_ID = 0;
        private readonly CurveType Type;
        private readonly int Id = EDITOR_ID++;

        public readonly AvfxEnum<CurveBehavior> PreBehavior = new( "Pre Behavior", "BvPr" );
        public readonly AvfxEnum<CurveBehavior> PostBehavior = new( "Post Behavior", "BvPo" );
        public readonly AvfxEnum<RandomType> Random = new( "RandomType", "RanT" );
        public readonly AvfxCurveKeys KeyList;
        public List<AvfxCurveKey> Keys => KeyList.Keys;

        private readonly List<AvfxBase> Parsed;
        private readonly List<IUiItem> Display;

        public AvfxCurve( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( name, avfxName, locked ) {
            Type = type;

            KeyList = new( this );

            Parsed = [
                PreBehavior,
                PostBehavior,
                Random,
                KeyList
            ];

            Display = [
                PreBehavior,
                PostBehavior,
            ];
            if( type != CurveType.Color ) Display.Add( Random );
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

        protected override void DrawBody() {
            DrawItems( Display );
            DrawEditor();
        }
    }
}
