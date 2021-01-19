using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class Method: StereotypeBaseE, ICompiledEntity
	{
		public Method(){}

		public Method(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.Boolean Unsafe = false;
		public System.Boolean Interface = true;
		public System.String AccessModifier = "public";
		public System.String Parameters = "";
		public System.Boolean DoNotInherit = false;
		public System.String Attributes = "";
		public System.String Description = "";
		public System.String Text = "";
		public System.Boolean Transactional = false;
		public System.String ReturnType = "void";

		#region ћетоды
		#region GetParameters
		public System.String GetParameters(bool RemoveDefaultValues)
		{
        string result = this.Parameters;
			        if (result == null)
			            result = "";
			        if (RemoveDefaultValues)
			        {
			            System.Text.StringBuilder sbNewParams = new System.Text.StringBuilder();
			            string[] ParamsWithValues = result.Split(',');
			            for (int p = 0; p < ParamsWithValues.Length; p++)
			            {
			                if (p > 0)
			                    sbNewParams.Append(", ");
			                string ParamWithValue = ParamsWithValues[p];
			                string PureParam = ParamWithValue.Split('=')[0].Trim();
			                sbNewParams.Append(PureParam);
			            }
			            result = sbNewParams.ToString();
			        }
			        return result;
			
		}
		#endregion
		#region GenerateInterface
		public System.String GenerateInterface()
		{
if (!this.Interface) return "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("\t\t[ObjectMethod(");
			sb.Append("\"" + Description + "\"");
			
			if (this.AccessModifier.IndexOf("protected") > -1 || this.AccessModifier.IndexOf("private") > -1)
				throw new Exception("<»Ќ“≈–‘≈…—Ќџ…> метод " + this.Name + "\nу " + this.Parent.Name + " не может быть protected или private");
			
			if (this.AccessModifier.IndexOf("static") > -1)
				throw new Exception("<»Ќ“≈–‘≈…—Ќџ…> метод " + this.Name + "\nу " + this.Parent.Name + " не может быть static");
			
			if (this.Transactional)
			{
				sb.Append(", true");
			}
			else
			{
				sb.Append(", false");
			}
			sb.Append(")]\n");
			sb.Append("\t\t" + ReturnType + " " + Name + "(" + Parameters + ");\n");
			return sb.ToString();
		}
		#endregion
		#region GetMapKey
		public System.String GetMapKey()
		{
	return this.AccessModifier + " " + this.ReturnType + " " + this.Name + " " + this.Parameters;
		}
		#endregion
		#region GetDeclarationString
		public System.String GetDeclarationString()
		{
return "\t\t" + this.AccessModifier + " " + this.ReturnType + " " + this.Name + "(" + this.GetParameters(true) + ")";
		}
		#endregion
		#region Generate
		public System.String Generate()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("\t\t#region " + this.Name + "\n");
			if ((this.Attributes != null)&&(this.Attributes != String.Empty))
			{
				sb.Append("\t\t" + this.Attributes + "\n");
			}
			
			//	if (this.Transactional && (this.AccessModifier.IndexOf("protected") > -1 || this.AccessModifier.IndexOf("private") > -1))
			//		throw new Exception("<“–јЌ«ј ÷»ќЌЌџ…> ћетод " + this.Name + "\nу " + this.Parent.Name + " не может быть protected или private");
			//		
			//	if (this.Transactional && this.AccessModifier.IndexOf("static") > -1)
			//		throw new Exception("<“–јЌ«ј ÷»ќЌЌџ…> ћетод " + this.Name + "\nу " + this.Parent.Name + " не может быть static");
					
			//	if (!Unsafe)
			//	{
			//		int indexOf = 0;
			//		while (indexOf != -1)
			//		{
			//			indexOf = this.Text.IndexOf("this", indexOf);
			//			if (indexOf == -1) break;
			//			if (this.Text.Length >= indexOf + 4)
			//			{
			//				if (this.Text[indexOf + 4] == '.')
			//				{
			//					indexOf = indexOf + 4;
			//					continue;
			//				}
			//			}
			//			throw new Exception("ѕотенциально опасный код!\nѕроверьте тело метода " + this.Name + " у объекта " + this.Parent.Name);
			//		}
			//	}
			
			if(this.Parent is Entity)
			{
				sb.Append("\t\t[ObjectMethod(");
				sb.Append("\"" + Description + "\"");
			
				if (this.Transactional)
				{
					sb.Append(", true");
				}
				else
				{
					sb.Append(", false");
				}
				sb.Append(")]\n");
			}
			
			sb.Append(this.GetDeclarationString());
			
			sb.Append("\n\t\t{\n");
			string MethodBody = this.Text;
			if(this._parent != null && this.Parent is StereotypeE)
			{
				sb.Append("\t\t\t");
				MethodBody = MethodBody.Replace("\n", "\n\t\t\t");
			}
			sb.Append(MethodBody);
			sb.Append("\n\t\t}\n");
			sb.Append("\t\t#endregion\n");
			return sb.ToString();
		}
		#endregion
		#endregion
	}
}
