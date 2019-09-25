using System;
using DIOS.Common;
using DIOS.Common.Interfaces;
using DIOS.ObjectLib;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Dios.Securities
{
	[LocalizedName("Граф")]
	[DataContract]
	public class InterdepositoryGraph: DIOS.ObjectLib.Object
	{
		public InterdepositoryGraph():base(){}

		public InterdepositoryGraph(UniStructView v, ObjectFactory f) : base(v, f) { }

		public const string EntityClassName = "INTERDEPOSITORY_GRAPH";
		#region interdepository_graph
		protected SqlInt32 _interdepository_graph;
		[ObjectPropertyAttribute("#", true, false, 0, false, true)]
		public SqlInt32 interdepository_graph
		{
			get
			{
				return _interdepository_graph;
			}
			set
			{
				_interdepository_graph = value;
			}
		}
		#endregion
		#region Константы
		#endregion
		#region RefObjects
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
			dataStore[i++] = interdepository_graph;
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