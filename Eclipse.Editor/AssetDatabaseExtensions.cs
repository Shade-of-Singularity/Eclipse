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
using System.Text;
using UnityEditor;

namespace Eclipse.Editor
{
    public static class AssetDatabaseExtensions
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static void EnsurePathExists(string path)
        {
            string[] sequence = path.Split('\\', '/');
            if (sequence.Length == 0)
            {
                return;
            }
            else if (sequence.Length == 1)
            {
                if (string.Equals(sequence[0], "Assets"))
                {
                    // The assets folder will always exist.
                    return;
                }
                else
                {
                    // Ensures that "Assets" always present in the list.
                    Array.Resize(ref sequence, 2);
                    sequence[1] = sequence[0];
                    sequence[0] = "Assets";
                }
            }

            string last = sequence[0];
            StringBuilder builder = new StringBuilder(sequence[0]); // "Assets" always exist.
            for (int i = 1; i < sequence.Length; i++)
            {
                builder.Append('\\');
                builder.Append(sequence[i]);

                string tmp = builder.ToString();
                if (!AssetDatabase.IsValidFolder(tmp))
                {
                    AssetDatabase.CreateFolder(last, sequence[i]);
                }

                last = tmp;
            }
        }
    }
}
