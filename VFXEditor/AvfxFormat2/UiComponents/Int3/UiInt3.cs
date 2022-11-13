using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiInt3 : IUiBase {
        public readonly string Name;
        public readonly AvfxInt Literal1;
        public readonly AvfxInt Literal2;
        public readonly AvfxInt Literal3;
        private readonly List<AvfxBase> Literals = new();

        public UiInt3( string name, AvfxInt literal1, AvfxInt literal2, AvfxInt literal3 ) {
            Name = name;
            Literals.Add( Literal1 = literal1 );
            Literals.Add( Literal2 = literal2 );
            Literals.Add( Literal3 = literal3 );
        }

        public void Draw( string id ) {
            // Unassigned
            if( AvfxBase.DrawAddButton( Literals, Name, id ) ) return;

            var value = new Vector3( Literal1.GetValue(), Literal2.GetValue(), Literal3.GetValue() );
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new AvfxIntCommand( Literal1, ( int )value.X ) );
                command.Add( new AvfxIntCommand( Literal2, ( int )value.Y ) );
                command.Add( new AvfxIntCommand( Literal3, ( int )value.Z ) );
                CommandManager.Avfx.Add( command );
            }

            AvfxBase.DrawRemoveContextMenu( Literals, Name, id );
        }
    }
}
