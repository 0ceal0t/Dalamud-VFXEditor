using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.EidFormat.BindPoint;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public class EidFile : FileManagerFile {
        public readonly List<EidBindPoint> BindPoints = new();
        public readonly SimpleDropdown<EidBindPoint> Dropdown;

        private readonly short Version1;
        private readonly short Version2;
        private readonly uint Unk1;

        private bool NewData => Version1 == 0x3132;

        public EidFile( BinaryReader reader, bool checkOriginal = true ) : base( new CommandManager( Plugin.EidManager ) ) {
            Dropdown = new( "Bind Point", BindPoints,
                ( EidBindPoint item, int idx ) => $"Bind Point {item.GetId()}", () => new EidBindPointNew(), () => CommandManager.Eid );

            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            reader.ReadInt32(); // magic 00656964
            Version1 = reader.ReadInt16();
            Version2 = reader.ReadInt16();
            var count = reader.ReadInt32();
            Unk1 = reader.ReadUInt32();

            for( var i = 0; i < count; i++ ) {
                BindPoints.Add( NewData ? new EidBindPointNew( reader ) : new EidBindPointOld( reader ) );
            }

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( 0x00656964 );
            writer.Write( Version1 );
            writer.Write( Version2 );
            writer.Write( BindPoints.Count );
            writer.Write( Unk1 );

            foreach( var bindPoint in BindPoints ) bindPoint.Write( writer );
        }

        public override void Draw() {
            ImGui.Separator();
            Dropdown.Draw();
        }
    }
}
