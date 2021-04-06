using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;
using ImGuizmoNET;
using VFXEditor.Structs.Vfx;

namespace VFXEditor.UI.Graphics {
    public class VFXManipulator {
        public Plugin _plugin;
        public bool Enabled = false;
        public bool CanBeEnabled => ( _plugin.MainUI.SpawnVfx != null );

        public VFXManipulator( Plugin plugin ) {
            _plugin = plugin;
        }

        private static float[] identityMatrix =
        {
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f
        };

        OPERATION _operation = OPERATION.TRANSLATE;
        MODE _mode = MODE.LOCAL;

        public void Draw() {
            if( !Enabled || !CanBeEnabled ) return;

            ImGui.SetNextWindowSize( new Vector2( 300, 170 ), ImGuiCond.FirstUseEver );
            if(ImGui.Begin( "VFX Manipulator", ref Enabled ) ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "This is a work-in-progress" );
                ImGui.Text( "Mode" );
                if( ImGui.RadioButton( "Local##mode", _mode == MODE.LOCAL ) ) {
                    _mode = MODE.LOCAL;
                }
                ImGui.SameLine();
                if( ImGui.RadioButton( "World##mode", _mode == MODE.WORLD ) ) {
                    _mode = MODE.WORLD;
                }

                ImGui.Text( "Operation" );
                if( ImGui.RadioButton( "Translate##operation", _operation == OPERATION.TRANSLATE ) ) {
                    _operation = OPERATION.TRANSLATE;
                }
                ImGui.SameLine();
                if( ImGui.RadioButton( "Rotate##operation", _operation == OPERATION.ROTATE ) ) {
                    _operation = OPERATION.ROTATE;
                }
                ImGui.SameLine();
                if( ImGui.RadioButton( "Scale##operation", _operation == OPERATION.SCALE ) ) {
                    _operation = OPERATION.SCALE;
                }

                ImGui.End();
            }

            var matrixSingleton = _plugin.ResourceLoader.GetMatrixSingleton();
            if( matrixSingleton == IntPtr.Zero ) return;
            float[] viewProjectionMatrix = new float[16];
            float width, height;
            unsafe {
                var rawMatrix = ( float* )( matrixSingleton + 0x1b4 ).ToPointer();
                for( var i = 0; i < 16; i++, rawMatrix++ )
                    viewProjectionMatrix[i] = *rawMatrix;
                width = *rawMatrix;
                height = *( rawMatrix + 1 );
            }

            ImGuizmo.Enable( true );
            ImGuizmo.BeginFrame();

            ImGuizmo.SetOrthographic( false );

            ImGuiIOPtr io = ImGui.GetIO();
            var vp = ImGui.GetMainViewport();
            ImGui.SetNextWindowSize( vp.Size );
            ImGui.SetNextWindowPos( new Vector2( 0, 0 ), ImGuiCond.Always );
            ImGui.SetNextWindowViewport( vp.ID );

            ImGui.Begin( "Gizmo", ref Enabled, ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoInputs );
            ImGui.BeginChild( "##gizmoChild", new Vector2( -1, -1 ), false, ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs |  ImGuiWindowFlags.NoInputs );

            ImGuizmo.SetDrawlist();
            float windowWidth = ImGui.GetWindowWidth();
            float windowHeight = ImGui.GetWindowHeight();
            ImGuizmo.SetRect( ImGui.GetWindowPos().X, ImGui.GetWindowPos().Y, windowWidth, windowHeight );

            if(_plugin.MainUI.SpawnVfx != null ) {
                if( ImGuizmo.Manipulate( ref viewProjectionMatrix[0], ref identityMatrix[0], _operation, _mode, ref _plugin.MainUI.SpawnVfx.matrix[0] ) ) {
                    _plugin.MainUI.SpawnVfx.PullMatrixUpdate();
                }
            }

            ImGui.EndChild();
            ImGui.End();
        }
    }
}
