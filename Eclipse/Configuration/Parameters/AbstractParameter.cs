/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using Eclipse.Structs;

namespace Eclipse.Configuration.Parameters
{
    /// <summary>
    /// Base parameter which can be serialized to- or deserialized from <see cref="Storages.IDataStorage"/> via <see cref="ConfigurationService"/>.
    /// </summary>
    public abstract class AbstractParameter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate void ParameterChangeHandler(AbstractParameter parameter);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Called when <see cref="Name"/> has changed in any way.
        /// </summary>
        /// <remarks>
        /// It is not recommended to change <see cref="FullName.Mod"/> outside of the initialization.
        /// You can do it, but it will cause UI rebuilds, and might break stuff.
        /// </remarks>
        public event ParameterChangeHandler? OnNameChanged;

        // Fire-on-add events:
        /// <inheritdoc cref="OnNameChanged"/>
        public event ParameterChangeHandler FireWithNameChanged
        {
            remove => OnNameChanged -= value;
            add
            {
                if (value != null)
                {
                    OnNameChanged += value;
                    value(this);
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Fully qualified name of the parameter.
        /// </summary>
        /// <remarks>
        /// Note: please, refrain from modifying <see cref="FullName.Mod"/> here.
        /// This might cause a lot of UI updates, and might break stuff at times.
        /// </remarks>
        public FullName Name => m_Name;

        /// <summary>
        /// Whether property awaits being applied.
        /// </summary>
        public abstract bool IsDirty { get; }

        /// <summary>
        /// Whether current value of the property is different than a default value.
        /// </summary>
        public abstract bool IsModified { get; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:
        protected FullName m_Name;

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AbstractParameter(FullName name) => m_Name = name;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Applies all changes made to the parameter.
        /// </summary>
        public abstract void ApplyChanges();

        /// <summary>
        /// Applies all the changes and forcefully fires callbacks,
        /// similar to <see cref="Parameter{TValue}.OnValueApplied"/> even when nothing has changed.
        /// </summary>
        /// <remarks>
        /// <see cref="ConfigurationService"/> will use <see cref="ApplyChangesForceFireCallbacks"/> after <see cref="Engine.OnEngineInitialized"/>.
        /// </remarks>
        public abstract void ApplyChangesForceFireCallbacks();

        /// <summary>
        /// Reverts all changes made to the parameter.
        /// Will fire related callbacks only if parameter changed after applying.
        /// </summary>
        public abstract void RevertChanges();

        /// <summary>
        /// Reverts all changes made to the parameter, and forcefully
        /// similar to <see cref="Parameter{TValue}.OnValueApplied"/> even when nothing has changed.
        /// </summary>
        /// <remarks>
        /// Nothing at the moment fires this callback in the <see cref="Eclipse"/>, but implement it regardless please.
        /// </remarks>
        public abstract void RevertChangesForceFireCallbacks();

        /// <summary>
        /// Serializes parameter into a string.
        /// </summary>
        /// <returns>
        /// A raw string data describing a stored object.
        /// </returns>
        public abstract string Serialize();

        /// <summary>
        /// Safe method for deserializing property data.
        /// </summary>
        /// <remarks>
        /// You can throw here, but it is "safe" in regards that it will just keep the same DefaultValue if deserialization failed.
        /// </remarks>
        /// <param name="raw">Data which was previously returned by <see cref="Serialize"/> - raw string to deserialize.</param>
        public abstract void Deserialize(string raw);

        /// <summary>
        /// Retrieves value of the parameter as an <see cref="object"/> type.
        /// </summary>
        /// <remarks>
        /// Consider using <see cref="Parameter{TValue}.Value"/> or <see cref="SolidParameter{TValue}.Value"/> instead, if you have explicit type on your hands.
        /// This is needed to avoid packing/unpacking.
        /// For general solutions, independent from variable type, feel free to use this method.
        /// </remarks>
        /// <returns>Value of the parameter as <see cref="object"/>.</returns>
        public abstract object GetValue();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    }
}
