using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX {
    public class UIUtils {
        public static Vector4 RED_COLOR = new( 0.85098039216f, 0.32549019608f, 0.30980392157f, 1.0f );
        public static Vector4 GREEN_COLOR = new( 0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f );

        public static bool EnumComboBox(string label, string[] options, ref int choiceIdx) {
            var ret = false;
            if (ImGui.BeginCombo(label, options[choiceIdx])) {
                for (var idx = 0; idx < options.Length; idx++) {
                    var is_selected = (choiceIdx == idx);
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

        public static bool DisabledButton(string label, bool enabled, bool small = false) {
            if(!enabled ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
            if( (small ? ImGui.SmallButton(label) : ImGui.Button(label)) && enabled ) return true;
            if( !enabled ) ImGui.PopStyleVar();
            return false;
        }

        public static bool RemoveButton(string label, bool small = false) {
            var ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, RED_COLOR );
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
            var data = new byte[] { ( byte )Color.X, (byte)Color.Y, (byte)Color.Z, (byte)Color.W };
            return AVFXLib.Main.Util.Bytes4ToInt( data );
        }

        public static Vector4 IntToColor(int Color ) {
            var colors = AVFXLib.Main.Util.IntTo4Bytes( Color );
            return new Vector4( colors[0], colors[1], colors[2], colors[3] );
        }
    }
}
