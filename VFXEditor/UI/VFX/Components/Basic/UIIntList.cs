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

        public delegate void Change( LiteralIntList literal );
        public Change ChangeFunction;

        public UIIntList( string id, LiteralIntList literal, Change changeFunction = null)
        {
            Id = id;
            Literal = literal;
            if( changeFunction != null )
                ChangeFunction = changeFunction;
            else
                ChangeFunction = DoNothing;
            // =====================
            Value = Literal.Value;
        }

        public override void Draw( string id )
        {
            int v0 = Value[0];
            if( ImGui.InputInt( Id + id, ref v0 ) )
            {
                Literal.Value[0] = v0;
                Value[0] = v0;
                ChangeFunction( Literal );
            }
        }

        public static void DoNothing( LiteralIntList literal ) { }
    }
}
