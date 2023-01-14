using Dalamud.Game.ClientState.Objects.Types;
using System;

namespace VfxEditor.Structs.Vfx {
    public unsafe class ActorVfx : BaseVfx {
        public ActorVfx( GameObject caster, GameObject target, string path ) : this( caster.Address, target.Address, path ) { }

        public ActorVfx( IntPtr caster, IntPtr target, string path ) : base( path ) {
            Vfx = ( VfxStruct* )Plugin.ResourceLoader.ActorVfxCreate( path, caster, target, -1, ( char )0, 0, ( char )0 );
        }

        public override void Remove() {
            Plugin.ResourceLoader.ActorVfxRemove( ( IntPtr )Vfx, ( char )1 );
        }
    }
}