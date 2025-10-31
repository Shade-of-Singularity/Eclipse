using Eclipse.Configuration.Parameters;
using UnityEngine;

namespace Eclipse.Configuration.UI
{
    /// <summary>
    /// UI component for an <see cref="Parameters.Parameter"/>.
    /// </summary>
    /// <remarks>
    /// When implementing, don't forget that <see cref="Parameter"/>s can be modified in the editor, on <see cref="Application.isPlaying"/> == <c>false</c>.
    /// Account for that by not modifying the UI in such cases, but you can modify the parameters themselves.
    /// </remarks>
    /// <typeparam name="TParameter">Special type of the parameter to try to look for. Specify <see cref="Parameters.Parameter"/> for any type.</typeparam>
    public abstract class ParameterUI<TParameter> : MonoBehaviour where TParameter : Parameter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Events:

        // Properties:
        public TParameter? Parameter { get; protected set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Serialized Fields:
        [Header("Settings")]
        [Tooltip("Name of the parameter to process.")]
        [SerializeField] private string m_Parameter = string.Empty;
        [Tooltip("Whether to set value of the UI as a default value of the parameter.")]
        [SerializeField] private bool m_SetAsDefault = true;

        // Encapsulated Fields:

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Unity Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected virtual void Awake()
        {
            SetupParameter(EngineService<ConfigurationService>.Instance.FindOrThrow<TParameter>(m_Parameter));
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected virtual void SetupParameter(TParameter parameter)
        {
            Parameter = parameter;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

    }
}
