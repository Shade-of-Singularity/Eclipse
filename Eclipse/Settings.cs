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
            public static readonly BooleanParameter StreamerMode = new BooleanParameter(
                new Structs.FullName(nameof(StreamerMode), Mod.EmptyModName), Flags.StreamerMode);
        }
    }
}
