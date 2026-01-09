/// - - -    Copyright (c) 2026     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the MIT License. Permission is hereby granted, free of charge,
/// to any person obtaining a copy of this software and associated documentation
/// files to deal in the Software without restriction. Full license terms are
/// available in the LICENSE.md file located at the following repository path:
///   
///                 "Eclipse/Eclipse.Riptide/LICENSE.md"
/// 
/// Note: Eclipse.Riptide and Eclipse are licensed under different licenses.
/// See "Eclipse/LICENSE.md" for details.
/// 
/// ]]>

namespace Eclipse.Riptide.Messages
{
    /// <summary>
    /// Interface for custom network groups.
    /// </summary>
    /// <typeparam name="T">Type that implemented custom group.</typeparam>
    public interface INetworkGroup<T> : INetworkGroup where T : INetworkGroup<T>
    {
        public static readonly byte GroupID = NetworkIndex.NextGroupID();
        public static byte GetGroupID() => GroupID;
    }

    /// <summary>
    /// Unused at the moment, but might be used to identify generic <see cref="INetworkGroup{T}"/> interfaces.
    /// </summary>
    public interface INetworkGroup
    {
        /// <summary>
        /// Name of the readonly GroupID field in a generic interface above.
        /// Used in reflections by <see cref="NetworkIndex"/> class.
        /// </summary>
        public const string GroupIDFieldName = "GroupID";
    }
}
