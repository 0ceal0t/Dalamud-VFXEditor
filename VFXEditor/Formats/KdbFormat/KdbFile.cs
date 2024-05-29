using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Formats.KdbFormat.Nodes;
using VfxEditor.Formats.KdbFormat.Nodes.Types;
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
        private readonly ParsedUInt UnknownB1 = new( "Unknown B1" );
        private readonly ParsedUInt UnknownB2 = new( "Unknown B2" );

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
            if( dataArrayCount != 5 ) Dalamud.Error( $"Data array count is {dataArrayCount}" );
            var dataArrayPosition = reader.BaseStream.Position + reader.ReadUInt32();

            for( var i = 0; i < 7; i++ ) reader.ReadUInt32(); // reserved

            reader.BaseStream.Position = dataArrayPosition;

            var dataArrayPositions = new List<(ArrayType, long)>();
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
                    UnknownOperation.Read( reader ); // TODO
                    reader.BaseStream.Position = arrayPosition;

                    var nodes = new List<KdbNode>();
                    for( var nodeIdx = 0; nodeIdx < count; nodeIdx++ ) {
                        reader.ReadUInt32(); // index
                        var nodeType = ( KdbNodeType )reader.ReadByte();

                        KdbNode newNode = nodeType switch {
                            KdbNodeType.EffectorExpr => new KdbNodeEffectorExpr( reader ),
                            KdbNodeType.EffectorEZParamLink => new KdbNodeEffectorEZParamLink( reader ),
                            KdbNodeType.EffectorEZParamLinkLinear => new KdbNodeEffectorEZParamLinkLinear( reader ),
                            KdbNodeType.SourceOther => new KdbNodeSourceOther( reader ),
                            KdbNodeType.SourceRotate => new KdbNodeSourceRotate( reader ),
                            KdbNodeType.SourceTranslate => new KdbNodeSourceTranslate( reader ),
                            KdbNodeType.TargetBendSTRoll => new KdbNodeTargetBendSTRoll( reader ),
                            KdbNodeType.TargetRotate => new KdbNodeTargetRotate( reader ),
                            KdbNodeType.TargetTranslate => new KdbNodeTargetTranslate( reader ),
                            KdbNodeType.TargetBendRoll => new KdbNodeTargetBendRoll( reader ),
                            // ========================
                            KdbNodeType.Connection => new KdbConnection( reader ),
                            _ => null
                        };

                        if( newNode == null ) Dalamud.Error( $"Unknown node type {nodeType}" );
                        else nodes.Add( newNode );
                    }

                    foreach( var node in nodes.Where( x => x is not KdbConnection ) ) NodeGraph.AddToCanvas( node, false );
                    foreach( var node in nodes ) {
                        if( node is not KdbConnection connection ) continue;

                        var sourceNode = nodes[connection.SourceIdx];
                        var targetNode = nodes[connection.TargetIdx];
                        var sourceSlot = sourceNode.FindOutput( connection.SourceType );
                        var targetSlot = targetNode.FindInput( connection.TargetType );
                        if( sourceSlot == null ) {
                            Dalamud.Error( $"Could not find output {connection.SourceType} for {sourceNode.Type}" );
                            continue;
                        }
                        if( targetSlot == null ) {
                            Dalamud.Error( $"Could not find input {connection.TargetType} for {targetNode.Type}" );
                            continue;
                        }
                        targetSlot.ConnectTo( sourceSlot );
                    }
                }
                else if( type == ArrayType.B ) {
                    UnknownB1.Read( reader );
                    UnknownB2.Read( reader );
                    reader.BaseStream.Position = arrayPosition;

                    // TODO
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
