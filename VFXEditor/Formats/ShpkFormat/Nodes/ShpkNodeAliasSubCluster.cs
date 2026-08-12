using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkNodeAliasSubCluster : IUiItem {
        private const int DATA_CAPACITY = 97;

        private readonly ParsedShort OwnIndex = new( "Index" );
        private readonly List<ShpkAlias> Aliases = [];
        private readonly CommandSplitView<ShpkAlias> AliasView;
        private readonly List<uint> ExtraData = [];

        public ShpkNodeAliasSubCluster() {
            AliasView = new( "Alias", Aliases, false, null, () => new() );
        }

        public ShpkNodeAliasSubCluster( BinaryReader reader ) : this() {
            OwnIndex.Read( reader );
            var aliasCount = reader.ReadUInt16();

            for( var i = 0; i < aliasCount; i++ ) {
                Aliases.Add( new( reader ) );
            }
            for( var i = ( aliasCount * 2 ); i < DATA_CAPACITY; i++ ) {
                // TODO
                ExtraData.Add( reader.ReadUInt32() );
            }
        }

        public void Write( BinaryWriter writer ) {
            OwnIndex.Write( writer );
            writer.Write( (ushort) Aliases.Count );

            Aliases.ForEach( x => x.Write( writer ) );

            var padding = DATA_CAPACITY - ( Aliases.Count * 2 );
            for( var i = 0; i < padding; i++ ) {
                writer.Write( i < ExtraData.Count ? ExtraData[i] : 0u );
            }
        }

        public void Draw() {
            OwnIndex.Draw();
            ImGui.Separator();
            AliasView.Draw();
        }
    }
}
