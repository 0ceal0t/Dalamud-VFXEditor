using FFXIVClientStructs.Havok;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Interop.Structs.Animation;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui.Components;

namespace VfxEditor.SklbFormat.Mapping {
    public unsafe class SklbMapping {
        public readonly SklbBones Bones;
        public readonly SkeletonMapper* Mapper;

        public readonly List<SklbSimpleMapping> SimpleMappings = new();
        public readonly SimpleSplitview<SklbSimpleMapping> SimpleMappingView;

        public hkaSkeleton* SkeletonA => Mapper->Mapping.SkeletonA.ptr;
        public hkaSkeleton* SkeletonB => Mapper->Mapping.SkeletonB.ptr;

        public SklbMapping( SklbBones bones, SkeletonMapper* mapper ) {
            Bones = bones;
            Mapper = mapper;

            var data = Mapper->Mapping;
            var simpleMappings = data.SimpleMappings;
            for( var i = 0; i < simpleMappings.Length; i++ ) {
                SimpleMappings.Add( new( this, simpleMappings[i] ) );
            }

            SimpleMappingView = new( "Mapping", SimpleMappings, false, ( SklbSimpleMapping item, int idx ) => item.GetText(),
                () => new( this ), () => CommandManager.Sklb );

            // PluginLog.Log( $"{data.ChainMappings.Length} {data.ChainMappingPartitionRanges.Length} | {data.KeepUnmappedLocal} {data.UnmappedBones.Length} | {data.PartitionMap.Length} | {data.Type.Value}" );
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
