using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using Dersa.Common;
namespace DersaStereotypes
{
	[Serializable()]
	public class Version: Entity, ICompiledEntity
	{
		public Version(){}

		public Version(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}

		#region Методы
		#region CheckIn
		public string CheckIn(object[] callParams)
		{
            string userName = callParams[0].ToString();
			            int id = this.Id;
			            EntityVC pEntity = this.Parent as EntityVC;
			            if (pEntity == null)
			                return "alert('Parent of " + id.ToString() + " is undefined!!!');";
			            string versionPackageId = DersaUtil.GetUserSetting(userName, "id папки для версий");

            DersaUtil.EntityAddRelation(userName, id, pEntity.Id, "Change", "version_" + pEntity.Id.ToString(), "Version", id);
            DersaUtil.EntityAddChild(userName, id.ToString(), versionPackageId, 0);
            DersaUtil.EntitySetAttribute(userName, pEntity.Id, "LastVersionId", this.Id.ToString());
			            return "var vname=prompt('Подтвердите название версии', '" + pEntity.Name + "_" + DateTime.Now.ToString() + "'); if(vname) RenameNode(" + id.ToString() + ", vname);";
			
			
		}
		#endregion
		#region AllowDrop
		public override bool AllowDrop()
		{
return false;
		}
		#endregion
		#endregion
	}
}
