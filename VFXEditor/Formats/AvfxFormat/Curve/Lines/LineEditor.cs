using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.Utils;
using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Curve.Lines {
    public unsafe class LineEditor : AvfxItem {
        public readonly string Name;
        public readonly List<LineEditorGroup> Groups;

        public LineEditor( AvfxCurveData curve ) : base( curve.Name ) {
            Name = curve.Name;
            Groups = [new( curve )];
            Assigned = true;
        }

        public LineEditor( string name, List<LineEditorGroup> groups ) : base( name ) {
            Name = name;
            Groups = groups;
            Assigned = true;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( Name );

            if( Groups.Count == 1 ) {
                Groups[0].Draw();
                return;
            }

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var group in Groups ) {
                using var style = ImRaii.PushColor( ImGuiCol.Text, group.Assigned switch {
                    LinesAssigned.All => UiUtils.PARSED_GREEN,
                    LinesAssigned.Some => UiUtils.DALAMUD_YELLOW,
                    LinesAssigned.None => UiUtils.DALAMUD_RED,
                    _ => *ImGui.GetStyleColorVec4( ImGuiCol.Text )
                } );

                using var tabItem = ImRaii.TabItem( group.Name );
                if( !tabItem ) continue;
                style.Dispose();

                using var __ = ImRaii.PushId( group.Name );
                group.Draw();
            }
        }

        public override string GetDefaultText() => Name;

        public override void ReadContents( BinaryReader reader, int size ) { }

        public override void WriteContents( BinaryWriter writer ) { }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }
    }
}
