using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Assign;
using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxString : AvfxLiteral<ParsedString, string> {
        private readonly bool Pad;

        public AvfxString( string name, string avfxName, bool remove, bool pad, List<ParsedStringIcon> icons = null, bool forceLower = false ) : base( avfxName, new( name, icons, forceLower ) ) {
            if( remove ) {
                Parsed.Icons.Add( new() {
                    Icon = () => FontAwesomeIcon.Trash,
                    Remove = true,
                    Action = ( string value ) => CommandManager.Add( new AvfxAssignCommand( this, false ) )
                } );
            }
            Pad = pad;
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( size == 0 ) return;
            Parsed.Read( reader );
            if( Pad ) FileUtils.PadTo( reader, 4 );
        }

        public override void WriteContents( BinaryWriter writer ) {
            if( string.IsNullOrEmpty( Parsed.Value ) ) return;
            Parsed.Write( writer );
            if( Pad ) FileUtils.PadTo( writer, 4 );
        }

        public override void Draw() => Draw( 0 );

        public void Draw( float offset ) {
            // Unassigned
            AssignedCopyPaste( Parsed.Name );
            if( DrawAssignButton( Parsed.Name ) ) return;

            Parsed.DrawInput( 255, Parsed.Name, offset, ImGuiInputTextFlags.None );

            DrawUnassignPopup( Parsed.Name );

            Parsed.DrawIcons();
        }
    }
}
