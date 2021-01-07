using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;

namespace VFXEditor.UI.VFX
{
    public class UIIntList : UIBase
    {
        public string Id;
        public int Value;
        public LiteralInt Literal;

        public delegate void Change( LiteralInt literal );
        public Change ChangeFunction;

        public UIIntList( string id, LiteralInt literal, Change changeFunction = null )
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
            if( ImGui.InputInt( Id + id, ref Value ) )
            {
                Literal.GiveValue( Value );
                ChangeFunction( Literal );
            }
        }

        public static void DoNothing( LiteralInt literal ) { }
    }
}
