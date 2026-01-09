using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Serialization
{
    /// <summary>
    /// Holder of an (de)serializers for a specific value type.
    /// Used in <see cref="Configuration.Parameters.Parameter{TValue}"/> to provide type-specific serialization.
    /// </summary>
    /// <typeparam name="TValue">Target type which an serializer is handling.</typeparam>
    public static class Serializers<TValue>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Function to serialize <typeparamref name="TValue"/> to a <see cref="string"/>.
        /// </summary>
        public delegate string Serialize(TValue value);

        /// <summary>
        /// Function to deserialize <paramref name="raw"/> data to get <typeparamref name="TValue"/> back.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public delegate TValue Deserialize(string raw);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Serializer to use for this specific <typeparamref name="TValue"/> value type.
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
        /// Deserializer to use for this specific <typeparamref name="TValue"/> value type.
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
        private static string DefaultSerializer(TValue value) => JsonUtility.ToJson(value, Serializers.UsePrettyPrint);
        private static TValue DefaultDeserializer(string raw) => JsonUtility.FromJson<TValue>(raw);
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
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const bool UsePrettyPrint = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, object> m_Serializers = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, object> m_Deserializers = new Dictionary<Type, object>();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves serializer used for a specific type.
        /// </summary>
        /// <typeparam name="TSerializer">Serializer type to retrieve.</typeparam>
        /// <param name="def">Default serializer to be used instead.</param>
        /// <returns>Custom serializer or a default one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="def"/> serializer is <c>null</c>. (it's mandatory to provide one)</exception>
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
        /// <typeparam name="TDeserializer">Deserializer type to retrieve.</typeparam>
        /// <param name="def">Default deserializer to be used instead.</param>
        /// <returns>Custom deserializer or a default one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="def"/> deserializer is <c>null</c>. (it's mandatory to provide one)</exception>
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
        /// <typeparam name="TSerializer">Type of serializer to use.</typeparam>
        /// <typeparam name="TDeserializer">Type of deserializer to use.</typeparam>
        /// <param name="serializer">Reference to a serializer to use.</param>
        /// <param name="deserializer">Reference to a deserializer to use.</param>
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
        /// <typeparam name="TSerializer">Type of serializer to use.</typeparam>
        /// <param name="serializer">Reference to a serializer to use.</param>
        public static void SetSerializer<TSerializer>(TSerializer serializer)
        {
            if (serializer is null) throw new ArgumentNullException(nameof(serializer));
            m_Serializers[typeof(TSerializer)] = serializer;
        }

        /// <summary>
        /// Sets custom deserializer for a specific deserializer type.
        /// </summary>
        /// <typeparam name="TDeserializer">Type of deserializer to use.</typeparam>
        /// <param name="deserializer">Reference to a deserializer to use.</param>
        public static void SetDeserializer<TDeserializer>(TDeserializer deserializer)
        {
            if (deserializer is null) throw new ArgumentNullException(nameof(deserializer));
            m_Deserializers[typeof(TDeserializer)] = deserializer;
        }
    }
}
