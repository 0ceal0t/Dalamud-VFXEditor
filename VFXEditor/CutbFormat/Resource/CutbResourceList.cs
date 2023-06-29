using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.CutbFormat.Resource {
    public class CutbResourceList : CutbHeader {
        public const string MAGIC = "CTRL";
        public override string Magic => MAGIC;

        public readonly ParsedUInt Size1 = new( "Size 1" );
        public readonly ParsedUInt Size2 = new( "Size 2" );
        public readonly ParsedUInt Size3 = new( "Size 3" );
        public readonly ParsedUInt Size4 = new( "Size 4" );

        public readonly List<CutbResource> Resources = new();

        private readonly SimpleSplitview<CutbResource> ResourceSplitView;

        public CutbResourceList( BinaryReader reader ) {
            var offset = reader.ReadUInt32(); // is this always 0x18?
            var count = reader.ReadUInt32();

            Size1.Read( reader );
            Size2.Read( reader );
            Size3.Read( reader );
            Size4.Read( reader );

            for( var i = 0; i < count; i++ ) {
                Resources.Add( new( reader ) );
            }

            ResourceSplitView = new( "Resource", Resources, false,
                null, () => new(), () => CommandManager.Cutb );
        }

        public override void Draw() {
            Size1.Draw( CommandManager.Cutb );
            Size2.Draw( CommandManager.Cutb );
            Size3.Draw( CommandManager.Cutb );
            Size4.Draw( CommandManager.Cutb );

            ImGui.Separator();

            ResourceSplitView.Draw();
        }
    }
}
