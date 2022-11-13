using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiFloat2 : IUiBase {
        public readonly string Name;
        public readonly AvfxFloat Literal1;
        public readonly AvfxFloat Literal2;
        private readonly List<AvfxBase> Literals = new();

        public UiFloat2( string name, AvfxFloat literal1, AvfxFloat literal2 ) {
            Name = name;
            Literals.Add( Literal1 = literal1 );
            Literals.Add( Literal2 = literal2 );
        }

        public void Draw( string id ) {
            // Unassigned
            if( AvfxBase.DrawAddButton( Literals, Name, id ) ) return;

            var value = new Vector2( Literal1.GetValue(), Literal2.GetValue() );
            if( ImGui.InputFloat2( Name + id, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new AvfxFloatCommand( Literal1, value.X ) );
                command.Add( new AvfxFloatCommand( Literal2, value.Y ) );
                CommandManager.Avfx.Add( command );
            }

            AvfxBase.DrawRemoveContextMenu( Literals, Name, id );
        }
    }
}

