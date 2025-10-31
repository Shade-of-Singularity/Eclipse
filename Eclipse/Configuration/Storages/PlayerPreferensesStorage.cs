using Eclipse.Configuration.Parameters;
using UnityEngine;

namespace Eclipse.Configuration.Storages
{
    public sealed class PlayerPreferenceStorage : ParameterStorage<PlayerPreferenceStorage>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override void Load(Parameter parameter) => parameter.Deserialize(PlayerPrefs.GetString(parameter.Name, string.Empty));
        public override void Save(Parameter parameter) => PlayerPrefs.SetString(parameter.Name, parameter.Serialize());




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    }
}
