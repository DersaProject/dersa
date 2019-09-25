using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Instrumentation;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace Dersa.Common
{
    public class MyTextWriter: TextWriter
    {
        private TextWriter _base;
        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }
        }
        public MyTextWriter(TextWriter TW)
        {
            _base = TW;
        }
        public override Encoding Encoding 
        { 
            get
            {
                return _base.Encoding;// Encoding.Default;
            }
        }
        public override void Write(string value)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////здесь можно перехватить!
            //_base.Write(value);
            _result = value;
        }
    }
    public class MyResponseBase: HttpResponseBase
    {
        HttpResponseBase _base;
        TextWriter _output;
        public override TextWriter Output 
        { 
            get
            {
                //return _base.Output;
                return _output;
            }
            set { } 
        }
        public MyResponseBase(HttpResponseBase RB)
            : base()
        {
            _base = RB;
            _output = new MyTextWriter(RB.Output);
        }
        public override void Write(string s)
        {
            base.Write(s);
        }
        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
        }
    }
    public class CustomContextBase: HttpContextBase
    {
        HttpResponseBase _response;
        HttpContextBase _base;
        public CustomContextBase(HttpContextBase CB): base()
        {
            _base = CB;
            _response = new MyResponseBase(CB.Response);
        }
        public override HttpResponseBase Response 
        { 
            get
            {
                return _response;
            }
        }
        public override IDictionary Items 
        { 
            get
            {
                return _base.Items;
            }
        }
        public override HttpRequestBase Request 
        { 
            get
            {
                return _base.Request;
            }
        }
    }
    public class CustomContext: ControllerContext
    {
        public CustomContext(ControllerContext C)
        {
            HttpContext = new CustomContextBase(C.HttpContext);
        }
        public override HttpContextBase HttpContext { get; set; }
    }
    public class CustomActionResult : ActionResult
    {
        ActionResult _main;
        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }
        }

        public CustomActionResult(ActionResult main, ControllerContext context)
        {
            _main = main;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            //ActionResult emptyRes = new EmptyResult();
            //emptyRes.ExecuteResult(context);
            TextWriter baseOutput = context.HttpContext.Response.Output;
            MyTextWriter W = new MyTextWriter(baseOutput);
            context.HttpContext.Response.Output = W;
            _main.ExecuteResult(context);
            context.HttpContext.Response.Output = baseOutput;
            //CustomContext MyContext = new CustomContext(context);
            //_main.ExecuteResult(MyContext);

            //context.HttpContext.Response.Write("<div style='width:100%;text-align:center;'>" +
            //    "<br><h1>DIOS applications</h1><p>&copy; " + DateTime.Now.Year.ToString() + "</p>");

            _result = W.Result;
        }
    }
}