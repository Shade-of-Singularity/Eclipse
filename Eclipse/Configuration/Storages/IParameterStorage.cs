using Eclipse.Configuration.Parameters;

namespace Eclipse.Configuration.Storages
{
    public abstract class ParameterStorage<T> : IParameterStorage where T : ParameterStorage<T>, new()
    {
        /// <summary>
        /// Static instance of an <see cref="IParameterStorage"/> implementation.
        /// </summary>
        public static readonly T Instance = new();

        /// <inheritdoc cref="IParameterStorage.Load(Parameter)"/>
        public abstract void Load(Parameter parameter);

        /// <inheritdoc cref="IParameterStorage.Save(Parameter)"/>
        public abstract void Save(Parameter parameter);
    }

    /// <summary>
    /// Processor interface which controls where data is stored.
    /// </summary>
    public interface IParameterStorage
    {
        /// <summary>
        /// If has any data about it - will call <see cref="Parameter.Deserialize(string)"/> using a raw string data about this parameter.
        /// </summary>
        /// <param name="parameter">Parameter to be updated.</param>
        public void Load(Parameter parameter);

        /// <summary>
        /// Calls <see cref="Parameter.Serialize"/> method and stores a return value 
        /// </summary>
        /// <param name="parameter"></param>
        public void Save(Parameter parameter);
    }
}
