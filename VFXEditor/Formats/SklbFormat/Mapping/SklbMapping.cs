using FFXIVClientStructs.Havok;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Formats.SklbFormat.Mapping;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Structs.Animation;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Bones;

namespace VfxEditor.SklbFormat.Mapping {
    public unsafe class SklbMapping {
        public static string TempMappingHkx => Path.Combine( Plugin.Configuration.WriteLocation, $"temp_hkx_mapping.hkx" ).Replace( '\\', '/' );

        public readonly ParsedString Name = new( "Name" );

        public readonly SklbBones Bones;
        public readonly SkeletonMapper* Mapper;

        public readonly List<SklbSimpleMapping> SimpleMappings = new();
        public readonly SklbSimpleMappingSplitView SimpleMappingView;

        public hkaSkeleton* ThisSkeleton => Mapper->Mapping.SkeletonA.ptr;
        public hkaSkeleton* MappedSkeleton => Mapper->Mapping.SkeletonB.ptr;

        public SklbMapping( SklbBones bones, SkeletonMapper* mapper, string name ) {
            Bones = bones;
            Mapper = mapper;
            Name.Value = name;

            var data = Mapper->Mapping;
            var simpleMappings = data.SimpleMappings;
            for( var i = 0; i < simpleMappings.Length; i++ ) {
                SimpleMappings.Add( new( this, simpleMappings[i] ) );
            }

            SimpleMappingView = new( this, SimpleMappings );
        }

        public void Write( HashSet<nint> handles ) {
            var data = Mapper->Mapping;

            var simpleMappings = SimpleMappings.Select( x => x.ToHavok() ).ToList();
            data.SimpleMappings = HavokData.CreateArray( handles, data.SimpleMappings, simpleMappings );
            Mapper->Mapping = data;
        }

        public void Draw() {
            Name.Draw( CommandManager.Sklb );
            if( ImGui.Button( "Replace Mapped Skeleton" ) ) {
                FileDialogManager.OpenFileDialog( "Select a Skeleton", "Skeleton{.hkx,.sklb},.*", ( ok, res ) => {
                    if( !ok ) return;

                    var hkxPath = res;
                    if( res.Contains( ".sklb" ) ) {
                        SimpleSklb.LoadFromLocal( res ).SaveHavokData( TempMappingHkx );
                        hkxPath = TempMappingHkx;
                    }

                    var havokData = new HavokBones( hkxPath, true );
                    CommandManager.Sklb.Add( new SklbMappingCommand( Mapper, havokData.Skeleton ) );
                } );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

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
