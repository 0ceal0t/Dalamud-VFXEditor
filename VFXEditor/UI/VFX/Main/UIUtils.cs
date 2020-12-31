using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UIUtils
    {
        public static bool EnumComboBox(string label, string[] options, ref int choiceIdx)
        {
            bool ret = false;
            if (ImGui.BeginCombo(label, options[choiceIdx]))
            {
                for (int idx = 0; idx < options.Length; idx++)
                {
                    bool is_selected = (choiceIdx == idx);
                    if(ImGui.Selectable(options[idx], is_selected))
                    {
                        choiceIdx = idx;
                        ret = true;
                    }

                    if (is_selected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            return ret;
        }

        public static bool RemoveButton(string label)
        {
            bool ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0, 0, 1));
            if (ImGui.Button(label))
            {
                ret = true;
            }
            ImGui.PopStyleColor();
            return ret;
        }
    }
}
