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
        public readonly ParsedFloat3 Position = new( "Translation" );
        public readonly ParsedQuat Rotation = new( "Rotation" );
        public readonly ParsedFloat3 Scale = new( "Scale" );

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

            var transform = mapper->Mapping.ExtractedMotionMapping;
            Position.Value = new( transform.Translation.X, transform.Translation.Y, transform.Translation.Z );
            Rotation.Value = ParsedQuat.ToEuler( new( transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, transform.Rotation.W ) );
            Scale.Value = new( transform.Scale.X, transform.Scale.Y, transform.Scale.Z );

            var data = Mapper->Mapping;
            var simpleMappings = data.SimpleMappings;
            for( var i = 0; i < simpleMappings.Length; i++ ) {
                SimpleMappings.Add( new( this, simpleMappings[i] ) );
            }

            SimpleMappingView = new( this, SimpleMappings );
        }

        public void Write( HashSet<nint> handles ) {
            var rotation = Rotation.Quat;

            Mapper->Mapping.ExtractedMotionMapping = new() {
                Translation = new() {
                    X = Position.Value.X,
                    Y = Position.Value.Y,
                    Z = Position.Value.Z,
                },
                Rotation = new() {
                    X = rotation.X,
                    Y = rotation.Y,
                    Z = rotation.Z,
                    W = rotation.W
                },
                Scale = new() {
                    X = Scale.Value.X,
                    Y = Scale.Value.Y,
                    Z = Scale.Value.Z,
                }
            };

            var simpleMappings = SimpleMappings.Select( x => x.ToHavok() ).ToList();
            Mapper->Mapping.SimpleMappings = HavokData.CreateArray( handles, Mapper->Mapping.SimpleMappings, simpleMappings );
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawSimpleMappings();
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var child = ImRaii.Child( "Child" );
            using var _ = ImRaii.PushId( "Parameters" );

            Name.Draw( CommandManager.Sklb );
            Position.Draw( CommandManager.Sklb );
            Rotation.Draw( CommandManager.Sklb );
            Scale.Draw( CommandManager.Sklb );

            if( ImGui.Button( "Replace Mapped Skeleton" ) ) {
                FileDialogManager.OpenFileDialog( "Select a Skeleton", "Skeleton{.hkx,.sklb},.*", ( ok, res ) => {
                    if( !ok ) return;

                    var hkxPath = res;
                    if( res.EndsWith( ".sklb" ) ) {
                        SimpleSklb.LoadFromLocal( res ).SaveHavokData( TempMappingHkx );
                        hkxPath = TempMappingHkx;
                    }

                    var havokData = new HavokBones( hkxPath, true );
                    CommandManager.Sklb.Add( new SklbMappingCommand( Mapper, havokData.Skeleton ) );
                } );
            }
        }

        private void DrawSimpleMappings() {
            using var tabItem = ImRaii.TabItem( "Simple Mappings" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "SimpleMappings" );

            SimpleMappingView.Draw();
        }
    }
}
