using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxFloat : AvfxDrawable {
        public readonly string Name;

        private float Value = 0.0f;

        public AvfxFloat( string name, string avfxName ) : base( avfxName ) {
            Name = name;
        }

        public float GetValue() => Value;

        public void SetValue( float value ) {
            SetAssigned( true );
            Value = value;
        }

        public override void ReadContents( BinaryReader reader, int _ ) {
            Value = reader.ReadSingle();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Value );
        }

        public override void Draw( string id ) {
            // Unassigned
            if( DrawAddButton( this, Name, id ) ) return;

            var value = Value;
            if( ImGui.InputFloat( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new AvfxFloatCommand( this, value ) );
            }

            DrawRemoveContextMenu( this, Name, id );
        }
    }
}
