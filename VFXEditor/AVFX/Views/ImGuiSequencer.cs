using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VFXEditor.AVFX.VFX;

namespace VFXEditor.AVFX.Views {
    public abstract class ImGuiSequencer<T> : IUIBase where T : UIItem {
        private static readonly int ItemHeight = 20;
        private static readonly int LegendWidth = 200;
        private static readonly float MinBarWidth = 44f;

        private float FramePixelWidth = 10f;
        private float FramePixelWidthTarget = 10f;

        private readonly List<T> Items;
        private T Selected = null;

        private T MovingEntry = null;
        private int MovingPos;
        private int MovingPart;

        private int FirstFrame = 0;

        private bool MovingScrollBar = false;

        private bool PanningView = false;
        private Vector2 PanningViewSource;
        private int PanningViewFrame;

        private bool SizingLBar = false;
        private bool SizingRBar = false;

        private struct Rect {
            public Vector2 Min;
            public Vector2 Max;
        }

        public ImGuiSequencer( List<T> items ) {
            Items = items;
            SetupIdx();
        }

        private void SetupIdx() {
            for( var i = 0; i < Items.Count; i++ ) {
                Items[i].Idx = i;
            }
        }

        public void DrawInline( string parentId ) {
            var io = ImGui.GetIO();
            var cursorX = ( int )io.MousePos.X;
            var cursorY = ( int )io.MousePos.Y;

            T deleteEntry = null;
            var newItemAdded = false;

            ImGui.BeginGroup();
            var drawList = ImGui.GetWindowDrawList();
            var canvasPos = ImGui.GetCursorScreenPos();
            var canvasSize = ImGui.GetContentRegionAvail();
            canvasSize.Y = 400;

            var count = Items.Count;
            var controlHeight = count * ItemHeight;

            var firstFrameUsed = FirstFrame;

            var frameMax = 0;
            var frameMin = 1;
            foreach( var item in Items ) {
                frameMin = Math.Min( GetStart( item ), frameMin );
                frameMax = Math.Max( GetEnd( item ), frameMax );
            }
            var frameCount = Math.Max( frameMax - frameMin, 1 );

            var visibleFrameCount = ( int )Math.Floor( ( canvasSize.X - LegendWidth ) / FramePixelWidth );
            var maxFrameVisible = firstFrameUsed + visibleFrameCount;
            var barWidthRatio = Math.Min( visibleFrameCount / ( float )frameCount, 1f );
            var barWidthInPixels = barWidthRatio * ( canvasSize.X - LegendWidth );

            if( ImGui.IsWindowFocused() && io.KeyAlt && ImGui.IsMouseDown( ImGuiMouseButton.Right ) ) {
                if( ( !PanningView ) ) {
                    PanningViewSource = io.MousePos;
                    PanningView = true;
                    PanningViewFrame = FirstFrame;
                }
                FirstFrame = PanningViewFrame - ( int )( ( io.MousePos.X - PanningViewSource.X ) / FramePixelWidth );
                FirstFrame = Math.Clamp( FirstFrame, frameMin, frameMax - visibleFrameCount );
            }
            if( PanningView && !ImGui.IsMouseDown( ImGuiMouseButton.Right ) ) {
                PanningView = false;
            }
            FramePixelWidthTarget = Math.Clamp( FramePixelWidthTarget, 0.1f, 50f );
            FramePixelWidth = Lerp( FramePixelWidth, FramePixelWidthTarget, 0.33f );

            frameCount = frameMax - frameMin;
            if( visibleFrameCount >= frameCount ) { // TODO: if (visibleFrameCount >= frameCount && firstFrame)
                FirstFrame = frameMin;
            }

            var headerSize = new Vector2( canvasSize.X, ItemHeight );
            var scrollBarSize = new Vector2( canvasSize.X, 14f );
            ImGui.InvisibleButton( "SeqTopBar", headerSize );
            drawList.AddRectFilled( canvasPos, canvasPos + headerSize, 0xFFFF0000, 0 );
            var childFramePos = ImGui.GetCursorScreenPos();
            var childFrameSize = new Vector2( canvasSize.X, canvasSize.Y - 8f - headerSize.Y - scrollBarSize.Y );
            var childFramePosMax = childFramePos + childFrameSize;
            ImGui.PushStyleColor( ImGuiCol.FrameBg, 0 );
            ImGui.BeginChildFrame( 889, childFrameSize );
            ImGui.InvisibleButton( "SeqContentBar", new Vector2( canvasSize.X, controlHeight ) );

            var contentMin = ImGui.GetItemRectMin();
            var contentMax = ImGui.GetItemRectMax();

            drawList.AddRectFilled( canvasPos, canvasPos + canvasSize, 0xFF242424, 0 );

            drawList.AddRectFilled( canvasPos, new Vector2( canvasPos.X + canvasSize.X, canvasPos.Y + ItemHeight ), 0xFF3D3837, 0 );

            if( AddDeleteButton( drawList, new Vector2( canvasPos.X + LegendWidth - ItemHeight, canvasPos.Y + 2 ), true ) && MouseClicked() ) {
                Selected = OnNew();
                Selected.Idx = Items.Count;
                Items.Add( Selected );
                newItemAdded = true;
            }

            var modFrameCount = 10;
            var frameStep = 1;
            while( ( modFrameCount * FramePixelWidth ) < 150 ) {
                modFrameCount *= 2;
                frameStep *= 2;
            }
            var halfModFrameCount = modFrameCount / 2;

            void DrawHeaderTick( int i, int regionHeight ) {
                var baseIndex = ( ( i % modFrameCount ) == 0 ) || ( i == frameMax || i == frameMin );
                var halfIndex = ( i % halfModFrameCount ) == 0;
                var px = ( int )canvasPos.X + ( int )( i * FramePixelWidth ) + LegendWidth - ( int )( firstFrameUsed * FramePixelWidth );
                var tiretStart = baseIndex ? 4 : ( halfIndex ? 10 : 14 );
                var tiretEnd = baseIndex ? regionHeight : ItemHeight;

                if( px <= ( canvasSize.X + canvasPos.X ) && px >= ( canvasPos.X + LegendWidth ) ) {
                    drawList.AddLine( new Vector2( px, canvasPos.Y + tiretStart ), new Vector2( px, canvasPos.Y + tiretEnd - 1 ), 0xFF606060, 1 );
                    drawList.AddLine( new Vector2( px, canvasPos.Y + ItemHeight ), new Vector2( px, canvasPos.Y + regionHeight - 1 ), 0x30606060, 1 );
                }

                if( baseIndex && px > ( canvasPos.X + LegendWidth ) ) {
                    drawList.AddText( new Vector2( px + 3f, canvasPos.Y ), 0xFFBBBBBB, $"{i}" );
                }
            }

            void DrawVerticalLine( int i ) {
                var px = ( int )canvasPos.X + ( int )( i * FramePixelWidth ) + LegendWidth - ( int )( firstFrameUsed * FramePixelWidth );
                var tiretStart = ( int )contentMin.Y;
                var tiretEnd = ( int )contentMax.Y;

                if( px <= ( canvasPos.X + canvasSize.X ) && px >= ( canvasPos.X + LegendWidth ) ) {
                    drawList.AddLine( new Vector2( px, tiretStart ), new Vector2( px, tiretEnd ), 0x30606060, 1 );
                }
            }

            drawList.PushClipRect( canvasPos, canvasPos + new Vector2( canvasSize.X, ItemHeight ) );

            for( var i = frameMin; i <= frameMax; i += frameStep ) {
                DrawHeaderTick( i, ItemHeight );
            }
            DrawHeaderTick( frameMin, ItemHeight );
            DrawHeaderTick( frameMax, ItemHeight );

            drawList.PopClipRect();

            drawList.PushClipRect( childFramePos, childFramePosMax );

            // ======= LEFT SIDE SELECT ======

            for( var i = 0; i < count; i++ ) {
                var ignoreSelected = false;
                var item = Items[i];
                var itemPosBase = new Vector2( contentMin.X, contentMin.Y + i * ItemHeight );
                var itemPos = new Vector2( contentMin.X + 3, contentMin.Y + i * ItemHeight + 2 );

                var overCheck = CheckBox( drawList, itemPos, IsEnabled(item) );
                drawList.AddText( itemPos + new Vector2(25, -1), 0xFFFFFFFF, item.GetText() );
                if( !newItemAdded && overCheck && MouseClicked() && MouseOver( childFramePos, childFramePosMax ) ) {
                    ignoreSelected = true;
                    Toggle( item );
                }

                var overDelete = AddDeleteButton( drawList, new Vector2( contentMin.X + LegendWidth - ItemHeight + 2 - 10, itemPos.Y ), false );
                if( !newItemAdded && overDelete && MouseClicked() && MouseOver( childFramePos, childFramePosMax ) ) {
                    ignoreSelected = true;
                    deleteEntry = item;
                }

                if( !newItemAdded && !ignoreSelected && Contains( itemPosBase, itemPosBase + new Vector2( 150, ItemHeight ), io.MousePos ) && MouseClicked() && MouseOver( childFramePos, childFramePosMax ) ) Selected = item;
            }

            for( var i = 0; i < count; i++ ) {
                var color = ( i & 1 ) == 1 ? 0xFF3A3636 : 0xFF413D3D;

                var itemPos = new Vector2( contentMin.X + LegendWidth, contentMin.Y + ItemHeight * i + 1 );
                var itemSize = new Vector2( canvasSize.X + canvasPos.X, itemPos.Y + ItemHeight - 1 );
                if( cursorY >= itemPos.Y && cursorY < itemPos.Y + ItemHeight && MovingEntry == null && cursorX > contentMin.X && cursorX < contentMin.X + canvasSize.X ) {
                    color += 0x80201008;
                    itemPos.X -= LegendWidth;
                }
                drawList.AddRectFilled( itemPos, itemSize, color, 0 );
            }

            drawList.PushClipRect( childFramePos + new Vector2( LegendWidth, 0 ), childFramePos + childFrameSize );

            for( var i = frameMin; i <= frameMax; i += frameStep ) {
                DrawVerticalLine( i );
            }
            DrawVerticalLine( frameMin );
            DrawVerticalLine( frameMax );

            if( Selected != null ) {
                drawList.AddRectFilled( new Vector2( contentMin.X, contentMin.Y + ItemHeight * Selected.Idx ), new Vector2( contentMin.X + canvasSize.X, contentMin.Y + ItemHeight * ( Selected.Idx + 1 ) ), 0x801080FF, 1 );
            }

            for( var i = 0; i < count; i++ ) {
                var item = Items[i];
                var start = GetStart( item );
                var end = GetEnd( item );
                var infinite = end == -1;
                if( infinite ) end = maxFrameVisible + 5; // infinite, so extend it off the edge of the view

                var itemPos = new Vector2( contentMin.X + LegendWidth - firstFrameUsed * FramePixelWidth, contentMin.Y + ItemHeight * i + 1 );
                var slotP1 = new Vector2( itemPos.X + start * FramePixelWidth, itemPos.Y + 2 );
                var slotP2 = new Vector2( itemPos.X + end * FramePixelWidth + FramePixelWidth, itemPos.Y + ItemHeight - 2 );
                var slotP3 = new Vector2( itemPos.X + end * FramePixelWidth + FramePixelWidth, itemPos.Y + ItemHeight - 2 );

                var color = 0xFFAA8080;
                var slotColor = color | 0xFF000000;
                var slotColorHalf = ( color & 0xFFFFFF ) | 0x40000000;

                if( slotP1.X <= ( canvasSize.X + contentMin.X ) && slotP2.X >= ( contentMin.X + LegendWidth ) ) {
                    drawList.AddRectFilled( slotP1, slotP3, slotColorHalf, 2 );
                    drawList.AddRectFilled( slotP1, slotP2, slotColor, 2 );
                }

                var rects = new[] {
                    new Rect {
                        Min = slotP1,
                        Max = new Vector2(slotP1.X + FramePixelWidth / 2, slotP2.Y)
                    },
                    new Rect {
                        Min = new Vector2(slotP2.X - FramePixelWidth / 2, slotP1.Y),
                        Max = slotP2
                    },
                    new Rect {
                        Min = slotP1,
                        Max = slotP2
                    }
                };

                var quadColor = new[] {
                    0xFFFFFFFF,
                    0xFFFFFFFF,
                    ( uint )(slotColor + ( Selected != null ? 0x0 : 0x202020) )
                };

                if( MovingEntry == null ) {
                    for( var j = 2; j >= 0; j-- ) {
                        var rect = rects[j];
                        if( !Contains( rect.Min, rect.Max, io.MousePos ) ) continue;
                        drawList.AddRectFilled( rect.Min, rect.Max, quadColor[j], 2 );
                    }

                    for( var j = 0; j < 3; j++ ) {
                        var rect = rects[j];
                        if( !Contains( rect.Min, rect.Max, io.MousePos ) ) continue;
                        if( !Contains( childFramePos, childFramePos + childFrameSize, io.MousePos ) ) continue;
                        if( MouseClicked() && !MovingScrollBar ) {
                            MovingEntry = item;
                            MovingPos = cursorX;
                            MovingPart = j + 1;
                            break;
                        }
                    }
                }

                if( infinite ) drawList.AddText( UiBuilder.IconFont, 12, new Vector2( contentMax.X - 22, contentMin.Y + i * ItemHeight + 5 ), 0xFFFFFFFF, $"{( char )FontAwesomeIcon.Infinity}" );
            }

            if( MovingEntry != null ) {
                // TODO: ImGui::CaptureMouseFromApp();
                var diffFrame = ( int )( ( cursorX - MovingPos ) / FramePixelWidth );

                if( Math.Abs( diffFrame ) > 0 ) {
                    var l = GetStart( MovingEntry );
                    var r = GetEnd( MovingEntry );
                    var isInfinite = r == -1;

                    Selected = MovingEntry;

                    var changeLeft = ( MovingPart == 1 || MovingPart == 3 );
                    if( changeLeft ) l += diffFrame;

                    if( isInfinite ) {
                        if( l < 0 ) l = 0;
                    }
                    else {
                        var changeRight = ( MovingPart == 2 || MovingPart == 3 );
                        if( changeRight ) r += diffFrame;

                        if( l < 0 ) {
                            if( changeRight ) r -= l;
                            l = 0;
                        }
                        if( changeLeft && l > r ) l = r;
                        if( changeRight && r < l ) r = l;
                    }
                    MovingPos += ( int )( diffFrame * FramePixelWidth );

                    SetStart( MovingEntry, l );
                    SetEnd( MovingEntry, r );
                }
                if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) {
                    if( diffFrame == 0 && MovingPart > 0 ) {
                        Selected = MovingEntry;
                    }

                    MovingEntry = null;
                }
            }

            drawList.PopClipRect();
            drawList.PopClipRect();

            ImGui.EndChildFrame();
            ImGui.PopStyleColor();

            ImGui.InvisibleButton( "SeqScollBar", scrollBarSize );
            var scrollBarMin = ImGui.GetItemRectMin();
            var scrollBarMax = ImGui.GetItemRectMax();

            var startFrameOffset = ( ( float )( firstFrameUsed - frameMin ) / frameCount ) * ( canvasSize.X - LegendWidth );
            var scrollBarA = new Vector2( scrollBarMin.X + LegendWidth, scrollBarMin.Y - 2 );
            var scrollBarB = new Vector2( scrollBarMin.X + canvasSize.X, scrollBarMax.Y - 1 );
            drawList.AddRectFilled( scrollBarA, scrollBarB, 0xFF222222, 0 );

            var inScrollBar = Contains( scrollBarA, scrollBarB, io.MousePos );
            drawList.AddRectFilled( scrollBarA, scrollBarB, 0xFF101010, 8 );

            var scrollBarC = new Vector2( scrollBarMin.X + LegendWidth + startFrameOffset, scrollBarMin.Y );
            var scrollBarD = new Vector2( scrollBarMin.X + LegendWidth + barWidthInPixels + startFrameOffset, scrollBarMax.Y - 2 );
            drawList.AddRectFilled( scrollBarC, scrollBarD, ( inScrollBar || MovingScrollBar ) ? 0xFF606060 : 0xFF505050, 6 );

            var barHandleLeft = new Rect {
                Min = scrollBarC,
                Max = new Vector2( scrollBarC.X + 14, scrollBarD.Y )
            };
            var barHandleRight = new Rect {
                Min = new Vector2( scrollBarD.X - 14, scrollBarC.Y ),
                Max = scrollBarD
            };

            var onLeft = Contains( barHandleLeft.Min, barHandleLeft.Max, io.MousePos );
            var onRight = Contains( barHandleRight.Min, barHandleRight.Max, io.MousePos );

            drawList.AddRectFilled( barHandleLeft.Min, barHandleLeft.Max, ( onLeft || SizingLBar ) ? 0xFFAAAAAA : 0xFF666666, 6 );
            drawList.AddRectFilled( barHandleRight.Min, barHandleRight.Max, ( onRight || SizingRBar ) ? 0xFFAAAAAA : 0xFF666666, 6 );

            var scrollBarThumb = new Rect {
                Min = scrollBarC,
                Max = scrollBarD
            };

            if( SizingRBar ) {
                if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) SizingRBar = false;
                else {
                    var barNewWidth = Math.Max( barWidthInPixels + io.MouseDelta.X, MinBarWidth );
                    var barRatio = barNewWidth / barWidthInPixels;
                    FramePixelWidthTarget = FramePixelWidth /= barRatio;
                    var newVisibleFrameCount = ( int )( ( canvasSize.X - LegendWidth ) / FramePixelWidthTarget );
                    var lastFrame = FirstFrame + newVisibleFrameCount;
                    if( lastFrame > frameMax ) {
                        FramePixelWidthTarget = FramePixelWidth = ( canvasSize.X - LegendWidth ) / ( frameMax - FirstFrame );
                    }
                }
            }
            else if( SizingLBar ) {
                if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) SizingLBar = false;
                else {
                    var barNewWidth = Math.Max( barWidthInPixels - io.MouseDelta.X, MinBarWidth );
                    var barRatio = barNewWidth / barWidthInPixels;
                    var previousFramePixelWidthTarget = FramePixelWidthTarget;
                    FramePixelWidthTarget = FramePixelWidth /= barRatio;
                    var newVisibleFrameCount = ( int )( visibleFrameCount / barRatio );
                    var newFirstFrame = FirstFrame + newVisibleFrameCount - visibleFrameCount;
                    newFirstFrame = Math.Clamp( newFirstFrame, frameMin, Math.Max( frameMax - visibleFrameCount, frameMin ) );
                    if( newFirstFrame == FirstFrame ) {
                        FramePixelWidth = FramePixelWidthTarget = previousFramePixelWidthTarget;
                    }
                    else {
                        FirstFrame = newFirstFrame;
                    }
                }
            }
            else {
                if( MovingScrollBar ) {
                    if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) MovingScrollBar = false;
                    else {
                        var framesPerPixelInBar = barWidthInPixels / visibleFrameCount;
                        FirstFrame = ( int )( ( ( io.MousePos.X - PanningViewSource.X ) / framesPerPixelInBar ) - PanningViewFrame );
                        FirstFrame = Math.Clamp( FirstFrame, frameMin, Math.Max( frameMax - visibleFrameCount, frameMin ) );
                    }
                }
                else {
                    if( Contains( scrollBarThumb.Min, scrollBarThumb.Max, io.MousePos ) && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && MovingEntry == null ) {
                        MovingScrollBar = true;
                        PanningViewSource = io.MousePos;
                        PanningViewFrame = -FirstFrame;
                    }
                    if( !SizingRBar && onRight && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) SizingRBar = true;
                    if( !SizingLBar && onLeft && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) SizingLBar = true;
                }
            }

            ImGui.EndGroup();

            if( deleteEntry != null ) {
                if( deleteEntry == Selected ) Selected = null;
                Items.Remove( deleteEntry );
                OnDelete( deleteEntry );
                SetupIdx();
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( Selected != null ) {
                Selected.DrawInline( parentId );
            }
            else {
                ImGui.Text( "Select an item..." );
            }
        }

        public abstract int GetStart( T item );
        public abstract int GetEnd( T item );
        public abstract void SetStart( T item, int start );
        public abstract void SetEnd( T item, int end );
        public abstract void OnDelete( T item );
        public abstract T OnNew();
        public abstract bool IsEnabled( T item );
        public abstract void Toggle( T item );

        private static bool AddDeleteButton( ImDrawListPtr drawList, Vector2 pos, bool add = true ) {
            var size = new Vector2( 16, 16 );
            var posMax = pos + size;
            var overDelete = MouseOver( pos, posMax );
            var deleteColor = overDelete ? 0xFF1080FF : 0xFFBBBBBB;
            var midY = pos.Y + 16 / 2 - 0.5f;
            var midX = pos.X + 16 / 2 - 0.5f;
            drawList.AddRect( pos, posMax, deleteColor, 4 );
            drawList.AddLine( new Vector2( pos.X + 3, midY ), new Vector2( posMax.X - 3, midY ), deleteColor, 2 );
            if( add ) drawList.AddLine( new Vector2( midX, pos.Y + 3 ), new Vector2( midX, posMax.Y - 3 ), deleteColor, 2 );
            return overDelete;
        }

        private static bool CheckBox( ImDrawListPtr drawList, Vector2 pos, bool on ) {
            var size = new Vector2( 16, 16 );
            var posMax = pos + size;
            var overDelete = MouseOver( pos, posMax );
            var deleteColor = overDelete ? 0xFF1080FF : 0xFFBBBBBB;
            drawList.AddRect( pos, posMax, deleteColor, 4 );
            if( on ) drawList.AddRectFilled( pos + new Vector2(2), posMax - new Vector2(2), deleteColor, 4 );
            return overDelete;
        }

        private static bool MouseOver( Vector2 start, Vector2 end ) {
            var io = ImGui.GetIO();
            return Contains( start, end, io.MousePos );
        }

        private static bool MouseClicked() => ImGui.IsMouseClicked( ImGuiMouseButton.Left );

        private static bool Contains( Vector2 min, Vector2 max, Vector2 point ) {
            return point.X >= min.X && point.Y >= min.Y && point.X <= max.X && point.Y <= max.Y;
        }

        private static float Lerp( float firstFloat, float secondFloat, float by ) {
            return firstFloat * ( 1 - by ) + secondFloat * by;
        }
    }
}
