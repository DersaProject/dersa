using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using DIOS.Common;
using DIOS.Common.Interfaces;
using System.Collections;
namespace DersaStereotypes
{
	[Serializable()]
	public class Script: StereotypeBaseE, ICompiledEntity
	{
		public Script(){}

		public Script(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public System.String Parameters = "";
		public System.String Source = "CSharp";
		public System.String Code = "";
		public string Using = "";
		public System.String ReturnType = "System.String";
		public System.String PhisicalName = "";
		public Dersa.Common.DersaUserSqlManager userSqlManager = new Dersa.Common.DersaUserSqlManager();

		#region Методы
		#region ExecWithoutParams
		public string ExecWithoutParams()
		{
	return this.Exec(new object[0]);
		}
		#endregion
		#region code
		public string code(string userName)
		{
         return Dersa.Common.DersaUtil.GetAttributeValue(userName, this.Id, "Code", 5);
		}
		#endregion
		#region Execute
		public System.Object Execute(ICompiledEntity owner, object[] args, string userName)
		{
if ((this.Code == null)||(this.Code.Length == 0)) return null;
			
			if (this.CompileScript())
			{
			        string[] usingAssemblies = this.Using.Split(',');
			        string[] Using = new string[usingAssemblies.Length];
			        for(int i = 0; i < usingAssemblies.Length; i++)
			        {
			             Using[i] = usingAssemblies[i].Contains("bin\\")? AppDomain.CurrentDomain.BaseDirectory + usingAssemblies[i] : usingAssemblies[i];
			        }
			        object[] argsWithUserName = new object[args.Length+1];
			        argsWithUserName[0] = userName;
			        for(int a = 0; a < args.Length; a++)
			             argsWithUserName[a+1] = args[a];
			        string paramsWithUserName = "System.String userName"; 
			        if(!string.IsNullOrEmpty(this.Parameters))
			               paramsWithUserName += ",";                
			        paramsWithUserName += this.Parameters.Replace("B64\"", "\"");
				return Static.CompileAndExecuteAditionalMethod(owner, this.ReturnType, this.Name, this.code(userName), paramsWithUserName, argsWithUserName, Using);
			}
			else
			{
				return this.Code;
			}
			return null;
		}
		#endregion
		#region Exec
[MethodPermissionsAttribute("admin")]
		public string Exec(object[] callParams)
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			if(this.Source == "SQL")
			{
			
			    if(callParams.Length == 1)//минимальная длина, передано только имя пользователя
			    {
			
			        sb.Append("var xhr = new XMLHttpRequest();");
			        sb.Append("args = 'id=" + this.Id.ToString() + "&method_name=ExecWithoutParams';");
			        sb.Append("xhr.open('GET', 'node/ExecMethodForm?' + args, false);");
			        sb.Append("xhr.send();");
			
			        sb.Append("var attrs = JSON.parse(xhr.responseText);");
			        sb.Append("var form = new mxForm('');");
			        sb.Append("var Props = CreateProperties(form, attrs, 'query/ExecSql', 'alert');");
			        sb.Append("var win_scrolled_value = parseInt($(window).scrollTop());");
			        sb.Append("var wnd = new mxWindow('exec SQL', Props, 100, win_scrolled_value + 100, 400, 600, false, true);");
			        sb.Append("form.window = wnd;");
			        sb.Append("wnd.setVisible(true);");
			        return sb.ToString();
			
			
			//        sb.Append("xhr=new XMLHttpRequest();");
			//        sb.Append("xhr.open('GET', 'Query/GetAction?MethodName=Exec&id="); 
			//        sb.Append(this.Id.ToString());
			//        sb.Append("&paramString=[0]'");
			//        sb.Append(", false);");
			//        sb.Append("xhr.send();");
			//        sb.Append("eval(xhr.responseText);");
			//        return sb.ToString();
			    }
			    else
			    {
				return this.Code;
			//        sb.Append("var attrs = [{Name:\"SQL\",Value:\"");
			//	sb.Append(this.Code); 
			//	sb.Append("\",ControlType:\"textarea\"");
			//	sb.Append(",Height:200");
			//	sb.Append(",Width:300");
			//	sb.Append(",WriteUnchanged:true}");
			//      sb.Append(",{Name:\"entity_id\",Value:\"");
			//	sb.Append(this.Id.ToString()); 
			//	sb.Append("\",ControlType:\"hidden\"");
			//	sb.Append(",WriteUnchanged:true}");
			//	sb.Append("];");
			//        sb.Append("var form = new mxForm('');");
			//        sb.Append("var Props = CreateProperties(form, attrs, 'query/ExecSql', 'alert');");
			//        sb.Append("var win_scrolled_value = parseInt($(window).scrollTop());");
			//        sb.Append("var wnd = new mxWindow('exec SQL', Props, 100, win_scrolled_value + 100, 400, 600, false, true);");
			//        sb.Append("form.window = wnd;");
			//        sb.Append("wnd.setVisible(true);");
			
			//        return sb.ToString();
			    }
			}
			else if(this.Source == "JScript")
			{
			     sb.Append("var paramsArray = new Array();");
			     for(int i=1; i < callParams.Length; i++)
			     {
			          //sb.Append("var Param" + i.ToString() + "=");
			          //sb.Append(callParams[i].ToString());
			          //sb.Append(";");
			
			          sb.Append("paramsArray[" + (i-1).ToString() + "] = ");
			          sb.Append(callParams[i].ToString());
			          sb.Append(";");
			     }
			     sb.Append(this.Code);
			     return sb.ToString();
			}
			else if(this.Source == "CSharp")
			{
			    bool hasParams = !string.IsNullOrEmpty(this.Parameters);
			    bool execResultScript = this.ReturnType == "Exec";
			    if(execResultScript)
			        this.ReturnType = "System.Object";
			    if(callParams.Length == 1)//минимальная длина, передано только имя пользователя
			    {
			        if(hasParams)
			        {
			            sb.Append("par = prompt('Введите значение параметров', '" + this.Parameters + "');");
			            sb.Append("par = par.replace(new RegExp('\"', 'g'),'\\\\\"');");
			            sb.Append("par = encodeURIComponent(par);");
			        }
			//var body = 'user_name=' + encodeURIComponent(username.value);
			//console.log(body);
			//xhr.open('POST',  "/account/register/", false);
			//xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
			//xhr.send(body);
			        sb.Append("xhr=new XMLHttpRequest();");
			        sb.Append("xhr.open('GET', 'Query/GetAction?MethodName=Exec&id="); 
			        sb.Append(this.Id.ToString());
			        if(hasParams)
			            sb.Append("&paramString=[\"'+par+'\"]'");
			        else
			            sb.Append("&paramString=[0]'");
			        sb.Append(", false);");
			        sb.Append("xhr.send();");
			        sb.Append("eval(xhr.responseText);");
			        return sb.ToString();
			    }
			    JSForm F = new JSForm();
			    object res = null;
			    try
			    {
			        object[] methodCallParams = new object[0];
			        if(hasParams)
			        {
			//в этом варианте все параметры передаются в одной строке как перечисление через запятую конструкций вида "System.Int32 x=1"
			
			      try
			      {
			             Hashtable HT = Static.GetParamValues(callParams[1].ToString());
			             string[] paramsDescr = this.Parameters.Split(',');
			             methodCallParams = new object[paramsDescr.Length];
			             for(int i = 0; i < paramsDescr.Length; i++)
			             {
			                  string paramWithTypeAndVal = paramsDescr[i].Trim();
			                  string paramName = paramWithTypeAndVal
			                     .Split('=')[0]
			                     .Split(' ')[1];
			                  string paramTypeName = paramWithTypeAndVal
			                     .Split('=')[0]
			                     .Split(' ')[0];
			                  System.Type T = System.Type.GetType(paramTypeName);
			                  object descrVal = HT[paramName];
			                  methodCallParams[i] = Dersa.Common.DersaUtil.Convert(descrVal, T);
			             }
			      }
			      catch
			      {
			            string paramsString = callParams[1].ToString();
			            string[] paramsVals = paramsString.Split(',');
			            methodCallParams = new object[paramsVals.Length];
			            for(int i=0; i < paramsVals.Length; i++)
			            {
			                methodCallParams[i] = Static.GetParamValue(paramsVals[i]);
			            }
			      }
			/*
			            methodCallParams = new object[callParams.Length-1];//в этом варианте параметры передаются как json массив
			//перечисление в [ ]
			            for(int i=1; i < callParams.Length; i++)
			                methodCallParams[i-1] = callParams[i];
			*/
			        }
			        res = Execute(this, methodCallParams, callParams[0].ToString());
			        if(execResultScript && res != null)
				{
					if(res is string)
				            return res.ToString();
					else
					    return Newtonsoft.Json.JsonConvert.SerializeObject(res);
				}
			    }
			    catch(System.Exception exc)
			    {
			        F.innerHTML = exc.Message;
			        return F.Show(callParams);
			    }
			    if(res == null)
			        F.innerHTML = "null";
			    else
			        F.innerHTML = res.ToString();
			    return F.Show(new object[]{callParams[0]});
			}
			else
			    return null;
		}
		#endregion
		#region CompileScript
		public System.Boolean CompileScript()
		{
        return this.Source == "CSharp";
		}
		#endregion
		#region Execute
		public System.Object Execute(ICompiledEntity owner, object[] args)
		{
return this.Execute(owner, args, "");
		}
		#endregion
		#region AllowExecuteMethod
		public override bool AllowExecuteMethod(string userName, string methodName)
		{
        this.Reinitialize();
			        var inst = Dersa.Common.DersaUtil.CreateInstance(this.Object, new Dersa.Common.DersaAnonimousSqlManager());
			        int userPermissions = Dersa.Common.DersaUtil.GetUserPermissions(userName);
				return (userPermissions & 4) > 0 || (inst is Script && ((Script)inst).Source == "JScript");
			
		}
		#endregion
		#endregion
	}
}
