using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Nodes {
    public class ShpkNode : IUiItem {
        public readonly ParsedUIntHex Selector = new( "Selector" );

        private readonly List<ParsedSByte> PassIndexes = [];
        private readonly List<ShpkPass> Passes = [];

        private readonly List<ShpkNodeKey> SystemKeys = [];
        private readonly List<ShpkNodeKey> SceneKeys = [];
        private readonly List<ShpkNodeKey> MaterialKeys = [];
        private readonly List<ShpkNodeKey> SubViewKeys = [];

        private readonly CommandSplitView<ShpkPass> PassView;

        private readonly CommandListView<ShpkNodeKey> SystemKeyView;
        private readonly CommandListView<ShpkNodeKey> SceneKeyView;
        private readonly CommandListView<ShpkNodeKey> MaterialKeyView;
        private readonly CommandListView<ShpkNodeKey> SubViewKeyView;

        public ShpkNode() {
            for( var i = 0; i < 16; i++ ) {
                PassIndexes.Add( new( $"##Pass {i}", -1 ) );
            }

            PassView = new( "Pass", Passes, false, null, () => new() );

            SystemKeyView = new( SystemKeys, () => new(), true );
            SceneKeyView = new( SceneKeys, () => new(), true );
            MaterialKeyView = new( MaterialKeys, () => new(), true );
            SubViewKeyView = new( SubViewKeys, () => new(), true );
        }

        public ShpkNode( BinaryReader reader, int systemKeyCount, int sceneKeyCount, int materialKeyCount, int subViewKeyCount ) : this() {
            Selector.Read( reader );

            var passCount = reader.ReadUInt32();

            foreach( var passIdx in PassIndexes ) {
                passIdx.Value = ( sbyte )reader.ReadByte();
            }

            for( var i = 0; i < systemKeyCount; i++ ) SystemKeys.Add( new( reader ) );
            for( var i = 0; i < sceneKeyCount; i++ ) SceneKeys.Add( new( reader ) );
            for( var i = 0; i < materialKeyCount; i++ ) MaterialKeys.Add( new( reader ) );
            for( var i = 0; i < subViewKeyCount; i++ ) SubViewKeys.Add( new( reader ) );

            for( var i = 0; i < passCount; i++ ) Passes.Add( new( reader ) );
        }

        public void Write( BinaryWriter writer ) {
            Selector.Write( writer );
            writer.Write( Passes.Count );
            PassIndexes.ForEach( x => x.Write( writer ) );
            SystemKeys.ForEach( x => x.Write( writer ) );
            SceneKeys.ForEach( x => x.Write( writer ) );
            MaterialKeys.ForEach( x => x.Write( writer ) );
            SubViewKeys.ForEach( x => x.Write( writer ) );
            Passes.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Node" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) DrawParameters();
            }

            using( var tab = ImRaii.TabItem( "Passes" ) ) {
                if( tab ) PassView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Keys" ) ) {
                if( tab ) DrawKeys();
            }
        }

        private void DrawParameters() {
            using var _ = ImRaii.PushId( "Parameters" );
            using var child = ImRaii.Child( "Child" );

            Selector.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.TextDisabled( "Pass Indexes:" );
            foreach( var passIdx in PassIndexes ) passIdx.Draw();
        }

        private void DrawKeys() {
            using var _ = ImRaii.PushId( "Keys" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "System" ) ) {
                if( tab ) SystemKeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Scene" ) ) {
                if( tab ) SceneKeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Material" ) ) {
                if( tab ) MaterialKeyView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Sub-View" ) ) {
                if( tab ) SubViewKeyView.Draw();
            }
        }
    }
}
