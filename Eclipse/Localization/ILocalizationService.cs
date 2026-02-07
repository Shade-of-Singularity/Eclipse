using Eclipse.Configuration;

namespace Eclipse.Localization
{
    /// <summary>
    /// General language service for asset localization.
    /// </summary>
    /// <remarks>
    /// Planned to support localization for:
    /// [ ] - Text.
    /// [ ] - Images.
    /// [ ] - GameObjects.
    /// [ ] - Audio.
    /// [ ] - Misc resources.
    /// </remarks>
    public interface ILocalizationService : IService<ILocalizationService>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Prefix for messages sent to the console from this class.
        /// </summary>
        public const string LogPrefix = Engine.LogPrefix + "[LocalizationService]";
    }
}
