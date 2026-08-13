using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Interop.Havok {
    public static unsafe class SkeletonMatcher {
        // ===================
        // Disk cache layout (under <pluginConfig>/SkeletonIndex/):
        //   _index.json  - { ModDir, Paths: [...] }   (just the list of sklb file paths)
        //   <hash>.json  - { Path, LastWrite, BoneCount, FloatSlotCount, BoneNames: [...] }
        // The index is small and read on every plugin load. Individual skeleton infos are
        // lazy-loaded from their cache files only when a candidate is actually considered
        // during matching, and only reparsed from the sklb if the file's LastWriteTime
        // has changed since it was cached.
        // ===================

        public class SkeletonInfo {
            public string Path;
            public DateTime LastWrite;
            public int BoneCount;
            public int FloatSlotCount;
            public List<string> BoneNames = [];
        }

        public class MatchMetadata {
            public string ModName = "";
            public string RaceCode = "";
            public string RaceName = "";
            public string SkeletonType = "";
            public int BoneCount;
            public int FloatSlotCount;
        }

        public class IndexFile {
            public string ModDir = "";
            public List<string> Paths = [];
        }

        private static readonly Dictionary<string, SkeletonInfo> MemoryCache = [];
        private static List<string> PenumbraSklbPaths = [];
        private static bool Scanned = false;
        private static bool RescannedThisSession = false;

        private static string CacheDir => Path.Combine( Plugin.Configuration.WriteLocation, "SkeletonIndex" );
        private static string IndexPath => Path.Combine( CacheDir, "_index.json" );

        private static string TempHkxPath => Path.Combine( Plugin.Configuration.WriteLocation, "_skelmatch_scan.hkx" );

        private static readonly Regex RaceCodePattern = new( @"skl_([cdmw]\d{4})b\d{4}", RegexOptions.IgnoreCase );

        private static string HashPath( string path ) {
            var bytes = SHA256.HashData( Encoding.UTF8.GetBytes( path ) );
            return Convert.ToHexString( bytes, 0, 8 ); // 16 hex chars, enough to avoid collisions
        }

        private static string CacheFilePath( string localPath ) => Path.Combine( CacheDir, HashPath( localPath ) + ".json" );

        // ===================
        // Scanning
        // ===================

        public static void EnsureScanned() {
            if( Scanned ) return;
            Scanned = true;

            // Try to load the path list from disk cache first (fast, no directory walk)
            if( LoadIndexFromDisk() ) return;

            // No disk cache — do the full directory walk and save the index
            FullScan();
        }

        private static bool LoadIndexFromDisk() {
            try {
                if( !File.Exists( IndexPath ) ) return false;

                var json = File.ReadAllText( IndexPath );
                var index = JsonConvert.DeserializeObject<IndexFile>( json );
                if( index == null ) return false;

                // Invalidate if the Penumbra mod directory has changed
                var modDir = Plugin.PenumbraIpc?.GetModDirectory() ?? "";
                if( !string.IsNullOrEmpty( modDir ) && index.ModDir != modDir ) return false;

                // Drop paths that no longer exist on disk
                PenumbraSklbPaths = [.. index.Paths.Where( File.Exists )];
                Dalamud.Log( $"[SkeletonMatcher] Loaded {PenumbraSklbPaths.Count} cached sklb paths from disk" );
                return true;
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error loading skeleton index from disk" );
                return false;
            }
        }

        private static void FullScan() {
            try {
                var modDir = Plugin.PenumbraIpc?.GetModDirectory() ?? "";
                if( string.IsNullOrEmpty( modDir ) || !Directory.Exists( modDir ) ) {
                    PenumbraSklbPaths = [];
                    return;
                }

                PenumbraSklbPaths = [.. Directory.GetFiles( modDir, "*.sklb", SearchOption.AllDirectories )];
                SaveIndexToDisk( modDir );
                Dalamud.Log( $"[SkeletonMatcher] Full scan found {PenumbraSklbPaths.Count} sklb files" );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error scanning Penumbra directory for sklb files" );
                PenumbraSklbPaths = [];
            }
        }

        private static void SaveIndexToDisk( string modDir ) {
            try {
                Directory.CreateDirectory( CacheDir );
                var index = new IndexFile { ModDir = modDir, Paths = PenumbraSklbPaths };
                var json = JsonConvert.SerializeObject( index, Formatting.Indented );
                File.WriteAllText( IndexPath, json );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error saving skeleton index to disk" );
            }
        }

        public static void Rescan() {
            Scanned = false;
            MemoryCache.Clear();
            PenumbraSklbPaths = [];
            EnsureScanned();
        }

        // ===================
        // Loading individual skeleton infos (with disk cache)
        // ===================

        private static SkeletonInfo LoadInfo( string localPath ) {
            if( MemoryCache.TryGetValue( localPath, out var cached ) ) return cached;
            if( !File.Exists( localPath ) ) { MemoryCache[localPath] = null; return null; }

            // Try disk cache first (validated by LastWriteTime); fall back to parsing
            var info = TryLoadFromDiskCache( localPath );
            if( info == null ) {
                info = ParseSklbFile( localPath );
                if( info != null ) SaveDiskCache( info );
            }

            MemoryCache[localPath] = info;
            return info;
        }

        private static SkeletonInfo TryLoadFromDiskCache( string localPath ) {
            try {
                var cacheFile = CacheFilePath( localPath );
                if( !File.Exists( cacheFile ) ) return null;

                var lastWrite = File.GetLastWriteTimeUtc( localPath );
                var json = File.ReadAllText( cacheFile );
                var info = JsonConvert.DeserializeObject<SkeletonInfo>( json );
                if( info == null ) return null;

                // Stale cache — the sklb was modified (or replaced) since we cached it
                if( info.LastWrite != lastWrite ) return null;

                return info;
            }
            catch {
                return null;
            }
        }

        private static void SaveDiskCache( SkeletonInfo info ) {
            try {
                Directory.CreateDirectory( CacheDir );
                var json = JsonConvert.SerializeObject( info, Formatting.Indented );
                File.WriteAllText( CacheFilePath( info.Path ), json );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error saving skeleton cache file" );
            }
        }

        private static SkeletonInfo ParseSklbFile( string localPath ) {
            HavokData data = null;
            try {
                var simple = SimpleSklb.LoadFromLocal( localPath );
                simple.SaveHavokData( TempHkxPath );
                data = new HavokData( TempHkxPath, true );

                if( data.AnimationContainer == null || data.AnimationContainer->Skeletons.Length == 0 ) return null;

                var skeleton = data.AnimationContainer->Skeletons[0].ptr;
                var boneNames = new List<string>( skeleton->Bones.Length );
                for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                    boneNames.Add( skeleton->Bones[i].Name.String ?? "" );
                }

                return new SkeletonInfo {
                    Path = localPath,
                    LastWrite = File.GetLastWriteTimeUtc( localPath ),
                    BoneCount = skeleton->Bones.Length,
                    FloatSlotCount = skeleton->FloatSlots.Length,
                    BoneNames = boneNames
                };
            }
            catch {
                return null;
            }
            finally {
                data?.RemoveReference();
                try { if( File.Exists( TempHkxPath ) ) File.Delete( TempHkxPath ); } catch { }
            }
        }

        // ===================
        // Matching
        // ===================

        private static string ExtractRaceCode( string path ) {
            if( string.IsNullOrEmpty( path ) ) return null;
            var match = RaceCodePattern.Match( path );
            return match.Success ? match.Groups[1].Value.ToLower() : null;
        }

        // bone/float indices are from the animation binding (fixed regardless of skeleton).
        // currentBoneNames is the bone-name-ordered list of the originally loaded (vanilla) skeleton.
        // Returns the local path of the best matching sklb, or null if none found.
        public static string FindBestMatch(
            string gameSklbPath,
            List<string> currentBoneNames,
            List<short> allTransformIndices,
            List<short> allFloatIndices
        ) {
            EnsureScanned();
            var match = FindBestMatchInternal( gameSklbPath, currentBoneNames, allTransformIndices, allFloatIndices );

            // If nothing matched, do a fresh rescan ONCE per session to pick up newly installed
            // skeleton mods, then try again. This is the only time the directory walk re-runs.
            if( match == null && !RescannedThisSession ) {
                RescannedThisSession = true;
                Dalamud.Log( "[SkeletonMatcher] No match in cached skeletons — doing a fresh rescan" );
                Rescan();
                match = FindBestMatchInternal( gameSklbPath, currentBoneNames, allTransformIndices, allFloatIndices );
            }

            return match;
        }

        private static string FindBestMatchInternal(
            string gameSklbPath,
            List<string> currentBoneNames,
            List<short> allTransformIndices,
            List<short> allFloatIndices
        ) {
            if( PenumbraSklbPaths.Count == 0 ) return null;

            var raceCode = ExtractRaceCode( gameSklbPath );
            var vanillaBoneCount = currentBoneNames?.Count ?? 0;

            string bestPath = null;
            int bestScore = -1;

            foreach( var candidatePath in PenumbraSklbPaths ) {
                // Filter by race code when possible (e.g. c0801 = Hrothgar Female)
                var candidateRace = ExtractRaceCode( candidatePath );
                if( raceCode != null && candidateRace != null && candidateRace != raceCode ) continue;

                var info = LoadInfo( candidatePath );
                if( info == null ) continue;

                // All animation indices must be in range for this skeleton
                var valid = true;
                foreach( var idx in allTransformIndices ) {
                    if( idx >= 0 && idx >= info.BoneCount ) { valid = false; break; }
                }
                if( valid ) {
                    foreach( var idx in allFloatIndices ) {
                        if( idx >= 0 && idx >= info.FloatSlotCount ) { valid = false; break; }
                    }
                }
                if( !valid ) continue;

                // Score: bone-name compatibility with the vanilla skeleton (ordered),
                // then prefer the skeleton with the fewest extra bones (closest match)
                var compatCount = 0;
                var minCount = Math.Min( vanillaBoneCount, info.BoneNames.Count );
                for( var i = 0; i < minCount; i++ ) {
                    if( info.BoneNames[i] == currentBoneNames[i] ) compatCount++;
                }
                var compatScore = vanillaBoneCount > 0 ? ( compatCount * 10000 ) / vanillaBoneCount : 0;
                var extraBones = info.BoneCount - vanillaBoneCount;
                var sizeScore = extraBones >= 0 ? Math.Max( 0, 10000 - extraBones ) : 0;
                var score = compatScore * 1000 + sizeScore;

                if( score > bestScore ) {
                    bestScore = score;
                    bestPath = candidatePath;
                }
            }

            if( bestPath != null ) {
                Dalamud.Log( $"[SkeletonMatcher] Auto-matched skeleton: {bestPath}" );
            }

            return bestPath;
        }

        // ===================
        // Metadata extraction
        // ===================

        private static readonly Dictionary<string, string> RaceNames = new() {
            { "c0101", "Midlander M" }, { "c0201", "Midlander F" },
            { "c0301", "Highlander M" }, { "c0401", "Highlander F" },
            { "c0501", "Elezen M" }, { "c0601", "Elezen F" },
            { "c0701", "Miqo'te M" }, { "c0801", "Miqo'te F" },
            { "c0901", "Roegadyn M" }, { "c1001", "Roegadyn F" },
            { "c1101", "Lalafell M" }, { "c1201", "Lalafell F" },
            { "c1301", "Au Ra M" }, { "c1401", "Au Ra F" },
            { "c1501", "Hrothgar M" }, { "c1601", "Hrothgar F" },
            { "c1701", "Viera M" }, { "c1801", "Viera F" },
        };

        private static string ExtractModName( string localPath ) {
            try {
                var modDir = Plugin.PenumbraIpc?.GetModDirectory() ?? "";
                if( string.IsNullOrEmpty( modDir ) || !localPath.StartsWith( modDir, StringComparison.OrdinalIgnoreCase ) ) return "";

                var relative = localPath[modDir.Length..].TrimStart( '\\', '/' );
                var firstSep = relative.IndexOfAny( ['\\', '/'] );
                return firstSep > 0 ? relative[..firstSep] : relative;
            }
            catch { return ""; }
        }

        private static string ClassifySkeletonType( string modName, List<string> boneNames, int boneCount ) {
            // Primary: mod-name keyword matching
            var name = modName ?? "";
            var hasNflb = name.Contains( "NFLB", StringComparison.OrdinalIgnoreCase ) || name.Contains( "Noffle", StringComparison.OrdinalIgnoreCase );
            var hasYas = name.Contains( "YAS", StringComparison.OrdinalIgnoreCase );
            var hasIvcs = name.Contains( "IVCS", StringComparison.OrdinalIgnoreCase );
            var hasIvcs2 = name.Contains( "IVCS2", StringComparison.OrdinalIgnoreCase ) ||
                           ( hasIvcs && ( name.Contains( "v2", StringComparison.OrdinalIgnoreCase ) || name.Contains( " 2" ) ) );

            if( hasNflb && hasYas ) return "YAS + NFLB";
            if( hasNflb ) return "NFLB";
            if( hasYas ) return "YAS";
            if( hasIvcs2 ) return "IVCS2";
            if( hasIvcs ) return "IVCS";
            if( name.Contains( "Bibo", StringComparison.OrdinalIgnoreCase ) ) return "Bibo+";

            // Secondary: characteristic bone-name patterns
            if( boneNames != null ) {
                foreach( var bone in boneNames ) {
                    if( bone == null ) continue;
                    if( bone.Contains( "ivcs", StringComparison.OrdinalIgnoreCase ) ) return "IVCS";
                    if( bone.Contains( "nflb", StringComparison.OrdinalIgnoreCase ) ) return "NFLB";
                }
            }

            // Tertiary: if it clearly adds bones beyond vanilla, mark as modded
            if( boneCount > 0 && !string.IsNullOrEmpty( name ) ) {
                // Most vanilla human skeletons have <= ~210 bones
                if( boneCount > 215 ) return "Modded";
            }

            return "Vanilla";
        }

        public static MatchMetadata GetMetadata( string sklbPath, List<string> boneNames, int boneCount, int floatSlotCount ) {
            var raceCode = ExtractRaceCode( sklbPath ) ?? "";
            var raceName = RaceNames.TryGetValue( raceCode, out var rn ) ? rn : raceCode;
            var modName = ExtractModName( sklbPath );
            var skelType = string.IsNullOrEmpty( modName )
                ? ( boneCount > 215 ? "Modded" : "Vanilla" )
                : ClassifySkeletonType( modName, boneNames, boneCount );

            return new MatchMetadata {
                ModName = modName,
                RaceCode = raceCode,
                RaceName = raceName,
                SkeletonType = skelType,
                BoneCount = boneCount,
                FloatSlotCount = floatSlotCount,
            };
        }
    }
}