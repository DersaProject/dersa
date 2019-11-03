using System;  
using System.Runtime.Serialization;
using Dersa.Interfaces;

namespace DersaStereotypes
{
[Serializable()]
    public class WebView : StereotypeE, ICompiledEntity
{
	public WebView()
	{
	}
	public WebView(IDersaEntity obj)
	{
		_object = obj;
		if (_object != null)
		{
			_name = _object.Name;
			_id = _object.Id;
		}
	}

	#region Атрибуты
	#region PhisicalName
	public System.String PhisicalName;
	#endregion
	#region FolderName
	public System.String FolderName;
	#endregion
	#region Title
	public System.String Title;
	#endregion
	#endregion

	#region Операции
	public System.String Generate()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		Entity pEntity = this.Parent as Entity;
		if(pEntity == null)
			return "no parent entity";
		System.Collections.IList attrs = pEntity.GetCSharpAttributes(false);
		
		sb.Append("\t@using System.Data\n");
		sb.Append("\t@model DataTable\n");
		
		sb.Append("\t@{\n");
		sb.Append("\t    ViewBag.Title = \"" + this.Title + "\";\n");
		sb.Append("\t}\n");
		
		sb.Append("\t<h2>" + this.Title + "</h2>\n");
		
		sb.Append("\t<table>\n");
		sb.Append("\t    <tr>\n");
		foreach (Attribute attr in attrs)
		{
			if(attr.UseInWebForm)
			{
				sb.Append("\t        <th>\n");
				sb.Append("\t            <strong>" + attr.Name + "</strong>\n");
				sb.Append("\t        </th>\n");
			}
		}
			
		sb.Append("\t        <th></th>\n");
		sb.Append("\t    </tr>\n");
		sb.Append("\t    @foreach (DataRow item in Model.Rows)\n");
		sb.Append("\t    {\n");
		sb.Append("\t        <tr>\n");
		foreach (Attribute attr in attrs)
		{
			if(attr.UseInWebForm)
			{
				sb.Append("\t            <td style=\"padding:2px 5px\">\n");
				sb.Append("\t                @Html.DisplayFor(modelItem => item[\"" + attr.GetSqlName() + "\"], \"string\")\n");
				sb.Append("\t            </td>\n");
			}
		}
		sb.Append("\t            <td style=\"padding:2px 5px\">\n");
		sb.Append("\t                @Html.ActionLink(\"Edit\", \"Edit\", new { id = -5 })\n");
		sb.Append("\t                | @Html.ActionLink(\"Delete\", \"Delete\", new { id = -5 })\n");
		sb.Append("\t            </td>\n");
		sb.Append("\t        </tr>\n");
		sb.Append("\t    }\n");
		
		sb.Append("\t</table>\n");
		sb.Append("\t@Html.ActionLink(\"Create\", \"Create\")\n");
		sb.Append("\t<br>\n");
		sb.Append("\t<br>\n");
		
		string result = sb.ToString();
		string fileName = "undefined";
		//Package package = this.Parent.Parent as Package;
		//if(package != null)
		//{
        string WebDirectoryName = AppDomain.CurrentDomain.BaseDirectory + this.FolderName;
			//fileName = package.GetDirectory() + this.FolderName + "\\" + this.Name + ".cshtml";
            fileName = WebDirectoryName + "\\" + this.Name + ".cshtml";
            Static.SaveToFile(fileName, result);
		//}
		Console.WriteLine("Generated to: " + fileName);
		Console.WriteLine(result);
		return result;
	}
	#endregion
}
}