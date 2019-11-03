using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
	[Serializable()]
	public class BugFix: StereotypeBaseE, ICompiledEntity
	{
		public BugFix(){}

		public BugFix(IEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public string Description = "";
		public string JIRA_code = "";

		#region Методы
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
		#endregion
	}
}
