using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VfxEditor.Formats.AtchFormat {
    // Penumbra does not load .atch files, it only applies ATCH meta manipulations on top of the vanilla file for a race.
    // A manipulation can only overwrite an existing (point, state) entry, so anything else is silently ignored:
    // https://github.com/xivdev/Penumbra/blob/master/Penumbra/Collections/Cache/AtchCache.cs (Apply)
    // https://github.com/xivdev/Penumbra/blob/master/Penumbra/Meta/AtchManager.cs (only playable gender/races)
    public static partial class AtchPenumbraSupport {
        public const int MaxBoneNameLength = 34;

        [GeneratedRegex( @"c(\d{4})\.atch$", RegexOptions.IgnoreCase )]
        private static partial Regex RaceRegex();

        private static readonly Dictionary<string, AtchFile> VanillaCache = [];

        // c0101 (Midlander Male) to c1801 (Viera Female)
        public static bool TryGetGenderRace( string gamePath, out ushort genderRace ) {
            genderRace = 0;
            if( string.IsNullOrEmpty( gamePath ) ) return false;
            var match = RaceRegex().Match( gamePath );
            if( !match.Success || !ushort.TryParse( match.Groups[1].Value, out genderRace ) ) return false;
            var race = genderRace / 100;
            return genderRace % 100 == 1 && race >= 1 && race <= 18;
        }

        private static AtchFile GetVanilla( string gamePath ) {
            var key = gamePath.ToLower();
            if( VanillaCache.TryGetValue( key, out var cached ) ) return cached;

            AtchFile ret = null;
            try {
                var data = Dalamud.DataManager.GetFile( key )?.Data;
                if( data != null ) {
                    using var reader = new BinaryReader( new MemoryStream( data ) );
                    ret = new AtchFile( reader );
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not read vanilla {key}" );
            }

            VanillaCache[key] = ret;
            return ret;
        }

        // Changes in this file that will not show up through Penumbra
        public static List<string> GetUnsupported( string gamePath, AtchFile file ) {
            var ret = new List<string>();
            if( file == null || string.IsNullOrEmpty( gamePath ) ) return ret;

            if( !TryGetGenderRace( gamePath, out _ ) ) {
                ret.Add( "Penumbra only supports .atch edits for playable races (c0101 to c1801). None of these changes will apply" );
                return ret;
            }

            var vanilla = GetVanilla( gamePath );
            if( vanilla == null ) return ret;

            var vanillaEntries = vanilla.Entries.GroupBy( x => x.Name.Value ).ToDictionary( x => x.Key, x => x.First() );
            var names = file.Entries.Select( x => x.Name.Value ).ToList();

            var added = names.Where( x => !vanillaEntries.ContainsKey( x ) ).Distinct().ToList();
            if( added.Count > 0 ) ret.Add( $"New points cannot be added, these will be ignored: {string.Join( ", ", added )}" );

            var removed = vanillaEntries.Keys.Where( x => !names.Contains( x ) ).ToList();
            if( removed.Count > 0 ) ret.Add( $"Points cannot be removed, these will keep their vanilla values: {string.Join( ", ", removed )}" );

            var duplicates = names.GroupBy( x => x ).Where( x => x.Count() > 1 ).Select( x => x.Key ).ToList();
            if( duplicates.Count > 0 ) ret.Add( $"Only the first of each duplicate point is used: {string.Join( ", ", duplicates )}" );

            var accessory = file.Entries.Where( x => vanillaEntries.TryGetValue( x.Name.Value, out var v ) && v.Accessory.Value != x.Accessory.Value ).Select( x => x.Name.Value ).Distinct().ToList();
            if( accessory.Count > 0 ) ret.Add( $"Changes to the Accessory flag will not apply: {string.Join( ", ", accessory )}" );

            var longBones = file.Entries.SelectMany( x => x.States ).Select( x => x.Bone.Value ?? "" ).Where( x => x.Length > MaxBoneNameLength ).Distinct().ToList();
            if( longBones.Count > 0 ) ret.Add( $"Bone names longer than {MaxBoneNameLength} characters are not supported: {string.Join( ", ", longBones )}" );

            return ret;
        }
    }
}
