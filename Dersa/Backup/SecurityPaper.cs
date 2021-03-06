using System;
using DIOS.BusinessBase;
using DIOS.Common;
using DIOS.Common.Interfaces;
using DIOS.ObjectLib;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dios.Securities
{
	[LocalizedName("������ ������")]
	[DataContract]
	public class SecurityPaper: DIOS.ObjectLib.Object
	{
		public SecurityPaper():base(){}

		public SecurityPaper(UniStructView v, ObjectFactory f) : base(v, f) { }

		public const string EntityClassName = "SECURITY_PAPER";
		#region security_paper
		protected SqlInt32 _security_paper;
		[ObjectPropertyAttribute("#", true, false, 0, false, true)]
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
		#endregion
		#region emitent
		protected SqlInt32 _emitent;
		[ObjectPropertyAttribute("�������", true, false, 0, false, false)]
		public SqlInt32 emitent
		{
			get
			{
				return _emitent;
			}
			set
			{
				_emitent = value;
			}
		}
		[DataMember]
		private object __emitent
		{	get
			{
				if (_emitent.IsNull) return null;
				return _emitent.Value;
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
		#region ISIN
		protected SqlString _ISIN;
		[ObjectPropertyAttribute("ISIN", false, false, 0, true, false)]
		public SqlString ISIN
		{
			get
			{
				return _ISIN;
			}
			set
			{
				_ISIN = value;
			}
		}
		[DataMember]
		private object __ISIN
		{	get
			{
				if (_ISIN.IsNull) return null;
				return _ISIN.Value;
			}
		}
		#endregion
		#region nominal_value
		protected SqlMoney _nominal_value;
		[ObjectPropertyAttribute("�������", false, false, 0, true, false)]
		public SqlMoney nominal_value
		{
			get
			{
				return _nominal_value;
			}
			set
			{
				_nominal_value = value;
			}
		}
		[DataMember]
		private object __nominal_value
		{	get
			{
				if (_nominal_value.IsNull) return null;
				return _nominal_value.Value;
			}
		}
		#endregion
		#region registration_number
		protected SqlString _registration_number;
		[ObjectPropertyAttribute("��������������� �����", false, false, 255, true, false)]
		public SqlString registration_number
		{
			get
			{
				return _registration_number;
			}
			set
			{
				_registration_number = value;
			}
		}
		[DataMember]
		private object __registration_number
		{	get
			{
				if (_registration_number.IsNull) return null;
				return _registration_number.Value;
			}
		}
		#endregion
		#region is_verified
		protected SqlBoolean _is_verified;
		[ObjectPropertyAttribute("���������", false, false, 0, true, false)]
		public SqlBoolean is_verified
		{
			get
			{
				return _is_verified;
			}
			set
			{
				_is_verified = value;
			}
		}
		[DataMember]
		private object __is_verified
		{	get
			{
				if (_is_verified.IsNull) return null;
				return _is_verified.Value;
			}
		}
		#endregion
		#region ���������
		#endregion
		#region RefObjects
		#region emitentObject
		[ObjectPropertyAttribute("�������", true, false)]
		[ClassKeyName("EMITENT", "emitent")]
		public Dios.Securities.Emitent emitentObject
		{
			get
			{
				if (this.emitent.IsNull) return null;
				return (Dios.Securities.Emitent)GetObject("EMITENT", this.emitent);
			}
			set
			{
				if (value != null)
					emitent = value.emitent;
				else
					this.emitent = System.DBNull.Value;
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
		#region AfterNew
		protected override void AfterNew(IParameterCollection parameters)
		{
Logger.LogInfo(this);
IParameterCollection Params = new ParameterCollection();
Params.Add("company", this.emitent);
Params.Add("security_paper", this.security_paper);
Logger.LogInfo(Params);
ObjectFactory F = Manager.GetFactory("SECURITY_PAPER_TO_COMPANY");
if(!F.Exists(Params))
        F.Create(Params);

		}
		#endregion
		#endregion
		#region GetUniView()
		protected override  UniStructView GetUniView()
		{
			IndexerPropertyDescriptorCollection props = this.GetObjectProperties();
			object[] dataStore = new object[props.Count];
			int i = 0;
			dataStore[i++] = security_paper;
			dataStore[i++] = emitent;
			dataStore[i++] = changer;
			dataStore[i++] = created_at;
			dataStore[i++] = updated_at;
			dataStore[i++] = UID;
			dataStore[i++] = ISIN;
			dataStore[i++] = nominal_value;
			dataStore[i++] = registration_number;
			dataStore[i++] = is_verified;
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