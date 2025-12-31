using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Formats.KdbFormat.Nodes;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Effector;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Source;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Target;
using VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint;
using VfxEditor.Formats.KdbFormat.UnknownB;
using VfxEditor.Formats.KdbFormat.UnknownF;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.Ui;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Components.Tables;
using VfxEditor.Utils;
using VFXEditor.Formats.KdbFormat.UnknownE;

namespace VfxEditor.Formats.KdbFormat {
    public enum ArrayType : ushort {
        Operations, // size = 0x10
        B, // size = 0x10
        C, // size = 0x08
        Unused, // ???
        E, // size = 0x08
        F, // size = 0x08, actually has values
    }

    /*
     * HUGE THANKS TO WINTER FOR FIGURING OUT MOST OF THIS
     */

    public unsafe class KdbFile : FileManagerFile {
        public static List<(int, int)> VerifyIgnore { get; private set; } = null;

        public static string SklbTempPath => Path.Combine( Plugin.RootLocation, "Files", "kb_havok.hkx" );
        private readonly SkeletonSelector Selector;
        public readonly List<string> BoneList = [];

        private readonly uint MajorVersion;
        private readonly uint MinorVersion;
        private readonly uint PatchVersion;
        private readonly uint CheckFileSize;

        private readonly ParsedFnvHash FileName = new( "File Name" );

        private readonly ParsedUInt Unknown1 = new( "Unknown 1" );
        private readonly ParsedUInt Unknown2 = new( "Unknown 2" );

        public readonly List<UnknownBItem> ItemsB = [];
        private readonly CommandSplitView<UnknownBItem> SplitViewB;

        public readonly List<UnknownEItem> ItemsE = [];
        private readonly CommandSplitView<UnknownEItem> SplitViewE;

        public readonly List<UnknownFItem> ItemsF = [];
        private readonly CommandTable<UnknownFItem> TableViewF;

        private readonly ParsedDouble UnknownOperation = new( "Unknown Operation" );

        public readonly KdbNodeGraphViewer NodeGraph = new();
        public List<KdbNode> Nodes => NodeGraph.Canvas.Nodes;

        public KdbFile( BinaryReader reader, string sourcePath, bool verify ) : base() {
            Selector = new( GetSklbPath( sourcePath ), UpdateSkeleton );
            TableViewF = new( "Rows", true, false, ItemsF, [
                ( "Index", ImGuiTableColumnFlags.None, -1 ),
                ( "X", ImGuiTableColumnFlags.None, -1 ),
                ( "Y", ImGuiTableColumnFlags.None, -1 ),
            ], () => new() );

            SplitViewB = new( "Item", ItemsB, false, null, () => new( Nodes ) );
            SplitViewE = new( "Item", ItemsE, false, null, () => new() );

            // ==========================

            VerifyIgnore = [];
            var connectionMissing = false;

            MajorVersion = reader.ReadUInt32();
            MinorVersion = reader.ReadUInt32();
            PatchVersion = reader.ReadUInt32();
            CheckFileSize = reader.ReadUInt32();
            reader.ReadUInt32(); // file size

            FileName.Read( reader, sourcePath.Split( '\\' )[^1].Split( '.' )[0] ); // encoded like kdi_c0101t0778

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

            Unknown1.Read( reader );
            Unknown2.Read( reader );

            foreach( var (type, position) in dataArrayPositions ) {
                reader.BaseStream.Position = position;
                var count = reader.ReadUInt32();
                var arrayPosition = reader.BaseStream.Position + reader.ReadUInt32(); // offset is 0 if count = 0

                if( type == ArrayType.Operations ) {
                    UnknownOperation.Read( reader );
                    reader.BaseStream.Position = arrayPosition;

                    var nodes = new List<KdbNode>();
                    for( var nodeIdx = 0; nodeIdx < count; nodeIdx++ ) {
                        reader.ReadUInt32(); // index
                        var nodeType = ( KdbNodeType )reader.ReadByte();


                        KdbNode? newNode = nodeType switch {
                            KdbNodeType.EffectorExpr => new KdbNodeEffectorExpr( reader ),
                            KdbNodeType.EffectorEZParamLink => new KdbNodeEffectorEZParamLink( reader ),
                            KdbNodeType.EffectorEZParamLinkLinear => new KdbNodeEffectorEZParamLinkLinear( reader ),
                            KdbNodeType.SourceRotate => new KdbNodeSourceRotate( reader ),
                            KdbNodeType.SourceTranslate => new KdbNodeSourceTranslate( reader ),
                            KdbNodeType.TargetBendSTRoll => new KdbNodeTargetBendSTRoll( reader ),
                            KdbNodeType.TargetBendRoll => new KdbNodeTargetBendRoll( reader ),
                            KdbNodeType.TargetTranslate => new KdbNodeTargetTranslate( reader ),
                            KdbNodeType.TargetScale => new KdbNodeTargetScale( reader ),
                            KdbNodeType.TargetRotate => new KdbNodeTargetRotate( reader ),
                            KdbNodeType.TargetPosContraint => new KdbNodeTargetPosConstraint( reader ),
                            KdbNodeType.TargetOrientationConstraint => new KdbNodeTargetOrientationConstraint( reader ),
                            // ========================
                            KdbNodeType.Connection => new KdbConnection( reader ),
                            _ => null
                        };

                        if( newNode == null ) Dalamud.Error( $"Unknown node type {nodeType}" );
                        else nodes.Add( newNode );
                    }

                    var ignoreStart = ( int )reader.BaseStream.Position;
                    FileUtils.PadTo( reader, 8 );
                    VerifyIgnore.Add( (ignoreStart, ( int )reader.BaseStream.Position) );

                    foreach( var node in nodes.Where( x => x is not KdbConnection ) ) NodeGraph.AddToCanvas( node, false );
                    foreach( var node in nodes ) {
                        if( node is not KdbConnection connection ) continue;

                        var sourceNode = nodes[connection.SourceIdx];
                        var targetNode = nodes[connection.TargetIdx];
                        var sourceSlot = sourceNode.FindOutput( connection.SourceType );
                        var targetSlot = targetNode.FindInput( connection.TargetType );
                        if( sourceSlot == null ) {
                            Dalamud.Error( $"Could not find output {connection.SourceType} for {sourceNode.Type}" );
                            connectionMissing = true;
                            continue;
                        }
                        if( targetSlot == null ) {
                            Dalamud.Error( $"Could not find input {connection.TargetType} for {targetNode.Type}" );
                            connectionMissing = true;
                            continue;
                        }
                        if( !targetSlot.AcceptMultiple && targetSlot.GetConnections().Count > 0 ) {
                            connectionMissing = true;
                            Dalamud.Error( $"{connection.TargetType} for {targetNode.Type} should accept multiple inputs" );
                        }
                        targetSlot.Connect( sourceSlot, node.NameHash, connection.Coeff, connection.Unknown );
                    }

                    NodeGraph.Canvas.Organize();
                }
                else if( type == ArrayType.B ) {
                    var bCount = reader.ReadUInt32();
                    var bArrayPosition = reader.BaseStream.Position + reader.ReadUInt32();
                    reader.BaseStream.Position = bArrayPosition;
                    for( var i = 0; i < bCount; i++ ) ItemsB.Add( new( reader, Nodes ) );
                }
                else if( type == ArrayType.E ) {
                    reader.BaseStream.Position = arrayPosition;
                    for( var i = 0; i < count; i++ ) ItemsE.Add( new( reader ) );
                }
                else if( type == ArrayType.F ) {
                    reader.BaseStream.Position = arrayPosition;
                    for( var i = 0; i < count; i++ ) ItemsF.Add( new( reader ) );
                }
                else if( count > 0 ) {
                    Dalamud.Error( $"{type} >>> {count} / {reader.BaseStream.Position:X8} / {arrayPosition:X8}" );
                }
            }

            if( verify ) {
                Verified = FileUtils.Verify( reader, ToBytes(), VerifyIgnore );
                if( connectionMissing ) Verified = VerifiedStatus.ERROR;
            }
        }

        public unsafe void UpdateSkeleton( SimpleSklb sklbFile ) {
            sklbFile.SaveHavokData( SklbTempPath );
            var bones = new HavokBones( SklbTempPath, true );
            BoneList.Clear();
            var skeleton = bones.AnimationContainer->Skeletons[0].ptr;
            for( var i = 0; i < skeleton->Bones.Length; i++ ) BoneList.Add( skeleton->Bones[i].Name.String );
            foreach( var node in Nodes ) node.UpdateBones( BoneList );
            bones.RemoveReference();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( MajorVersion );
            writer.Write( MinorVersion );
            writer.Write( PatchVersion );
            writer.Write( CheckFileSize );
            var fileSizePlaceholder = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            FileName.Write( writer );
            writer.Write( 5 ); // data array count
            var dataArrayPositionPlaceholder = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder

            for( var i = 0; i < 7; i++ ) writer.Write( 0 ); // reserved

            UpdatePlaceholder( dataArrayPositionPlaceholder, writer.BaseStream.Position, writer );
            var operationsDataPlaceholder = WriteDataArrayOffset( ArrayType.Operations, writer );
            var bDataPlaceholder = WriteDataArrayOffset( ArrayType.B, writer );
            var cDataPlaceholder = WriteDataArrayOffset( ArrayType.C, writer );
            var eDataPlaceholder = WriteDataArrayOffset( ArrayType.E, writer );
            var fDataPlaceholder = WriteDataArrayOffset( ArrayType.F, writer );

            Unknown1.Write( writer );
            Unknown2.Write( writer );
            var operationsPlaceholder = WriterDataArrayHeader( Nodes.Count +
                Nodes.Select( x => x.Inputs.Select( y => y.GetConnections().Count ).Sum() ).Sum(), writer );
            UnknownOperation.Write( writer );
            var bPlaceholder = WriterDataArrayHeader( 0, writer );

            writer.Write( ItemsB.Count );
            var bPlaceholder2 = writer.BaseStream.Position;
            writer.Write( 0 );

            var cPlaceholder = WriterDataArrayHeader( 0, writer );
            var ePlaceholder = WriterDataArrayHeader( ItemsE.Count, writer );
            var fPlaceholder = WriterDataArrayHeader( ItemsF.Count, writer );

            UpdatePlaceholder( operationsDataPlaceholder, operationsPlaceholder.Item1, writer );
            UpdatePlaceholder( bDataPlaceholder, bPlaceholder.Item1, writer );
            UpdatePlaceholder( cDataPlaceholder, cPlaceholder.Item1, writer );
            UpdatePlaceholder( eDataPlaceholder, ePlaceholder.Item1, writer );
            UpdatePlaceholder( fDataPlaceholder, fPlaceholder.Item1, writer );

            // ===== OPERATIONS =======

            UpdatePlaceholder( operationsPlaceholder.Item2, writer.BaseStream.Position, writer );

            var allNodes = new List<KdbNode>();
            var connections = new List<KdbConnection>();

            foreach( var node in Nodes ) {
                foreach( var slot in node.Inputs ) {
                    foreach( var (_connectedSlot, idx) in slot.GetConnections().WithIndex() ) {
                        var connectedSlot = ( KdbSlot )_connectedSlot;
                        var data = slot.Data.TryGetValue( connectedSlot, out var _data ) ? _data : new();
                        var connection = new KdbConnection(
                            data.Name, ( KdbNode )connectedSlot.Node, ( KdbNode )slot.Node,
                            idx, connectedSlot.Type, slot.Type, data.Coeff.Value, data.Unknown.Value );
                        allNodes.Add( connection );
                        connections.Add( connection );
                    }
                }
                allNodes.Add( node );
            }

            foreach( var connection in connections ) connection.UpdateIndexes( allNodes );

            var nodePositions = new Dictionary<KdbNode, long>();
            foreach( var (node, idx) in allNodes.WithIndex() ) {
                writer.Write( idx );
                writer.Write( ( byte )node.Type );
                node.Write( writer, nodePositions );
            }

            FileUtils.PadTo( writer, 8 );

            foreach( var node in allNodes ) {
                UpdatePlaceholder( nodePositions[node], writer.BaseStream.Position, writer );
                node.WriteBody( writer );
            }

            // ==== B =======

            if( ItemsB.Count > 0 ) {
                UpdatePlaceholder( bPlaceholder2, writer.BaseStream.Position, writer );
                foreach( var item in ItemsB ) item.Write( writer );
            }

            // ==== E =======

            if( ItemsE.Count > 0 ) {
                UpdatePlaceholder( ePlaceholder.Item2, writer.BaseStream.Position, writer );
                foreach( var item in ItemsE ) item.Write( writer );
            }

            // ==== F =======

            if( ItemsF.Count > 0 ) {
                UpdatePlaceholder( fPlaceholder.Item2, writer.BaseStream.Position, writer );
                foreach( var item in ItemsF ) item.Write( writer );
            }

            // ===== FILE SIZE ========

            writer.BaseStream.Position = fileSizePlaceholder;
            writer.Write( ( uint )writer.BaseStream.Length );
        }

        private static long WriteDataArrayOffset( ArrayType type, BinaryWriter writer ) {
            writer.Write( ( ushort )type );
            writer.Write( ( ushort )1 );
            var res = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder
            return res;
        }

        private static (long, long) WriterDataArrayHeader( int count, BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;
            writer.Write( count );
            var res = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder
            return (pos, res);
        }

        private static void UpdatePlaceholder( long offsetPos, long dataPos, BinaryWriter writer ) {
            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = offsetPos;
            writer.Write( ( uint )( dataPos - offsetPos ) );
            writer.BaseStream.Position = savePos;
        }

        public override void Draw() {
            ImGui.Separator();

            Selector.Draw();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Nodes" ) ) {
                if( tab ) { DrawNodes(); }
            }

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) { DrawParameters(); }
            }

            using( var tab = ImRaii.TabItem( "Unknown B" ) ) {
                if( tab ) { DrawB(); }
            }

            using( var tab = ImRaii.TabItem( "Unknown E" ) ) {
                if( tab ) { DrawE(); }
            }

            using( var tab = ImRaii.TabItem( "Unknown F" ) ) {
                if( tab ) { DrawF(); }
            }
        }

        private void DrawNodes() {
            UnknownOperation.Draw();

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
            node.Draw( BoneList );
        }

        private void DrawParameters() {
            using var _ = ImRaii.PushId( "Parameters" );
            using var child = ImRaii.Child( "Child" );

            FileName.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
        }

        private void DrawB() {
            using var _ = ImRaii.PushId( "B" );
            SplitViewB.Draw();
        }

        private void DrawE() {
            using var _ = ImRaii.PushId( "E" );
            SplitViewE.Draw();
        }

        private void DrawF() {
            using var _ = ImRaii.PushId( "F" );
            TableViewF.Draw();
        }

        private static string GetSklbPath( string sourcePath ) {
            // chara/human/c0101/skeleton/met/m0005/skl_c0101m0005.sklb
            // chara/human/c0101/skeleton/top/t6188/skl_c0101t6188.sklb
            // chara/monster/m0011/skeleton/base/b0001/skl_m0011b0001.sklb
            // chara/demihuman/d1002/skeleton/base/b0001/skl_d1002b0001.sklb
            // chara/human/c0401/skeleton/base/b0001/skl_c0401b0001.sklb
            // chara/human/c0401/skeleton/hair/h0004/skl_c0401h0004.sklb
            // chara/human/c0401/skeleton/face/f0202/skl_c0401f0202.sklb

            var combined = sourcePath.Split( "_" )[^1].Split( "." )[0]; // .../kdi_c1701f0002.kdb 0> c1701f0002
            var part1 = combined[..5]; // c1701
            var part2 = combined.Substring( 5, 5 ); // f0002
            var type1 = part1[0]; // c
            var type2 = part2[0]; // f

            var string1 = type1 switch {
                'd' => "demihuman",
                'm' => "monster",
                _ => "human"
            };

            var string2 = type2 switch {
                'f' => "face",
                'h' => "hair",
                't' => "top",
                'm' => "met",
                _ => "base'"
            };

            return $"chara/{string1}/{part1}/skeleton/{string2}/{part2}/skl_{combined}.sklb";
        }
    }
}
