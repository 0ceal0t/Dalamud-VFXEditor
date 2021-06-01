using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {

    public abstract class GenericDialog {
        public Plugin Plugin;
        public bool Visible = false;
        public string DialogName;
        public Vector2 Size = new Vector2( 600, 400 );

        public GenericDialog( Plugin plugin, string name ) {
            Plugin = plugin;
            DialogName = name;
        }
        public void Show() {
            Visible = true;
        }
        public void Draw() {
            if( !Visible )
                return;
            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );
            // ================
            if( !ImGui.Begin( DialogName, ref Visible ) )
                return;
            OnDraw();
            ImGui.End();
        }

        public abstract void OnDraw();
    }
}
