using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using VfxEditor.Formats.AtchFormat;

namespace VfxEditor.Interop.Penumbra.Meta {
    public readonly record struct AtchChange( string TypeName, ushort GenderRace, ushort Index, string Bone, float Scale, Vector3 Offset, Vector3 Rotation );

    public static partial class PenumbraMetaWriter {
        private const byte Version = 2;
        public const int MaxBoneNameLength = 34;
        private static readonly byte[] Magic = Encoding.ASCII.GetBytes( "META0002" );
        private const uint AtchKey = ( 'A' << 24 ) | ( 'T' << 16 ) | ( 'C' << 8 ) | 'H';

        // chara/xls/attachoffset/m0980.atch
        // chara/xls/attachOffset/c0501.atch
        // NOTE: monster/demihuman atch editing is not supported in penumbra
        [GeneratedRegex( @"c(\d{4})\.atch$", RegexOptions.IgnoreCase )]
        private static partial Regex RaceRegex();

        // Need to match the ones expected by Penumbra
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

        public static bool TryGetGenderRace( string gamePath, out ushort genderRace ) {
            genderRace = 0;
            if( string.IsNullOrEmpty( gamePath ) ) return false;
            var match = RaceRegex().Match( gamePath );
            if( !match.Success || !ushort.TryParse( match.Groups[1].Value, out genderRace ) ) return false; // get the number component
            var race = genderRace / 100;
            return genderRace % 100 == 1 && race >= 1 && race <= 18;
        }

        private static uint TypeToUint( string name ) {
            uint ret = 0;
            foreach( var c in name ) ret = ( ret << 8 ) | ( byte )c; // shift over by 8 bytes each char, and append latest char to end
            return ret;
        }

        // Dedup by multiple changes trying to target the same thing
        private static List<AtchChange> GetAllChanges( List<(string GamePath, AtchFile File)> files ) =>
            [.. files.SelectMany( x => GetChanges( x.GamePath, x.File, true ) ).GroupBy( x => (x.TypeName, x.GenderRace, x.Index) ).Select( x => x.First() )];

        public static List<AtchChange> GetChanges( string gamePath, AtchFile file, bool filterByVanilla ) {
            var ret = new List<AtchChange>();
            if( file == null || !TryGetGenderRace( gamePath, out var genderRace ) ) return ret;

            var vanilla = filterByVanilla ? file.UpdateVanilla( gamePath ) : null;

            foreach( var entry in file.Entries ) {
                var name = entry.Name.Value;
                if( string.IsNullOrEmpty( name ) || name.Length > 4 ) continue;

                for( var index = 0; index < entry.States.Count; index++ ) {
                    var state = entry.States[index];
                    var bone = state.Bone.Value ?? "";
                    if( bone.Length == 0 || bone.Length > MaxBoneNameLength ) continue;

                    ret.Add( new( name, genderRace, ( ushort )index, bone, state.Scale.Value, state.Offset.Value, state.Rotation.Value ) );
                }

            }
            return ret;
        }


        public static string ToBase64( List<(string GamePath, AtchFile File)> files ) {
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
                    writer.Write( TypeToUint( change.TypeName ) );
                    writer.Write( change.GenderRace );
                    writer.Write( change.Index );

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

        public static List<object> ToJson( string gamePath, AtchFile file ) {
            var ret = new List<object>();
            foreach( var change in GetChanges( gamePath, file, true ) ) {
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