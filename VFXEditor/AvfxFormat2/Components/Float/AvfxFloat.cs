using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxFloat : AvfxDrawable {
        public readonly ParsedFloat Parsed;

        public AvfxFloat( string name, string avfxName ) : base( avfxName ) {
            Parsed = new( name );
        }

        public float GetValue() => Parsed.Value;

        public void SetValue( float value ) {
            SetAssigned( true );
            Parsed.Value = value;
        }

        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader, _ );

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) => Parsed.Write( writer );

        public override void Draw( string id ) {
            // Unassigned
            if( DrawAddButton( this, Parsed.Name, id ) ) return;

            Parsed.Draw( id, CommandManager.Avfx );

            DrawRemoveContextMenu( this, Parsed.Name, id );
        }
    }
}
