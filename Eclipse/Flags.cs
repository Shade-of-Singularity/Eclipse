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
using UnityEngine;

namespace Eclipse
{
    public static partial class Engine
    {
        /// <summary>
        /// Contains information about recognized flags provided at application start.
        /// </summary>
        public static class Flags
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                 Constants
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Flag for <see cref="StreamerMode"/> parameter.
            /// </summary>
            public const string StreamerModeFlag = "-streamer";

            /// <summary>
            /// Flag for <see cref="ResetMode"/> parameter.
            /// </summary>
            [Obsolete("Should not be used until we can reliably prevent repeated setting reset within one session.")]
            public const string ResetModeFlag = "-reset";

            /// <summary>
            /// Makes engine log flags on flag startup.
            /// </summary>
            public const string LogFlagsFlag = "-logFlags";




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Static Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Raw view of all the flags.
            /// </summary>
            public static IReadOnlyDictionary<string, string[]> Raw => m_Args;

            /// <summary>
            /// Name of the process specified in command line arguments.
            /// </summary>
            public static string ProcessName => m_ProcessName;

            /// <summary>
            /// Forcefully enabled streamer mode.
            /// </summary>
            /// <remarks>
            /// Streamer mode should block any networking functionality, potentially leaking some info during streams, etc.
            /// Mods which violate this rule will be prohibited from usage on streams.
            /// </remarks>
            public static bool StreamerMode { get; private set; }

            /// <summary>
            /// Resets service states on engine initialization.
            /// TODO: We need a way to make it a one time only reset. Otherwise user will reset service state each time they try to load-in the Engine.
            /// Which can happen multiple times within one session.
            /// </summary>
            [Obsolete("Should not be used until we can reliably prevent repeated setting reset within one session.")]
            public static bool ResetMode { get; private set; }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static readonly Dictionary<string, string[]> m_Args;
            private static readonly string m_ProcessName = string.Empty;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            static Flags()
            {
                RetrieveArgs(out m_Args, out m_ProcessName);
                if (m_Args.ContainsKey(LogFlagsFlag))
                {
                    LogFlags(m_Args);
                }

                StreamerMode = m_Args.ContainsKey(StreamerModeFlag);

#pragma warning disable CS0618 // Type or member is obsolete
                ResetMode = m_Args.ContainsKey(ResetModeFlag);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            static void RetrieveArgs(out Dictionary<string, string[]> flags, out string process)
            {
                string[] args = Environment.GetCommandLineArgs();
                Debug.Log("Logging environment args:");
                Array.ForEach(args, Debug.Log);

                // Isolates process name.
                flags = new Dictionary<string, string[]>();
                if (args.Length == 0)
                {
                    process = string.Empty;
                    return;
                }
                else
                {
                    process = args[0];
                }

                // Isolates flags.
                string? flag = null;
                List<string> values = new List<string>(8);
                for (int i = 1; i < args.Length; i++)
                {
                    var temp = args[i];
                    if (string.IsNullOrWhiteSpace(temp)) continue;

                    if (temp.StartsWith('-'))
                    {
                        flags[temp] = values.ToArray();
                    }
                    else if (temp.StartsWith('"') && temp.EndsWith('"'))
                    {
                        values.Add(temp[1..^1]);
                    }
                    else
                    {
                        values.Add(temp);
                    }
                }

                if (flag != null)
                {
                    flags[flag] = values.ToArray();
                }
            }

            static void LogFlags(in Dictionary<string, string[]> args)
            {
                Debug.Log("Logging Command line args:");
                uint counter = 0;
                foreach (var pair in m_Args)
                {
                    Debug.Log($"[{counter++}] {pair.Key}: {string.Join(", ", pair.Value)}");
                }
            }
        }
    }
}
