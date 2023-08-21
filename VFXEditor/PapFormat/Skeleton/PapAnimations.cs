using FFXIVClientStructs.Havok;
using VfxEditor.Interop.Havok;

namespace VfxEditor.PapFormat.Skeleton {
    public unsafe class PapAnimations : HavokData {
        private readonly PapFile File;
        private readonly string SklbTempPath;
        private HavokBones Skeleton;

        private string SklbPreviewPath = "chara/human/c0101/skeleton/base/b0001/skl_c0101b0001.sklb";
        private hkaAnimatedSkeleton* AnimatedSkeleton;
        private hkaAnimationControl* AnimationControl;

        public PapAnimations( PapFile file, string havokPath ) : base( havokPath ) {
            File = file;
            SklbTempPath = Path.Replace( ".hkx", "_sklb.hkx" );

            var sklbPath = GetSklbPath();
            if( Plugin.DataManager.FileExists( sklbPath ) ) SklbPreviewPath = sklbPath;

            // TODO: load skeleton
        }

        protected override void OnLoad() {
            for( var i = 0; i < AnimationContainer->Animations.Length; i++ ) {
                var anim = AnimationContainer->Animations[i].ptr;
                var binding = AnimationContainer->Bindings[i].ptr;

                // PluginLog.Log( $"{binding->OriginalSkeletonName.String}" );
                var motion = anim->ExtractedMotion;
            }
        }

        private void UpdateAnimationControllers() {
            // TODO: need to clean up existing
            // TODO: also do this when replacing stuff
        }

        private void UpdateSkeleton() {
            // TODO: need to clean up everything else
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

            var format = modelType switch {
                SkeletonType.Monster => "chara/monster/m{0:D4}/skeleton/base/b{1:D4}/skl_m{0:D4}b{1:D4}.sklb",
                SkeletonType.DemiHuman => "chara/demihuman/d{0:D4}/skeleton/base/b{1:D4}/skl_d{0:D4}b{1:D4}.sklb",
                SkeletonType.Human => "chara/human/c{0:D4}/skeleton/base/b{1:D4}/skl_c{0:D4}b{1:D4}.sklb",
                SkeletonType.Weapon => "chara/weapon/w{0:D4}/skeleton/base/b{1:D4}/skl_w{0:D4}b{1:D4}.sklb",
                _ => ""
            };

            return string.Format( format, modelId, 1 ); // TODO: is this always 1?
        }

        private void UpdateData() {

        }
    }
}
