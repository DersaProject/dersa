using System;
using System.Runtime.Serialization;
using Dersa.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using DIOS.Common;
using DIOS.Common.Interfaces;
namespace DersaStereotypes
{
	[Serializable()]
	public class Report: StereotypeBaseE, ICompiledEntity
	{
		public Report(){}

		public Report(IDersaEntity obj)
		{
			_object = obj;
			if (_object != null)
			{
				_name = _object.Name;
				_id = _object.Id;
			}
		}
		public static IParameterCollection LastParameters;
		public string SQLName = "";

		#region Методы
		#region GetReport
		public string GetReport(object[] callParams)
		{
            string procName = null;
            Script scr = null;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            FilterForm fForm = null;
            System.Collections.IList children = this.Children;
            for (int i = 0; i < children.Count; i++)
            {
                ICompiledEntity obj = (ICompiledEntity)children[i];
                if (obj is Procedure)
                {
                    procName = (obj as Procedure).GetSqlName(this);
                }
                if (obj is Parameter)
                {
                    parameters.Add(obj.Name, "");
                }
                if (obj is FilterForm)
                {
                    fForm = obj as FilterForm;
                }
                if (obj is Script)
                {
                    scr = obj as Script;
                }
            }
            if(procName == null && scr == null)
                   return "alert('Процедура или скрипт для отчета не определены')";

            if(scr != null)
            {
                    string[] scriptParams = new string[]{};
                    if(!string.IsNullOrEmpty(scr.Parameters))
                    {
                         scriptParams = scr.Parameters.Split(',');
                    }
                    object[] scriptCallParams = new object[scriptParams.Length];
                    for(int i = 0; i < scriptCallParams.Length; i++)
                    {
                         if(callParams.Length > 1 + i)
                             scriptCallParams[i] = callParams[1 + i];
                         else
                             scriptCallParams[i] = "";
                    }
                    object res = scr.Execute(this, scriptCallParams);
                    if(res == null)
                        return null;
                    return res.ToString();
            }

            if(fForm != null)
            {
                 string[] paramNames = new string[parameters.Count];
                 int p = 0;
                 foreach(string key in parameters.Keys)
                 {
                     paramNames[p++] = key;
                 }
                 return fForm.GenerateJScript (procName, paramNames);
            }
            else
            {
                 string paramsString = JsonConvert.SerializeObject(parameters); //"{\"entity\":}";
                 return "var val=prompt('Введите значение параметров', '" + paramsString + "');if(val){var obj=JSON.parse(val);var arr=new Array();var i=0;for(var key in obj){var x=new Object();x['Name']=key;x['Value']=obj[key];arr[i++] = x;}var p=new Object();p['Name']='proc_name';p['Value']='" + procName + "';arr[i]=p;var url='/Query/Report?parameters='+JSON.stringify(arr);window.open(url);}";
            }
            
          //return "var val = prompt('Введите значение параметра');var url = '/Query/Report?proc_name=ENTITY$GetInfo&parameters=[{Name:%22entity%22,Value:' + val + '}]';window.open(url);";
		}
		#endregion
		#region GetSqlName
		public string GetSqlName()
		{
         if(!string.IsNullOrEmpty(this.SQLName))
             return this.SQLName;
         return "REPORT";
		}
		#endregion
		#endregion
	}
}
