using Eclipse.Configuration.Parameters;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Configuration.Categorization
{
    /// <summary>
    /// UI category in which an Parameter resides.
    /// </summary>
    public sealed class Category
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:
        public delegate void CategoryChangeHandler(Category category);
        //public delegate void NameChangeHandler(string name);
        //public delegate void VisibilityChangeHandler(bool visible);
        //public delegate void ParameterListChanged(Category category);

        // Events:
        public event CategoryChangeHandler? OnNameChanged;
        public event CategoryChangeHandler? OnVisibilityChanged;
        public event CategoryChangeHandler? OnParameterListChanged;

        // Properties:
        public IReadOnlyCollection<Parameter> Parameters
        {
            get
            {
                if (!m_Ordered)
                {
                    SortByOrder();
                }

                return m_Parameters;
            }
        }

        public string Name
        {
            get => m_Name;
            set
            {
                m_Name = value;
                OnNameChanged?.Invoke(this);
            }
        }

        // TODO: Also check whether all the parameters in a category is also visible. Notify when all are not visible.
        public bool Visible
        {
            get => m_Visible;
            set
            {
                m_Visible = value;
                OnVisibilityChanged?.Invoke(this);
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:
        private readonly List<Parameter> m_Parameters = new();
        private bool m_Visible;
        private string m_Name;

        // Local Fields:
        private bool m_Ordered;



        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public Category(string name, params Parameter[] parameters)
        {
            m_Name = name;
            m_Parameters.Capacity = Mathf.NextPowerOfTwo(parameters.Length);
            m_Parameters.AddRange(parameters);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public void Add(Parameter parameter)
        {
            m_Parameters.Add(parameter);
            m_Ordered = false;
            OnParameterListChanged?.Invoke(this);
        }

        public void AddRange(Parameter[] parameter)
        {
            m_Parameters.AddRange(parameter);
            m_Ordered = false;
            OnParameterListChanged?.Invoke(this);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void SortByOrder()
        {
            m_Parameters.Sort((a, b) => a.Category.Order.CompareTo(b.Category.Order));
            m_Ordered = true;
        }
    }
}
