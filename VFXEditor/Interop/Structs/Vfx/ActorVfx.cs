using Dalamud.Game.ClientState.Objects.Types;
using System;

namespace VfxEditor.Structs.Vfx {
    public unsafe class ActorVfx : BaseVfx {
        public ActorVfx( GameObject caster, GameObject target, string path ) : base( path ) {
            Vfx = ( VfxStruct* )Plugin.ResourceLoader.ActorVfxCreate( path, caster.Address, target.Address, -1, ( char )0, 0, ( char )0 );
        }

        public override void Remove() {
            Plugin.ResourceLoader.ActorVfxRemove( ( IntPtr )Vfx, ( char )1 );
        }
    }
}