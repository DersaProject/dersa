using System;
using DIOS.Common;
using DIOS.Common.Interfaces;
using DIOS.ObjectLib;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dios.Securities
{
	[LocalizedName("�����������")]
	[DataContract]
	public class Depository: DIOS.ObjectLib.Object
	{
		public Depository():base(){}

		public Depository(UniStructView v, ObjectFactory f) : base(v, f) { }

		public const string EntityClassName = "DEPOSITORY";
		#region depository
		protected SqlInt32 _depository;
		[ObjectPropertyAttribute("#", true, false, 0, false, true)]
		public SqlInt32 depository
		{
			get
			{
				return _depository;
			}
			set
			{
				_depository = value;
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
		#region INN
		protected SqlString _INN;
		[ObjectPropertyAttribute("INN", false, false, 0, true, false)]
		public SqlString INN
		{
			get
			{
				return _INN;
			}
			set
			{
				_INN = value;
			}
		}
		[DataMember]
		private object __INN
		{	get
			{
				if (_INN.IsNull) return null;
				return _INN.Value;
			}
		}
		#endregion
		#region name
		protected SqlString _name;
		[ObjectPropertyAttribute("name", false, false, 255, true, false)]
		public SqlString name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		[DataMember]
		private object __name
		{	get
			{
				if (_name.IsNull) return null;
				return _name.Value;
			}
		}
		#endregion
		#region ���������
		#endregion
		#region RefObjects
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

		#region HasIdentityKey
		protected override bool HasIdentityKey()
		{
			return false;
		}
		#endregion
		#endregion
		#region GetUniView()
		protected override  UniStructView GetUniView()
		{
			IndexerPropertyDescriptorCollection props = this.GetObjectProperties();
			object[] dataStore = new object[props.Count];
			int i = 0;
			dataStore[i++] = depository;
			dataStore[i++] = changer;
			dataStore[i++] = created_at;
			dataStore[i++] = updated_at;
			dataStore[i++] = UID;
			dataStore[i++] = INN;
			dataStore[i++] = name;
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