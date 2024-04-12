using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Penumbra {
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct ImcManipulation {
        public ImcEntry Entry;
        public ushort PrimaryId;
        public ushort SecondaryId;
        public byte Variant;

        [JsonConverter( typeof( StringEnumConverter ) )]
        public ObjectType ObjectType;

        [JsonConverter( typeof( StringEnumConverter ) )]
        public EquipSlot EquipSlot;

        [JsonConverter( typeof( StringEnumConverter ) )]
        public BodySlot BodySlot;
    }
}
