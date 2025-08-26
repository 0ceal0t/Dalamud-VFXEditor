using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok.Animation.Rig;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.SklbFormat.Mapping;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Structs.Animation;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Bones;

namespace VfxEditor.SklbFormat.Mapping {
    public unsafe class SklbMapping {
        public static string TempMappingHkx => Path.Combine( Plugin.Configuration.WriteLocation, "temp_hkx_mapping.hkx" ).Replace( '\\', '/' );

        public readonly ParsedString Name = new( "Name" );
        public readonly ParsedFloat4 Position = new( "Translation" );
        public readonly ParsedQuat Rotation = new( "Rotation" );
        public readonly ParsedFloat4 Scale = new( "Scale" );

        public readonly SklbBones Bones;
        public readonly SkeletonMapper* Mapper;

        public readonly List<SklbSimpleMapping> SimpleMappings = [];
        public readonly SklbSimpleMappingSplitView SimpleMappingView;

        public hkaSkeleton* MappedSkeleton => Mapper->Mapping.SkeletonA.ptr;
        public hkaSkeleton* ThisSkeleton => Mapper->Mapping.SkeletonB.ptr;

        public SklbMapping( SklbBones bones, SkeletonMapper* mapper, string name ) {
            Bones = bones;
            Mapper = mapper;

            Name.Value = name;

            var transform = mapper->Mapping.ExtractedMotionMapping;
            Position.Value = new( transform.Translation.X, transform.Translation.Y, transform.Translation.Z, transform.Translation.W );
            Rotation.Quaternion = new( transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z, transform.Rotation.W );
            Scale.Value = new( transform.Scale.X, transform.Scale.Y, transform.Scale.Z, transform.Scale.W );

            var data = Mapper->Mapping;
            var simpleMappings = data.SimpleMappings;
            for( var i = 0; i < simpleMappings.Length; i++ ) {
                SimpleMappings.Add( new( this, simpleMappings[i] ) );
            }

            SimpleMappingView = new( this, SimpleMappings );
        }

        public void Write( HashSet<nint> handles ) {
            var rotation = Rotation.Quaternion;

            Mapper->Mapping.ExtractedMotionMapping = new() {
                Translation = new() {
                    X = Position.Value.X,
                    Y = Position.Value.Y,
                    Z = Position.Value.Z,
                    W = Position.Value.W
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
                    W = Scale.Value.W
                }
            };

            var simpleMappings = SimpleMappings.Select( x => x.ToHavok() ).ToList();
            Mapper->Mapping.SimpleMappings = HavokData.CreateArray( handles, Mapper->Mapping.SimpleMappings, simpleMappings );
        }

        public void Draw( int idx ) {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawSimpleMappings( idx );
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var child = ImRaii.Child( "Child" );
            using var _ = ImRaii.PushId( "Parameters" );

            Name.Draw();
            Position.Draw();
            Rotation.Draw();
            Scale.Draw();
        }

        private void DrawSimpleMappings( int idx ) {
            using var tabItem = ImRaii.TabItem( "Simple Mappings" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "SimpleMappings" );

            if( ImGui.Button( "Replace Skeleton" ) ) {
                FileBrowserManager.OpenFileDialog( "Select a Skeleton", "Skeleton{.hkx,.sklb},.*", ( ok, res ) => {
                    if( !ok ) return;

                    var hkxPath = res;
                    if( res.EndsWith( ".sklb" ) ) {
                        SimpleSklb.LoadFromLocal( res ).SaveHavokData( TempMappingHkx );
                        hkxPath = TempMappingHkx;
                    }

                    var havokData = new HavokBones( hkxPath, true );
                    CommandManager.Add( new SklbMappingCommand( Mapper, havokData.Skeleton ) );
                } );
            }

            ImGui.SameLine();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.Button( "Generate" ) ) {
                    GenerateMapping( idx, Plugin.Configuration.SklbMappingUpdateExisting );
                }
                ImGui.SameLine();
                if( ImGui.Checkbox( "Update Existing", ref Plugin.Configuration.SklbMappingUpdateExisting ) ) Plugin.Configuration.Save();
            }

            ImGui.Separator();
            SimpleMappingView.Draw();
        }

        public void GenerateMapping( int idx, bool updateExisting ) {
            var commands = new List<ICommand>();

            var mappedBoneNames = new Dictionary<string, int>();
            for( var i = 0; i < MappedSkeleton->Bones.Length; i++ ) {
                mappedBoneNames[MappedSkeleton->Bones[i].Name.String] = i;
            }

            // Which of the skeleton bones indexes already have a mapping associated with them
            var alreadyMapped = new HashSet<int>();
            foreach( var simple in SimpleMappings ) {
                alreadyMapped.Add( simple.BoneB.Value );
            }

            var allMappings = new List<SklbSimpleMapping>();
            if( updateExisting ) allMappings.AddRange( SimpleMappings );

            foreach( var (bone, boneIdx) in Bones.Bones.WithIndex() ) {
                if( alreadyMapped.Contains( boneIdx ) ) continue;
                var name = bone.Name.Value;
                if( !mappedBoneNames.TryGetValue( name, out var mappedIdx ) ) continue;

                var newSimple = new SklbSimpleMapping( this );
                newSimple.BoneA.Value = mappedIdx;
                newSimple.BoneB.Value = boneIdx;

                allMappings.Add( newSimple );
                commands.Add( new ListAddCommand<SklbSimpleMapping>( SimpleMappings, newSimple ) );
            }

            foreach( var simple in allMappings ) {
                var mappedBone = MappedSkeleton->Bones[simple.BoneA.Value];
                var mappedPose = MappedSkeleton->ReferencePose[simple.BoneA.Value];
                var thisBone = Bones.Bones[simple.BoneB.Value];

                var _mappedPos = mappedPose.Translation;
                var _mappedRot = mappedPose.Rotation;
                var _mappedScl = mappedPose.Scale;

                var _thisPos = thisBone.Pos;
                var _thisRot = thisBone.Rot;
                var _thisScl = thisBone.Scl;

                var mappedPos = new Vector3( _mappedPos.X, _mappedPos.Y, _mappedPos.Z );
                var mappedRot = new Quaternion( _mappedRot.X, _mappedRot.Y, _mappedRot.Z, _mappedRot.W );
                var mappedScl = new Vector3( _mappedScl.X, _mappedScl.Y, _mappedScl.Z );

                var thisPos = new Vector3( _thisPos.X, _thisPos.Y, _thisPos.Z );
                var thisRot = _thisRot;
                var thisScl = new Vector3( _thisScl.X, _thisScl.Y, _thisScl.Z );

                var resultScl = mappedPos.Length() < 0.01f ? 1 : thisPos.Length() / mappedPos.Length();
                var resultPos = thisPos - ( mappedPos * resultScl );

                var resultRot = Quaternion.Multiply( thisRot, Quaternion.Inverse( mappedRot ) );
                if( resultRot.W < 0 ) resultRot *= -1f;
                if( idx % 2 == 0 ) resultRot = new( 0f, 0f, 0f, 1f );

                commands.Add( new ParsedSimpleCommand<Vector4>( simple.Translation, new( resultPos, 0 ) ) );
                commands.Add( new ParsedSimpleCommand<(Quaternion, Vector3)>( simple.Rotation, (resultRot, ParsedQuat.ToEuler( resultRot )) ) );
                commands.Add( new ParsedSimpleCommand<Vector4>( simple.Scale, new( resultScl ) ) );
            }

            CommandManager.Add( new CompoundCommand( commands ) );
        }
    }
}
