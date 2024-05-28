using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat {
    public enum ArrayType : ushort {
        Operations, // size = 0x10
        B, // size = 0x10
        C, // size = 0x08
        Unused, // ???
        E, // size = 0x08
        F, // size = 0x08, actually has values
    }

    public class KdbFile : FileManagerFile {
        private readonly uint MajorVersion;
        private readonly uint MinorVersion;
        private readonly uint PatchVersion;
        private readonly uint CheckFileSize;

        private readonly ParsedFnvHash FileName = new( "File Name" );

        private readonly ParsedDouble UnknownOperation = new( "Unknown Operation" );
        private readonly ParsedUInt UnknownB1 = new( "Unknown B 1" );
        private readonly ParsedUInt UnknownB2 = new( "Unknown B 2" );

        public readonly KdbNodeGraphViewer NodeGraph = new();

        public KdbFile( BinaryReader reader, string sourcePath, bool verify ) : base() {
            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();
            PatchVersion = reader.ReadUInt32();
            CheckFileSize = reader.ReadUInt32();
            reader.ReadUInt32(); // file size

            FileName.Read( reader, sourcePath.Split( '\\' )[^1].Split( '.' )[0] ); // encoded like kdi_c0101t0778
            reader.ReadUInt16(); // name offset, 0
            reader.ReadUInt16(); // padding

            var dataArrayCount = reader.ReadUInt32();
            var dataArrayPosition = reader.BaseStream.Position;
            var dataArrayOffset = reader.ReadUInt32();

            if( dataArrayCount != 5 ) Dalamud.Error( $"Data array count is {dataArrayCount}" );

            for( var i = 0; i < 7; i++ ) reader.ReadUInt32(); // reserved

            var dataArrayPositions = new List<(ArrayType, long)>();
            reader.BaseStream.Position = dataArrayPosition + dataArrayOffset;
            for( var i = 0; i < dataArrayCount; i++ ) {
                var type = ( ArrayType )reader.ReadUInt16();
                var unknown = reader.ReadUInt16();
                if( unknown != 1 ) Dalamud.Error( $"Value is {unknown}" );
                dataArrayPositions.Add( (type, reader.BaseStream.Position + reader.ReadUInt32()) ); // add offset
            }

            foreach( var (type, position) in dataArrayPositions ) {
                reader.BaseStream.Position = position;
                var count = reader.ReadUInt32();
                var arrayPosition = reader.BaseStream.Position + reader.ReadUInt32(); // offset is 0 if count = 0
                if( type == ArrayType.Operations ) {
                    UnknownOperation.Read( reader );
                }
                else if( type == ArrayType.B ) {
                    UnknownB1.Read( reader );
                    UnknownB2.Read( reader );
                }
            }
        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {
            ImGui.Separator();

            ImGui.TextDisabled( $"Version: {MajorVersion}.{MinorVersion}.{PatchVersion}" );
            FileName.Draw();
            UnknownOperation.Draw();
            UnknownB1.Draw();
            UnknownB2.Draw();

            ImGui.Separator();

            using( var graphChild = ImRaii.Child( "GraphChild", new( -1, ImGui.GetContentRegionAvail().Y / 2f ) ) ) {
                NodeGraph.Draw();
            }

            using var selectedChild = ImRaii.Child( "SelectedChild" );
            if( NodeGraph.Canvas.SelectedNodes.Count == 0 ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

            var node = NodeGraph.Canvas.SelectedNodes.FirstOrDefault();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) NodeGraph.Canvas.RemoveNode( node );
            }
            node.Draw();
        }
    }
}
