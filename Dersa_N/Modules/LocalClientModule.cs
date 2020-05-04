﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Dersa.Common;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using DIOS.Common.Interfaces;

namespace Dersa_N
{

    public class LocalClientSettings
    {
        public bool needSave = false;
        public string TempDir
        {
            get
            {
                return Properties.Settings.Default.TempDir;
            }
            set
            {
                needSave = true;
                Properties.Settings.Default.TempDir = value;
            }
        }
        public string WordDir
        {
            get
            {
                return Properties.Settings.Default.WordDir;
            }
            set
            {
                needSave = true;
                Properties.Settings.Default.WordDir = value;
            }
        }
        public string EditTextCommand
        {
            get
            {
                return Properties.Settings.Default.EditTextCommand;
            }
            set
            {
                needSave = true;
                Properties.Settings.Default.EditTextCommand = value;
            }
        }
        public string AfterSaveCommand
        {
            get
            {
                return Properties.Settings.Default.AfterSaveCommand;
            }
            set
            {
                needSave = true;
                Properties.Settings.Default.AfterSaveCommand = value;
            }
        }
        public bool UseUniqueFileNames
        {
            get
            {
                return Properties.Settings.Default.UseUniqueFileNames;
            }
            set
            {
                needSave = true;
                Properties.Settings.Default.UseUniqueFileNames = value;
            }
        }
        public bool DeleteFileAfterSaveOnServer
        {
            get
            {
                return Properties.Settings.Default.DeleteFileAfterSaveOnServer;
            }
            set
            {
                needSave = true;
                Properties.Settings.Default.DeleteFileAfterSaveOnServer = value;
            }
        }

        public void Save()
        {
            if (needSave)
            {
                Properties.Settings.Default.Save();
                needSave = false;
            }
        }
    }
    public class LocalClientModule: NancyModule
    {
        public LocalClientModule()
        {
            Post("/Edit/{textId}", p => EditText(p.textId));

            Get("/LocalClient/Settings", p => View["Settings", new LocalClientSettings()]);
            Post("/LocalClient/Settings", _ => SaveModel());
        }

        private object SaveModel()
        {
            LocalClientSettings LCS = this.Bind<LocalClientSettings>();
            if (this.Request.Form["DeleteFileAfterSaveOnServer"] == null)
                LCS.DeleteFileAfterSaveOnServer = false;
            if (this.Request.Form["UseUniqueFileNames"] == null)
                LCS.UseUniqueFileNames = false;
            LCS.Save();
            return View["Close", null];
            //return View["Settings", LCS];
        }
        private static string EditText(string textId)
        {
            string TempDirPath = Properties.Settings.Default.TempDir; //"c:\\Temp\\";
            //QueryExecuteService.QueryExecuteServiceClient sClient = new QueryExecuteService.QueryExecuteServiceClient();
            //sClient.Endpoint.Address = new EndpointAddress(ServerURL);
            //if (_userToken == null)
            //{
            //    _userToken = sClient.GetUserToken(_userName, "*************");
            //}
            string textToEditJSON = QueryControllerAdapter.GetString(textId, false); //sClient.GetText(textId, _userToken);
            dynamic textToEditObject = JsonConvert.DeserializeObject(textToEditJSON);
            string textToEdit = textToEditObject.attrValue;
            textToEdit = textToEdit
                .Replace("\r", "")
                .Replace("\n", "\r\n");
            string attrName = textToEditObject.attrName;
            string entityId = "-";
            try
            {
                entityId = textToEditObject.entityId.ToString();
            }
            catch { }
            bool uniqueName = Properties.Settings.Default.UseUniqueFileNames;
            if (attrName == "WordText")
            {
                TempDirPath = Properties.Settings.Default.WordDir;
                uniqueName = true;
            }
            string fileExtension = textToEditObject.fileExtension; //".sql";
            //if(attrName == "Code" || attrName == "Text")
            //    fileExtension = ".cs";
            //else if (attrName == "WordText")
            //    fileExtension = ".html";
            string fileName = Path.Combine(TempDirPath, (uniqueName ? Guid.NewGuid().ToString() : "entity." + entityId + "." + attrName) + fileExtension);
            File.WriteAllText(fileName, textToEdit, System.Text.Encoding.UTF8);
            //if (attrName == "WordText")
            //{
            //    DIOS.WordAdapter.WordDocument doc = new DIOS.WordAdapter.WordDocument();
            //    doc.NewDocument(fileName);
            //    doc.Preview();
            //    return "OK";
            //}
            Process proc = new Process();
            string complexCommand = Properties.Settings.Default.EditTextCommand;
            string editTextCommand = complexCommand;
            string commandArgs = fileName;
            if (complexCommand.Contains(" "))
            {
                string[] complexCommandParts = complexCommand.Split(' ');
                editTextCommand = complexCommandParts[0];
                for (int i = complexCommandParts.Length - 1; i > 0; i--)
                {
                    commandArgs = complexCommandParts[i] + " " + commandArgs;
                }
            }
            proc.StartInfo.FileName = editTextCommand;
            proc.StartInfo.Arguments = commandArgs;
            proc.Start();
            proc.WaitForExit();
            string result = File.ReadAllText(fileName);
            bool needDelFile = Properties.Settings.Default.DeleteFileAfterSaveOnServer;
            if (result != textToEdit)
            {
                needDelFile = false;
                try
                {
                    try
                    {
                        NodeControllerAdapter.SetAttribute(null, AttributeOwnerType.Entity, entityId, attrName, result, 2); //"OK";// sClient.SetAttrValue(attrName, entityId, result, _userToken);
                        result = "";
                    }
                    catch(Exception exc)
                    {
                        result = exc.Message;
                    }
                    needDelFile = Properties.Settings.Default.DeleteFileAfterSaveOnServer; //false;// result == "";
                    if (result == "")
                        result = "information saved successfully";
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.AfterSaveCommand))
                    {
                        proc = new Process();
                        proc.StartInfo.FileName = Properties.Settings.Default.AfterSaveCommand;
                        proc.Start();
                    }

                }
                catch (Exception exc)
                {
                    result = exc.Message;
                }
            }
            else
                result = "information was not changed";

            if (needDelFile)
                File.Delete(fileName);
            return result;
        }

    }
}