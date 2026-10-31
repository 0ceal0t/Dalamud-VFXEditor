using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using VfxEditor.Formats.AtchFormat;
using static VfxEditor.Formats.AtchFormat.AtchPenumbraSupport;

namespace VfxEditor.Interop.Penumbra {
    // Penumbra applies .atch edits as meta manipulations, never as files. This produces them in the two formats Penumbra reads:
    // - the binary (version 2) string accepted by AddTemporaryModAll, for live previews
    //   https://github.com/xivdev/Penumbra/blob/master/Penumbra/Api/Api/MetaApi.cs (ConvertManipsV2)
    // - the JSON "Manipulations" entries stored in a mod's option containers, for exports
    //   https://github.com/xivdev/Penumbra/blob/master/Penumbra/Files/MetaDeserialization.cs (ReadAtch)
    //   https://github.com/xivdev/Penumbra/blob/master/Penumbra/Meta/Manipulations/MetaDictionary.cs (Atch)
    // Penumbra rejects the entire binary string if any identifier is invalid or duplicated, so AtchPenumbraSupport.GetChanges filters first.
    public static class PenumbraMeta {
        private const byte Version = 2;
        private static readonly byte[] Magic = Encoding.ASCII.GetBytes( "META0002" );
        private const uint AtchKey = ( 'A' << 24 ) | ( 'T' << 16 ) | ( 'C' << 8 ) | 'H';

        // Index is the race part of the cXXXX code (c0101 -> 1). Odd = male, even = female.
        // These are ModelRace member names, which both of Penumbra's JSON readers accept (Newtonsoft's ToObject<ModelRace> requires them)
        private static readonly string[] RaceNames = [
            "Unknown",
            "Midlander", "Midlander",
            "Highlander", "Highlander",
            "Elezen", "Elezen",
            "Miqote", "Miqote",
            "Roegadyn", "Roegadyn",
            "Lalafell", "Lalafell",
            "AuRa", "AuRa",
            "Hrothgar", "Hrothgar",
            "Viera", "Viera",
        ];

        private static List<AtchChange> GetAllChanges( List<(string GamePath, AtchFile File)> files ) =>
            [.. files.SelectMany( x => GetChanges( x.GamePath, x.File ) ).GroupBy( x => (x.TypeName, x.GenderRace, x.Index) ).Select( x => x.First() )];

        // ========== BINARY (temporary mods) ===========

        public static string FromAtch( List<(string GamePath, AtchFile File)> files ) {
            var changes = GetAllChanges( files );
            if( changes.Count == 0 ) return string.Empty;

            using var output = new MemoryStream();
            using( var zip = new GZipStream( output, CompressionMode.Compress, true ) )
            using( var writer = new BinaryWriter( zip ) ) {
                writer.Write( Version );
                writer.Write( Magic );
                writer.Write( AtchKey );
                writer.Write( changes.Count );

                foreach( var change in changes ) {
                    // AtchIdentifier: AtchType (uint), GenderRace (ushort), EntryIndex (ushort)
                    writer.Write( AtchType( change.TypeName ) );
                    writer.Write( change.GenderRace );
                    writer.Write( change.Index );

                    // AtchEntry: 35 byte name buffer, name length, scale, offset xyz, rotation xyz
                    var bone = Encoding.ASCII.GetBytes( change.Bone );
                    var nameBuffer = new byte[MaxBoneNameLength + 1];
                    bone.CopyTo( nameBuffer, 0 );
                    writer.Write( nameBuffer );
                    writer.Write( ( byte )bone.Length );
                    writer.Write( change.Scale );
                    writer.Write( change.Offset.X );
                    writer.Write( change.Offset.Y );
                    writer.Write( change.Offset.Z );
                    writer.Write( change.Rotation.X );
                    writer.Write( change.Rotation.Y );
                    writer.Write( change.Rotation.Z );
                }
            }
            return Convert.ToBase64String( output.ToArray() );
        }

        // "2ax" -> '2' << 16 | 'a' << 8 | 'x', matching Penumbra's AtchType values
        private static uint AtchType( string name ) {
            uint ret = 0;
            foreach( var c in name ) ret = ( ret << 8 ) | ( byte )c;
            return ret;
        }

        // ========== JSON (exported mods) ===========

        // { "Type": "Atch", "Manipulation": { "Entry": { ... }, "Gender": "Male", "Race": "Midlander", "Type": "2ax", "Index": 0 } }
        public static List<object> ToJson( string gamePath, AtchFile file ) {
            var ret = new List<object>();
            foreach( var change in GetChanges( gamePath, file ) ) {
                var race = change.GenderRace / 100;
                ret.Add( new JObject {
                    ["Type"] = "Atch",
                    ["Manipulation"] = new JObject {
                        ["Entry"] = new JObject {
                            ["Bone"] = change.Bone,
                            ["Scale"] = change.Scale,
                            ["OffsetX"] = change.Offset.X,
                            ["OffsetY"] = change.Offset.Y,
                            ["OffsetZ"] = change.Offset.Z,
                            ["RotationX"] = change.Rotation.X,
                            ["RotationY"] = change.Rotation.Y,
                            ["RotationZ"] = change.Rotation.Z,
                        },
                        ["Gender"] = race % 2 == 1 ? "Male" : "Female",
                        ["Race"] = RaceNames[race],
                        ["Type"] = change.TypeName,
                        ["Index"] = change.Index,
                    }
                } );
            }
            return ret;
        }
    }
}
