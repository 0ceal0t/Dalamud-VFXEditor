using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.PapFormat {
    public unsafe class PapGltfExportModal : Modal {
        private readonly PapMotion Motion;
        private readonly string AnimationName;
        private readonly string ExportPath;

        private bool SkipUnanimated = true;

        public PapGltfExportModal( PapMotion motion, string animationName, string exportPath ) : base( "Animation Export" ) {
            Motion = motion;
            AnimationName = animationName;
            ExportPath = exportPath;
        }

        protected override void DrawBody() {
            ImGui.Checkbox( "Skip Unanimated Bones", ref SkipUnanimated );
        }

        protected override void OnCancel() { }

        protected override void OnOk() {
            GltfAnimation.ExportAnimation(
                Motion.File.MotionData.Skeleton,
                new List<string>( new[] { AnimationName } ),
                new List<PapMotion>( new[] { Motion } ),
                SkipUnanimated,
                ExportPath
            );
            UiUtils.OkNotification( "Havok data exported" );
        }
    }
}
