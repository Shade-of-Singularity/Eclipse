using Eclipse.Riptide.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using UnityEngine.UIElements;

namespace Eclipse.Riptide.Testing
{
    public static class TestConnection
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static EclipseServer Server { get; } = new EclipseServer();
        public static EclipseClient Client { get; } = new EclipseClient();
        public static ushort ServerPort { get; } = 52323;
        public static ushort ClientPort { get; } = 54372;
        public static bool Enabled
        {
            get => m_Enabled;
            set
            {
                if (m_Enabled == value) return;
                if (m_Enabled = value)
                {
                    OnEnabled();
                }
                else
                {
                    OnDisabled();
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static bool m_Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static void Start() => Enabled = true;
        public static void End() => Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static void OnEnabled()
        {
            Server.Start(ServerPort, 1, );
        }

        private static void OnDisabled()
        {

        }
    }
}
