using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Data.DirectX;
using VFXEditor.Data.Texture;

namespace VFXEditor.UI.VFX.Particle.UVSet {
    public class UVAnimation : UIItem {
        UIParticleUVSet UVSet;

        float CurrentRotation = 0;
        Vector2 CurrentScale = new Vector2( 1, 1 );
        Vector2 CurrentScroll = new Vector2( 0, 0 );

        Stopwatch timer;

        UVPreview _UVPreview;

        public UVAnimation(UIParticleUVSet uvSet ) {
            UVSet = uvSet;
            Assigned = true;
            _UVPreview = DirectXManager.Manager.UVView;
            SetFrame();
            timer = new Stopwatch();
            timer.Stop();
        }

        public int Frame;
        public UITexture SelectedTexture = null;
        public UIModel SelectedModel = null;

        public override void DrawBody( string parentId ) {
            if( _UVPreview._CurrentUV != this ) {
                UpdatePreview();
                _UVPreview._CurrentUV = this;
            }

            if( SelectedModel != null && SelectedModel.IsDeleted ) SelectedModel = null;
            if( SelectedTexture != null && SelectedTexture.IsDeleted ) SelectedTexture = null;

            ImGui.TextColored( new Vector4( 0.9f, 0.1f, 0.1f, 1.0f ), "This is just a preview, and does not change any data" );

            // ======== MODEL SELECT ========
            var modelText = SelectedModel == null ? "[NONE]" : SelectedModel.GetText();
            if(ImGui.BeginCombo("Model" + parentId, modelText ) ) {
                var idx = 0;
                foreach(var item in UVSet.Particle.Main.Models.Items ) {
                    if( ImGui.Selectable(item.GetText() + parentId + idx, item == SelectedModel) ) {
                        SelectedModel = item;
                        if(SelectedTexture != null ) {
                            UpdatePreview();
                        }
                    }
                    idx++;
                }
                ImGui.EndCombo();
            }
            // ======= TEXTURE SELECT ==========
            var texText = SelectedTexture == null ? "[NONE]" : SelectedTexture.GetText();
            if( ImGui.BeginCombo( "Texture" + parentId, texText ) ) {
                var idx = 0;
                foreach( var item in UVSet.Particle.Main.Textures.Items ) {
                    if( ImGui.Selectable( item.GetText() + parentId + idx, item == SelectedTexture ) ) {
                        SelectedTexture = item;
                        UpdateBitmap();
                        if( SelectedModel != null ) {
                            UpdatePreview();
                        }
                    }
                    idx++;
                }
                ImGui.EndCombo();
            }

            if(ImGui.InputInt("Frame" + parentId, ref Frame ) ) {
                if( Frame < 0 ) Frame = 0;
                SetFrame();
            }

            if( timer.IsRunning ) {
                if( ImGui.Button( "Stop" + parentId ) ) {
                    timer.Stop();
                }
            }
            else {
                if( ImGui.Button( "Play" + parentId ) ) {
                    timer.Start();
                }
            }
            ImGui.SameLine();
            if( ImGui.Button( "Reset" + parentId ) ) {
                timer.Reset();
                Frame = 0;
                SetFrame();
            }

            if( timer.IsRunning ) {
                int _frame = ( int )( 15.0f * timer.ElapsedMilliseconds / 1000.0f );
                if( _frame != Frame ) {
                    Frame = _frame;
                    SetFrame();
                }
            }

            var cursor = ImGui.GetCursorScreenPos();
            ImGui.BeginChild( "UVViewChild" );

            var space = ImGui.GetContentRegionAvail();
            _UVPreview.Resize( space );

            ImGui.ImageButton( _UVPreview.RenderShad.NativePointer, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                _UVPreview.Drag( delta, true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                _UVPreview.Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                _UVPreview.IsDragging = false;
            }
            if( ImGui.IsItemHovered() ) {
                _UVPreview.Zoom( ImGui.GetIO().MouseWheel );
            }
            ImGui.EndChild();
        }

        Bitmap _bitmap;
        public void UpdateBitmap() {
            if( SelectedTexture == null ) return;
            var tex = TextureManager.Manager.GetTexture( SelectedTexture.Path.Literal.Value.Trim( '\0' ) );
            _bitmap = new Bitmap( tex.Header.Width, tex.Header.Height );
            for( int i = 0; i < tex.Header.Height; i++ ) {
                for( int j = 0; j < tex.Header.Width; j++ ) {
                    int _idx = ( i * tex.Header.Width + j ) * 4;
                    int r = tex.ImageData[_idx];
                    int g = tex.ImageData[_idx + 1];
                    int b = tex.ImageData[_idx + 2];
                    int a = tex.ImageData[_idx + 3];
                    _bitmap.SetPixel( j, i, Color.FromArgb( a, r, g, b ) );
                }
            }
        }

        public void UpdatePreview() {
            if( SelectedModel != null && SelectedTexture != null ) {
                _UVPreview.LoadModel( SelectedModel.Model );
                _UVPreview.LoadTexture( _bitmap );
                SetFrame();
            }
            else {
                _UVPreview.LoadModel( null );
            }
        }

        public void SetFrame( ) {
            CurrentRotation = GetCurveValue( UVSet.Rotation, Frame, 0 );
            CurrentScale = Get2AxisValue( UVSet.Scale, Frame, new Vector2( 1, 1 ) );
            CurrentScroll = Get2AxisValue( UVSet.Scroll, Frame, new Vector2( 0, 0 ) );

            if(_UVPreview._CurrentUV == this ) {
                _UVPreview.AnimData[0] = CurrentScale.X; // kind of scuffed, but whatever
                _UVPreview.AnimData[1] = CurrentScale.Y;
                _UVPreview.AnimData[2] = CurrentScroll.X;
                _UVPreview.AnimData[3] = CurrentScroll.Y;
                _UVPreview.AnimData[4] = CurrentRotation;

                _UVPreview.Draw();
            }
        }

        public static Vector2 Get2AxisValue(UICurve2Axis curve, int frame, Vector2 defaultValue) {
            if( !curve.Assigned ) return default;
            if( curve._AxisConnect.Literal.Value == AxisConnect.X_Y || curve._AxisConnect.Literal.Value == AxisConnect.X_YZ ) {
                var v = GetCurveValue( curve._X, frame, defaultValue.X );
                return new Vector2(v,v);
            }
            else if( curve._AxisConnect.Literal.Value == AxisConnect.Y_X || curve._AxisConnect.Literal.Value == AxisConnect.Y_XZ ) {
                var v = GetCurveValue( curve._Y, frame, defaultValue.Y );
                return new Vector2( v, v );
            }
            return new Vector2(
                GetCurveValue( curve._X, frame, defaultValue.X ),
                GetCurveValue( curve._Y, frame, defaultValue.Y )
            );
        }
        public static float GetCurveValue(UICurve curve, int frame, float defaultValue ) {
            return curve.Assigned && curve.Curve.Keys.Count > 0 ? curve.CurveEdit.GetAtTime( frame ) : defaultValue;
        }

        public override string GetDefaultText() {
            return "UV Animation";
        }
    }
}
