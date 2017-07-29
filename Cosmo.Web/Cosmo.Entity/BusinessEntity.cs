using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    [Serializable]
    /// <summary>
    /// The root class for all Business Entities. Represents an Entity with a unique ID.   
    /// </summary>
    public class BusinessEntity
    {
        #region Declarations

        #region Variables

        private int _id = -1;
        protected const int EMPTY_ID = -1;

        #endregion

        #region Properties

        /// <summary>
        /// The unique identifer of this BusinessEntity
        /// </summary>
        /// <value>A <see langref="int">int</see> representing the unique identifer of this BusinessEntity</value>
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        public bool IsNull
        {
            get { return (_id == EMPTY_ID); }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new instance of object
        /// </summary>
        public BusinessEntity()
        {
            //Allow an empty constructor
        }

        /// <summary>
        /// Constructor a BusinessEntity with a given ID
        /// </summary>
        /// <param name="ID">The ID to use</param>
        public BusinessEntity(int baseID)
        {
            _id = baseID;
        }

        /// <summary>
        /// Constructor a BusinessEntity with a given ID
        /// </summary>
        /// <param name="IDString"></param>
        public BusinessEntity(string anID)
            : this(Int32.Parse(anID))
        {
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Override the equals method
        /// </summary>
        /// <param name="anObject"></param>
        /// <returns></returns>
        public override bool Equals(object anObject)
        {
            if (anObject == null)
                return false;

            //Assume ID's are unique. So two NamedBusinessEntity with the same ID are equal
            if (!(typeof(BusinessEntity).IsAssignableFrom(anObject.GetType())))
                return false;

            return (this.ID == ((BusinessEntity)anObject).ID);
        }

        /// <summary>
        /// override the GetHashCode() method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ((int)this.ID);
        }

        #endregion
    }
}
