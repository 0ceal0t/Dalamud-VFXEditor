using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidFile : FileManagerFile {
        public readonly List<EidBindPoint> BindPoints = new();
        public readonly EidBindPointDropdown Dropdown;

        public EidFile( BinaryReader reader, bool checkOriginal = true ) : base( new CommandManager( Plugin.EidManager.GetCopyManager() ) ) {
            Dropdown = new( BindPoints );

            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            reader.ReadInt32(); // magic 00656964
            reader.ReadInt32(); // version 31303132
            var count = reader.ReadInt32();
            reader.ReadInt32(); // padding

            for( var i = 0; i < count; i++ ) {
                BindPoints.Add( new EidBindPoint( reader ) );
            }

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( 0x00656964 );
            writer.Write( 0x31303132 );
            writer.Write( BindPoints.Count );
            writer.Write( 0 );

            foreach( var bindPoint in BindPoints ) bindPoint.Write( writer );
        }

        public override void Draw( string id ) {
            ImGui.Separator();
            Dropdown.Draw( id );
        }
    }
}
