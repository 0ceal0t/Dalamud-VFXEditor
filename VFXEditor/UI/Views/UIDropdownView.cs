using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VFXEditor.UI.VFX
{
    public abstract class UIDropdownView : UIBase
    {
        public int Selected = -1;
        public string[] Options;
        public string id;
        public string defaultText;

        public bool AllowNew;
        public bool AllowDelete;

        public UIDropdownView(string _id, string _defaultText, bool allowNew = true, bool allowDelete = true )
        {
            id = _id;
            defaultText = _defaultText;
            AllowNew = allowNew;
            AllowDelete = allowDelete;
        }

        public abstract void RefreshDesc( int idx );
        public abstract void OnNew();
        public abstract void OnDelete( int idx );
        public abstract void OnDraw( int idx );
        public abstract byte[] OnExport( int idx );
        public abstract void OnImport( AVFXLib.AVFX.AVFXNode node );

        public override void Draw( string parentId = "" )
        {
            bool validSelect = ViewSelect( id, defaultText, ref Selected, Options );
            if( AllowNew )
            {
                ImGui.SameLine();
                if( ImGui.SmallButton( "+ New" + id ) )
                {
                    ImGui.OpenPopup( "New_Popup" + id );
                }
                if( ImGui.BeginPopup( "New_Popup" + id ) )
                {
                    if( ImGui.Selectable( "Create" + id ) )
                    {
                        OnNew();
                        Init();
                    }
                    if( ImGui.Selectable("Load" + id ) )
                    {
                        Load();
                    }
                    ImGui.EndPopup();
                }
            }
            if( validSelect && AllowDelete )
            {
                ImGui.SameLine();
                if( ImGui.SmallButton( "Save" + id ) )
                {
                    Save( Selected );
                }
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "Delete" + id, small:true) )
                {
                    OnDelete( Selected );
                    Init();
                    validSelect = false;
                }
            }
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                OnDraw( Selected );
            }
        }
        // ========================
        public static bool ViewSelect( string id, string defaultText, ref int Selected, string[] Options ) {
            bool validSelect = ( Selected >= 0 && Selected < Options.Length );
            var selectedString = validSelect ? Options[Selected] : defaultText;
            if( ImGui.BeginCombo( "Select" + id, selectedString ) ) {
                for( int i = 0; i < Options.Length; i++ ) {
                    bool isSelected = ( Selected == i );
                    if( ImGui.Selectable( Options[i] + id, isSelected ) ) {
                        Selected = i;
                    }
                    if( isSelected ) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            return validSelect;
        }
        // ========= DIALOGS ==================
        public void Load()
        {
            Task.Run( async () => {
                var picker = new OpenFileDialog
                {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a File Location.",
                    CheckFileExists = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var node = AVFXLib.Main.Reader.readAVFX( picker.FileName, out List<string> messages );
                        foreach( string message in messages ) {
                            PluginLog.Log( message );
                        }
                        OnImport( node );
                        Init();
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }

        public void Save( int idx)
        {
            Task.Run( async () => {
                var picker = new SaveFileDialog
                {
                    Filter = "Partial AVFX (*.vfxedit)|*.vfxedit*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "vfxedit",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK ) {
                    try {
                        var data = OnExport( idx );
                        File.WriteAllBytes( picker.FileName, data );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a file" );
                    }
                }
            } );
        }
    }
}
