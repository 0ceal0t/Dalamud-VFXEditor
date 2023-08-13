using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.Animation;
using VfxEditor.DirectX;
using VfxEditor.Utils;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.PhybFormat.Skeleton {
    public class SkeletonView {
        private readonly PhybFile File;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";

        private static PhybPreview PhybPreview => Plugin.DirectXManager.PhybPreview;

        private Dictionary<string, Bone> BoneMatrixes;
        private List<Bone> BoneList;

        public SkeletonView( PhybFile file, string sourcePath ) {
            File = file;
            var sklbPath = sourcePath.Replace( ".phyb", ".sklb" ).Replace( "phy_", "skl_" );
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;
        }

        public void Draw() {
            if( BoneMatrixes == null ) LoadSklbPath();
            else if( PhybPreview.CurrentFile != File ) UpdateSkeleton();

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
            var inputSize = UiUtils.GetOffsetInputSize( checkSize + 200 );
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##SklbPath", ref SklbPreviewPath, 255 );

            var imguiStyle = ImGui.GetStyle();
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vec2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) LoadSklbPath();

                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.FileUpload.ToIconString() ) ) {
                    FileDialogManager.OpenFileDialog( "Select a File", ".sklb,.*", ( ok, res ) => {
                        if( !ok ) return;
                        UpdateBones( SklbFile.LoadFromLocal( res ) );
                        UpdateSkeleton();
                        UpdatePhysicsObjects();
                    } );
                }
            }

            ImGui.SameLine();

            if( ImGui.Checkbox( "Bone Names", ref Plugin.Configuration.PhybShowBoneName ) ) Plugin.Configuration.Save();

            if( BoneMatrixes == null ) return;
            if( File.PhysicsUpdated ) UpdatePhysicsObjects();
            PhybPreview.DrawInline();
        }

        private void LoadSklbPath() {
            if( Plugin.DataManager.FileExists( SklbPreviewPath ) ) {
                UpdateBones( Plugin.DataManager.GetFile<SklbFile>( SklbPreviewPath ) );
                UpdateSkeleton();
                UpdatePhysicsObjects();
            }
            else {
                PluginLog.Error( $"File does not exist: {SklbPreviewPath}" );
            }
        }

        private unsafe void UpdateBones( SklbFile sklbFile ) {
            BoneMatrixes = new();
            BoneList = new();

            try {
                var tempPath = Path.Combine( Plugin.Configuration.WriteLocation, $"phyb_skl_temp.hkx" );
                sklbFile.SaveHavokData( tempPath.Replace( '\\', '/' ) );

                var path = Marshal.StringToHGlobalAnsi( tempPath );
                var loadOptions = stackalloc hkSerializeUtil.LoadOptions[1];
                loadOptions->TypeInfoRegistry = hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry();
                loadOptions->ClassNameRegistry = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry();
                loadOptions->Flags = new hkFlags<hkSerializeUtil.LoadOptionBits, int> {
                    Storage = ( int )hkSerializeUtil.LoadOptionBits.Default
                };

                var resource = hkSerializeUtil.LoadFromFile( ( byte* )path, null, loadOptions );

                if( resource == null ) {
                    PluginLog.Error( $"Could not read file: {SklbPreviewPath}" );
                }

                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    var a = ( hkRootLevelContainer* )resource->GetContentsPointer( n1, hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry() );
                    var animationName = @"hkaAnimationContainer"u8;
                    fixed( byte* n2 = animationName ) {
                        var anim = ( hkaAnimationContainer* )a->findObjectByName( n2, null );
                        var skeleton = anim->Skeletons[0].ptr;

                        BoneMatrixes = new();
                        BoneList = new();

                        var parents = new List<int>();
                        var refPoses = new List<Matrix>();
                        var bindPoses = new List<Matrix>();

                        for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                            var pose = skeleton->ReferencePose[i];
                            var pos = pose.Translation;
                            var rot = pose.Rotation;
                            var scl = pose.Scale;

                            var matrix = AnimationData.CleanMatrix( Matrix.AffineTransformation(
                                scl.X,
                                new Quaternion( rot.X, rot.Y, rot.Z, rot.W ),
                                new Vector3( pos.X, pos.Y, pos.Z )
                            ) );

                            parents.Add( skeleton->ParentIndices[i] );
                            refPoses.Add( matrix );
                            bindPoses.Add( Matrix.Identity );
                        }

                        for( var target = 0; target < skeleton->Bones.Length; target++ ) {
                            var current = target;
                            while( current >= 0 ) {
                                bindPoses[target] = Matrix.Multiply( bindPoses[target], refPoses[current] );
                                current = parents[current];
                            }
                        }

                        for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                            var name = skeleton->Bones[i].Name.String;
                            var bone = new Bone {
                                BindPose = bindPoses[i],
                                ParentIndex = skeleton->ParentIndices[i],
                                Name = name
                            };

                            BoneList.Add( bone );
                            BoneMatrixes[name] = bone;
                        }
                    }
                }

                ( ( hkReferencedObject* )resource )->RemoveReference();
            }
            catch( Exception e ) {
                PluginLog.Error( $"Could not read file: {SklbPreviewPath}", e );
            }
        }

        private void UpdatePhysicsObjects() {
            File.PhysicsUpdated = false;
            if( BoneList.Count == 0 ) return;

            MeshBuilders meshes = new() {
                Collision = new MeshBuilder( true, false ),
                Simulation = new MeshBuilder( true, false ),
                Spring = new MeshBuilder( true, false )
            };

            File.AddPhysicsObjects( meshes, BoneMatrixes );
            PhybPreview.LoadPhysics( meshes.Collision.ToMesh(), meshes.Simulation.ToMesh(), meshes.Spring.ToMesh() );
        }

        private void UpdateSkeleton() {
            if( BoneList?.Count == 0 ) PhybPreview.LoadEmpty( File );
            else PhybPreview.LoadSkeleton( File, BoneList, AnimationData.CreateSkeletonMesh( BoneList ) );
        }

        public void Dispose() {
            if( PhybPreview.CurrentFile == File ) PhybPreview.ClearFile();
        }
    }
}
