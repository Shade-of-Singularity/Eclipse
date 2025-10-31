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
    public abstract class Parameter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate void ParameterChangeHandler(Parameter parameter);
        public delegate void VisibilityChangeHandler(bool visible);




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
        public event ParameterChangeHandler OnNameChanged;

        /// <summary>
        /// Called when either <see cref="FullCategory.Name"/>
        /// or both <see cref="FullCategory.Name"/> AND <see cref="FullCategory.Visible"/> was changed.
        /// (see also: <see cref="Category"/>)
        /// </summary>
        /// <remarks>
        /// Was made this way so updates to <see cref="FullCategory.Visible"/> won't force UI to rebuild constantly.
        /// </remarks>
        public event ParameterChangeHandler OnCategoryChanged;

        /// <summary>
        /// Called when <see cref="FullCategory.Visible"/> has changed.
        /// (see also: <see cref="Category"/>)
        /// </summary>
        public event VisibilityChangeHandler OnVisibilityChanged;


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

        /// <inheritdoc cref="OnCategoryChanged"/>
        public event ParameterChangeHandler FireWithCategoryChanged
        {
            remove => OnCategoryChanged -= value;
            add
            {
                if (value != null)
                {
                    OnCategoryChanged += value;
                    value(this);
                }
            }
        }

        /// <inheritdoc cref="OnVisibilityChanged"/>
        public event VisibilityChangeHandler FireWithVisibilityChanged
        {
            remove => OnVisibilityChanged -= value;
            add
            {
                if (value != null)
                {
                    OnVisibilityChanged += value;
                    value(m_Category.Visible);
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
        public FullName Name
        {
            get => m_Name;
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnNameChanged?.Invoke(this);
                    EngineService<ConfigurationService>.Instance.NotifyCategorizationChanged();
                }
            }
        }

        public FullCategory Category
        {
            get => m_Category;
            set
            {
                if (m_Category != value)
                {
                    bool invokeVisibility = m_Category.Visible != value.Visible;

                    m_Category = value;
                    OnCategoryChanged?.Invoke(this);
                    if (invokeVisibility)
                    {
                        OnVisibilityChanged?.Invoke(value.Visible);
                    }
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:
        protected FullName m_Name;
        protected FullCategory m_Category;

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public Parameter(FullName name)
        {
            m_Name = name;
            EngineService<ConfigurationService>.Instance.Register(this);
        }




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
        /// similar to <see cref="AbstractParameter{TValue}.OnValueApplied"/> even when nothing has changed.
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
        /// similar to <see cref="AbstractParameter{TValue}.OnValueApplied"/> even when nothing has changed.
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




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    }
}
