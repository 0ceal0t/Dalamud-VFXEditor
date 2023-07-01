using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.CutbFormat.Resource {
    public class CutbResourceList : CutbHeader {
        public const string MAGIC = "CTRL";
        public override string Magic => MAGIC;

        public readonly List<CutbResource> Resources = new();

        private readonly SimpleSplitview<CutbResource> ResourceSplitView;

        public CutbResourceList( BinaryReader reader ) {
            reader.ReadUInt32(); // resources offset; is this always 0x18?
            var count = reader.ReadUInt32();

            ReadParsed( reader );

            for( var i = 0; i < count; i++ ) {
                Resources.Add( new( reader ) );
            }

            ResourceSplitView = new( "Resource", Resources, false,
                null, () => new(), () => CommandManager.Cutb );
        }

        public override void Draw() {
            DrawParsed( CommandManager.Cutb );

            ImGui.Separator();

            ResourceSplitView.Draw();
        }

        // Winter speculated that there are related to the total resource size for PS3 optimization
        protected override List<ParsedBase> GetParsed() => new() {
            new ParsedUInt( "Unknown Size 1" ),
            new ParsedUInt( "Unknown Size 2" ),
            new ParsedUInt( "Unknown Size 3" ),
            new ParsedUInt( "Unknown Size 4" ),
        };
    }
}
