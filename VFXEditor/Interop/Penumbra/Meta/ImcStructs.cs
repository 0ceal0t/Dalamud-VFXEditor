using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VfxEditor.Interop.Penumbra.Meta {
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct ImcIdentifier {
        public ushort PrimaryId;
        public byte Variant;
        public ObjectType ObjectType;
        public ushort SecondaryId;
        public EquipSlot EquipSlot;
        public BodySlot BodySlot;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct ImcEntry {
        public byte MaterialId;
        public byte DecalId;
        public ushort AttributeAndSound;
        public byte VfxId;
        public byte MaterialAnimationId;
    }
}