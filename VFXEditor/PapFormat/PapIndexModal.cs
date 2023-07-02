using ImGuiNET;
using VfxEditor.Interop;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapIndexModal : Modal {
        private readonly PapAnimation Animation;
        private readonly string ImportPath;
        private int Index;

        public PapIndexModal( PapAnimation animation, string importPath ) : base( "Animation Index" ) {
            Animation = animation;
            ImportPath = importPath;
        }

        protected override void DrawBody() {
            ImGui.PushTextWrapPos( 240 );
            ImGui.TextWrapped( "Select the index of the animation being imported" );
            ImGui.PopTextWrapPos();

            ImGui.InputInt( "##Index", ref Index );
        }

        protected override void OnCancel() { }

        protected override void OnOk() {
            Animation.File.Command.Add( new PapHavokFileCommand( Animation, Animation.HkxTempLocation, () => {
                HavokInterop.ReplaceHavokAnimation(
                    Animation.HkxTempLocation,
                    Animation.HavokIndex,
                    ImportPath,
                    Index,
                    Animation.HkxTempLocation
                );
            } ) );
            UiUtils.OkNotification( "Havok data replaced" );
        }
    }
}
