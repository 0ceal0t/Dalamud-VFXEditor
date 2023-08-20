using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui.Components;

namespace VfxEditor.SklbFormat.Mapping {
    public unsafe class SklbMapping {
        public readonly SklbBones Bones;
        public readonly SkeletonMapper* Mapper;

        public readonly List<SklbSimpleMapping> SimpleMappings = new();
        public readonly SimpleSplitview<SklbSimpleMapping> SimpleMappingView;
        // TODO: unmapped bones
        // TODO: chain mappings
        // TODO: partitions?
        // TODO: parsedBoneIndex class

        public SklbMapping( SklbBones bones, SkeletonMapper* mapper ) {
            Bones = bones;
            Mapper = mapper;

            var data = Mapper->Mapping;
            var simpleMappings = data.SimpleMappings;
            for( var i = 0; i < simpleMappings.Length; i++ ) {
                SimpleMappings.Add( new( simpleMappings[i] ) );
            }

            SimpleMappingView = new( "Mapping", SimpleMappings, false, null, () => new(), () => CommandManager.Sklb );
        }

        public void Write( List<nint> handles ) {
            var data = Mapper->Mapping;

            var simpleMappings = SimpleMappings.Select( x => x.ToHavok() ).ToList();
            data.SimpleMappings = SklbBones.CreateArray( data.SimpleMappings, simpleMappings, out var simpleHandle );
            handles.Add( simpleHandle );

            Mapper->Mapping = data;
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawSimpleMappings();
        }

        private void DrawSimpleMappings() {
            using var tabItem = ImRaii.TabItem( "Simple Mappings" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "SimpleMappings" );

            SimpleMappingView.Draw();
        }
    }
}
