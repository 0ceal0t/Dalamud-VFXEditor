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
        public readonly ParsedUInt Id = new( "Id" );
        public readonly ParsedInt ParentId = new( "Parent Id" );
        public readonly ParsedInt NextSiblingId = new( "Next Sibling Id" );
        public readonly ParsedInt PrevSiblingId = new( "Prev Sibling Id" );
        public readonly ParsedInt ChildNodeId = new( "Child Node Id" );

        private bool IsComponentNode = false;
        public readonly ParsedEnum<NodeType> Type = new( "Type" ); // TODO: command
        public readonly ParsedInt ComponentTypeId = new( "Component Id" ); // TODO: change on update

        public UldNode( List<UldComponent> components ) {

        }

        public UldNode( BinaryReader reader, List<UldComponent> components ) : this( components ) {
            var pos = reader.BaseStream.Position;

            Id.Read( reader );
            ParentId.Read( reader );
            NextSiblingId.Read( reader );
            PrevSiblingId.Read( reader );
            ChildNodeId.Read( reader );

        }

        public void Write( BinaryWriter writer ) {

        }

        public void Draw( string id ) {

        }
    }
}
