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

using Eclipse.Configuration;
using System;

namespace Eclipse.Structs
{
    /// <summary>
    /// Category of an <see cref="Configuration.Parameters.Parameter"/> or any other thing.
    /// It is used to provide automatic categorization and some utils for external settings editing,
    /// but does not affect serialization (since values of <see cref="Parameter"/> is serialize based only on <see cref="FullName"/>)
    /// (see also: <see cref="Parameter.Category"/>)
    /// </summary>
    /// <remarks>
    /// Categorization is also used to automatically generate settings UI for mods.
    /// Parameters that were implemented on the UI manually, anywhere, will not be automatically generated.
    /// <para>
    /// Note: <see cref="Visible"/> flag does not affect whether paratemeter will be visible 
    /// </para>
    /// And if you plan on manually exposing the parameter later-on with a menu.
    /// </remarks>
    public readonly struct FullCategory : IEquatable<FullCategory>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const string DefaultCategory = "Uncategorized";
        public const int DefaultOrder = 0;

        /// <summary>
        /// Range which covers a specific block on the Settings UI before a split-line is created.
        /// For example - default region covers [-500_000 : 500_000], the one or the right: [500_001 : 1000_000], and left: [-1000_000 : -500_001]
        /// </summary>
        public const int SplitOrderRange = 500_000;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Category name, usually, without mod name (e.g. with <see cref="Modding.Mod.EmptyModName"/>).
        /// </summary>
        public readonly FullName Name;

        /// <summary>
        /// Whether this <c>parameter</c> is visible or not.
        /// </summary>
        /// <remarks>
        /// Does not affect category visibility - it is automatically set to be "Visible" when any parameter in it is visible.
        /// Use <see cref="ConfigurationService.TryGetCategory"/> with <see cref="Categorization.Category.Visible"/> to hide it.
        /// </remarks>
        public readonly bool Visible;

        /// <summary>
        /// Order of this parameter in the category.
        /// </summary>
        /// <remarks>
        /// A split-line between parameters will be created on UI if order will be inside a different <see cref="SplitOrderRange"/>.
        /// (see also: <seealso cref="Configuration.UI.ParameterUI{TParameter}"/>) // TODO: Update the reference to the Settings UI.
        /// (see docs for more info)
        /// </remarks>
        public readonly int Order;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public FullCategory(string name, bool visible = true) : this(name, DefaultOrder, visible) { }
        public FullCategory(string name, int order, bool visible = true)
        {
            Name = string.IsNullOrEmpty(name) ? DefaultCategory : name;
            Visible = visible;
            Order = order;
        }

        /// <summary>
        /// Internal constructor for, well, internal and more optimized usage.
        /// </summary>
        private FullCategory(int order, bool visible, string name)
        {
            Name = name;
            Visible = visible;
            Order = order;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override string ToString() => $"{Name}  Order: {Order}  Visible: {Visible}";
        public override int GetHashCode() => HashCode.Combine(Name, Visible, Order);
        public override bool Equals(object obj) => obj is FullCategory category && this == category;
        public bool Equals(FullCategory other) => this == other;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public FullCategory Set(string name, int order, bool visible)
        {
            // Method added in case we will change the logic of that.
            if (Visible == visible && Order == order)
            {
                if (string.IsNullOrEmpty(name)) name = DefaultCategory;
                if (string.Equals(Name, name, StringComparison.Ordinal))
                {
                    return this;
                }
                else
                {
                    return new FullCategory(order, visible, name);
                }
            }
            else
            {
                return new FullCategory(order, visible, name);
            }
        }

        public FullCategory SetName(string name)
        {
            if (string.IsNullOrEmpty(name)) name = DefaultCategory;
            if (string.Equals(Name, name))
            {
                return this;
            }
            else
            {
                return new FullCategory(Order, Visible, name);
            }
        }

        public FullCategory SetOrder(int order)
        {
            if (Order == order)
            {
                return this;
            }
            else
            {
                return new FullCategory(order, Visible, Name);
            }
        }

        public FullCategory SetVisibility(bool visible)
        {
            if (Visible == visible)
            {
                return this;
            }
            else
            {
                return new FullCategory(Order, visible, Name);
            }
        }




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
        public static bool operator ==(FullCategory a, FullCategory b)
            => a.Visible == b.Visible && string.Equals(a.Name, b.Name, StringComparison.Ordinal);
        public static bool operator !=(FullCategory a, FullCategory b)
            => a.Visible != b.Visible || !string.Equals(a.Name, b.Name, StringComparison.Ordinal);
    }
}
