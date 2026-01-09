using Eclipse.Configuration;
using Eclipse.Configuration.Parameters;

namespace Eclipse
{
    public static partial class Engine
    {
        /// <summary>
        /// General settings of the engine.
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Streamer mode hides sensitive info on the screen.
            /// </summary>
            /// <remarks>
            /// (TODO) Forcefully set to 'true' if <see cref="Flags"/> contains <see cref="Flags.StreamerModeFlag"/>.
            /// </remarks>
            public static readonly Parameter<bool> StreamerMode = Parameter<bool>.Get(nameof(StreamerMode), false);



            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            [ServiceAfterloadMethod(typeof(ConfigurationService))]
            internal static void Initialize()
            {
                if (Flags.StreamerMode)
                {
                    StreamerMode.DefaultValue = StreamerMode.Value = true;
                }
            }
        }
    }
}
