using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    [Serializable]
    /// <summary>
    /// Base class for the named business code
    /// </summary>
    public class NamedBusinessCode: BusinessCode
    {
        #region Declarations

        #region Private Variables

        private string _name = string.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// The NamedBusinessEntity's name
        /// </summary>
        /// <value>A <see langword="string">string</see> representing the Value Object's name</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion Public Properties

        #endregion Declarations

        #region Constructors

        /// <summary>
        /// Instantiate a new instance of NamedBusinessEntity
        /// </summary>
        public NamedBusinessCode()
        {
        }

        /// <summary>
        /// Instantiate a new instance of NamedBusinessEntity
        /// </summary>        
        /// <param name="anID">Initializes the <see cref="ID">ID</see> property</param>
        /// <param name="aName">Initializes the <see cref="Name">Name</see> property</param>
        public NamedBusinessCode(string aCode, string aName)
            : base(aCode)
        {
            this._name = aName;
        }

        /// <summary>
        /// Instantiate a new instance of NamedBusinessEntity
        /// </summary>        
        /// <param name="IDinteger">Initializes the <see cref="ID">ID</see> property</param>
        /// <param name="name">Initializes the <see cref="Name">Name</see> property</param>
        public NamedBusinessCode(int anID, string aName)
            : base(anID)
        {
            this._name = aName;
        }

        #endregion
    }
}
