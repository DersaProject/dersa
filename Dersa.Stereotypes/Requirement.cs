using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using Newtonsoft.Json;
using System.Linq;

namespace DersaStereotypes
{
	[Serializable()]
	public class Requirement: StereotypeBaseE, ICompiledEntity
	{
		public Requirement(){}

		public Requirement(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string JIRA_code = "";
		public string Description = "";

		#region Методы
		#region OpenJIRA
		public string OpenJIRA(object[] Params)
		{
	string key = this.JIRA_code;
				if(!string.IsNullOrEmpty(key))
				        return "window.open('http://jira-app-pc:8080/browse/" + key + "');";
				else
					return "alert('Заявка JIRA еще не создана');";
		}
		#endregion
		#region GetChildren
		private object GetChildren()
		{
System.Collections.ArrayList children = this.Children as System.Collections.ArrayList;
			children.Sort(new Dersa.Common.NameComparer());
			
			var query = from ICompiled obj in children where obj is Requirement
				select new
				{
					header = (obj as Requirement).Name,
					main_text = (obj as Requirement).Description,
					child_nodes = (obj as Requirement).GetChildren()
				};
			
			return query;
			//return JsonConvert.SerializeObject(query);
			
		}
		#endregion
		#region GetJson
		public string GetJson()
		{
var query = new
				{
					header = this.Name,
					main_text = this.Description,
					child_nodes = this.GetChildren()
				};
			
			return JsonConvert.SerializeObject(query);
		}
		#endregion
		#region CreateJIRA_issue
		public string CreateJIRA_issue(object[] Params)
		{
string userName = Params[0].ToString();
			string token = Dersa.Common.Util.GetUserSetting(userName, "JIRA token");
			string destUrl = "http://localhost:13663/Report/CreateJiraIssue";
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(!string.IsNullOrEmpty(this.JIRA_code.Trim()))
			{
				sb.Append("var err_message = 'по данному требованию уже создана заявка JIRA " + this.JIRA_code + "';");
				sb.Append("alert(err_message);");
				return sb.ToString();
			}
			sb.Append("var token = '" + token + "';");
			sb.Append("var summary = '" + this.Name + "';");
			sb.Append("var description = '" + Newtonsoft.Json.JsonConvert.SerializeObject(this.Description) + "';");
			sb.Append("var body = new Object();");
			sb.Append("body.token = token;");
			sb.Append("var sbody = {fields:{project:{id:10505},summary:summary,description:description,issuetype:{id:10200}}};");
			sb.Append("body.sbody = JSON.stringify(sbody);");
			sb.Append("submit(body);");
			
			sb.Append("async function submit(body) {");
			sb.Append("let response = await fetch('" + destUrl + "',{");
			sb.Append("  method: 'POST',");
			sb.Append("  headers: {");
			sb.Append("    'Content-Type': 'application/json'");
			sb.Append("  },");
			sb.Append("  body: JSON.stringify(body)");
			sb.Append("});");
			sb.Append("if (response.ok) { ");
			sb.Append("let json = await response.json();");
			sb.Append("xhr=new XMLHttpRequest();");
			sb.Append("xhr.open('POST', '/Node/SetProperties', true);");
			sb.Append("var body = new Object();");
			sb.Append("body.json_params = JSON.stringify([{Name:'entity',Value:" + this.Id + "},{Name:'JIRA_code',Value:json.key}]);");
			sb.Append("xhr.setRequestHeader('Content-Type', 'application/json');");
			sb.Append("xhr.send(JSON.stringify(body));");
			sb.Append("alert('Создана заявка ' + json.key);");
			sb.Append("}");
			sb.Append("else {alert('Ошибка HTTP: ' + response.status);}}");
			
			return sb.ToString();
		}
		#endregion
		#region GenerateHtml
		public string GenerateHtml(object[] callParams)
		{
if(callParams != null && callParams.Length > 1)
			    return GetHtmlText();
			
			JSForm F = new JSForm();
			F.Name = this.Name;
			F.WithPrintButton = true;
			F.innerHTML = GetHtmlText();
			return F.Show(callParams);
			
			/*очень сложный вариант
			object[] formCallParams = new object[4];
			formCallParams[0] = callParams[0];
			formCallParams[1] = "GenerateHtml";
			formCallParams[2] = this.Id;
			formCallParams[3] = "[1]";
			return F.Show(formCallParams);
			*/
			/*
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("mywindow = open('','newokno','width=700,height=700,status=1,menubar=1');");
			sb.Append("mywindow.document.open();");
			sb.Append("mywindow.document.write('<html><head><title>");
			sb.Append(this.Name);
			sb.Append("');");
			sb.Append("mywindow.document.write('</title></head><body>");
			sb.Append("<script>function Print(){B=document.getElementById(\"bnPrint\");B.style.display=\"none\";window.print();B.style.display=\"\"};</script>");
			sb.Append("<button id=\"bnPrint\" onclick=\"Print()\">Печать</button><br>");
			sb.Append(GetHtmlText());
			sb.Append("');");
			sb.Append("mywindow.document.write('</body></html>');");
			sb.Append("mywindow.document.close();");
			return sb.ToString();
			*/
		}
		#endregion
		#region GetChildrenText
		private string GetChildrenText()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<ul>");
			System.Collections.ArrayList children = this.Children as System.Collections.ArrayList;
			children.Sort(new Dersa.Common.NameComparer());
			foreach(ICompiled obj in children)
			{
			    if(obj is Requirement)
			    {
			        Requirement req = obj as Requirement;
			        sb.Append("<br><li><b>");
			        sb.Append(req.Name);
			        sb.Append("</b><br>");
			        if(string.IsNullOrEmpty(req.Description) && req.Children.Count < 1)
			            continue;
			        if(!string.IsNullOrEmpty(req.Description))
			            sb.Append(req.Description.Replace("\r\n","\n").Replace("\n","<br>"));
			        if(req.Children.Count > 0)
			            sb.Append(req.GetChildrenText());
			        sb.Append("</li>");
			    }
			}
			sb.Append("</ul>");
			return sb.ToString();
		}
		#endregion
		#region GetHtmlText
		public string GetHtmlText()
		{
System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(this.Description.Replace("\r\n","\n").Replace("\n","<br>"));
			sb.Append(GetChildrenText());
			return sb.ToString();
		}
		#endregion
		#endregion
	}
}
