// Modified on 20-Jun-2013 by Justin Stenning
// From original code by Alexandre Mutel.
// -------------------------------------------------------------------
// Original source in SharpDX.Toolkit.Graphics.FileIncludeHandler
// -------------------------------------------------------------------
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.DirectX {
    public class HLSLFileIncludeHandler : CallbackBase, Include {
        public readonly Stack<string> CurrentDirectory;
        public readonly List<string> IncludeDirectories;


        public HLSLFileIncludeHandler( string initialDirectory ) {
            IncludeDirectories = new List<string>();
            CurrentDirectory = new Stack<string>();
            CurrentDirectory.Push( initialDirectory );
        }

        public Stream Open( IncludeType type, string fileName, Stream parentStream ) {
            var currentDirectory = CurrentDirectory.Peek();
            if( currentDirectory == null )
                currentDirectory = Environment.CurrentDirectory;

            var filePath = fileName;

            if( !Path.IsPathRooted( filePath ) ) {
                var directoryToSearch = new List<string> { currentDirectory };
                directoryToSearch.AddRange( IncludeDirectories );
                foreach( var dirPath in directoryToSearch ) {
                    var selectedFile = Path.Combine( dirPath, fileName );
                    if( NativeFile.Exists( selectedFile ) ) {
                        filePath = selectedFile;
                        break;
                    }
                }
            }

            if( filePath == null || !NativeFile.Exists( filePath ) ) {
                throw new FileNotFoundException( string.Format( "Unable to find file [{0}]", filePath ?? fileName ) );
            }

            NativeFileStream fs = new NativeFileStream( filePath, NativeFileMode.Open, NativeFileAccess.Read );
            CurrentDirectory.Push( Path.GetDirectoryName( filePath ) );
            return fs;
        }

        public void Close( Stream stream ) {
            stream.Dispose();
            CurrentDirectory.Pop();
        }
    }
}