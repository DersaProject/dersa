using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Index: StereotypeBaseE, ICompiledEntity
	{
		public Index(){}

		public Index(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Fields = "";
		public System.Boolean Clustered = false;
		public System.Boolean Unique = true;
		public System.String FillFactor = "";
		public System.String Filegroup = "INDEX";

		#region Ìועמהû
		#region Generate
		public System.String Generate(System.Boolean dialog, Dersa.Interfaces.ICompiledEntity owner)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (owner == null) owner = this.Parent;
			if (!(owner is Entity)) return "";
			string sqlName = ((Entity)owner).GetSqlName();
//			sb.Append("if exists(select * from sysindexes (NOLOCK)\n");
//			sb.Append("\twhere indid not in (0, 255) and name = '" + sqlName + "_" + this.Name + "')\n");
//			sb.Append("\t\tdrop index " + sqlName + "." + sqlName + "_" + this.Name);
//			sb.Append("\ngo\n");
			sb.Append("create ");
			if (this.Unique) sb.Append("unique ");
			if (this.Clustered) sb.Append("clustered ");
			sb.Append("index " + sqlName + "_" + this.Name);
			sb.Append(" on " + sqlName + "(" + this.Fields + ")");
Package entityPackage = owner.Parent as Package;
entityPackage.Reinitialize();
Configuration dbSettings = entityPackage.GetConfiguration("Db Settings");
if(dbSettings != null)
{
     string indexTablespace = dbSettings.GetSetting("Index Tablespace");
     if(indexTablespace != null)
         sb.Append(" tablespace " + indexTablespace + "\n");
}
			sb.Append("\ngo\n");
			string sOut = sb.ToString();
			/*
			if (dialog)
			{
				SqlExecForm.Exec(sOut);
				return null;
			}*/
			return sOut;
		}
		#endregion
		#endregion
	}
}
