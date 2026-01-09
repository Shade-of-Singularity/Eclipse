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
