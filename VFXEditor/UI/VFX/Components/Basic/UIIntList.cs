using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using Dalamud.Plugin;

namespace VFXEditor.UI.VFX
{
    public class UIIntList : UIBase
    {
        public string Id;
        public List<int> Value;
        public LiteralIntList Literal;


        public UIIntList( string id, LiteralIntList literal)
        {
            Id = id;
            Literal = literal;
            // =====================
            Value = Literal.Value;
        }

        public override void Draw( string id )
        {
            var v0 = Value[0];
            if( ImGui.InputInt( Id + id, ref v0 ) )
            {
                Literal.Value[0] = v0;
                Value[0] = v0;
            }
        }
    }
}
