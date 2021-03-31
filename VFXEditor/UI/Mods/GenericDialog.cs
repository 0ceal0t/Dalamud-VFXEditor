using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {

    public abstract class GenericDialog {
        public Plugin _plugin;
        public bool Visible = false;
        public string DialogName;

        public GenericDialog( Plugin plugin, string name ) {
            _plugin = plugin;
            DialogName = name;
        }
        public void Show() {
            Visible = true;
        }
        public bool DrawOnce = false;
        public void Draw() {
            if( !Visible )
                return;
            if( !DrawOnce ) {
                ImGui.SetNextWindowSize( new Vector2( 600, 400 ) );
                DrawOnce = true;
            }
            // ================
            var ret = ImGui.Begin( DialogName, ref Visible );
            if( !ret )
                return;
            OnDraw();
            ImGui.End();
        }

        public abstract void OnDraw();
    }
}
