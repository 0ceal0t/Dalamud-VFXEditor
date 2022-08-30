using Dalamud.Game.ClientState.Objects.Types;
using System;

namespace VFXEditor.Structs.Vfx {
    public unsafe class ActorVfx : BaseVfx {
        public ActorVfx( GameObject caster, GameObject target, string path ) : base( path ) {
            Vfx = ( VfxStruct* )VfxEditor.ResourceLoader.ActorVfxCreate( path, caster.Address, target.Address, -1, ( char )0, 0, ( char )0 );
        }

        public override void Remove() {
            VfxEditor.ResourceLoader.ActorVfxRemove( ( IntPtr )Vfx, ( char )1 );
        }
    }
}