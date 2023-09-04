using Dalamud.Logging;
using HelixToolkit.SharpDX.Core.Animations;
using ImGuiNET;
using ImGuizmoNET;
using SharpDX;
using SharpDX.Direct3D11;
using VfxEditor.SklbFormat.Bones;
using Vec2 = System.Numerics.Vector2;

namespace VfxEditor.DirectX {
    public class BoneGizmoPreview : BoneNamePreview {
        private Vec2 GizmoSize = new();
        private Vec2 GizmoPos = new();

        private SklbBone Selected;
        private Bone Parent;
        private Matrix SelectedMatrix;

        private bool UsingGizmo = false;

        // TODO: only add command after a delay
        // TODO: ^ reset when selected changes
        // TODO: update when parent changes and changes happen manually

        private Matrix t1 = Matrix.Identity;
        private Matrix t2 = Matrix.Identity;

        public BoneGizmoPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) { }

        protected override void DrawInlineExtra() {
            if( Selected == null ) {
                UsingGizmo = false;
                return;
            }

            ImGuizmo.SetImGuiContext( ImGui.GetCurrentContext() );

            ImGuizmo.BeginFrame();
            ImGuizmo.SetRect( GizmoPos.X, GizmoPos.Y, GizmoSize.X, GizmoSize.Y );
            ImGuizmo.SetDrawlist();

            var view = Matrix.Multiply( Matrix.Scaling( 0.0002f ), ViewMatrix );
            var proj = ProjMatrix;
            var matrix = Matrix.Identity;
            var delta = Matrix.Identity;

            ImGuizmo.Manipulate( ref view.M11, ref proj.M11, OPERATION.ROTATE, MODE.LOCAL, ref t1.M11, ref t2.M11 );

            UsingGizmo = ImGuizmo.IsUsing();
            if( UsingGizmo ) PluginLog.Log( "using" );
        }

        public void UpdateSelected( SklbBone selected, Bone bone, Bone parent ) {
            if( selected == Selected ) return; // same one

            Selected = selected;
            if( Selected == null ) return;

            ImGuizmo.Enable( true );

            Parent = parent;
            var parentMatrix = Selected.Parent == null ? Matrix.Identity : parent.BindPose;
            SelectedMatrix = bone.BindPose;
        }

        public override void DrawInline() {
            GizmoPos = ImGui.GetCursorScreenPos();
            GizmoSize = ImGui.GetContentRegionAvail();

            base.DrawInline();
        }

        protected override bool CanDrag() => !UsingGizmo;
    }
}
