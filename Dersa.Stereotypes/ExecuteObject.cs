using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;


namespace DersaStereotypes
{
[Serializable()]
public class ExecuteObject
{
	public ExecuteObject()
	{
	}
	#region ����������� ��������
	#endregion
	#region ����������� ������
	#endregion

	#region ��������
	#region Args
	public System.Object[] Args;
	#endregion
	#region MethodName
	public System.String MethodName;
	#endregion
	#region Parameters
	public System.String Parameters;
	#endregion
	#region ReturnType
	public System.String ReturnType;
	#endregion
	#region Text
	public System.String Text;
	#endregion
	#endregion

	#region ��������
	#endregion
}
}