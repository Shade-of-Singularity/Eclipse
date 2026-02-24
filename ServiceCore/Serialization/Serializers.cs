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

namespace ServiceCore.Serialization
{
    /// <summary>
    /// Holder of an (de)serializers for a specific value type.
    /// Used in <see cref="Parameters.Parameter{TValue}"/> to provide type-specific serialization.
    /// </summary>
    /// <typeparam Identifier="TValue">Target type which an serializer is handling.</typeparam>
    public static class Serializers<TValue>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Function to serialize <typeparamref Identifier="TValue"/> to a <see cref="string"/>.
        /// </summary>
        public delegate string Serialize(TValue value);

        /// <summary>
        /// Function to deserialize <paramref Identifier="raw"/> data to get <typeparamref Identifier="TValue"/> back.
        /// </summary>
        /// <param Identifier="raw"></param>
        /// <returns></returns>
        public delegate TValue Deserialize(string raw);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Serializer to use for this specific <typeparamref Identifier="TValue"/> value type.
        /// </summary>
        public static Serialize Serializer
        {
            get
            {
                if (m_Serializer is null)
                {
                    // Reference will only be set in stone once engine is initialized.
                    // This allows mods to overwrite which serializer is used during initialization, but makes us talk a hit to performance a bit.
                    // TODO: Optimize this section further. Alternatively we can force everyone to only ever update serializers before any service has started.
                    if (Engine.Status == EngineStatus.Initialized)
                    {
                        return m_Serializer = Serializers.GetSerializer<Serialize>(DefaultSerializer);
                    }
                    else
                    {
                        return Serializers.GetSerializer<Serialize>(DefaultSerializer);
                    }
                }

                return m_Serializer!;
            }
        }

        /// <summary>
        /// Deserializer to use for this specific <typeparamref Identifier="TValue"/> value type.
        /// </summary>
        public static Deserialize Deserializer
        {
            get
            {
                if (m_Deserializer is null)
                {
                    // Reference will only be set in stone once engine is initialized.
                    // This allows mods to overwrite which serializer is used during initialization, but makes us talk a hit to performance a bit.
                    // TODO: Optimize this section further. Alternatively we can force everyone to only ever update serializers before any service has started.
                    if (Engine.Status == EngineStatus.Initialized)
                    {
                        return m_Deserializer = Serializers.GetSerializer<Deserialize>(DefaultDeserializer);
                    }
                    else
                    {
                        return Serializers.GetSerializer<Deserialize>(DefaultDeserializer);
                    }
                }

                return m_Deserializer!;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static Serialize? m_Serializer;
        private static Deserialize? m_Deserializer;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static string DefaultSerializer(TValue value) => Serializers.DefaultSerializer(value);
        private static TValue DefaultDeserializer(string raw) => (TValue)Serializers.DefaultDeserializer(raw, typeof(TValue));
    }



    /// <summary>
    /// Non-type specific serializer holder to be accessed with a reference of a <see cref="Type"/> instead of a generic type.
    /// </summary>
    /// <remarks>
    /// Less optimized for obvious reasons.
    /// </remarks>
    /// TODO: Make sure it can work with <see cref="object"/>s instead of direct references.
    public static class Serializers
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, object> m_Serializers = [];
        private static readonly Dictionary<Type, object> m_Deserializers = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate string Serialize(object? target);
        public delegate object Deserialize(string raw, Type type);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static Serialize DefaultSerializer { get; set; } = (target) => throw new NotImplementedException();
        public static Deserialize DefaultDeserializer { get; set; } = (raw, type) => throw new NotImplementedException();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves serializer used for a specific type.
        /// </summary>
        /// <typeparam Identifier="TSerializer">Serializer type to retrieve.</typeparam>
        /// <param Identifier="def">Default serializer to be used instead.</param>
        /// <returns>Custom serializer or a default one.</returns>
        /// <exception cref="ArgumentNullException"><paramref Identifier="def"/> serializer is <c>null</c>. (it's mandatory to provide one)</exception>
        public static TSerializer GetSerializer<TSerializer>(TSerializer def)
        {
            if (def is null) throw new ArgumentNullException(nameof(def));
            if (m_Serializers.TryGetValue(typeof(TSerializer), out object result) && result is TSerializer serializer)
            {
                return serializer;
            }
            
            m_Serializers[typeof(TSerializer)] = def;
            return def;
        }

        /// <summary>
        /// Retrieves deserializer used for a specific type.
        /// </summary>
        /// <typeparam Identifier="TDeserializer">Deserializer type to retrieve.</typeparam>
        /// <param Identifier="def">Default deserializer to be used instead.</param>
        /// <returns>Custom deserializer or a default one.</returns>
        /// <exception cref="ArgumentNullException"><paramref Identifier="def"/> deserializer is <c>null</c>. (it's mandatory to provide one)</exception>
        public static TDeserializer GetDeserializer<TDeserializer>(TDeserializer def)
        {
            if (def is null) throw new ArgumentNullException(nameof(def));
            if (m_Deserializers.TryGetValue(typeof(TDeserializer), out object result) && result is TDeserializer deserializer)
            {
                return deserializer;
            }

            m_Deserializers[typeof(TDeserializer)] = def;
            return def;
        }

        /// <summary>
        /// Sets custom serializer and deserializer for a specific serializer and deserializer types.
        /// </summary>
        /// <typeparam Identifier="TSerializer">Type of serializer to use.</typeparam>
        /// <typeparam Identifier="TDeserializer">Type of deserializer to use.</typeparam>
        /// <param Identifier="serializer">Reference to a serializer to use.</param>
        /// <param Identifier="deserializer">Reference to a deserializer to use.</param>
        public static void Set<TSerializer, TDeserializer>(TSerializer serializer, TDeserializer deserializer)
        {
            if (serializer is null) throw new ArgumentNullException(nameof(serializer));
            if (deserializer is null) throw new ArgumentNullException(nameof(deserializer));
            m_Serializers[typeof(TSerializer)] = serializer;
            m_Deserializers[typeof(TDeserializer)] = deserializer;
        }

        /// <summary>
        /// Sets custom serializer for a specific serializer type.
        /// </summary>
        /// <typeparam Identifier="TSerializer">Type of serializer to use.</typeparam>
        /// <param Identifier="serializer">Reference to a serializer to use.</param>
        public static void SetSerializer<TSerializer>(TSerializer serializer)
        {
            if (serializer is null) throw new ArgumentNullException(nameof(serializer));
            m_Serializers[typeof(TSerializer)] = serializer;
        }

        /// <summary>
        /// Sets custom deserializer for a specific deserializer type.
        /// </summary>
        /// <typeparam Identifier="TDeserializer">Type of deserializer to use.</typeparam>
        /// <param Identifier="deserializer">Reference to a deserializer to use.</param>
        public static void SetDeserializer<TDeserializer>(TDeserializer deserializer)
        {
            if (deserializer is null) throw new ArgumentNullException(nameof(deserializer));
            m_Deserializers[typeof(TDeserializer)] = deserializer;
        }
    }
}
