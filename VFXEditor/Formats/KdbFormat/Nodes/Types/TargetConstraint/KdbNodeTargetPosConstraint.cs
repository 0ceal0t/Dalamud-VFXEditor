using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint {
    public class KdbNodeTargetPosConstraint : KdbNodeTargetConstraint {
        public override KdbNodeType Type => KdbNodeType.TargetPosContraint;

        public readonly ParsedDouble3 Unknown3 = new( "Unknown 3" );
        public readonly ParsedByteBool Unknown4 = new( "Unknown 4" );
        public readonly ParsedByteBool Unknown5 = new( "Unknown 5" );

        public KdbNodeTargetPosConstraint() : base() { }

        public KdbNodeTargetPosConstraint( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        protected override void ReadTargetConstraintBody( BinaryReader reader ) {
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            reader.ReadBytes( 6 ); // padding
        }

        protected override void WriteTargetConstraintBody( BinaryWriter writer ) {
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            FileUtils.Pad( writer, 6 ); // padding
        }

        protected override void DrawBody( List<string> bones ) {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Bones" ) ) {
                if( tab ) { BoneTable.Draw(); }
            }

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) {
                    using var _ = ImRaii.PushId( "Parameters" );
                    using var child = ImRaii.Child( "Child" );

                    Bone.Draw();
                    Unknown3.Draw();
                    Unknown4.Draw();
                    Unknown5.Draw();
                }
            }
        }
    }
}
