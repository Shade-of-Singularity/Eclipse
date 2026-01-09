using Eclipse.Configuration.Parameters;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Default implementation of <see cref="ConfigurationService"/>.
    /// </summary>
    [Service(InitializationOrder = InitializationOrder)]
    public sealed class DefaultConfigurationService : ConfigurationService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Properties:
        public override bool IsDirty => throw new System.NotImplementedException();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                             Private Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:

        // Local Fields:




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void InitializeParameters() => ApplyForceCallbacks();

        /// <inheritdoc cref="EngineService.Initialize"/>
        protected override void Initialize()
        {
            Engine.OnEngineInitialized += InitializeParameters;
            // TODO: Make get methods abstract in parent, and implement them in default implementation.
            //LoadResources();
            LoadInternal();
        }

        /// <inheritdoc cref="EngineService.Unload"/>
        protected override void Unload()
        {
            Engine.OnEngineInitialized -= InitializeParameters;
            SaveInternal();
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        //        private void LoadResources()
        //        {
        //            // TODO: Allow overwriting engine configurations at runtime, if needed.
        //            m_EngineConfigurations.Clear();
        //            EngineConfiguration[] configurations = Resources.LoadAll<EngineConfiguration>(string.Empty);
        //            foreach (var configuration in configurations)
        //            {
        //                Type key = configuration.GetType();
        //#if DEBUG
        //                if (m_EngineConfigurations.ContainsKey(key))
        //                {
        //                    Debug.LogWarning($"Found additional instance of {key.Name}. Using new one.");
        //                }
        //#endif

        //                m_EngineConfigurations[key] = configuration;
        //            }
        //        }

        /// <summary>
        /// Forcefully loads-in all data about registered parameters.
        /// </summary>
        private void LoadInternal()
        {
            foreach (var parameters in ParameterManager.Parameters)
            {
                Storage.Load(parameters);
            }
        }

        /// <summary>
        /// Forcefully saves a save file data about all registered parameters.
        /// </summary>
        /// <remarks>
        /// Will not check for <see cref="IsDirty"/>.
        /// </remarks>
        private void SaveInternal()
        {
            foreach (var parameter in ParameterManager.Parameters)
            {
                Storage.Save(parameter);
            }
        }

        /// <inheritdoc/>
        public override bool Set(string id, bool value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, byte value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, sbyte value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, short value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, ushort value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, char value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, int value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, uint value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, long value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, ulong value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, float value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, double value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, decimal value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Set(string id, string value)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out bool value, bool def = true)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out byte value, byte def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out sbyte value, sbyte def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out short value, short def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out ushort value, ushort def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out char value, char def = '\0')
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out int value, int def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out uint value, uint def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out long value, long def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out ulong value, ulong def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out float value, float def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out double value, double def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out decimal value, decimal def = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Get(string id, out string value, string def = "")
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsPending(string id)
        {
            throw new System.NotImplementedException("Pending listing, needed for performance, wasn't implemented yet.");
        }

        /// <inheritdoc/>
        public override bool RemovePending(string id)
        {
            throw new System.NotImplementedException("Pending listing, needed for performance, wasn't implemented yet.");
        }
    }
}
