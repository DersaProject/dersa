using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class JSForm: StereotypeBaseE, ICompiledEntity
	{
		public JSForm(){}

		public JSForm(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string innerHTML = "";
		public bool WithPrintButton = false;
		public int Height = 700;
		public int Width = 700;

		#region Методы
		#region Show
		public string Show(object[] callParams)
		{
if(callParams != null && callParams.Length == 2)
			    return this.innerHTML;
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("W = window.open('','newwin','width=" + Width.ToString() + ",height=" + Height.ToString() + ",status=1,menubar=1');");
			sb.Append("W.document.open();");
			sb.Append("W.document.write('<html><head><title>");
			sb.Append(this.Name);
			sb.Append("');");
			sb.Append("W.document.write('</title></head><body>");
			if(WithPrintButton)
			{
			    sb.Append("<script>function Print(){B=document.getElementById(\"bnPrint\");B.style.display=\"none\";window.print();B.style.display=\"\"};</script>");
			    sb.Append("<button id=\"bnPrint\" onclick=\"Print()\">Печать</button><br>");
			}
			sb.Append("');");
			
			if(this.Id == 0 && callParams.Length == 1)
			{
			    sb.Append("W.document.write('");
			    sb.Append(this.innerHTML
			       .Replace("\\","\\\\")
			       .Replace("'","\\'")
			       .Replace("\r","")
			       .Replace("\n","<br>"));
			    sb.Append("');");
			}
			else
			{
			    sb.Append("var xhr=new XMLHttpRequest();");
			    if(callParams.Length == 1)
			    {
			        sb.Append("xhr.open('GET', 'Query/GetAction?MethodName=Show&id="); 
			        sb.Append(this.Id.ToString());
			        sb.Append("&paramString=[1]',false);");
			    }
			    if(callParams.Length > 3)
			    {
			        sb.Append("xhr.open('GET', 'Query/GetAction?MethodName=");
			        sb.Append(callParams[1].ToString());
			        sb.Append("&id="); 
			        sb.Append(callParams[2].ToString());
			        sb.Append("&paramString=");
			        sb.Append(callParams[3].ToString());
			        sb.Append("',false);");
			    }
			    sb.Append("xhr.send();");
			    sb.Append("W.document.write(xhr.responseText);");
			}
			
			sb.Append("W.document.write('</body></html>');");
			sb.Append("W.document.close();");
			return sb.ToString();
		}
		#endregion
		#endregion
	}
}
