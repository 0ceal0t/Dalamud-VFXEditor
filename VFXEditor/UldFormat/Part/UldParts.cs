using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat.Part {
    public class UldParts : ISimpleUiBase {
        public readonly ParsedUInt Id = new( "Id" );
        public readonly List<UldPartItem> Parts = new();

        private int Offset => 12 + Parts.Count * 12;

        public UldParts() { }

        public UldParts( BinaryReader reader ) {
            Id.Read( reader );
            var partCount = reader.ReadInt32();
            reader.ReadInt32(); // skip offset

            for( var i = 0; i < partCount; i++ ) {
                Parts.Add( new UldPartItem( reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            writer.Write( Parts.Count );
            writer.Write( Offset );
            foreach( var part in Parts ) part.Write( writer );
        }

        public void Draw( string id ) {
            Id.Draw( id, CommandManager.Uld );

            for( var idx = 0; idx < Parts.Count; idx++ ) {
                var item = Parts[idx];
                if( ImGui.CollapsingHeader( $"Part #{idx}" ) ) {
                    ImGui.Indent();

                    if( UiUtils.RemoveButton( $"Delete{id}{idx}", true ) ) { // REMOVE
                        CommandManager.Uld.Add( new GenericRemoveCommand<UldPartItem>( Parts, item ) );
                        ImGui.Unindent(); break;
                    }

                    item.Draw( $"{id}{idx}" );
                    ImGui.Unindent();
                }
            }

            if( ImGui.Button( $"+ New{id}" ) ) { // NEW
                CommandManager.Uld.Add( new GenericAddCommand<UldPartItem>( Parts, new UldPartItem() ) );
            }
        }
    }
}
