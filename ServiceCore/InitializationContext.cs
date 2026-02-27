using ServiceCore.Loading;
using System;
using System.Collections.Generic;

namespace ServiceCore
{
    /// <summary>
    /// Describes how <see cref="Engine"/> should be initialized, and what it should initialize.
    /// </summary>
    public sealed class InitializationContext(Engine.AssemblySorter? sorter = null, IEnumerable<ILoadingSource>? sources = null)
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static readonly InitializationContext Default = new();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Sorter for native assemblies.
        /// </summary>
        /// <seealso cref="Engine.NativeAssemblies"/>
        public readonly Engine.AssemblySorter? NativeSorter = sorter;
        /// <summary>
        /// Enlists all <see cref="IService"/>s which should be initialized before main/default initialization loop happens.
        /// </summary>
        public readonly List<Type> BeforeInitialization = [];
        /// <summary>
        /// Enlists all <see cref="IService"/>s which should be initialized after main/default initialization loop happens.
        /// </summary>
        public readonly List<Type> AfterInitialization = [];
        /// <summary>
        /// All loading sources for <see cref="Engine"/> to initialize.
        /// </summary>
        public readonly IEnumerable<ILoadingSource>? Sources = sources;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private bool isWritingAfter;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Schedules given service to run before the default initialization sequence (or after, if run after <see cref="ScheduleDefault"/>).
        /// </summary>
        public void Schedule<T>() where T : class, IService
        {
            var collection = isWritingAfter ? AfterInitialization : BeforeInitialization;
            if (!collection.Contains(typeof(T)))
                collection.Add(typeof(T));
        }

        /// <summary>
        /// Schedules given services to run before the default initialization sequence (or after, if run after <see cref="ScheduleDefault"/>).
        /// </summary>
        public void Schedule<T>(IEnumerable<T> services) where T : class, IService
        {
            var collection = isWritingAfter ? AfterInitialization : BeforeInitialization;
            foreach (var service in services)
            {
                Type type = service.GetType();
                if (!collection.Contains(type))
                    collection.Add(type);
            }
        }

        /// <summary>
        /// Schedules all default services after already scheduled ones.
        /// </summary>
        public void ScheduleDefault() => isWritingAfter = true;
    }
}
