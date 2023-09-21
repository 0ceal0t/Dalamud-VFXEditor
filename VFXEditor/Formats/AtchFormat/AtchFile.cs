using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using VfxEditor.FileManager;
using VfxEditor.Formats.AtchFormat.Entry;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
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

        public readonly ushort NumStates;
        public readonly List<AtchEntry> Entries = new();
        private readonly AtchEntrySplitView EntryView;

        public AtchFile( BinaryReader reader ) : base( new CommandManager( Plugin.AtchManager ) ) {
            Verified = VerifiedStatus.UNSUPPORTED; // verifying these is fucked. The format is pretty simple though, so it's not a big deal

            var numEntries = reader.ReadUInt16();
            NumStates = reader.ReadUInt16();

            for( var i = 0; i < numEntries; i++ ) {
                Entries.Add( new( reader ) );
            }

            var bitFields = new List<uint>();
            for( var i = 0; i < 4; i++ ) bitFields.Add( reader.ReadUInt32() );

            for( var i = 0; i < numEntries; i++ ) {
                var bitField = bitFields[i >> 5];
                Entries[i].Accessory.Value = ( ( bitField >> ( i & 0x1F ) ) & 1 ) == 1;
            }

            var dataEnd = reader.BaseStream.Position;

            Entries.ForEach( x => x.ReadBody( reader, NumStates ) );
            EntryView = new( Entries );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( ushort )Entries.Count );
            writer.Write( NumStates );

            Entries.ForEach( x => x.Write( writer ) );

            var bitFields = new List<uint>();
            for( var i = 0; i < 4; i++ ) bitFields.Add( 0 );

            for( var i = 0; i < Entries.Count; i++ ) {
                var idx = i >> 5;
                var value = ( Entries[i].Accessory.Value ? 1u : 0u ) << ( i & 0x1F );
                bitFields[idx] = bitFields[idx] | value;
            }

            bitFields.ForEach( writer.Write );

            var stringStartPos = 2 + 2 + ( 4 * Entries.Count ) + 16 + ( 32 * Entries.Count * NumStates );
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

        private void DrawCurrentWeapons() {
            if( Dalamud.ClientState == null || Plugin.PlayerObject == null ) return;

            var weapons = new List<string>();

            var dataStart = Plugin.PlayerObject.Address + 0x6E8 + 32;
            for( var i = 0; i < 3; i++ ) {
                var data = dataStart + ( 104 * i );
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
