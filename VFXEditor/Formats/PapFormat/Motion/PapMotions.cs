using FFXIVClientStructs.Havok.Animation.Rig;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.FileBrowser;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.Ui;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.PapFormat.Motion {
    public unsafe class PapMotions : HavokData {
        public readonly PapFile File;
        public readonly string SklbTempPath;
        private readonly List<PapMotion> Motions = [];

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
        }

        public void UpdateSkeleton( SimpleSklb sklbFile ) {
            Bones?.RemoveReference();
            sklbFile.SaveHavokData( SklbTempPath );
            Bones = new( SklbTempPath, true );

            UpdateMotions();
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
            Motions[havokIndex].DrawPreview( havokIndex );
        }

        public void DrawExportAll() {
            Selector.Init();
            if( ImGui.Button( "Export All Motions" ) ) {
                FileBrowserManager.SaveFileDialog( "Select a Save Location", ".gltf", "motion", "gltf", ( bool ok, string res ) => {
                    if( !ok ) return;
                    GltfAnimation.ExportAnimation(
                        Skeleton,
                        File.Animations.Select( x => x.GetName() ).ToList(),
                        Motions,
                        true,
                        res
                    );
                } );
            }
        }

        public void DrawHavok( int havokIndex ) {
            Selector.Init();
            Motions[havokIndex].DrawHavok();
        }

        public void Write( HashSet<nint> handles ) {
            Selector.Init();
            Motions.ForEach( x => x.UpdateHavok( handles ) );
            WriteHavok();
        }

        public void Dispose() {
            Motions.ForEach( x => x.Dispose() );
        }
    }
}
