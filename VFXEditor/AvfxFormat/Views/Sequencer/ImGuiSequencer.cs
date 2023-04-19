using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public abstract class ImGuiSequencer<T> : IUiItem where T : class, ISelectableUiItem {
        private enum MovingType : int {
            LeftHandle = 1,
            RightHandle = 2,
            Middle = 3
        };

        private struct Rect {
            public Vector2 Min;
            public Vector2 Max;
        };

        private static readonly int ItemHeight = 20;
        private static readonly int LegendWidth = 200;
        private static readonly float MinBarWidth = 44f;

        private float FramePixelWidth = 10f;
        private float FramePixelWidthTarget = 10f;

        public readonly List<T> Items;
        public T Selected = null;

        private T MovingEntry = null;
        private int MovingMousePos;
        private MovingType MovingPart;
        private int MovingStart;
        private int MovingEnd;
        private bool MovingScrollBar = false;

        private int FirstFrame = 0;
        private bool PanningView = false;
        private Vector2 PanningViewSource;
        private int PanningViewFrame;

        private bool SizingLeft = false;
        private bool SizingRight = false;

        public ImGuiSequencer( List<T> items ) {
            Items = items;
            UpdateIdx();
        }

        public void Draw( string parentId ) {
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
            FramePixelWidth = UiUtils.Lerp( FramePixelWidth, FramePixelWidthTarget, 0.33f );

            frameCount = frameMax - frameMin;
            if( visibleFrameCount >= frameCount ) {
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

            if( AddDeleteButton( drawList, new Vector2( canvasPos.X + LegendWidth - ItemHeight, canvasPos.Y + 2 ), true ) && UiUtils.MouseClicked() ) {
                OnNew();
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

            var hoveredRow = -1;

            for( var i = 0; i < count; i++ ) {
                var ignoreSelected = false;
                var item = Items[i];
                var itemPosBase = new Vector2( contentMin.X, contentMin.Y + i * ItemHeight );
                var itemPos = new Vector2( contentMin.X + 3, contentMin.Y + i * ItemHeight + 2 );

                if( cursorY >= itemPosBase.Y && cursorY < itemPos.Y + ItemHeight && MovingEntry == null && cursorX > contentMin.X && cursorX < contentMin.X + canvasSize.X ) {
                    hoveredRow = i;
                    var color = ( ( i & 1 ) == 1 ? 0xFF3A3636 : 0xFF413D3D ) + 0x80201008;
                    drawList.AddRectFilled( itemPosBase + new Vector2( 0, 1 ), itemPosBase + new Vector2( LegendWidth, ItemHeight ), color, 0 );
                }

                var overCheck = CheckBox( drawList, itemPos, IsEnabled( item ) );
                var text = item.GetText().Length < 22 ? item.GetText() : item.GetText()[..19] + "...";
                var textColor = item == Selected ? Plugin.Configuration.TimelineSelectedColor : new Vector4( 1 ); // Selected text color <-----------------
                drawList.AddText( itemPos + new Vector2( 25, -1 ), ImGui.GetColorU32( textColor ), text );
                if( !newItemAdded && overCheck && UiUtils.MouseClicked() && UiUtils.MouseOver( childFramePos, childFramePosMax ) ) {
                    ignoreSelected = true;
                    Toggle( item );
                }

                var overDelete = AddDeleteButton( drawList, new Vector2( contentMin.X + LegendWidth - ItemHeight + 2 - 10, itemPos.Y ), false );
                if( !newItemAdded && overDelete && UiUtils.MouseClicked() && UiUtils.MouseOver( childFramePos, childFramePosMax ) ) {
                    ignoreSelected = true;
                    deleteEntry = item;
                }

                if( !newItemAdded && !ignoreSelected && UiUtils.Contains( itemPosBase, itemPosBase + new Vector2( 150, ItemHeight ), io.MousePos ) && UiUtils.MouseOver( childFramePos, childFramePosMax ) ) {
                    if( UiUtils.MouseClicked() ) Selected = item; // Select from left side
                    if( UiUtils.DoubleClicked() ) OnDoubleClick( item );
                }
            }

            // ==== RIGHT SIDE ========

            // Row background
            for( var i = 0; i < count; i++ ) {
                var color = ( i & 1 ) == 1 ? 0xFF3A3636 : 0xFF413D3D + ( hoveredRow == i ? 0 : 0x80201008 );

                var itemPos = new Vector2( contentMin.X + LegendWidth, contentMin.Y + ItemHeight * i + 1 );
                var itemSize = new Vector2( canvasSize.X + canvasPos.X, itemPos.Y + ItemHeight - 1 );

                drawList.AddRectFilled( itemPos, itemSize, color, 0 );
            }

            drawList.PushClipRect( childFramePos + new Vector2( LegendWidth, 0 ), childFramePos + childFrameSize );

            for( var i = frameMin; i <= frameMax; i += frameStep ) {
                DrawVerticalLine( i );
            }
            DrawVerticalLine( frameMin );
            DrawVerticalLine( frameMax );

            if( Selected != null ) {
                // Selected row background
                drawList.AddRectFilled(
                    new Vector2( contentMin.X, contentMin.Y + ItemHeight * Selected.GetIdx() ),
                    new Vector2( contentMin.X + canvasSize.X, contentMin.Y + ItemHeight * ( Selected.GetIdx() + 1 ) ),
                    ImGui.GetColorU32( Plugin.Configuration.TimelineSelectedColor * new Vector4( 1, 1, 1, 0.75f ) ),
                    1
                );
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

                var color = ImGui.GetColorU32( Plugin.Configuration.TimelineBarColor ); // <-------- Row color
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
                        if( !UiUtils.Contains( rect.Min, rect.Max, io.MousePos ) ) continue;
                        drawList.AddRectFilled( rect.Min, rect.Max, quadColor[j], 2 );
                    }

                    for( var j = 0; j < 3; j++ ) {
                        var rect = rects[j];
                        if( !UiUtils.Contains( rect.Min, rect.Max, io.MousePos ) ) continue;
                        if( !UiUtils.Contains( childFramePos, childFramePos + childFrameSize, io.MousePos ) ) continue;

                        // Start dragging an entry
                        if( UiUtils.MouseClicked() && !MovingScrollBar ) {
                            MovingEntry = item;
                            MovingMousePos = cursorX;
                            MovingPart = ( MovingType )j + 1;
                            MovingStart = GetStart( MovingEntry );
                            MovingEnd = GetEnd( MovingEntry );
                            break;
                        }
                    }
                }

                if( infinite ) drawList.AddText( UiBuilder.IconFont, 12, new Vector2( contentMax.X - 22, contentMin.Y + i * ItemHeight + 5 ), 0xFFFFFFFF, $"{( char )FontAwesomeIcon.Infinity}" );
            }

            if( MovingEntry != null ) {
                var diffFrame = ( int )( ( cursorX - MovingMousePos ) / FramePixelWidth );

                if( Math.Abs( diffFrame ) > 0 ) {
                    var l = GetStart( MovingEntry );
                    var r = GetEnd( MovingEntry );
                    var isInfinite = r == -1;

                    Selected = MovingEntry; // Select by dragging bar

                    var changeLeft = ( MovingPart == MovingType.LeftHandle || MovingPart == MovingType.Middle );
                    if( changeLeft ) l += diffFrame;

                    if( isInfinite ) {
                        if( l < 0 ) l = 0;
                    }
                    else {
                        var changeRight = ( MovingPart == MovingType.RightHandle || MovingPart == MovingType.Middle );
                        if( changeRight ) r += diffFrame;

                        if( l < 0 ) {
                            if( changeRight ) r -= l;
                            l = 0;
                        }
                        if( changeLeft && l > r ) l = r;
                        if( changeRight && r < l ) r = l;
                    }
                    MovingMousePos += ( int )( diffFrame * FramePixelWidth );

                    SetPos( MovingEntry, l, r );
                }

                if( UiUtils.DoubleClicked() ) OnDoubleClick( MovingEntry );

                if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) {
                    if( diffFrame == 0 && MovingPart > 0 ) Selected = MovingEntry; // Select by clicking bar
                    OnDragEnd( MovingEntry, MovingStart, GetStart( MovingEntry ), MovingEnd, GetEnd( MovingEntry ) );
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

            var inScrollBar = UiUtils.Contains( scrollBarA, scrollBarB, io.MousePos );
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

            var onLeft = UiUtils.Contains( barHandleLeft.Min, barHandleLeft.Max, io.MousePos );
            var onRight = UiUtils.Contains( barHandleRight.Min, barHandleRight.Max, io.MousePos );

            drawList.AddRectFilled( barHandleLeft.Min, barHandleLeft.Max, ( onLeft || SizingLeft ) ? 0xFFAAAAAA : 0xFF666666, 6 );
            drawList.AddRectFilled( barHandleRight.Min, barHandleRight.Max, ( onRight || SizingRight ) ? 0xFFAAAAAA : 0xFF666666, 6 );

            var scrollBarThumb = new Rect {
                Min = scrollBarC,
                Max = scrollBarD
            };

            if( SizingRight ) {
                if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) SizingRight = false;
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
            else if( SizingLeft ) {
                if( !ImGui.IsMouseDown( ImGuiMouseButton.Left ) ) SizingLeft = false;
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
                    if( UiUtils.Contains( scrollBarThumb.Min, scrollBarThumb.Max, io.MousePos ) && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && MovingEntry == null ) {
                        MovingScrollBar = true;
                        PanningViewSource = io.MousePos;
                        PanningViewFrame = -FirstFrame;
                    }
                    if( !SizingRight && onRight && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) SizingRight = true;
                    if( !SizingLeft && onLeft && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) SizingLeft = true;
                }
            }

            ImGui.EndGroup();

            if( deleteEntry != null ) {
                if( deleteEntry == Selected ) Selected = null;
                OnDelete( deleteEntry );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( Selected != null ) {
                ImGui.BeginChild( $"{parentId}-Child" );
                Selected.Draw( parentId );
                ImGui.EndChild();
            }
            else {
                ImGui.Text( "Select an item..." );
            }
        }

        public void UpdateIdx() {
            for( var i = 0; i < Items.Count; i++ ) Items[i].SetIdx( i );
        }

        public void ClearSelected() { Selected = null; }

        public abstract int GetStart( T item );
        public abstract int GetEnd( T item );
        public abstract void SetPos( T item, int start, int end );
        public abstract void OnDragEnd( T item, int startBegin, int startFinish, int endBegin, int endFinish );
        public abstract void OnDelete( T item );
        public abstract void OnNew();
        public abstract bool IsEnabled( T item );
        public abstract void Toggle( T item );
        public abstract void OnDoubleClick( T item );

        private static bool AddDeleteButton( ImDrawListPtr drawList, Vector2 pos, bool add = true ) {
            var size = new Vector2( 16, 16 );
            var posMax = pos + size;
            var overDelete = UiUtils.MouseOver( pos, posMax );
            var deleteColor = overDelete ? ImGui.GetColorU32( Plugin.Configuration.TimelineSelectedColor ) : 0xFFBBBBBB;
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
            var overDelete = UiUtils.MouseOver( pos, posMax );
            var deleteColor = overDelete ? ImGui.GetColorU32( Plugin.Configuration.TimelineSelectedColor ) : 0xFFBBBBBB;
            drawList.AddRect( pos, posMax, deleteColor, 4 );
            if( on ) drawList.AddRectFilled( pos + new Vector2( 2 ), posMax - new Vector2( 2 ), deleteColor, 4 );
            return overDelete;
        }
    }
}
