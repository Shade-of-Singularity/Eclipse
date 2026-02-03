using System;

namespace Eclipse
{
    /// <summary>
    /// Provides ways for you to change how engine logs messages.
    /// </summary>
    public static class EngineLogger
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Handler for handling regular messages.
        /// </summary>
        public delegate void LogHandler(string message);
        /// <summary>
        /// Handler for handling <see cref="Exception"/>s.
        /// </summary>
        public delegate void ExceptionHandler(Exception exception);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Handler for regular logs.
        /// </summary>
        public static event LogHandler? InformationReceived;
        /// <summary>
        /// Handler for warnings. Default logger also draws those messages in yellow.
        /// </summary>
        public static event LogHandler? WarningReceived;
        /// <summary>
        /// Handler for errors. Default logger also draws those messages in red.
        /// </summary>
        public static event LogHandler? ErrorReceived;
        /// <summary>
        /// Handler for exceptions. Default logger logs <see cref="Exception.Message"/> and <see cref="Exception.StackTrace"/>.
        /// </summary>
        public static event ExceptionHandler? ExceptionReceived;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static EngineLogger()
        {
            InformationReceived += (message) =>
            {
                ConsoleColor last = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
                Console.ForegroundColor = last;
            };

            WarningReceived += (message) =>
            {
                ConsoleColor last = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
                Console.ForegroundColor = last;
            };

            ErrorReceived += (message) =>
            {
                ConsoleColor last = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = last;
            };

            ExceptionReceived += (exception) =>
            {
                ConsoleColor last = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
                Console.ForegroundColor = last;
            };
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invokes <see cref="InformationReceived"/> with given <paramref name="obj"/>.
        /// Calls <see cref="object.ToString"/> on given <paramref name="obj"/>.
        /// </summary>
        public static void Log(object? obj) => InformationReceived?.Invoke(obj?.ToString() ?? string.Empty);
        /// <summary>
        /// Invokes <see cref="WarningReceived"/> with given <paramref name="obj"/>.
        /// Calls <see cref="object.ToString"/> on given <paramref name="obj"/>.
        /// </summary>
        public static void LogWarning(object? obj) => WarningReceived?.Invoke(obj?.ToString() ?? string.Empty);
        /// <summary>
        /// Invokes <see cref="ErrorReceived"/> with given <paramref name="obj"/>.
        /// Calls <see cref="object.ToString"/> on given <paramref name="obj"/>.
        /// </summary>
        public static void LogError(object? obj) => ErrorReceived?.Invoke(obj?.ToString() ?? string.Empty);
        /// <summary>
        /// Invokes <see cref="ExceptionReceived"/> with given <paramref name="exception"/>.
        /// </summary>
        /// <remarks>
        /// Replaces <paramref name="exception"/> with an empty exception as a fail-safe.
        /// </remarks>
        public static void LogException(Exception exception) => ExceptionReceived?.Invoke(exception ?? new Exception());
    }
}
