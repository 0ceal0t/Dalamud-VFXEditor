using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.CutbFormat.Scene {
    public enum FocusType {
        None = 0x00,
        Camera = 0x01,
        Actor = 0x02,
        Position = 0x03
    }

    public class CutbDefaultScene : CutbHeader {
        public const string MAGIC = "CTDS";
        public override string Magic => MAGIC;

        public readonly ParsedString LevelPath = new( "Level Path" );
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );

        public readonly List<CutbInstance> IgnoreList = new();
        private readonly SimpleSplitview<CutbInstance> IgnoreListSplitView;

        public CutbDefaultScene( BinaryReader reader ) {
            reader.ReadInt32(); // path offset, 0x54 + (0x08 * instance count)
            ReadParsed( reader );

            reader.ReadInt32(); // instance offset, 0x18
            var instanceCount = reader.ReadInt32();

            Unknown3.Read( reader ); // 0x02
            reader.ReadInt32(); // same as path offset
            reader.ReadInt32(); // 0
            reader.ReadInt32(); // 0

            for( var i = 0; i < instanceCount; i++ ) {
                IgnoreList.Add( new CutbInstance( reader ) );
            }

            LevelPath.Read( reader );

            IgnoreListSplitView = new( "Instance", IgnoreList, false,
                null, () => new(), () => CommandManager.Cutb );
        }

        public override void Draw() {
            LevelPath.Draw( CommandManager.Cutb );
            DrawParsed( CommandManager.Cutb );
            Unknown3.Draw( CommandManager.Cutb );

            ImGui.Separator();

            IgnoreListSplitView.Draw();
        }

        protected override List<ParsedBase> GetParsed() => new() {
            new ParsedShort( "Unknown Id 1" ),
            new ParsedShort( "Unknown Id 2" ),
            new ParsedEnum<FocusType>( "Focus Type" ),
            new ParsedInt( "Actor Instance Id" ),
            new ParsedFloat( "X" ),
            new ParsedFloat( "Y" ),
            new ParsedFloat( "Z" ),
            new ParsedFloat( "Size" ),
            new ParsedByteBool( "Use Weather" ),
            new ParsedByteBool( "Use Time" ),
            new ParsedByteBool( "Use Speed" ),
            new ParsedByteBool( "Use Angle" ),
            new ParsedByteBool( "Use Blend" ),
            new ParsedReserve( 3 ),
            new ParsedInt( "Weather Id" ),
            new ParsedInt( "Time" ),
            new ParsedFloat( "Speed" ),
            new ParsedInt( "Angle" ),
            new ParsedInt( "Blend" )
        };
    }
}
