using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;

namespace VFXEditor.Structs.Vfx {
    public class StaticVfx : BaseVfx {
        public StaticVfx( Plugin plugin, string path, Vector3 position) : base( plugin, path ) {
            Vfx = _Plugin.ResourceLoader.VfxCreate( path, "Client.System.Scheduler.Instance.VfxObject" );
            _Plugin.ResourceLoader.VfxRun( Vfx, 0.0f, 0xFFFFFFFF );
            UpdatePosition( position );
        }

        public override void Remove() {
            _Plugin.ResourceLoader.VfxRemove( Vfx );
        }

        public void UpdatePosition( Vector3 position ) {
            IntPtr addr = IntPtr.Add( Vfx, 0x50 );

            var x = BitConverter.GetBytes( position.X );
            var y = BitConverter.GetBytes( position.Y );
            var z = BitConverter.GetBytes( position.Z );

            Marshal.Copy( x, 0, addr, 4 );
            Marshal.Copy( z, 0, addr + 0x4, 4 );
            Marshal.Copy( y, 0, addr + 0x8, 4 );

            var flagAddr = IntPtr.Add( Vfx, 0x38 );
            byte currentFlag = Marshal.ReadByte( flagAddr );
            currentFlag |= 0x2;
            Marshal.WriteByte( flagAddr, currentFlag );
        }

        public void UpdatePosition( Actor actor ) {
            UpdatePosition( new Vector3( actor.Position.X, actor.Position.Y, actor.Position.Z ) );
        }
    }
}