using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Interop.Havok;
using VfxEditor.Ui.Components;

namespace VfxEditor.PapFormat {
    public unsafe class PapAddModal : Modal {
        public static string TempHkx => Path.Combine( Plugin.Configuration.WriteLocation, "temp_convert.hkx" ).Replace( '\\', '/' );

        private readonly PapFile PapFile;
        private readonly string ImportPath;
        private string IndexPreview = "0";
        private int Index;
        private int AnimationsLength;

        private string OkResult = "Havok data added";

        public PapAddModal( PapFile file, string importPath ) : base( "Animation Import Index", true ) {
            PapFile = file;
            ImportPath = importPath;

            SetAnimationsLength();
        }

        protected void SetAnimationsLength()
        {
            var hkxPath = ImportPath;
            if( ImportPath.Contains( ".pap" ) )
            {
                // Need to extract the havok data
                using var file = File.Open( ImportPath, FileMode.Open );
                using var reader = new BinaryReader( file );

                reader.BaseStream.Position = 0x12;
                var havokPosition = reader.ReadInt32();
                var footerPosition = reader.ReadInt32();
                var havokDataSize = footerPosition - havokPosition;
                reader.BaseStream.Position = havokPosition;
                File.WriteAllBytes( TempHkx, reader.ReadBytes( havokDataSize ) );

                hkxPath = TempHkx;
            }
            var newAnimation = new HavokData( hkxPath, true );

            AnimationsLength = newAnimation.AnimationContainer->Animations.Length;
        }

        protected override void DrawBody() {
            ImGui.PushTextWrapPos( 240 );
            ImGui.TextWrapped( "Select the index of the animation being imported" );
            ImGui.PopTextWrapPos();

            int LimitToArrayBounds( ImGuiInputTextCallbackData* data )
            {
                //not ideal but I can't figure out how to update the preview directly, this'll do.
                int parsedIndex = 0;
                Int32.TryParse( data->Buf->ToString(), out parsedIndex );
                parsedIndex = ( int )char.GetNumericValue( ( char )parsedIndex  ) ;
                if( parsedIndex < 0 )
                {
                    Index = 0;
                    OkResult = "Index defaulted to 0. Havok data added";
                }
                else if( parsedIndex >= AnimationsLength )
                {
                    var animationsMax = AnimationsLength - 1;
                    Index = animationsMax;
                    OkResult = "Index defaulted to " + animationsMax + ". Havok data added";
                }
                else
                {
                    Index = parsedIndex;
                    OkResult = "Havok data added";
                }
                return 0;
            }
            ImGui.InputText( "", ref IndexPreview, 255, ImGuiInputTextFlags.CharsDecimal | ImGuiInputTextFlags.CallbackEdit, LimitToArrayBounds );
        }

        protected override void OnCancel() { }

        protected override void OnOk() {
            var animation = new PapAnimation( PapFile, PapFile.HkxTempLocation );
            animation.ReadTmb( Path.Combine( Plugin.RootLocation, "Files", "default_pap_tmb.tmb" ) );

            var hkxPath = ImportPath;
            if( ImportPath.Contains( ".pap" ) ) {
                // Need to extract the havok data
                using var file = File.Open( ImportPath, FileMode.Open );
                using var reader = new BinaryReader( file );

                reader.BaseStream.Position = 0x12;
                var havokPosition = reader.ReadInt32();
                var footerPosition = reader.ReadInt32();
                var havokDataSize = footerPosition - havokPosition;
                reader.BaseStream.Position = havokPosition;
                File.WriteAllBytes( TempHkx, reader.ReadBytes( havokDataSize ) );

                hkxPath = TempHkx;
            }

            var commands = new List<ICommand> {
                new ListAddCommand<PapAnimation>( PapFile.Animations, animation, ( PapAnimation item, bool add ) => item.File.RefreshHavokIndexes()  ),
                new PapHavokCommand( PapFile, () => {
                    var newAnimation = new HavokData( hkxPath, true );
                    var container = PapFile.MotionData.AnimationContainer;

                    var anims = HavokData.ToList( container->Animations );
                    var bindings = HavokData.ToList( container->Bindings );
                    anims.Add( newAnimation.AnimationContainer->Animations[Index] );
                    bindings.Add( newAnimation.AnimationContainer->Bindings[Index] );

                    container->Animations = HavokData.CreateArray( PapFile.Handles, ( uint )container->Animations.Flags, anims, sizeof( nint ) );
                    container->Bindings = HavokData.CreateArray( PapFile.Handles, ( uint )container->Bindings.Flags, bindings, sizeof( nint ) );
                } )
            };
            Command.AddAndExecute( new CompoundCommand( commands ) );

            Dalamud.OkNotification( OkResult );
        }
    }
}
