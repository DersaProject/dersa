using System;
using DIOS.Common;
using DIOS.Common.Interfaces;
using DIOS.ObjectLib;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dios.Securities
{
	[LocalizedName("SECURITY_PAPER_TO_COMPANY")]
	[DataContract]
	public class SecurityPaperToCompany: DIOS.ObjectLib.Object
	{
		public SecurityPaperToCompany():base(){}

		public SecurityPaperToCompany(UniStructView v, ObjectFactory f) : base(v, f) { }

		public const string EntityClassName = "SECURITY_PAPER_TO_COMPANY";
		#region security_paper_to_company
		protected SqlInt32 _security_paper_to_company;
		[ObjectPropertyAttribute("#", true, false, 0, false, true)]
		public SqlInt32 security_paper_to_company
		{
			get
			{
				return _security_paper_to_company;
			}
			set
			{
				_security_paper_to_company = value;
			}
		}
		#endregion
		#region changer
		protected SqlString _changer;
		[ObjectPropertyAttribute("changer", false, false, 255, true, false)]
		public SqlString changer
		{
			get
			{
				return _changer;
			}
			set
			{
				_changer = value;
			}
		}
		[DataMember]
		private object __changer
		{	get
			{
				if (_changer.IsNull) return null;
				return _changer.Value;
			}
		}
		#endregion
		#region created_at
		protected SqlDateTime _created_at;
		[ObjectPropertyAttribute("created_at", false, false, 0, true, false)]
		public SqlDateTime created_at
		{
			get
			{
				return _created_at;
			}
			set
			{
				_created_at = value;
			}
		}
		[DataMember]
		private object __created_at
		{	get
			{
				if (_created_at.IsNull) return null;
				return _created_at.Value;
			}
		}
		#endregion
		#region updated_at
		protected SqlDateTime _updated_at;
		[ObjectPropertyAttribute("updated_at", false, false, 0, true, false)]
		public SqlDateTime updated_at
		{
			get
			{
				return _updated_at;
			}
			set
			{
				_updated_at = value;
			}
		}
		[DataMember]
		private object __updated_at
		{	get
			{
				if (_updated_at.IsNull) return null;
				return _updated_at.Value;
			}
		}
		#endregion
		#region UID
		protected SqlGuid _UID = Guid.NewGuid();
		[ObjectPropertyAttribute("UID", true, false, 0, true, false)]
		public SqlGuid UID
		{
			get
			{
				return _UID;
			}
			set
			{
				_UID = value;
			}
		}
		[DataMember]
		private object __UID
		{	get
			{
				if (_UID.IsNull) return null;
				return _UID.Value;
			}
		}
		#endregion
		#region security_paper
		protected SqlInt32 _security_paper;
		[ObjectPropertyAttribute("������ ������", false, false, 0, false, false)]
		public SqlInt32 security_paper
		{
			get
			{
				return _security_paper;
			}
			set
			{
				_security_paper = value;
			}
		}
		[DataMember]
		private object __security_paper
		{	get
			{
				if (_security_paper.IsNull) return null;
				return _security_paper.Value;
			}
		}
		#endregion
		#region company
		protected SqlInt32 _company;
		[ObjectPropertyAttribute("��������", false, false, 0, false, false)]
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
		#region ���������
		#endregion
		#region RefObjects
		#region security_paperObject
		[ObjectPropertyAttribute("������ ������", false, false)]
		[ClassKeyName("SECURITY_PAPER", "security_paper")]
		public Dios.Securities.SecurityPaper security_paperObject
		{
			get
			{
				if (this.security_paper.IsNull) return null;
				return (Dios.Securities.SecurityPaper)GetObject("SECURITY_PAPER", this.security_paper);
			}
			set
			{
				if (value != null)
					security_paper = value.security_paper;
				else
					this.security_paper = System.DBNull.Value;
			}
		}
		#endregion
		#region companyObject
		[ObjectPropertyAttribute("��������", false, false)]
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
		#region ������

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
			dataStore[i++] = security_paper_to_company;
			dataStore[i++] = changer;
			dataStore[i++] = created_at;
			dataStore[i++] = updated_at;
			dataStore[i++] = UID;
			dataStore[i++] = security_paper;
			dataStore[i++] = company;
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