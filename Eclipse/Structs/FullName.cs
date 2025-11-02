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

namespace Eclipse.Structs
{
    /// <summary>
    /// Fully qualified name of an thing.
    /// Allows to specify a mod and a name of a thing.
    /// <para>
    /// Used in <see cref=""/>
    /// </para>
    /// </summary>
    public readonly struct FullName : IEquatable<FullName>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string ParameterSeparator = "|";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Full name ("<see cref="Mod"/><see cref="ParameterSeparator"/><see cref="Parameter"/>") of the parameter.
        /// </summary>
        public readonly string Full;

        /// <summary>
        /// A mod name which introduced the parameter.
        /// </summary>
        public readonly string Mod;

        /// <summary>
        /// A last parameter of the name. See <see cref="Full"/> to see how full name looks like.
        /// </summary>
        public readonly string Parameter;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public FullName(string name) : this(name, Modding.Mod.EmptyModName) { }
        public FullName(string name, string mod)
        {
            // Overengineered AF, but thankfully this code won't execute on a main update loop.
            if (string.IsNullOrWhiteSpace(name))
            {
                Full = string.Empty;
                Parameter = string.Empty;
                Mod = string.IsNullOrWhiteSpace(mod) ? Modding.Mod.EmptyModName : mod;
                return;
            }

            if (string.IsNullOrWhiteSpace(mod))
            {
                Full = name;
                Parameter = name;
                Mod = Modding.Mod.EmptyModName;
            }
            else
            {
                Full = string.Join(ParameterSeparator, mod, name);
                Parameter = name;
                Mod = mod;
            }
        }

        private FullName(string name, string mod, string full)
        {
            Mod = mod;
            Parameter = name;
            Full = full;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override string ToString() => Full;
        public override int GetHashCode() => Full.GetHashCode();
        public bool Equals(FullName other) => this == other;
        public override bool Equals(object obj) => obj is FullName other && this == other;




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





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static bool operator ==(FullName a, FullName b) => string.Equals(a.Full, b.Full, StringComparison.Ordinal);
        public static bool operator !=(FullName a, FullName b) => !string.Equals(a.Full, b.Full, StringComparison.Ordinal);

        public static implicit operator string(FullName parameter) => parameter.Full;
        public static implicit operator FullName(string parameter)
        {
            if (parameter is null)
            {
                return new FullName(string.Empty, Modding.Mod.EmptyModName, string.Empty);
            }

            parameter = parameter.Trim();
            int index = parameter.IndexOf(ParameterSeparator);
            if (index == -1)
            {
                return new FullName(parameter, Modding.Mod.EmptyModName, parameter);
            }
            else if (index + 1 == parameter.Length)
            {
                return new FullName(parameter[..index], Modding.Mod.EmptyModName, parameter);
            }
            else
            {
                return new FullName(parameter[..index], parameter[(index + 1)..], parameter);
            }
        }
    }
}
