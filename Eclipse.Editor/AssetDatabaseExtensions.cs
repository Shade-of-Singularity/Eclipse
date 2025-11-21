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
