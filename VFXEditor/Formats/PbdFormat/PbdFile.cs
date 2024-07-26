using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdFile : FileManagerFile {
        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Files/PbdFile.cs
        // https://github.com/Ottermandias/Penumbra.GameData/blob/f5a74c70ad3861c5c66e1df6ae9a29fc7a0d736a/Data/RacialDeformer.cs#L7

        public readonly List<PbdDeformer> Deformers = [];
        public readonly List<PbdConnection> Connections = [];

        private PbdDeformer Selected;

        public PbdFile( BinaryReader reader, bool verify ) : base() {
            var count = reader.ReadInt32();
            // deformers and connections don't have matching orders for some reason. very cool, SE
            // so we have to track them separately for proper verification
            for( var i = 0; i < count; i++ ) {
                Deformers.Add( new( reader ) );
            }
            for( var i = 0; i < count; i++ ) {
                Connections.Add( new( Deformers, reader ) );
            }
            foreach( var connection in Connections ) connection.Populate( Connections );
        }

        public override void Write( BinaryWriter writer ) {
            // TODO: make sure to handle siblings for root nodes as well
        }

        public override void Draw() {
            ImGui.Separator();

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var table = ImRaii.Table( "Table", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoHostExtendY, new( -1, ImGui.GetContentRegionAvail().Y ) );
            if( !table ) return;
            style.Dispose();

            ImGui.TableSetupColumn( "##Left", ImGuiTableColumnFlags.WidthFixed, 200 );
            ImGui.TableSetupColumn( "##Right", ImGuiTableColumnFlags.WidthStretch );

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // TODO: controls

            using( var tree = ImRaii.Child( "Left" ) ) {
                using var indent = ImRaii.PushStyle( ImGuiStyleVar.IndentSpacing, 9 );
                foreach( var connection in Connections.Where( x => x.Parent == null ) ) DrawTree( connection );
            }

            ImGui.TableNextColumn();

            using var right = ImRaii.Child( "Right" );

            // TODO: delete

            Selected?.Draw();
        }

        private void DrawTree( PbdConnection connection ) {
            var deformer = connection.Item;
            var isLeaf = connection.Child == null;

            var flags =
                ImGuiTreeNodeFlags.DefaultOpen |
                ImGuiTreeNodeFlags.OpenOnArrow |
                ImGuiTreeNodeFlags.OpenOnDoubleClick |
                ImGuiTreeNodeFlags.SpanFullWidth;

            if( isLeaf ) flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            if( Selected == deformer ) flags |= ImGuiTreeNodeFlags.Selected;

            var nodeOpen = ImGui.TreeNodeEx( $"{deformer.SkeletonId.Value}", flags );
            if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) && !ImGui.IsItemToggledOpen() ) Selected = deformer;
            if( !isLeaf && nodeOpen ) {
                foreach( var child in Connections.Where( x => x.Parent == connection ) ) DrawTree( child );
                ImGui.TreePop();
            }
        }
    }
}
