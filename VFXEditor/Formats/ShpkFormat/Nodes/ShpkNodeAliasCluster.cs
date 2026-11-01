using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkNodeAliasCluster : IUiItem {
        private readonly ParsedUIntHex SubView2 = new( "Sub View 2" );
        private readonly ParsedUIntHex SubView1 = new( "Sub View 1" );
        private readonly ParsedUIntHex Unk141E = new( "Unknown" );

        private readonly List<ShpkNodeAliasSubCluster> SubClusters = [];
        private readonly CommandSplitView<ShpkNodeAliasSubCluster> ClusterView;

        public ShpkNodeAliasCluster() {
            ClusterView = new( "Sub Cluster", SubClusters, false, null, () => new() );
        }

        public ShpkNodeAliasCluster( BinaryReader reader ) : this() {
            SubView2.Read( reader );
            SubView1.Read( reader );
            var count = reader.ReadUInt32();
            Unk141E.Read( reader );

            for( var i = 0; i < count; i++ ) {
                SubClusters.Add( new( reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            SubView2.Write( writer );
            SubView1.Write( writer );
            writer.Write( SubClusters.Count );
            Unk141E.Write( writer );
            SubClusters.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            SubView2.Draw();
            SubView1.Draw();
            Unk141E.Draw();
            ImGui.Separator();
            ClusterView.Draw();
        }
    }
}
