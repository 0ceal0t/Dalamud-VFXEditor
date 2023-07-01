using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.CutbFormat.Scene {
    public enum AssetType {
        None = 0x00,
        BG = 0x01,
        Attribute = 0x02,
        LayLight = 0x03,
        VFX = 0x04,
    }

    public class CutbInstance : IUiItem {
        public readonly ParsedByte Flags = new( "Flags" );
        public readonly ParsedEnum<AssetType> AssetType = new( "Asset Type", size: 1 );
        public readonly ParsedShort LayerId = new( "Layer Id" );
        public readonly ParsedUInt InstanceId = new( "Instance Id" );

        public CutbInstance() { }

        public CutbInstance( BinaryReader reader ) {
            Flags.Read( reader );
            AssetType.Read( reader );
            LayerId.Read( reader );
            InstanceId.Read( reader );
        }

        public void Writer( BinaryWriter writer ) {
            Flags.Write( writer );
            AssetType.Write( writer );
            LayerId.Write( writer );
            InstanceId.Write( writer );
        }

        public void Draw() {
            Flags.Draw( CommandManager.Cutb );
            AssetType.Draw( CommandManager.Cutb );
            LayerId.Draw( CommandManager.Cutb );
            InstanceId.Draw( CommandManager.Cutb );
        }
    }
}
