using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.IO;
using Nancy.Security;

namespace Dersa_N
{
    public class DersaHttpModule : NancyModule
    {
        public DersaHttpModule()
        {
            this.RequiresAuthentication();
        }
        protected string GetRequestBodyAsString()
        {
            byte[] bts = new byte[this.Request.Body.Length];
            this.Request.Body.Read(bts, 0, bts.Length);
            return Encoding.UTF8.GetString(bts);
        }

        protected string GetRequestFileAsString()
        {
            byte[] bts = new byte[this.Request.Files.ElementAt<HttpFile>(0).Value.Length];
            this.Request.Files.ElementAt<HttpFile>(0).Value.Read(bts, 0, bts.Length);
            return Encoding.Default.GetString(bts);
        }

        protected object DownloadObject(object srcObject, string fileName)
        {
            byte[] bts = new byte[0];
            if (srcObject is string)
            {
                bts = System.Text.Encoding.UTF8.GetBytes((string)srcObject);
            }
            else if (srcObject is System.IO.FileInfo)
            {
                FileInfo fi = srcObject as System.IO.FileInfo;
                fileName = fi.Name;
                Stream S = fi.OpenRead();
                bts = new byte[S.Length];
                S.Read(bts, 0, bts.Length);
                S.Flush();
                S.Close();
                fi.Delete();
            }
            else if (srcObject is DynamicDictionaryValue)
            {
                DynamicDictionaryValue DV = srcObject as DynamicDictionaryValue;
                if (DV.HasValue)
                    bts = System.Text.Encoding.Default.GetBytes(DV.Value.ToString());
            }
            Stream outS = new MemoryStream(bts);
            //this.Response.Headers.Add("Content-Disposition", "attachment; filename=test.txt");
            //return this.Response.FromStream(outS, "application/force-download");
            var response = new Response();

            response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
            response.ContentType = "application/force-download";
            response.Contents = stream => { stream.Write(bts, 0, bts.Length); };

            return response;
        }

    }
}
