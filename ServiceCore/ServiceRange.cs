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
using System.Linq;

namespace ServiceCore
{
    /// <summary>
    /// Describes a range of service types this service instance defines.
    /// </summary>
    /// <remarks>
    /// Needed to handle when <see cref="Service{T}"/> defines additional <see cref="IService{T}"/> interfaces.
    /// </remarks>
    public readonly struct ServiceRange(ServiceDescriptor[] descriptors) : IEquatable<ServiceRange>
    {
        /// <summary>
        /// Invalid range of services.
        /// Returned by default by methods, like <see cref="ServiceRanges.Retrieve(Type)"/>.
        /// </summary>
        public static readonly ServiceRange Invalid = new([]);

        /// <summary>
        /// First <see cref="ServiceDescriptor"/> in <see cref="Descriptors"/> sequence.
        /// </summary>
        /// <remarks>
        /// First descriptor is always a top-most class descriptor of your service
        /// (e.g.: if you implement <see cref="Service{T}"/> - <see cref="Service{T}"/> will be on top)
        /// (If you want to use it yourself - don't forget to define <see cref="ServiceIdentifierAttribute"/> in your top-most class).
        /// This is used heavily in <see cref="Services.Get(Type)"/> and other methods.
        /// </remarks>
        public ServiceDescriptor? First => Descriptors.Length == 0 ? null : Descriptors[0];

        /// <summary>
        /// All descriptors associated with this <see cref="ServiceRange"/> instance.
        /// </summary>
        /// Never null. We return <seealso cref="Invalid"/> by default.
        public readonly ServiceDescriptor[] Descriptors = descriptors;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override string ToString() => string.Join(", ", Descriptors is null ? Enumerable.Empty<ServiceDescriptor>() : Descriptors);

        /// <inheritdoc/>
        public override int GetHashCode() => Descriptors.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is ServiceRange other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(ServiceRange other)
        {
            if (ReferenceEquals(Descriptors, other.Descriptors))
                return true;

            if (Descriptors.Length != other.Descriptors.Length)
                return false;

            return Descriptors.AsSpan().SequenceEqual(other.Descriptors);
        }
    }
}
