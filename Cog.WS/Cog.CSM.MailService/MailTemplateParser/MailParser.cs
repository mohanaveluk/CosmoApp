using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Cog.CSM.MailService
{

    /// <remarks>
    /// Template Parser allows setup variables and conditions block in template.
    /// Also you can use some of variable's modificators.
    ///
    ///     Author: Alexander Kleshevnikov
    ///     E-mail: seigo@icconline.com
    ///
    /// <example>There is the simple example of template for html page:
    /// <code>
    /// <html>
    /// <head><title>{{Title}}</title></head>
    /// <body><h1>{{Title:upper}}</h1>
    /// {{If--IsRegisteredUser}}
    /// Hello, {{UserName}}!
    /// {{Else--IsRegisteredUser}}
    /// Please sign in.
    /// {{EndIf--IsRegisteredUser}}
    /// </body>
    /// </html>
    /// </code>
    /// To parse this template you can use the following code:
    /// <code>
    /// ...
    /// Hashtable Variables = new Hashtable();
    /// Variables.Add("Title", "Login In");
    /// Variables.Add("IsRegisteredUser", true);
    /// Variables.Add("UserName", "seigo");
    /// TemplateParser tpl = new TemplateParser("template.htm", Variables);
    /// tpl.ParseToFile("result.htm");
    /// ...
    /// </code>
    /// </example>
    /// </remarks>
    public class MailParser
    {
        private string      _templateBlock;
        private Hashtable   _htValues;
        private Hashtable   _errorMessage = new Hashtable();
        private string      _parsedBlock;

        private Dictionary<string, MailParser> _blocks = new Dictionary<string, MailParser>();

        private string VariableTagBegin        = "{{";
        private string VariableTagEnd          = "}}";

        private string ModificatorTag          = ":";
        private string ModificatorParamSep     = ",";

        private string ConditionTagIfBegin     = "{{If--";
        private string ConditionTagIfEnd       = "}}";
        private string ConditionTagElseBegin   = "{{Else--";
        private string ConditionTagElseEnd     = "}}";
        private string ConditionTagEndIfBegin  = "{{EndIf--";
        private string ConditionTagEndIfEnd    = "}}";

		private string BlockTagBeginBegin      = "{{BlockBegin--";
		private string BlockTagBeginEnd        = "}}";
		private string BlockTagEndBegin        = "{{BlockEnd--";
		private string BlockTagEndEnd          = "}}";

        /// <value>Template block</value>
        public string TemplateBlock
        {
            get { return this._templateBlock; }
            set 
            { 
                this._templateBlock = value;
                ParseBlocks();
            }
        }

        /// <value>Template Variables</value>
        public Hashtable Variables
        {
            get { return this._htValues; }
            set { this._htValues = value; }
        }

        /// <value>Error Massage</value>
        public Hashtable ErrorMessage
        {
            get { return _errorMessage; }
        }

        /// <value>Blocks inside template</value>
        public Dictionary<string, MailParser> Blocks
        {
            get { return _blocks; }
        }

       

        #region Contructors
        /// <summary>
        /// Creates a new instance of MailParser class
        /// </summary>
        public MailParser()
        {
            this._templateBlock = "";
        }
        /// <summary>
        /// Creates a new instance of MailParser class
        /// </summary>
        public MailParser(string filePath)
        {
            ReadTemplateFromFile(filePath);
            ParseBlocks();
        }
        /// <summary>
        /// Creates a new instance of MailParser class
        /// </summary>
        public MailParser(Hashtable Variables)
        {
            this._htValues = Variables;
        }
        /// <summary>
        /// Creates a new instance of MailParser class
        /// </summary>
        public MailParser(string filePath, Hashtable Variables)
        {
            ReadTemplateFromFile(filePath);
            this._htValues = Variables;
            ParseBlocks();
        }
        #endregion

        /// <summary>
        /// Setup template from specified file
        /// </summary>
        /// <param name="path">Full phisical path to template file</param>
        public void SetTemplateFromFile(string path)
        {
            ReadTemplateFromFile(path);
        }

        /// <summary>
        /// Setup template as string block
        /// </summary>
        /// <param name="templateBlock">String template block</param>
        public void SetTemplate(string templateBlock)
        {
            this.TemplateBlock = templateBlock;
        }

        /// <summary>
        /// Parse template after setuping Template and Variables
        /// </summary>
        /// <returns>
        /// Parsed Block for Whole Template
        /// </returns>
        public string Parse()
        {
            ParseConditions();
            ParseVariables();
            return this._parsedBlock;
        }

        /// <summary>
        /// Parse Template Block
        /// </summary>
        /// <returns>
        /// Parsed Block for Specified BlockName
        /// </returns>
        public string ParseBlock(string blockName, Hashtable variables)
        {
            if (!this._blocks.ContainsKey(blockName))
            {
                throw new ArgumentException(String.Format("Could not find Block with Name '{0}'", blockName));
            }

            this._blocks[blockName].Variables = variables;
            return this._blocks[blockName].Parse();
        }

        /// <summary>
        /// Parse template and save result into specified file
        /// </summary>
        /// <param name="path">Full physical path to file</param>
        /// <param name="replaceIfExists">If true file which already exists
        /// will be replaced</param>
        /// <returns>True if new content has been written</returns>
        public bool ParseToFile(string path, bool replaceIfExists)
        {
            if (File.Exists(path) && !replaceIfExists)
            {
                return false;
            }
            else
            {
                StreamWriter sr = File.CreateText(path);
                sr.Write(Parse());
                sr.Close();
                return true;
            }
        }

        /// <summary>
        /// Read template content from specified file
        /// </summary>
        /// <param name="path">Full physical path to template file</param>
        private void ReadTemplateFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("Template file does not exist.");
            }

            StreamReader reader = new StreamReader(path);
            this.TemplateBlock = reader.ReadToEnd();
            reader.Close();
        }

        /// <summary>
        /// Parse all blocks in template
        /// </summary>
        private void ParseBlocks()
        {
            //int idxPrevious = 0;
            int idxCurrent = 0;
            while ((idxCurrent = this._templateBlock.IndexOf(this.BlockTagBeginBegin, idxCurrent)) != -1)
            {
                string BlockName;
                int idxBlockBeginBegin, idxBlockBeginEnd, idxBlockEndBegin;

                idxBlockBeginBegin = idxCurrent;
                idxCurrent += this.BlockTagBeginBegin.Length;

                // Searching for BlockBeginEnd Index

                idxBlockBeginEnd = this._templateBlock.IndexOf(this.BlockTagBeginEnd, idxCurrent);
                if (idxBlockBeginEnd == -1) throw new Exception("Could not find BlockTagBeginEnd");

                // Getting Block Name

                BlockName = this._templateBlock.Substring(idxCurrent, (idxBlockBeginEnd - idxCurrent));
                idxCurrent = idxBlockBeginEnd + this.BlockTagBeginEnd.Length;

                // Getting End of Block index

                string EndBlockStatment = this.BlockTagEndBegin + BlockName + this.BlockTagEndEnd;
                idxBlockEndBegin = this._templateBlock.IndexOf(EndBlockStatment, idxCurrent);
                if (idxBlockEndBegin == -1) throw new Exception("Could not find End of Block with name '" + BlockName + "'");

                // Add Block to Dictionary

                MailParser block = new MailParser();
                block.TemplateBlock = this._templateBlock.Substring(idxCurrent, (idxBlockEndBegin - idxCurrent));
                this._blocks.Add(BlockName, block);

                // Remove Block Declaration From Template

                this._templateBlock = this._templateBlock.Remove(idxBlockBeginBegin, (idxBlockEndBegin - idxBlockBeginBegin));

                idxCurrent = idxBlockBeginBegin;
            }
        }

        /// <summary>
        /// Parse all conditions in template
        /// </summary>
        private void ParseConditions()
        {
            int idxPrevious = 0;
            int idxCurrent = 0;
            this._parsedBlock = "";
            while ((idxCurrent = this._templateBlock.IndexOf(this.ConditionTagIfBegin, idxCurrent)) != -1)
            {
                string VarName;
                string TrueBlock, FalseBlock;
                string ElseStatment, EndIfStatment;
                int idxIfBegin, idxIfEnd, idxElseBegin, idxEndIfBegin;
                bool boolValue;

                idxIfBegin = idxCurrent;
                idxCurrent += this.ConditionTagIfBegin.Length;

                // Searching for EndIf Index

                idxIfEnd = this._templateBlock.IndexOf(this.ConditionTagIfEnd, idxCurrent);
                if (idxIfEnd == -1) throw new Exception("Could not find ConditionTagIfEnd");

                // Getting Value Name

                VarName = this._templateBlock.Substring(idxCurrent, (idxIfEnd-idxCurrent));

                idxCurrent = idxIfEnd + this.ConditionTagIfEnd.Length;

                // Compare ElseIf and EndIf Indexes

                ElseStatment = this.ConditionTagElseBegin+VarName+this.ConditionTagElseEnd;
                EndIfStatment = this.ConditionTagEndIfBegin+VarName+this.ConditionTagEndIfEnd;
                idxElseBegin = this._templateBlock.IndexOf(ElseStatment, idxCurrent);
                idxEndIfBegin = this._templateBlock.IndexOf(EndIfStatment, idxCurrent);
                if (idxElseBegin > idxEndIfBegin) throw new Exception("Condition Else Tag placed after Condition Tag EndIf for '"+VarName+"'");

                // Getting True and False Condition Blocks

                if (idxElseBegin != -1)
                {
                    TrueBlock = this._templateBlock.Substring(idxCurrent, (idxElseBegin-idxCurrent));
                    FalseBlock = this._templateBlock.Substring((idxElseBegin+ElseStatment.Length), (idxEndIfBegin-idxElseBegin-ElseStatment.Length));
                }
                else
                {
                    TrueBlock = this._templateBlock.Substring(idxCurrent, (idxEndIfBegin-idxCurrent));
                    FalseBlock = "";
                }

                // Parse Condition

                try
                {
                    boolValue = Convert.ToBoolean(this._htValues[VarName]);
                }
                catch
                {
                    boolValue = false;
                }

                string BeforeBlock = this._templateBlock.Substring(idxPrevious, (idxIfBegin-idxPrevious));
                if (this._htValues.ContainsKey(VarName) && boolValue)
                {
                    this._parsedBlock += BeforeBlock + TrueBlock.Trim();
                }
                else
                {
                    this._parsedBlock += BeforeBlock + FalseBlock.Trim();
                }

                idxCurrent = idxEndIfBegin + EndIfStatment.Length;
                idxPrevious = idxCurrent;
            }
            this._parsedBlock += this._templateBlock.Substring(idxPrevious);
        }

        /// <summary>
        /// Parse all variables in template
        /// </summary>
        private void ParseVariables()
        {
            int idxCurrent = 0;
            while ((idxCurrent = this._parsedBlock.IndexOf(this.VariableTagBegin, idxCurrent)) != -1)
            {
                string VarName, VarValue;
                int idxVarTagEnd;

                idxVarTagEnd = this._parsedBlock.IndexOf(this.VariableTagEnd, (idxCurrent+this.VariableTagBegin.Length));
                if (idxVarTagEnd == -1) throw new Exception(String.Format("Index {0}: could not find Variable End Tag", idxCurrent));

                // Getting Variable Name

                VarName = this._parsedBlock.Substring((idxCurrent+this.VariableTagBegin.Length), (idxVarTagEnd-idxCurrent-this.VariableTagBegin.Length));

                // Checking for Modificators

                string[] VarParts = VarName.Split(this.ModificatorTag.ToCharArray());
                VarName = VarParts[0];

                // Getting Variable Value
                // If Variable doesn't exist in _hstValue then
                // Variable Value equal empty string

                // [added 6/6/2006] If variable is null than it will also has empty string

                VarValue = String.Empty;
                if (this._htValues.ContainsKey(VarName) && this._htValues[VarName] != null)
                {
                    VarValue = this._htValues[VarName].ToString();
                }

                // Apply All Modificators to Variable Value

                for (int i = 1; i < VarParts.Length; i++)
                    this.ApplyModificator(ref VarValue, VarParts[i]);

                // Replace Variable in Template

                this._parsedBlock = this._parsedBlock.Substring(0, idxCurrent) + VarValue + this._parsedBlock.Substring(idxVarTagEnd+this.VariableTagEnd.Length);

                // Add Length of added value to Current index 
                // to prevent looking for variables in the added value
                // Fixed Date: April 5, 2006
                idxCurrent += VarValue.Length;
            }
        }

        /// <summary>
        /// Method for applying modificators to variable value
        /// </summary>
        /// <param name="Value">Variable value</param>
        /// <param name="Modificator">Determination statment</param>
        private void ApplyModificator(ref string Value, string Modificator)
        {
            // Checking for parameters
            
            string strModificatorName = "";
            string strParameters = "";
            int idxStartBrackets, idxEndBrackets;
            if ((idxStartBrackets = Modificator.IndexOf("(")) != -1) {
                idxEndBrackets = Modificator.IndexOf(")", idxStartBrackets);
                if (idxEndBrackets == -1)
                {
                    throw new Exception("Incorrect modificator expression");
                }
                else
                {
                    strModificatorName = Modificator.Substring(0, idxStartBrackets).ToUpper();             
                    strParameters = Modificator.Substring(idxStartBrackets+1, (idxEndBrackets-idxStartBrackets-1));
                }
            }
            else
            {
                strModificatorName = Modificator.ToUpper();
            }
            string[] arrParameters = strParameters.Split(this.ModificatorParamSep.ToCharArray());
            for (int i = 0; i < arrParameters.Length; i++)
                arrParameters[i] = arrParameters[i].Trim();

            try
            {
                Type typeModificator = Type.GetType("TRW.Net.Mail.TemplateParser.Modificators." + strModificatorName);
                if (typeModificator.IsSubclassOf(Type.GetType("TRW.Net.Mail.TemplateParser.Modificators.Modificator")))
                {
                    Modificator objModificator = (Modificator)Activator.CreateInstance(typeModificator);
                    objModificator.Apply(ref Value, arrParameters);
                }
            }
            catch
            {
                throw new Exception(String.Format("Could not find modificator '{0}'", strModificatorName));
            }
        }
    }
}