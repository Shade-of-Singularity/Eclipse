using Eclipse.Configuration;
using Eclipse.Configuration.Parameters;
using Eclipse.Modding;

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
            public static readonly Parameter<bool> StreamerMode = new Parameter<bool>(nameof(StreamerMode), Flags.StreamerMode);



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
