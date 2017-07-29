using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    [Serializable]
    public class BusinessCode: BusinessEntity
    {
        #region Declarations

        #region variables

        private string _code = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// The unique identifer of this BusinessCode
        /// </summary>
        /// <value>A <see langref="int">int</see> representing the unique identifer of this BusinessCode</value>
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
            }
        }

        public bool IsNull
        {
            get { return (string.IsNullOrEmpty(_code)); }
        }

        #endregion

        #endregion Declarations

        #region Constructors

        public BusinessCode()
        {
            //Allow an empty constructor
        }

        /// <summary>
        /// Construct a BusinessCode with a given ID
        /// </summary>
        /// <param name="ID">The ID to use</param>
        public BusinessCode(string baseCode)
        {
            _code = baseCode;
        }

        /// <summary>
        /// Construct a BusinessCode with a given ID
        /// </summary>
        /// <param name="IDString"></param>
        public BusinessCode(int integerCode)
            : base(integerCode)
        {

        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Overrides the equal methods
        /// </summary>
        /// <param name="anObject"></param>
        /// <returns></returns>
        public override bool Equals(object anObject)
        {
            if (anObject == null)
                return false;

            //Assume ID's are unique. So two NamedBusinessCode with the same ID are equal
            if (!(typeof(BusinessCode).IsAssignableFrom(anObject.GetType())))
                return false;

            return (this.Code == ((BusinessCode)anObject).Code);
        }

        #endregion Public Methods
    }
}
