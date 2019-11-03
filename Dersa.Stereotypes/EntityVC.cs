using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using Dersa.Common;
using Newtonsoft.Json;

namespace DersaStereotypes
{
	[Serializable()]
	public class EntityVC: Entity, ICompiledEntity
	{
		public EntityVC(){}

		public EntityVC(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public int LastVersionId;

		#region Ìועמהû
		#region AllowModifyChildren
		public override bool AllowModifyChildren()
		{
//            this.Reinitialize();
			//            var cInst = this.Object.GetCompiledInstance();
			//            EntityVC inst = cInst as EntityVC;
			//            return inst.LastVersionId == 0 && !inst.HasVersions();
			
			
			//new approach - changing stereotype!!
			
			return false;
		}
		#endregion
		#region HasVersions
		private bool HasVersions()
		{
                System.Collections.IList children = this.Children;
			                for (int i = 0; i < children.Count; i++)
			                {
			                    ICompiledEntity obj = (ICompiledEntity)children[i];
			                    if (obj is Version)
			                            return true;
			                }
			                return false;
		}
		#endregion
		#region AllowDrop
		public override bool AllowDrop()
		{
return false;
		}
		#endregion
		#region GenerateProduction
		public string GenerateProduction()
		{
            return "select 'generating sql for Production'";
		}
		#endregion
		#region CheckOut
		public string CheckOut(object[] callParams)
		{
            string userName = callParams[0].ToString();
			            int id = this.Id;
			            if(this.HasVersions())
			                return "alert('" + id.ToString() + " is already checked out!!!');";
            DersaUtil.EntityAddChild(userName, "Version", id.ToString(), 0);
			            this.ClearCache();
			            return "alert('" + id.ToString() + " Checked out.');";
		}
		#endregion
		#endregion
	}
}
