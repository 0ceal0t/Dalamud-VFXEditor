using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Numerics;
using VfxEditor.Ui;

namespace VfxEditor.Data {
    // The settings window. Configuration itself only holds the saved settings
    public unsafe class ConfigurationWindow : DalamudWindow {
        private static Configuration Config => Plugin.Configuration;

        public ConfigurationWindow() : base( "VFXEditor Settings", false, new( 300, 500 ), Plugin.WindowSystem ) { }

        public override void DrawBody() {
            using var _ = ImRaii.PushId( "Settings" );

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            if( ImGui.BeginTabItem( "Configuration" ) ) {
                DrawConfiguration();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Keybinds" ) ) {
                DrawKeybinds();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "File Browser" ) ) {
                DrawFileBrowser();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "3D Preview" ) ) {
                Draw3DView();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Vfx" ) ) {
                DrawVfx();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "Tmb" ) ) {
                DrawTmb();
                ImGui.EndTabItem();
            }
            if( ImGui.BeginTabItem( "File Editors" ) ) {
                DrawEditorSpecific();
                ImGui.EndTabItem();
            }
        }

        private static void DrawConfiguration() {
            using var child = ImRaii.Child( "Config" );

            if( ImGui.CollapsingHeader( "Saving", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                ImGui.TextDisabled( "Changes to the temp file location may require a restart to take effect" );
                if( ImGui.InputText( "Write Location", ref Config.WriteLocation, 255 ) ) Config.Save();

                if( ImGui.Checkbox( "Autosave Workspace", ref Config.AutosaveEnabled ) ) Config.Save();
                using var disabled = ImRaii.Disabled( !Config.AutosaveEnabled );
                using var indent = ImRaii.PushIndent();
                ImGui.SetNextItemWidth( 120 );
                if( ImGui.InputInt( "Autosave Time (seconds)", ref Config.AutosaveSeconds ) ) Config.Save();
                if( ImGui.Checkbox( "Backup Instead of Overwriting", ref Config.AutosaveBackups ) ) Config.Save();
                using var disabled2 = ImRaii.Disabled( !Config.AutosaveBackups );
                ImGui.SetNextItemWidth( 120 );
                if( ImGui.InputInt( "Backup Count", ref Config.BackupCount ) ) Config.Save();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.CollapsingHeader( "Logging", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                if( ImGui.Checkbox( "All Files", ref Config.LogAllFiles ) ) Config.Save();
                if( ImGui.Checkbox( "Debug Information", ref Config.LogDebug ) ) Config.Save();
                if( ImGui.Checkbox( "Vfx Debug Information", ref Config.LogVfxDebug ) ) Config.Save();
                if( ImGui.Checkbox( "Vfx Triggers", ref Config.LogVfxTriggers ) ) Config.Save();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.CollapsingHeader( "UI", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                if( ImGui.Checkbox( "Hide with Game UI", ref Config.HideWithUI ) ) Config.Save();
                if( ImGui.Checkbox( "Show Tab Bar", ref Config.ShowTabBar ) ) Config.Save();
                if( ImGui.Checkbox( "Lock Main Windows", ref Config.LockMainWindows ) ) Config.Save();
                ImGui.SetNextItemWidth( 135 );
                if( ImGui.InputInt( "Recent File Limit", ref Config.SaveRecentLimit ) ) {
                    Config.SaveRecentLimit = Math.Max( Config.SaveRecentLimit, 0 );
                    Config.Save();
                }
                ImGui.SetNextItemWidth( 135 );
                if( ImGui.InputFloat( "Overlay Remove Delay", ref Config.OverlayRemoveDelay ) ) Config.Save();
                if( ImGui.Checkbox( "Limit Overlay by Distance", ref Config.OverlayLimit ) ) Config.Save();
                if( ImGui.Checkbox( "Show Icons in Select Dialog", ref Config.SelectDialogIconsOnLeft ) ) Config.Save();
                if( ImGui.InputText( "Default Mod Author", ref Config.DefaultAuthor, 255 ) ) Config.Save();
            }
        }

        private static void DrawKeybinds() {
            if( ImGui.Checkbox( "Block Game Inputs When VFXEditor is Focused", ref Config.BlockGameInputsWhenFocused ) ) Config.Save();

            using var child = ImRaii.Child( "Keybinds", new Vector2( -1 ), false );

            if( Config.SaveKeybind.Draw( "Save" ) ) Config.Save();
            if( Config.SaveAsKeybind.Draw( "Save As" ) ) Config.Save();
            if( Config.OpenKeybind.Draw( "Open" ) ) Config.Save();
            if( Config.CopyKeybind.Draw( "Copy" ) ) Config.Save();
            if( Config.PasteKeybind.Draw( "Paste" ) ) Config.Save();
            if( Config.UndoKeybind.Draw( "Undo" ) ) Config.Save();
            if( Config.RedoKeybind.Draw( "Redo " ) ) Config.Save();
            if( Config.UpdateKeybind.Draw( "Update" ) ) Config.Save();
            if( Config.SpawnOnSelfKeybind.Draw( "Spawn on Self (Vfx only)" ) ) Config.Save();
            if( Config.SpawnOnGroundKeybind.Draw( "Spawn on Ground (Vfx only)" ) ) Config.Save();
            if( Config.SpawnOnTargetKeybind.Draw( "Spawn on Target (Vfx only)" ) ) Config.Save();
        }

        private static void DrawVfx() {
            using var child = ImRaii.Child( "Vfx" );

            if( ImGui.CollapsingHeader( "Curve Editor", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                if( ImGui.ColorEdit4( "Line Color", ref Config.CurveEditorLineColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Point Color", ref Config.CurveEditorPointColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Primary Selected Color", ref Config.CurveEditorPrimarySelectedColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Selected Color", ref Config.CurveEditorSelectedColor ) ) Config.Save();

                if( ImGui.InputInt( "Line Width", ref Config.CurveEditorLineWidth ) ) Config.Save();
                if( ImGui.InputInt( "Color Ring Width", ref Config.CurveEditorColorRingSize ) ) Config.Save();
                if( ImGui.InputInt( "Point Size", ref Config.CurveEditorPointSize ) ) Config.Save();
                if( ImGui.InputInt( "Primary Selected Size", ref Config.CurveEditorPrimarySelectedSize ) ) Config.Save();
                if( ImGui.InputInt( "Selected Size", ref Config.CurveEditorSelectedSize ) ) Config.Save();
                if( ImGui.InputInt( "Grab Distance", ref Config.CurveEditorGrabbingDistance ) ) Config.Save();
            }

            if( ImGui.CollapsingHeader( "Timeline Editor", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                if( ImGui.ColorEdit4( "Selected Color", ref Config.TimelineSelectedColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Bar Color", ref Config.TimelineBarColor ) ) Config.Save();
            }
        }

        private static void DrawTmb() {
            using var child = ImRaii.Child( "Tmb" );

            if( ImGui.CollapsingHeader( "Lua", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                using var indent = ImRaii.PushIndent( 10f );

                if( ImGui.ColorEdit4( "Parentheses Color", ref Config.LuaParensColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Function Color", ref Config.LuaFunctionColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Literal Color", ref Config.LuaLiteralColor ) ) Config.Save();
                if( ImGui.ColorEdit4( "Variable Color", ref Config.LuaVariableColor ) ) Config.Save();
            }
        }

        private static void DrawFileBrowser() {
            using var child = ImRaii.Child( "FileBrowser" );

            if( ImGui.Checkbox( "Preview Images", ref Config.FileBrowserImagePreview ) ) Config.Save();
            if( ImGui.Checkbox( "Skip File Overwriting Confirmation", ref Config.FileBrowserOverwriteDontAsk ) ) Config.Save();

            if( ImGui.ColorEdit4( "Selected Color", ref Config.FileBrowserSelectedColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "Folder Color", ref Config.FileBrowserFolderColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "Code File Color", ref Config.FileBrowserCodeColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "Misc File Color", ref Config.FileBrowserMiscColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "Image File Color", ref Config.FileBrowserImageColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "Archive File Color", ref Config.FileBrowserArchiveColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "FFXIV File Color", ref Config.FileBrowserFfxivColor ) ) Config.Save();
        }

        private static void Draw3DView() {
            using var child = ImRaii.Child( "3D View" );

            DrawDirectXCommon();

            if( ImGui.CollapsingHeader( "Skeleton" ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                DrawDirectXSkeleton();
            }

            if( ImGui.CollapsingHeader( "Material" ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                DrawDirectXMaterial();
            }

            if( ImGui.CollapsingHeader( "Vfx" ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                DrawDirectXVfx();
            }
        }

        public static void DrawDirectXCommon() {
            if( ImGui.ColorEdit4( "Background Color", ref Config.RendererBackground ) ) {
                Plugin.DirectXManager.Redraw();
                Config.Save();
            }
        }

        public static void DrawDirectXSkeleton() {
            if( ImGui.ColorEdit4( "Bone Name Color", ref Config.SkeletonBoneNameColor ) ) Config.Save();
            if( ImGui.ColorEdit4( "Connecting Line Color", ref Config.SkeletonBoneLineColor ) ) Config.Save();
        }

        public static void DrawDirectXMaterial() {
            var updated = false;
            updated |= ImGui.ColorEdit3( "Ambient Color", ref Config.MaterialAmbientColor );

            if( ImGui.CollapsingHeader( "Light 1" ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                using var __ = ImRaii.PushId( "Light1" );
                updated |= Config.Light1.Draw();
            }

            if( ImGui.CollapsingHeader( "Light 2" ) ) {
                using var _ = ImRaii.PushIndent( 10f );
                using var __ = ImRaii.PushId( "Light2" );
                updated |= Config.Light2.Draw();
            }

            if( updated ) {
                Plugin.DirectXManager.Redraw();
                Config.Save();
            }
        }

        public static void DrawDirectXVfx() {
            if( ImGui.InputFloat2( "Preview Pyramid Size", ref Config.ModelEmittersSize ) ) {
                Config.Save();
                Plugin.DirectXManager.ModelRenderer.UpdatePyramidMesh();
                Plugin.DirectXManager.Redraw();
            }
        }

        private static void DrawEditorSpecific() {
            using var child = ImRaii.Child( "EditorSpecific" );

            foreach( var config in Config.ManagerConfigs ) {
                using var _ = ImRaii.PushId( config.Key );

                if( ImGui.CollapsingHeader( config.Key ) ) {
                    using var indent = ImRaii.PushIndent( 5f );

                    if( ImGui.Checkbox( "Use Custom Window Color", ref config.Value.UseCustomWindowColor ) && config.Value.UseCustomWindowColor && !config.Value.HasTitleColors ) {
                        config.Value.SetTitleColors( *ImGui.GetStyleColorVec4( ImGuiCol.TitleBg ), *ImGui.GetStyleColorVec4( ImGuiCol.TitleBgActive ), *ImGui.GetStyleColorVec4( ImGuiCol.TitleBgCollapsed ) );
                    }
                    if( config.Value.UseCustomWindowColor ) {
                        if( ImGui.ColorEdit4( "Background", ref config.Value.TitleBg ) ) Config.Save();
                        if( ImGui.ColorEdit4( "Active", ref config.Value.TitleBgActive ) ) Config.Save();
                        if( ImGui.ColorEdit4( "Collapsed", ref config.Value.TitleBgCollapsed ) ) Config.Save();
                    }
                }
            }
        }
    }
}
