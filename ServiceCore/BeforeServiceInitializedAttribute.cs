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

namespace ServiceCore
{
    /// <summary>
    /// Attribute to flag methods that should be executed after an service was initialized.
    /// </summary>
    [AttributeUsage(Targets, Inherited = Inheritable, AllowMultiple = AllowsMultiple)]
    public sealed class BeforeServiceInitializedAttribute(Type service) : ServiceInitializationAttribute(service) { }
}
