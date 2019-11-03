using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
    [Serializable()]
    public class Function : StereotypeBaseE, ICompiledEntity
    {
        public Function() { }

        public Function(IDersaEntity obj)
        {
            _object = obj;
            if (_object != null)
            {
                _name = _object.Name;
                _id = _object.Id;
            }
        }
        public System.String SQL = "";
        public System.String Source = "SQL";
        public System.String Params = "()";
        public System.String Return = "int";
        public System.Boolean MakePrefix = true;
        public System.String PrefixDelimiter = "@";
        public System.Boolean ReturnsTable = false;

        #region Ìועמהû
        #region sqlGenerate
        public string sqlGenerate()
        {
            return this.Generate(this.Parent);

        }
        #endregion
        #region Generate
        public string Generate(Dersa.Interfaces.ICompiledEntity owner)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string sqlName = this.GetSqlName(owner);

            sb.Append("if exists (select * from sysobjects where id = object_id('dbo." + sqlName + "'))\n");
            sb.Append("\tdrop function dbo." + sqlName + "\n");
            sb.Append("go\n\n");

            sb.Append("create function dbo." + sqlName);
            sb.Append(this.Params + "\n");
            if (Return != "")
            {
                sb.Append("returns	");
                sb.Append(Return);
                sb.Append("\n\n");
            }

            sb.Append(this.SQL);
            sb.Append("\ngo\n\n");
            if (this.ReturnsTable)
                sb.Append("grant select on " + sqlName + " to public\n");
            else
                sb.Append("grant execute on " + sqlName + " to public\n");
            sb.Append("go\n\n");

            string sOut = sb.ToString();
            return sOut;

        }
        #endregion
        #region GetSqlName
        public System.String GetSqlName(Dersa.Interfaces.ICompiledEntity owner)
        {
            if (MakePrefix)
            {
                if (owner == null)
                {
                    owner = this.Parent;
                    if (owner == null)
                    {
                        return this.Name;
                    }
                }
                string sqlName = "";
                if (owner is Entity)
                {
                    sqlName = ((Entity)owner).GetSqlName();
                }
                else if (owner is View)
                {
                    sqlName = ((View)owner).GetSqlName();
                }
                return sqlName + this.PrefixDelimiter + this.Name;
            }
            else
            {
                return this.Name;
            }
        }
        #endregion
        #endregion
    }
}