using System;
using DIOS.Common;
using DIOS.Common.Interfaces;
using DIOS.ObjectLib;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dios.Securities
{
	[LocalizedName("Баланс по ц/б")]
	[DataContract]
	public class SecurityPaperBalance: DIOS.ObjectLib.Object
	{
		public SecurityPaperBalance():base(){}

		public SecurityPaperBalance(UniStructView v, ObjectFactory f) : base(v, f) { }

		public const string EntityClassName = "SECURITY_PAPER_BALANCE";
		#region security_paper_balance
		protected SqlInt32 _security_paper_balance;
		[ObjectPropertyAttribute("#", true, false, 0, false, true)]
		public SqlInt32 security_paper_balance
		{
			get
			{
				return _security_paper_balance;
			}
			set
			{
				_security_paper_balance = value;
			}
		}
		#endregion
		#region company
		protected SqlInt32 _company;
		[ObjectPropertyAttribute("компания", false, false, 0, false, false)]
		public SqlInt32 company
		{
			get
			{
				return _company;
			}
			set
			{
				_company = value;
			}
		}
		[DataMember]
		private object __company
		{	get
			{
				if (_company.IsNull) return null;
				return _company.Value;
			}
		}
		#endregion
		#region ISIN
		protected SqlString _ISIN;
		[ViewProperty]
		[ObjectPropertyAttribute("ISIN", false, true)]
		public SqlString ISIN
		{
			get
			{
				return _ISIN;
			}
		}
		#endregion
		#region owned_qty
		protected SqlInt32 _owned_qty;
		[ViewProperty]
		[ObjectPropertyAttribute("owned_qty", false, true)]
		public SqlInt32 owned_qty
		{
			get
			{
				return _owned_qty;
			}
		}
		#endregion
		#region deposed_qty_in
		protected SqlInt32 _deposed_qty_in;
		[ViewProperty]
		[ObjectPropertyAttribute("deposed_qty_in", false, true)]
		public SqlInt32 deposed_qty_in
		{
			get
			{
				return _deposed_qty_in;
			}
		}
		#endregion
		#region deposed_qty_out
		protected SqlInt32 _deposed_qty_out;
		[ViewProperty]
		[ObjectPropertyAttribute("deposed_qty_out", false, true)]
		public SqlInt32 deposed_qty_out
		{
			get
			{
				return _deposed_qty_out;
			}
		}
		#endregion
		#region Константы
		#endregion
		#region RefObjects
		#region companyObject
		[ObjectPropertyAttribute("Компания", false, false)]
		[ClassKeyName("COMPANY", "company")]
		public Dios.Securities.Company companyObject
		{
			get
			{
				if (this.company.IsNull) return null;
				return (Dios.Securities.Company)GetObject("COMPANY", this.company);
			}
			set
			{
				if (value != null)
					company = value.company;
				else
					this.company = System.DBNull.Value;
			}
		}
		#endregion
		#endregion
		#region Методы

		#region GetFactory
		public static ObjectFactory GetFactory()
		{
			ApplicationSqlManager M = CommonEnvironment.StaticManager;
			ObjectFactory F = M.GetFactory(EntityClassName);
			M.IsOccupied = false;
			return F;
		}
		#endregion
		#endregion
		#region GetUniView()
		protected override  UniStructView GetUniView()
		{
			IndexerPropertyDescriptorCollection props = this.GetObjectProperties();
			object[] dataStore = new object[props.Count];
			int i = 0;
			dataStore[i++] = security_paper_balance;
			dataStore[i++] = company;
			dataStore[i++] = ISIN;
			dataStore[i++] = owned_qty;
			dataStore[i++] = deposed_qty_in;
			dataStore[i++] = deposed_qty_out;
			for(int k = i; k < props.Count; k++)
			{
				dataStore[k] = this[props[k].Name];
			}
			UniStructView result = new UniStructView(dataStore, props);
			return result;
		}
		#endregion
		#region Refs
		#endregion
		#region Properties
		#endregion
	}
}