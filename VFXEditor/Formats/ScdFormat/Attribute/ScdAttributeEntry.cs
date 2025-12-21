using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public class ScdAttributeEntry : ScdEntry, IUiItem {
        public readonly ParsedByte Version = new( "Version" );
        public readonly ParsedReserve Reserved = new( 1 );
        public readonly ParsedShort AttributeId = new( "Attribute Id" );
        public readonly ParsedShort SearchAttributeId = new( "Search Attribute Id" );
        public readonly ParsedByte ConditionFirst = new( "First Condition" );
        public readonly ParsedByte ArgumentCount = new( "Argument Count" );
        public readonly ParsedInt SoundLabelLow = new( "Sound Label Low" );
        public readonly ParsedInt SoundLabelHigh = new( "Sound Label High" );

        public readonly AttributeResultCommand ResultFirst = new();
        public readonly AttributeExtendData Extend1 = new();
        public readonly AttributeExtendData Extend2 = new();
        public readonly AttributeExtendData Extend3 = new();
        public readonly AttributeExtendData Extend4 = new();

        private readonly List<ParsedBase> Parsed;

        public ScdAttributeEntry() {
            Parsed = [
                Version,
                Reserved,
                AttributeId,
                SearchAttributeId,
                ConditionFirst,
                ArgumentCount,
                SoundLabelLow,
                SoundLabelHigh,
            ];
        }

        public override void Read( BinaryReader reader ) {
            Parsed.ForEach( x => x.Read( reader ) );

            ResultFirst.Read( reader );
            Extend1.Read( reader );
            Extend2.Read( reader );
            Extend3.Read( reader );
            Extend4.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Parsed.ForEach( x => x.Write( writer ) );

            ResultFirst.Write( writer );
            Extend1.Write( writer );
            Extend2.Write( writer );
            Extend3.Write( writer );
            Extend4.Write( writer );
        }

        public void Draw() {
            Parsed.ForEach( x => x.Draw() );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            DrawResult();
            DrawExtendData( "Extend 1", Extend1 );
            DrawExtendData( "Extend 2", Extend2 );
            DrawExtendData( "Extend 3", Extend3 );
            DrawExtendData( "Extend 4", Extend4 );
        }

        private void DrawResult() {
            using var tabItem = ImRaii.TabItem( "Result" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Result" );
            ImGui.Separator();
            ResultFirst.Draw();
        }

        private static void DrawExtendData( string name, AttributeExtendData extendData ) {
            using var tabItem = ImRaii.TabItem( name );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( name );
            extendData.Draw();
        }
    }
}
