/// - - -    Copyright (c) 2026     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                 "Eclipse/Eclipse.Riptide/LICENSE.md"
/// 
/// Note: Eclipse.Riptide and Eclipse are licensed under different licenses.
/// See "Eclipse/LICENSE.md" for details.
/// 
/// ]]>

using Riptide;

namespace Eclipse.Riptide.Testing
{
    public static class Handlers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Client-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [EclipseMessage]
        public static void SendChunkHandler(SendChunk chunk)
        {

        }

        [EclipseMessage]
        public static void ValidateChunkHandler(ValidateChunk chunk)
        {

        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Server-side
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [EclipseMessage]
        public static void SendChunkHandler(ushort clientID, SendChunk chunk)
        {

        }

        [EclipseMessage]
        public static void ValidateChunkHandler(ushort clientID, ValidateChunk chunk)
        {

        }
    }
}
