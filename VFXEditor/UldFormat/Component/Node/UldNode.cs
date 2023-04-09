using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Component.Node {
    public enum NodeType : int {
        Image = 2,
        Text = 3,
        NineGrid = 4,
        Counter = 5,
        Collision = 8,
    }

    public class UldNode : ISimpleUiBase {
        private readonly List<UldComponent> Components;

        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedInt ParentId = new( "Parent Id" );
        public readonly ParsedInt NextSiblingId = new( "Next Sibling Id" );
        public readonly ParsedInt PrevSiblingId = new( "Prev Sibling Id" );
        public readonly ParsedInt ChildNodeId = new( "Child Node Id" );

        private bool IsComponentNode = false;
        public readonly ParsedEnum<NodeType> Type = new( "Type" ); // TODO: command
        public readonly ParsedInt ComponentTypeId = new( "Component Id" ); // TODO: change on update
        public UldNodeData Data = null;

        public readonly ParsedInt TabIndex = new( "Tab Index", size: 2 );
        public readonly ParsedInt Unk1 = new( "Unknown 1" );
        public readonly ParsedInt Unk2 = new( "Unknown 2" );
        public readonly ParsedInt Unk3 = new( "Unknown 3" );
        public readonly ParsedInt Unk4 = new( "Unknown 4" );
        public readonly ParsedShort X = new( "X" );
        public readonly ParsedShort Y = new( "Y" );
        public readonly ParsedUInt W = new( "Width", size: 2 );
        public readonly ParsedUInt H = new( "Height", size: 2 );
        public readonly ParsedFloat Rotation = new( "Rotation" );
        public readonly ParsedFloat2 Scale = new( "Scale" );
        public readonly ParsedShort OriginX = new( "Origin X" );
        public readonly ParsedShort OriginY = new( "Origin Y" );
        public readonly ParsedUInt Priority = new( "Priority", size: 2 );

        public UldNode( List<UldComponent> components ) {
            Components = components;
            // TODO: extra commands
        }

        public UldNode( BinaryReader reader, List<UldComponent> components ) : this( components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            ParentId.Read( reader );
            NextSiblingId.Read( reader );
            PrevSiblingId.Read( reader );
            ChildNodeId.Read( reader );

            // Weirdness with node type
            var nodeType = reader.ReadInt32();
            var offset = reader.ReadUInt32();

            if( nodeType > 1000 ) {
                IsComponentNode = true;
                ComponentTypeId.Value = nodeType;
            }
            else {
                Type.Value = ( NodeType )nodeType;
            }

            // TODO: update data
        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw( string id ) {

        }


    }
}
