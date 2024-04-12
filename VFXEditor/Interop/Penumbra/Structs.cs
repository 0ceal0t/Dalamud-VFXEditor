using Newtonsoft.Json;

namespace VfxEditor.Interop.Penumbra {
    public struct ImcEntry {
        public byte MaterialId;
        public byte DecalId;
        public ushort AttributeAndSound;
        public byte VfxId;
        public byte MaterialAnimationId;

        public ushort AttributeMask {
            readonly get => ( ushort )( AttributeAndSound & 0x3FF );
            init => AttributeAndSound = ( ushort )( ( AttributeAndSound & ~0x3FF ) | ( value & 0x3FF ) );
        }

        public byte SoundId {
            readonly get => ( byte )( AttributeAndSound >> 10 );
            init => AttributeAndSound = ( ushort )( AttributeMask | ( value << 10 ) );
        }

        [JsonConstructor]
        public ImcEntry( byte materialId, byte decalId, ushort attributeMask, byte soundId, byte vfxId, byte materialAnimationId ) {
            MaterialId = materialId;
            DecalId = decalId;
            AttributeAndSound = 0;
            VfxId = vfxId;
            MaterialAnimationId = materialAnimationId;
            AttributeMask = attributeMask;
            SoundId = soundId;
        }
    }
}