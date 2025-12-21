using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.HalfFloat;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.KdbFormat.UnknownF {
    public class UnknownFItem : IUiItem {
        public readonly ParsedUInt Index = new( "##Index" );
        public readonly ParsedHalf X = new( "##X" );
        public readonly ParsedHalf Y = new( "##Y" );

        public UnknownFItem() { }

        public UnknownFItem( BinaryReader reader ) {
            Index.Read( reader );
            X.Read( reader );
            Y.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Index.Write( writer );
            X.Write( writer );
            Y.Write( writer );
        }

        public void Draw() {
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 100 );
            Index.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 100 );
            X.Draw();

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth( 100 );
            Y.Draw();
        }
    }
}
