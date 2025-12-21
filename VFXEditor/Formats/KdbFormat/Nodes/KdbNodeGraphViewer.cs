using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Effector;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Source;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Target;
using VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint;
using VfxEditor.Ui.NodeGraphViewer;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbNodeGraphViewer : NodeGraphViewer<KdbNode, KdbSlot> {
        public KdbNodeGraphViewer() { }

        protected override void DrawUtilsBar() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                    ImGui.OpenPopup( "NewPopup" );
                }
            }

            using( var popup = ImRaii.Popup( "NewPopup" ) ) {
                if( popup ) {
                    if( ImGui.Selectable( "EffectorExpr" ) ) AddToCanvas( new KdbNodeEffectorExpr(), true );
                    if( ImGui.Selectable( "EffectorEZParamLink" ) ) AddToCanvas( new KdbNodeEffectorEZParamLink(), true );
                    if( ImGui.Selectable( "EffectorEZParamLinkLinear" ) ) AddToCanvas( new KdbNodeEffectorEZParamLinkLinear(), true );
                    if( ImGui.Selectable( "SourceRotate" ) ) AddToCanvas( new KdbNodeSourceRotate(), true );
                    if( ImGui.Selectable( "SourceTranslate" ) ) AddToCanvas( new KdbNodeSourceTranslate(), true );
                    if( ImGui.Selectable( "TargetBendSTRoll" ) ) AddToCanvas( new KdbNodeTargetBendSTRoll(), true );
                    if( ImGui.Selectable( "TargetBendRoll" ) ) AddToCanvas( new KdbNodeTargetBendRoll(), true );
                    if( ImGui.Selectable( "TargetTranslate" ) ) AddToCanvas( new KdbNodeTargetTranslate(), true );
                    if( ImGui.Selectable( "TargetScale" ) ) AddToCanvas( new KdbNodeTargetScale(), true );
                    if( ImGui.Selectable( "TargetRotate" ) ) AddToCanvas( new KdbNodeTargetRotate(), true );
                    if( ImGui.Selectable( "TargetPosConstraint" ) ) AddToCanvas( new KdbNodeTargetPosConstraint(), true );
                    if( ImGui.Selectable( "TargetOrientationConstraint" ) ) AddToCanvas( new KdbNodeTargetOrientationConstraint(), true );
                }
            }

            ImGui.SameLine();
            // ===================
            base.DrawUtilsBar();
        }
    }
}
