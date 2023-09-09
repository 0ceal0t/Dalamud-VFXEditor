using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.AtchFormat.Entry;
using VfxEditor.Ui.Components;
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
            { "aai", "Alchemist Tool" },
            { "aal", "Alchemist Tool" },
            { "aar", "Armorer Tool" },
            { "abl", "Armorer Tool" },
            { "aco", "Culinarian Tool" },
            { "agl", "Goldsmith Tool" },
            { "ali", "Alchemist Tool" },
            { "alm", "Alchemist Tool" },
            { "alt", "Leatherworker Tool" },
            { "ase", "Weaver Tool" },
            // { "atr", "" },
            // { "avt", "" },
            { "awo", "Carpenter Tool" },
            { "bag", "Astrologian Sleave" },
            { "chk", "Chakram" },
            // { "clb", "" },
            // { "clg", "" }, // Linked to books, maybe cudgel or something
            // { "cls", "" }, // Linked to axes
            { "clw", "Fist Weapon" },
            // { "col", "" },
            // { "cor", "" },
            // { "cos", "" },
            { "crd", "Katana Sheathe" },
            // { "crr", "" },
            // { "crt", "" },
            { "csl", "Carpenter Tool" },
            { "csr", "Carpenter Tool" },
            { "dgr", "Dagger" },
            { "drm", "Drum" },
            { "ebz", "Reaper Shroud" },
            // { "egp", "" },
            // { "elg", "" },
            // { "fch", "" },
            // { "fdr", "" },
            { "fha", "Fisher Tool" },
            // { "fl2", "Harp" },
            { "flt", "Flute" },
            // { "frg", "" },
            { "fry", "Leatherworker Tool" },
            { "fsh", "Fisher Tool" },
            { "fsw", "Fist Weapons" },
            // { "fud", "" },
            // { "gdb", "" },
            // { "gdh", "" },
            // { "gdl", "" }
            // { "gdr", "" },
            // { "gdt", "" },
            // { "gdw", "" },
            // { "gsl", "" },
            { "gsr", "Diadem Cannon" },
            // { "gun", "" },
            // { "hel", "" },
            // { "hmm", "" },
            { "hrp", "Bard Harp" },
            { "htc", "Botanist Tool" },
            { "ksh", "Red Mage Focus" },
            // { "let", "" },
            // { "lpr", "" }, // Linked to 1923
            // { "mlt", "" },
            { "mrb", "Alchemist Tool" },
            { "mrh", "Alchemist Tool" },
            // { "msg", "" }, // Linked to wing? (1913)
            // { "mwp", "" },
            { "ndl", "Weaver Tool" },
            // { "nik", "" }, // Linked to Nier pod, maybe Nikana or something
            { "nph", "Botanist Tool" },
            { "orb", "Blue Mage" },
            // { "oum", "" },
            // { "pen", "" }, // Linked to daggers
            { "pic", "Miner Weapon" },
            { "pra", "Machinist Weapon" }, // Linked to autocrossbow
            { "prf", "Leatherworker Tool" },
            { "qvr", "Quiver" },
            // { "rap", "" },
            { "rbt", "Ninja Rabbit" }, // Linked to frog
            // { "rod", "" }, // Linked to gunblades
            // { "rop", "" },
            { "saw", "Carpenter Tool" },
            // { "sht", "" },
            { "sic", "Fisher Tool" },
            { "sld", "Shield" },
            { "stf", "Staff" },
            { "stv", "Culinarian Tool" },
            { "swd", "Sword" },
            // { "syl", "" },
            // { "syr", "" },
            // { "syu", "" }, // Linked to ninja rabbit
            // { "tan", "" },
            { "tbl", "Goldsmith Tool" },
            // { "tcs", "" },
            { "tgn", "Goldsmith Tool" },
            { "tmb", "Weaver Tool" },
            // { "trm", "" }, // Linked to flute
            // { "trr", "" },
            // { "trw", "" }, // Linked to greatswords
            { "vln", "Machinist Weapon" }, // Linked to sniper
            { "whl", "Weaver Tool" },
            // { "wng", "" }, // Linked to Machinist Weapons
            // { "ypd", "" },
            { "ytk", "Armorer Tool" },
        };

        public readonly ushort NumStates;
        public readonly List<AtchEntry> Entries = new();
        private readonly SimpleSplitview<AtchEntry> EntryView;

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
                Entries[i].Offhand.Value = ( ( bitField >> ( i & 0x1F ) ) & 1 ) == 1;
            }

            var dataEnd = reader.BaseStream.Position;

            Entries.ForEach( x => x.ReadBody( reader, NumStates ) );
            EntryView = new( "Entry", Entries, false, ( AtchEntry item, int idx ) => item.WeaponName, () => new(), () => CommandManager.Atch );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( ushort )Entries.Count );
            writer.Write( NumStates );

            Entries.ForEach( x => x.Write( writer ) );

            var bitFields = new List<uint>();
            for( var i = 0; i < 4; i++ ) bitFields.Add( 0 );

            for( var i = 0; i < Entries.Count; i++ ) {
                var idx = i >> 5;
                var value = ( Entries[i].Offhand.Value ? 1u : 0u ) << ( i & 0x1F );
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
            ImGui.Separator();

            EntryView.Draw();
        }
    }
}
