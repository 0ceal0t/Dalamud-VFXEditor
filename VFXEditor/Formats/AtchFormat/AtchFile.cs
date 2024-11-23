using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VfxEditor.FileManager;
using VfxEditor.Formats.AtchFormat.Entry;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Files/AtchFile.cs

    public class AtchFile : FileManagerFile {
        public static readonly Dictionary<string, string> WeaponNames = new() {
            { "2ax", "Greataxe" },
            { "2bk", "Book" },
            { "2bw", "Bow" },
            { "2ff", "Nouliths" },
            { "2gb", "Gunblade" },
            { "2gl", "Globe" },
            { "2gn", "Gun" },
            { "2km", "Scythe" },
            { "2kt", "Katana" },
            { "2rp", "Rapier" },
            { "2sp", "Spear" },
            { "2st", "Greatstaff" },
            { "2sw", "Greatsword" },
            { "aai", "Alchemist" },
            { "aal", "Alchemist" },
            { "aar", "Armorer" },
            { "abl", "Armorer" },
            { "aco", "Culinarian" },
            { "agl", "Goldsmith" },
            { "ali", "Alchemist" },
            { "alm", "Alchemist" },
            { "alt", "Leatherworker" },
            { "ase", "Weaver" },
            // { "atr", "" },
            // { "avt", "" },
            { "awo", "Carpenter" },
            { "bag", "Machinist Bag" },
            { "chk", "Chakram" },
            // { "clb", "" },
            { "clg", "Glove" },
            // { "cls", "" }, // Linked to axes
            { "clw", "Claw" },
            // { "col", "" },
            // { "cor", "" },
            // { "cos", "" },
            { "crd", "Astrologian Deck" },
            // { "crr", "" },
            // { "crt", "" },
            { "csl", "Carpenter" },
            { "csr", "Carpenter" },
            { "dgr", "Dagger" },
            { "drm", "Drum" },
            { "ebz", "Reaper Shroud" },
            // { "egp", "" },
            // { "elg", "" },
            // { "fch", "" },
            // { "fdr", "" },
            { "fha", "Fisher" },
            // { "fl2", "Harp" },
            { "flt", "Flute" },
            { "frg", "Ninja Frog" },
            { "fry", "Leatherworker/Culinarian" },
            { "fsh", "Fisher" },
            { "fsw", "Fist Weapons" },
            // { "fud", "" },
            // { "gdb", "" },
            // { "gdh", "" },
            // { "gdl", "" }
            // { "gdr", "" },
            // { "gdt", "" },
            // { "gdw", "" },
            { "gsl", "Machinist Deployable" },
            // { "gsr", "" }, // Diadem cannon?
            // { "gun", "" },
            // { "hel", "" },
            { "hmm", "Blacksmith/Armorer" },
            { "hrp", "Bard Harp" },
            { "htc", "Botanist" },
            { "ksh", "Samurai Sheath" },
            // { "let", "" },
            // { "lpr", "" }, // Linked to 1923
            { "mlt", "Goldsmith" },
            { "mrb", "Alchemist" },
            { "mrh", "Alchemist" },
            { "msg", "Machinist Shotgun" },
            { "mwp", "Machinist Cannon" },
            { "ndl", "Weaver" },
            // { "nik", "" }, // Linked to Nier pod, maybe Nikana or something
            { "nph", "Botanist" },
            { "orb", "Red Mage Focus" },
            // { "oum", "" },
            // { "pen", "" }, // Linked to daggers
            { "pic", "Miner" },
            // { "pra", "" },
            { "prf", "Leatherworker" },
            { "qvr", "Quiver" },
            // { "rap", "" },
            { "rbt", "Ninja Rabbit" },
            { "rod", "Blue Mage Rod" },
            // { "rop", "" },
            { "saw", "Carpenter" },
            // { "sht", "" },
            { "sic", "Fisher" },
            { "sld", "Shield" },
            { "stf", "Staff" },
            { "stv", "Culinarian" },
            { "swd", "Sword" },
            { "syl", "Machinist Sniper" },
            // { "syr", "" },
            // { "syu", "" },
            // { "tan", "" },
            { "tbl", "Goldsmith" },
            // { "tcs", "" },
            { "tgn", "Goldsmith" },
            { "tmb", "Weaver" },
            // { "trm", "" }, // Linked to flute
            // { "trr", "" },
            // { "trw", "" }, // Linked to greatswords
            // { "vln", "" },
            { "whl", "Weaver" },
            // { "wng", "" },
            // { "ypd", "" },
            { "ytk", "Armorer" },
        };

        public const int BitFieldSize = 32;

        public readonly ushort NumStates;
        public readonly List<AtchEntry> Entries = [];
        private readonly AtchEntrySplitView EntryView;

        public unsafe AtchFile( BinaryReader reader ) : base() {
            Verified = VerifiedStatus.UNSUPPORTED; // verifying these is fucked. The format is pretty simple though, so it's not a big deal

            var numEntries = reader.ReadUInt16();
            NumStates = reader.ReadUInt16();

            for( var i = 0; i < numEntries; i++ ) {
                Entries.Add( new( reader ) );
            }

            var bitfield = stackalloc ulong[BitFieldSize / 8];
            for( var i = 0; i < BitFieldSize / 8; ++i )
                bitfield[i] = reader.ReadUInt64();

            for( var i = 0; i < numEntries; ++i ) {
                var bitIdx = i & 0x3F;
                var ulongIdx = i >> 6;
                Entries[i].Accessory.Value = ( ( bitfield[ulongIdx] >> bitIdx ) & 1 ) == 1;
            }

            Entries.ForEach( x => x.ReadBody( reader, NumStates ) );
            EntryView = new( Entries );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( ushort )Entries.Count );
            writer.Write( NumStates );

            Entries.ForEach( x => x.Write( writer ) );

            Span<byte> bitfield = stackalloc byte[BitFieldSize];
            foreach( var (entry, i) in Entries.WithIndex() ) {
                var bitIdx = i & 0x7;
                var byteIdx = i >> 3;
                if( Entries[i].Accessory.Value )
                    bitfield[byteIdx] |= ( byte )( 1 << bitIdx );
            }

            writer.Write( bitfield );

            var stringStartPos = 2 + 2 + ( 4 * Entries.Count ) + BitFieldSize + ( 32 * Entries.Count * NumStates );
            using var stringMs = new MemoryStream();
            using var stringWriter = new BinaryWriter( stringMs );
            var stringPos = new Dictionary<string, int>();

            Entries.ForEach( x => x.WriteBody( writer, stringStartPos, stringWriter, stringPos ) );

            writer.Write( stringMs.ToArray() );
        }

        public override void Draw() {
            DrawCurrentWeapons();

            ImGui.Separator();

            EntryView.Draw();
        }

        private unsafe void DrawCurrentWeapons() {
            if( Dalamud.ClientState == null || Plugin.PlayerObject == null ) return;

            var weapons = new List<string>();
            // https://github.com/aers/FFXIVClientStructs/blob/2c388216cb52d4b6c4dbdedb735e1b343d56a846/FFXIVClientStructs/FFXIV/Client/Game/Character/Character.cs#L78C20-L78C23
            var dataStart = ( nint )Unsafe.AsPointer( ref ( ( Character* )Plugin.PlayerObject.Address )->DrawData ) + 0x20;

            for( var i = 0; i < 3; i++ ) {
                var data = dataStart + ( DrawObjectData.Size * i );
                if( Marshal.ReadInt64( data + 8 ) == 0 || Marshal.ReadInt64( data + 16 ) == 0 || Marshal.ReadInt32( data + 32 ) == 0 ) continue;

                var nameArr = Marshal.PtrToStringAnsi( data + 32 ).ToCharArray();
                Array.Reverse( nameArr );
                weapons.Add( new string( nameArr ) );
            }

            if( weapons.Count == 0 ) return;

            ImGui.Separator();

            ImGui.TextDisabled( $"Current Weapons: {weapons.Aggregate( ( x, y ) => x + " | " + y )}" );
        }
    }
}
