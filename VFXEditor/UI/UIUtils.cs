using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX {
    public class UIUtils {

        public static bool EnumComboBox(string label, string[] options, ref int choiceIdx) {
            bool ret = false;
            if (ImGui.BeginCombo(label, options[choiceIdx])) {
                for (int idx = 0; idx < options.Length; idx++) {
                    bool is_selected = (choiceIdx == idx);
                    if(ImGui.Selectable(options[idx], is_selected)) {
                        choiceIdx = idx;
                        ret = true;
                    }

                    if (is_selected) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            return ret;
        }

        public static bool RemoveButton(string label, bool small = false) {
            bool ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4( 0.80f, 0.10f, 0.10f, 1.0f ) );
            if( small ) {
                if( ImGui.SmallButton( label ) ) {
                    ret = true;
                }
            }
            else {
                if( ImGui.Button( label ) ) {
                    ret = true;
                }
            }
            ImGui.PopStyleColor();
            return ret;
        }

        public static int ColorToInt(Vector4 Color ) {
            byte[] data = new byte[] { ( byte )Color.X, (byte)Color.Y, (byte)Color.Z, (byte)Color.W };
            return AVFXLib.Main.Util.Bytes4ToInt( data );
        }
        public static Vector4 IntToColor(int Color ) {
            byte[] colors = AVFXLib.Main.Util.IntTo4Bytes( Color );
            return new Vector4( colors[0], colors[1], colors[2], colors[3] );
        }
    }
}
