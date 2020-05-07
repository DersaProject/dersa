using System;
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

        private string _tempDir;
        public string TempDir
        {
            get
            {
                return Properties.Settings.Default.TempDir;
            }
            set
            {
                needSave = true;
                this._tempDir = value;
            }
        }

        private string _wordDir;
        public string WordDir
        {
            get
            {
                return Properties.Settings.Default.WordDir;
            }
            set
            {
                needSave = true;
                this._wordDir = value;
            }
        }

        private string _editTextCommand;
        public string EditTextCommand
        {
            get
            {
                return Properties.Settings.Default.EditTextCommand;
            }
            set
            {
                needSave = true;
                this._editTextCommand = value;
            }
        }

        private string _afterSaveCommand;
        public string AfterSaveCommand
        {
            get
            {
                return Properties.Settings.Default.AfterSaveCommand;
            }
            set
            {
                needSave = true;
                this._afterSaveCommand = value;
            }
        }

        private string _compareProgramPath;
        public string CompareProgramPath
        {
            get
            {
                return Properties.Settings.Default.CompareProgramPath;
            }
            set
            {
                needSave = true;
                this._compareProgramPath = value;
            }
        }

        private bool _useUniqueFileNames = false;
        public bool UseUniqueFileNames
        {
            get
            {
                return _useUniqueFileNames;
            }
            set
            {
                needSave = true;
                this._useUniqueFileNames = value;
            }
        }

        private bool _deleteFileAfterSaveOnServer = false;
        public bool DeleteFileAfterSaveOnServer
        {
            get
            {
                return _deleteFileAfterSaveOnServer;
            }
            set
            {
                needSave = true;
                this._deleteFileAfterSaveOnServer = value;
            }
        }

        public void Save()
        {
            if (needSave)
            {
                Properties.Settings.Default.DeleteFileAfterSaveOnServer = this._deleteFileAfterSaveOnServer;
                Properties.Settings.Default.UseUniqueFileNames = this._useUniqueFileNames;
                Properties.Settings.Default.CompareProgramPath = this._compareProgramPath;
                Properties.Settings.Default.AfterSaveCommand = this._afterSaveCommand;
                Properties.Settings.Default.EditTextCommand = this._editTextCommand;
                Properties.Settings.Default.WordDir = this._wordDir;
                Properties.Settings.Default.TempDir = this._tempDir;
                Properties.Settings.Default.Save();
                needSave = false;
            }
        }

        public LocalClientSettings(): this(false) { }
        public LocalClientSettings(bool loadSettings)
        {
            if(loadSettings)
            {
                this._deleteFileAfterSaveOnServer = Properties.Settings.Default.DeleteFileAfterSaveOnServer;
                this._useUniqueFileNames = Properties.Settings.Default.UseUniqueFileNames;
                this._compareProgramPath = Properties.Settings.Default.CompareProgramPath;
                this._afterSaveCommand = Properties.Settings.Default.AfterSaveCommand;
                this._editTextCommand = Properties.Settings.Default.EditTextCommand ;
                this._wordDir = Properties.Settings.Default.WordDir;
                this._tempDir = Properties.Settings.Default.TempDir;
                Properties.Settings.Default.Save();

            }
        }
    }
    public class LocalClientModule: DersaHttpModule
    {
        public LocalClientModule()
        {
            Post("/Edit/{textId}", p => EditText(p.textId));
            Post("/Compare/{attrName}/{itemArr}", p => CompareItems(p.attrName, p.itemArr));

            Get("/LocalClient/Settings", p => View["Settings", new LocalClientSettings(true)]);
            Post("/LocalClient/Settings", _ => SaveModel());
        }

        private object SaveModel()
        {
            LocalClientSettings LCS = this.Bind<LocalClientSettings>();
            //if (this.Request.Form["DeleteFileAfterSaveOnServer"] == null)
            //    LCS.DeleteFileAfterSaveOnServer = false;
            //if (this.Request.Form["UseUniqueFileNames"] == null)
            //    LCS.UseUniqueFileNames = false;
            LCS.Save();
            return View["Close", null];
            //return View["Settings", LCS];
        }
        public string CompareItems(string attr_name, string itemArrJson)
        {
            string[] itemArr = JsonConvert.DeserializeObject<string[]>(itemArrJson);
            string item1 = itemArr[0];
            string item2 = itemArr[1];
            string attr1 = DersaUtil.GetAttributeValue("localuser", int.Parse(item1), attr_name, 2); //sClient.GetAttrValue(attr_name, item1, _userToken);
            string attr2 = DersaUtil.GetAttributeValue("localuser", int.Parse(item2), attr_name, 2); //sClient.GetAttrValue(attr_name, item2, _userToken);
            string TempDirPath = Properties.Settings.Default.TempDir;
            string fileName1 = TempDirPath + item1 + "_" + attr_name + ".txt";
            string fileName2 = TempDirPath + item2 + "_" + attr_name + ".txt";
            File.WriteAllText(fileName1, attr1);
            File.WriteAllText(fileName2, attr2);
            Process proc = new Process();
            proc.StartInfo.FileName = Properties.Settings.Default.CompareProgramPath;
            proc.StartInfo.Arguments = fileName1 + " " + fileName2;
            proc.Start();
            return "";
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
