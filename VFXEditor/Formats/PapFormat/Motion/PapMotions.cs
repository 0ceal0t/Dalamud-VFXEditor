using FFXIVClientStructs.Havok.Animation.Rig;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileBrowser;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.Ui;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.PapFormat.Motion {
    public unsafe class PapMotions : HavokData {
        public readonly PapFile File;
        public readonly string SklbTempPath;
        private readonly List<PapMotion> Motions = [];
        public string AutoMatchPath { get; private set; }
        public SkeletonMatcher.MatchMetadata SkeletonMetadata { get; private set; }

        private bool _isAutoMatching = false;

        public HavokData Bones;
        public hkaSkeleton* Skeleton => Bones.AnimationContainer->Skeletons[0].ptr;

        private readonly SkeletonSelector Selector;

        public PapMotions( PapFile file, string havokPath, bool init ) : base( havokPath, init ) {
            File = file;
            SklbTempPath = Path.Replace( ".hkx", "_sklb.hkx" );

            // Initialize skeleton using best guess for sklb path
            Selector = new( GetSklbPath(), UpdateSkeleton );
        }

        public void UpdateMotions() {
            Motions.ForEach( x => x.Dispose() );
            Motions.Clear();

            for( var i = 0; i < AnimationContainer->Bindings.Length; i++ ) {
                Motions.Add( new( File, Bones, AnimationContainer->Bindings[i].ptr ) );
            }

            // Build skeleton metadata for the UI display
            BuildSkeletonMetadata();

            // After creating motions, check if the loaded skeleton actually fits the animation.
            // If not, search the user's Penumbra mod directory for a better-matching skeleton
            // (e.g. NFLB/YAS skeletons that have more bones than vanilla) and switch to it.
            if( !_isAutoMatching && Motions.Any( x => x.HasMismatchedSkeleton() ) ) {
                AutoMatchSkeleton();
            }
        }

        public void UpdateSkeleton( SimpleSklb sklbFile ) {
            Bones?.RemoveReference();
            sklbFile.SaveHavokData( SklbTempPath );
            Bones = new( SklbTempPath, true );

            UpdateMotions();
        }

        private void BuildSkeletonMetadata() {
            if( Bones?.AnimationContainer == null || Bones.AnimationContainer->Skeletons.Length == 0 ) {
                SkeletonMetadata = null;
                return;
            }

            var skel = Bones.AnimationContainer->Skeletons[0].ptr;
            var boneNames = new List<string>( skel->Bones.Length );
            for( var i = 0; i < skel->Bones.Length; i++ ) {
                boneNames.Add( skel->Bones[i].Name.String ?? "" );
            }

            var sklbPath = !string.IsNullOrEmpty( AutoMatchPath ) ? AutoMatchPath : ( Selector?.Path ?? GetSklbPath() );
            SkeletonMetadata = SkeletonMatcher.GetMetadata( sklbPath, boneNames, skel->Bones.Length, skel->FloatSlots.Length );
        }

        private void AutoMatchSkeleton() {
            _isAutoMatching = true;
            try {
                // Collect all bone/float indices the animations reference
                var allTransformIndices = new List<short>();
                var allFloatIndices = new List<short>();
                foreach( var motion in Motions ) {
                    for( var i = 0; i < motion.Binding->TransformTrackToBoneIndices.Length; i++ ) {
                        allTransformIndices.Add( motion.Binding->TransformTrackToBoneIndices[i] );
                    }
                    for( var i = 0; i < motion.Binding->FloatTrackToFloatSlotIndices.Length; i++ ) {
                        allFloatIndices.Add( motion.Binding->FloatTrackToFloatSlotIndices[i] );
                    }
                }

                // Get the current (vanilla) skeleton's bone names for compatibility scoring
                var currentBoneNames = new List<string>();
                if( Bones?.AnimationContainer != null && Bones.AnimationContainer->Skeletons.Length > 0 ) {
                    var skel = Bones.AnimationContainer->Skeletons[0].ptr;
                    for( var i = 0; i < skel->Bones.Length; i++ ) {
                        currentBoneNames.Add( skel->Bones[i].Name.String ?? "" );
                    }
                }

                var gameSklbPath = Selector?.Path ?? GetSklbPath();
                var matchPath = SkeletonMatcher.FindBestMatch( gameSklbPath, currentBoneNames, allTransformIndices, allFloatIndices );
                if( matchPath != null ) {
                    AutoMatchPath = matchPath;
                    Dalamud.Log( $"[PapMotions] Auto-matching skeleton to: {matchPath}" );
                    UpdateSkeleton( SimpleSklb.LoadFromLocal( matchPath ) );
                    return;
                }
            }
            catch( System.Exception e ) {
                Dalamud.Error( e, "[PapMotions] Failed to auto-match skeleton" );
            }
            finally {
                _isAutoMatching = false;
            }
        }

        private string GetSklbPath() {
            var modelType = File.ModelType.Value;
            var modelId = File.ModelId.Value;
            var sourcePath = File.SourcePath;

            if( !string.IsNullOrEmpty( sourcePath ) && sourcePath.Contains( "animation/f" ) ) {
                // chara/human/c1301/animation/f0002/nonresident/emot/airquotes.pap
                var split = sourcePath.Split( '/' );
                var charaType = split[2];
                var faceType = split[4];
                return $"chara/human/{charaType}/skeleton/face/{faceType}/skl_{charaType}{faceType}.sklb";
            }

            if( File.IsMaterial ) return "chara/common/animation/skl_material.sklb";

            var format = modelType switch {
                SkeletonType.Monster => "chara/monster/m{0:D4}/skeleton/base/b{1:D4}/skl_m{0:D4}b{1:D4}.sklb",
                SkeletonType.DemiHuman => "chara/demihuman/d{0:D4}/skeleton/base/b{1:D4}/skl_d{0:D4}b{1:D4}.sklb",
                SkeletonType.Human => "chara/human/c{0:D4}/skeleton/base/b{1:D4}/skl_c{0:D4}b{1:D4}.sklb",
                SkeletonType.Weapon => "chara/weapon/w{0:D4}/skeleton/base/b{1:D4}/skl_w{0:D4}b{1:D4}.sklb",
                _ => ""
            };

            return string.Format( format, modelId, 1 ); // TODO: is this always 1?
        }

        public void Draw( int havokIndex ) {
            if( File.IsMaterial ) {
                Selector.Init();
            }
            else {
                Selector.Draw();
            }
            DrawAutoMatchIndicator();
            Motions[havokIndex].DrawPreview( havokIndex );
        }

        public void DrawExportAll() {
            Selector.Init();
            if( ImGui.Button( "Export All Motions" ) ) {
                FileBrowserManager.SaveFileDialog( "Select a Save Location", ".gltf", "motion", "gltf", ( ok, res ) => {
                    if( !ok ) return;
                    GltfAnimation.ExportAnimation(
                        Skeleton,
                        [.. File.Animations.Select( x => x.GetName() )],
                        Motions,
                        true,
                        res
                    );
                } );
            }
        }

        public void DrawHavok( int havokIndex ) {
            Selector.Init();
            DrawAutoMatchIndicator();
            Motions[havokIndex].DrawHavok();
        }

        private void DrawAutoMatchIndicator() {
            if( SkeletonMetadata == null ) return;

            var isAutoMatched = !string.IsNullOrEmpty( AutoMatchPath );
            var typeColor = isAutoMatched ? UiUtils.GREEN_COLOR : new System.Numerics.Vector4( 0.6f, 0.6f, 0.6f, 1f );

            using( var color = ImRaii.PushColor( ImGuiCol.Text, typeColor ) ) {
                ImGui.TextWrapped( $"{SkeletonMetadata.SkeletonType}" );
            }
            ImGui.SameLine();
            using( var color = ImRaii.PushColor( ImGuiCol.Text, new System.Numerics.Vector4( 0.7f, 0.7f, 0.7f, 1f ) ) ) {
                var raceInfo = string.IsNullOrEmpty( SkeletonMetadata.RaceName )
                    ? SkeletonMetadata.RaceCode
                    : $"{SkeletonMetadata.RaceName} ({SkeletonMetadata.RaceCode})";
                ImGui.TextWrapped( $"| {raceInfo} | {SkeletonMetadata.BoneCount} bones" );

                if( !string.IsNullOrEmpty( SkeletonMetadata.ModName ) ) {
                    ImGui.SameLine();
                    ImGui.Text( "| " );
                    ImGui.SameLine();
                    using var modColor = ImRaii.PushColor( ImGuiCol.Text, UiUtils.GREEN_COLOR );
                    ImGui.Text( SkeletonMetadata.ModName );
                }
            }

            if( isAutoMatched ) {
                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() + "##RescanSkeletons" ) ) {
                        SkeletonMatcher.Rescan();
                        AutoMatchPath = null;
                        UpdateMotions();
                    }
                }
                if( ImGui.IsItemHovered() ) ImGui.SetTooltip( "Re-scan Penumbra directory for skeleton mods" );
            }
        }

        public void Write( HashSet<nint> handles ) {
            Selector.Init();
            Motions.ForEach( x => x.UpdateHavok( handles ) );

            // temporarily remove out-of-range track indices so the serialized animation cannot
            // write past the end of the skeleton's bone/float arrays when the game samples it
            Motions.ForEach( x => x.ClampTrackIndices( true ) );
            WriteHavok();
            Motions.ForEach( x => x.ClampTrackIndices( false ) );
        }

        public void Dispose() {
            Motions.ForEach( x => x.Dispose() );
        }
    }
}
