using Eclipse.Structs;
using System;
using System.Collections.Generic;

namespace Eclipse.Configuration.Parameters
{
    /// <summary>
    /// Manages all parameters in the application.
    /// </summary>
    /// <remarks>
    /// Works outside of a <see cref="Engine"/> scope as parameters are initialized at game initialization.
    /// </remarks>
    public static class ParameterManager
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string LogPrefix = Engine.LogPrefix + "[" + nameof(ParameterManager) + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<string, AbstractParameter> m_Parameters = new Dictionary<string, AbstractParameter>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Registers an parameter in a parameter list.
        /// </summary>
        /// <remarks>
        /// Does not overwrite parameters under the same name.
        /// </remarks>
        public static void Register(FullName name, AbstractParameter parameter)
        {
            lock (m_Parameters)
            {
                if (!m_Parameters.TryAdd(name, parameter))
                {
                    throw new Exception($"{LogPrefix} ");
                }
            }
        }

        /// <summary>
        /// Retrieves an existing parameter of a given <typeparamref name="TParameter"/> type, or creates new one.
        /// </summary>
        /// <typeparam name="TParameter">Type of the parameter to look for.</typeparam>
        /// <typeparam name="TValue">Value of a given parameter.</typeparam>
        /// <param name="name">Full identifier of the <typeparamref name="TParameter"/>.</param>
        /// <param name="def">Default value parameter should get.</param>
        /// <returns>Parameter</returns>
        public static TParameter GetOrNew<TParameter, TValue>(FullName name, TValue def = default!)
            where TParameter : AbstractParameter
            where TValue : IEquatable<TValue>
        {
            return GetOrNew<TParameter, TValue>(name.Full, def);
        }

        /// <summary>
        /// Retrieves an existing parameter of a given <typeparamref name="TParameter"/> type, or creates new one.
        /// </summary>
        /// <typeparam name="TParameter">Type of the parameter to look for.</typeparam>
        /// <typeparam name="TValue">Value of a given parameter.</typeparam>
        /// <param name="name">Full identifier of the <typeparamref name="TParameter"/>.</param>
        /// <param name="def">Default value parameter should get.</param>
        /// <returns>Parameter</returns>
        public static TParameter GetOrNew<TParameter, TValue>(FullName name, Func<TValue> def)
            where TParameter : AbstractParameter
            where TValue : IEquatable<TValue>
        {
            return GetOrNew<TParameter, TValue>(name.Full, def);
        }

        /// <summary>
        /// Retrieves an existing parameter of a given <typeparamref name="TParameter"/> type, or creates new one.
        /// </summary>
        /// <typeparam name="TParameter">Type of the parameter to look for.</typeparam>
        /// <typeparam name="TValue">Value of a given parameter.</typeparam>
        /// <param name="id">Full identifier of the <typeparamref name="TParameter"/>.</param>
        /// <param name="def">Default value parameter should get.</param>
        /// <returns>Parameter</returns>
        public static TParameter GetOrNew<TParameter, TValue>(string id, TValue def = default!)
            where TParameter : AbstractParameter
            where TValue : IEquatable<TValue>
        {
            lock (m_Parameters)
            {
                if (m_Parameters.TryGetValue(id, out AbstractParameter result))
                {
                    if (result is TParameter parameter)
                    {
                        return parameter;
                    }
                    else
                    {
                        throw new Exception($"{LogPrefix} Cannot retrieve parameter ({id}) of a type ({typeof(TParameter).Name}) - Parameter of a type ({result.GetType().Name}) already exist under the same ID.\nHave you forgot to provide a mod name in your parameter ID?");
                    }
                }
                else
                {
                    m_Parameters[id] = 
                }
            }
        }

        /// <summary>
        /// Retrieves an existing parameter of a given <typeparamref name="TParameter"/> type, or creates new one.
        /// </summary>
        /// <typeparam name="TParameter">Type of the parameter to look for.</typeparam>
        /// <typeparam name="TValue">Value of a given parameter.</typeparam>
        /// <param name="id">Full identifier of the <typeparamref name="TParameter"/>.</param>
        /// <param name="def">Default value parameter should get.</param>
        /// <returns>Parameter</returns>
        public static TParameter GetOrNew<TParameter, TValue>(string id, Func<TValue> def)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public static TParameter Get<TParameter>(FullName name) where TParameter : AbstractParameter
        {

        }

        /// <summary>
        /// Attempts to retrieve an existing parameter of any type.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <returns></returns>
        public static AbstractParameter Get(FullName name)
        {

        }

        public static bool TryGet<TParameter>(FullName name, out TParameter parameter) where TParameter : AbstractParameter
        {

        }

        public static bool TryGet(FullName name, out AbstractParameter parameter)
        {

        }

        public static bool Has(FullName name)
        {

        }
    }
}
