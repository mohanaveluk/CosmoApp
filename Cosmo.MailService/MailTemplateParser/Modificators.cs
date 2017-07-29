using System;
using System.Collections;

namespace Cosmo.MailService
{

    /// <summary>
    /// Abstract class for Modificators
    /// </summary>
    public abstract class Modificator
    {
        private Hashtable _parameters = new Hashtable();

        /// <summary>
        /// Gets the parameters
        /// </summary>
        public Hashtable Parameters
        { 
            get { return _parameters; }
        }
        /// <summary>
        /// Apply the modification
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>
        public abstract void Apply(ref string Value, params string[] Parameters);
    }
    /// <summary>
    /// LineBreak to Html Break tag Modificator
    /// </summary>
    class NL2BR : Modificator
    {
        /// <summary>
        /// Apply the modification from linebreak to Html Line Break Tag
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("\n", "<br>");
        }
    }

    /// <summary>
    /// HtmlEncode Modificator
    /// </summary>
    class HTMLENCODE : Modificator
    {
        /// <summary>
        /// Apply the modification for HtmlEncode 
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>  
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("&", "&amp;");
            Value = Value.Replace("<", "&lt;");
            Value = Value.Replace(">", "&gt;");
        }
    }

    /// <summary>
    /// Upper case Modificator
    /// </summary>
    class UPPER : Modificator
    {
        /// <summary>
        /// Apply the modification to change the value to upper case Html Break tag
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToUpper();
        }
    }

    /// <summary>
    /// Lower case Modificator
    /// </summary>
    class LOWER : Modificator
    {
        /// <summary>
        /// Apply the modification to change the value to lower case 
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToLower();
        }
    }

    /// <summary>
    /// Trim Modificator. Remove empty string suffix and prefix .
    /// </summary>
    class TRIM : Modificator
    {
        /// <summary>
        /// Apply the modification to remove the empty string prefix and suffix 
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>

        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Trim();
        }
    }

    /// <summary>
    /// Trim Modificator. Remove empty string suffix .
    /// </summary>
    class TRIMEND : Modificator
    {
        /// <summary>
        /// Apply the modification to remove the empty string suffix
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>

        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimEnd();
        }
    }

    /// <summary>
    /// Trim Modificator. Remove empty string  prefix .
    /// </summary>
    class TRIMSTART : Modificator
    {
        /// <summary>
        /// Apply the modification to remove the empty string prefix 
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>

        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimStart();
        }
    }

    /// <summary>
    /// Default Modificator. Change the value to another default value
    /// </summary> 
    class DEFAULT : Modificator
    {
        /// <summary>
        /// Apply the modification to change the value to a default value
        /// </summary>
        /// <param name="Value">The value to be modified</param>
        /// <param name="Parameters">An array of System.String to which the value will be modified.</param>
 
        public override void Apply(ref string Value, params string[] Parameters)
        {
            if (Value == null || Value.Trim() == string.Empty)
            {
                Value = Parameters[0];
            }
        }
    }
}