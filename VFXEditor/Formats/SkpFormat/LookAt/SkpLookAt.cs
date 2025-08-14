using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.SkpFormat.LookAt {
    public class SkpLookAt {
        private readonly List<SkpLookAtParam> Params = [];
        private readonly List<SkpLookAtGroup> Groups = [];

        private readonly CommandSplitView<SkpLookAtParam> ParamView;
        private readonly CommandSplitView<SkpLookAtGroup> GroupView;

        public SkpLookAt() {
            ParamView = new( "Parameter", Params, true, ( SkpLookAtParam param, int idx ) => param.GetText(), () => new() );
            GroupView = new( "Group", Groups, true, ( SkpLookAtGroup item, int idx ) => item.Id.Value, () => new() );
        }

        public void Read( BinaryReader reader ) {
            var numParams = reader.ReadByte();
            var numGroups = reader.ReadByte();
            reader.ReadBytes( numGroups ); // number of elements per group

            for( var i = 0; i < numParams; i++ ) Params.Add( new( reader ) );
            for( var i = 0; i < numGroups; i++ ) Groups.Add( new( reader ) );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( byte )Params.Count );
            writer.Write( ( byte )Groups.Count );
            foreach( var group in Groups ) writer.Write( ( byte )group.Elements.Count );

            Params.ForEach( x => x.Write( writer ) );
            Groups.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Setup Parameters" ) ) {
                if( tab ) ParamView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Groups" ) ) {
                if( tab ) GroupView.Draw();
            }
        }
    }
}
