using System;

namespace Dersa.Interfaces
{
	//public delegate void NotifyObjectEventHandler(IQueueObject qo);

	public enum SelectState {Select, Insert, Update, Delete}
	public enum ObjectState {osPool, osNew, osLoad, osDrop, osDisposed}

	[Flags]
	public enum AccountType
	{
		atNone			= 0x0000,
		atRead			= 0x0001,
		atExecute		= 0x0002,
		atWriteOwned	= 0x0004,
		atWrite			= 0x0008, 
		atManage		= 0x0010
	}
	public enum ValueType
	{
		Value		= 1,
		CSharp		= 2,
		SQL			= 3,
		PlainText	= 4
	}
	public enum ArrowType
	{
		None,
		Arrow,
		Triangle,
		Ellipse,
		Rect,
		Cross
	}
	public enum LineFormType
	{
		None,
		Line,
		Curve
	}
	public enum StereotypeType
	{
		Abstract,
		Entity,
		Relation, 
		Class
	}
	[Serializable]
	public enum AuthentificationType
	{
		Windows, 
		Forms
	}
	[Serializable]
	public enum PropertyKey
	{
		None,
		ParentId,
		ParentTypeName,
		AId,
		ATypeName,
		BId,
		BTypeName
	}
	[Serializable]
	public enum ObjectOperation
	{
		None,
		Update,
		Remove
	}
	[Serializable]
	public struct Property
	{
		public PropertyKey Key;
		public object Value;
	}
	[Serializable]
	public enum SearchType
	{
		Name,
		Property,
		Code
	}
}
