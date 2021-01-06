using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIHelp : UIBase
    {
        public string Text;
        public UIHelp(string text )
        {
            Text = text;
        }

        public override void Draw( string parentId )
        {
            HelpText( Text );
        }

        public static void HelpText( string text )
        {
            ImGui.SameLine();
            ImGui.SameLine();
            ImGui.TextDisabled( "(?)" );
            if( ImGui.IsItemHovered() )
            {
                ImGui.BeginTooltip();
                ImGui.Text( text );
                ImGui.EndTooltip();
            }
        }
    }
}
