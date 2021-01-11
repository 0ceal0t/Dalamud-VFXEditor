using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public abstract class UIBase
    {
        public bool Assigned = true;
        public List<UIBase> Attributes = new List<UIBase>();
        public int Idx;

        public bool HasHelpText = false;
        public string HelpText;

        public abstract void Draw(string parentId);
        public virtual void DrawBody( string parentId ) { }
        public virtual void DrawSelect(string parentId, ref UIBase selected  ) { }

        public void DrawAttrs(string parentId)
        {
            DrawList(Attributes, parentId);
        }

        public virtual void Init()
        {
            Assigned = true;
            Attributes = new List<UIBase>();
        }

        public void DrawList(List<UIBase> items, string parentId)
        {
            foreach(var item in items )
            {
                if( item.Assigned )
                {
                    item.Draw( parentId );
                }
            }
            foreach( var item in items )
            {
                if( !item.Assigned )
                {
                    item.Draw( parentId );
                }
            }
        }

        public void SetHelp(string helpText ) {
            if(helpText.Length > 0 ) {
                HelpText = helpText;
                HasHelpText = true;
            }
        }
        public void DrawHelp() {
            if( !HasHelpText ) return;
            DrawHelpToolTip( HelpText );
        }
        // generic utility method
        public static void DrawHelpToolTip(string text ) {
            ImGui.SameLine();
            ImGui.TextDisabled( "(?)" );
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.Text( text );
                ImGui.EndTooltip();
            }
        }
    }
}
