using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxString : AvfxLiteral<ParsedString, string> {
        private readonly bool Pad;

        public AvfxString( string name, string avfxName, bool showUnassigned, bool pad, List<ParsedStringIcon> icons = null ) : base( avfxName, new( name, icons ?? new() ) ) {
            if( showUnassigned ) {
                Parsed.Icons.Add( new() {
                    Icon = () => FontAwesomeIcon.Trash,
                    Remove = true,
                    Action = ( string value ) => CommandManager.Avfx.Add( new AvfxAssignCommand( this, false ) )
                } );
            }
            Pad = pad;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Parsed.Read( reader );
            if( Pad ) FileUtils.PadTo( reader, 4 );
        }

        public override void WriteContents( BinaryWriter writer ) {
            Parsed.Write( writer );
            if( Pad ) FileUtils.PadTo( writer, 4 );
        }

        public override void Draw() => Draw( 0 );

        public void Draw( float offset ) {
            // Unassigned
            AssignedCopyPaste( this, Parsed.Name );
            if( DrawAddButton( this, Parsed.Name ) ) return;

            Parsed.DrawInput( CommandManager.Avfx, 255, Parsed.Name, offset, ImGuiInputTextFlags.None );

            DrawRemoveContextMenu( this, Parsed.Name );

            Parsed.DrawIcons();
        }
    }
}
