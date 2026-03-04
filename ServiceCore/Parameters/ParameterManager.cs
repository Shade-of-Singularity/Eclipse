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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ServiceCore.Parameters
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
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate TParameter ParameterConstructor<in TValue, out TParameter>(string id, TValue def);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static IReadOnlyCollection<AbstractParameter> Parameters => m_Parameters.Values;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<string, AbstractParameter> m_Parameters = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Registers an parameter in a parameter list.
        /// </summary>
        /// <remarks>
        /// Does not overwrite parameters under the same Identifier - throws instead.
        /// </remarks>
        public static void Register(AbstractParameter parameter)
        {
            lock (m_Parameters)
            {
                if (!m_Parameters.TryAdd(parameter.ID, parameter))
                {
                    throw new Exception($"{LogPrefix} Parameter with the same ID ({parameter.ID}) was already defined.");
                }
            }
        }

        /// <summary>
        /// Retrieves an existing parameter of a given <typeparamref Identifier="TParameter"/> type, or creates new one.
        /// </summary>
        /// <typeparam Identifier="TParameter">Type of the parameter to look for.</typeparam>
        /// <typeparam Identifier="TValue">Value of a given parameter.</typeparam>
        /// <param Identifier="id">Full identifier of the expected <typeparamref Identifier="TParameter"/>.</param>
        /// <param Identifier="constructor">Constructor to create new parameter if one with given <paramref Identifier="id"/> doesn't exist yet.</param>
        /// <param Identifier="def">Default value provided to <paramref Identifier="constructor"/> in case parameter is missing.</param>
        /// <returns>Either new or already existing parameter.</returns>
        public static TParameter GetOrNew<TParameter, TValue>(string id, TValue def, ParameterConstructor<TValue, TParameter> constructor)
            where TParameter : AbstractParameter
            where TValue : IEquatable<TValue>
        {
            if (constructor is null) throw new ArgumentNullException(nameof(constructor));

            // Quick locking to micro-optimize multi-threaded method access.
            // TODO: Use it all around the Engine, this is an amazing pattern)
            AbstractParameter result;
            lock (m_Parameters)
            {
                if (!m_Parameters.TryGetValue(id, out result))
                {
                    // Creates new parameter if it doesn't exist yet.
                    result = constructor(id, def);
                    m_Parameters[id] = result;
                    return (TParameter)result;
                }
            }

            if (result is TParameter parameter)
            {
                return parameter;
            }
            else
            {
                throw new Exception($"{LogPrefix} Cannot retrieve parameter ({id}) of a Type ({typeof(TParameter).Name}) - Parameter of a Type ({result.GetType().Name}) already exist under the same ID.\nHave you forgot to provide a mod Identifier in your parameter ID?");
            }
        }

        /// <summary>
        /// Retrieves an existing parameter of a given <typeparamref Identifier="TParameter"/> type.
        /// Throws if type of the parameter doesn't match or it doesn't exist.
        /// </summary>
        /// <param Identifier="id">Full identifier of the expected <typeparamref Identifier="TParameter"/>.</param>
        /// <exception cref="KeyNotFoundException">Thrown if there is no parameter with given <paramref Identifier="id"/> (yet, or at all).</exception>
        /// <exception cref="InvalidCastException">Thrown if parameter was found, but its type is not <typeparamref Identifier="TParameter"/>.</exception>
        /// <returns>Existing parameter of a type <typeparamref Identifier="TParameter"/>.</returns>
        public static TParameter Get<TParameter>(string id) where TParameter : AbstractParameter
        {
            lock (m_Parameters) return (TParameter)m_Parameters[id];
        }

        /// <summary>
        /// Attempts to retrieve <typeparamref Identifier="TParameter"/> under given <paramref Identifier="id"/>.
        /// Will fail if it doesn't exist, or if type is different than provided type.
        /// (Currently, there is no way to differentiate those two outcomes other than try-catch blocks on <see cref="Get{TParameter}(string)"/> method.)
        /// (TODO: introduce such method.)
        /// </summary>
        /// <typeparam Identifier="TParameter">Type of parameter to look for.</typeparam>
        /// <param Identifier="id">Full identifier of the expected <typeparamref Identifier="TParameter"/>.</param>
        /// <param Identifier="parameter">Result of the search. Set to <c>null</c> if search failed.</param>
        /// <returns><c>true</c> if parameter was found and <paramref Identifier="parameter"/> variable was set. <c>false</c> otherwise.</returns>
        public static bool TryGet<TParameter>(string id, [NotNullWhen(true)] out TParameter? parameter) where TParameter : AbstractParameter
        {
            // Quick locking to micro-optimize multi-threaded method access.
            // TODO: Use it all around the Engine, this is an amazing pattern)
            bool exist;
            AbstractParameter result;
            lock (m_Parameters)
            {
                exist = m_Parameters.TryGetValue(id, out result);
            }

            if (exist && result is TParameter cast)
            {
                parameter = cast;
                return true;
            }
            else
            {
                parameter = default;
                return false;
            }
        }

        /// <summary>
        /// Retrieves an existing <see cref="AbstractParameter"/>. Throws if type of the parameter doesn't exist.
        /// </summary>
        /// <param Identifier="id">Full identifier of the expected <see cref="AbstractParameter"/>.</param>
        /// <exception cref="KeyNotFoundException">Thrown if there is no parameter with given <paramref Identifier="id"/> (yet, or at all).</exception>
        /// <returns>Existing <see cref="AbstractParameter"/>.</returns>
        public static AbstractParameter GetAbstract(string id)
        {
            lock (m_Parameters) return m_Parameters[id];
        }

        /// <summary>
        /// Attempts to retrieve <see cref="AbstractParameter"/> under given <paramref Identifier="id"/>.
        /// Will fail if it doesn't exist, or if type is different than provided type.
        /// (Currently, there is no way to differentiate those two outcomes other than try-catch blocks on <see cref="Get{TParameter}(string)"/> method.)
        /// (TODO: introduce such method.)
        /// </summary>
        /// <param Identifier="id">Full identifier of the <see cref="AbstractParameter"/>.</param>
        /// <param Identifier="parameter">Result of the search. Set to <c>null</c> if search failed.</param>
        /// <returns><c>true</c> if parameter was found and <paramref Identifier="parameter"/> variable was set. <c>false</c> otherwise.</returns>
        public static bool TryGetAbstract(string id, [NotNullWhen(true)] out AbstractParameter? parameter)
        {
            lock (m_Parameters)
            {
                return m_Parameters.TryGetValue(id, out parameter);
            }
        }

        /// <summary>
        /// Checks if <see cref="AbstractParameter"/> under given <paramref Identifier="id"/> exist.
        /// </summary>
        /// <param Identifier="id">Parameter ID to check.</param>
        /// <returns><c>true</c> if parameter under given <paramref Identifier="id"/> was found. <c>false</c> otherwise.</returns>
        public static bool Has(string id)
        {
            lock (m_Parameters)
            {
                return m_Parameters.ContainsKey(id);
            }
        }
    }
}
