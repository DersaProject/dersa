﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dersa.Common.WordGeneratorService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WordGeneratorService.IObjectWcfService")]
    public interface IObjectWcfService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/KeyName", ReplyAction="http://tempuri.org/IObjectWcfService/KeyNameResponse")]
        string KeyName(string class_name, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/KeyName", ReplyAction="http://tempuri.org/IObjectWcfService/KeyNameResponse")]
        System.Threading.Tasks.Task<string> KeyNameAsync(string class_name, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/ExecMethod", ReplyAction="http://tempuri.org/IObjectWcfService/ExecMethodResponse")]
        string ExecMethod(string class_name, string method_name, string id, string args, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/ExecMethod", ReplyAction="http://tempuri.org/IObjectWcfService/ExecMethodResponse")]
        System.Threading.Tasks.Task<string> ExecMethodAsync(string class_name, string method_name, string id, string args, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/GetUserToken", ReplyAction="http://tempuri.org/IObjectWcfService/GetUserTokenResponse")]
        string GetUserToken(string name, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/GetUserToken", ReplyAction="http://tempuri.org/IObjectWcfService/GetUserTokenResponse")]
        System.Threading.Tasks.Task<string> GetUserTokenAsync(string name, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/SetObjectView", ReplyAction="http://tempuri.org/IObjectWcfService/SetObjectViewResponse")]
        string SetObjectView(string class_name, string json_object, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/SetObjectView", ReplyAction="http://tempuri.org/IObjectWcfService/SetObjectViewResponse")]
        System.Threading.Tasks.Task<string> SetObjectViewAsync(string class_name, string json_object, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/GetObjectView", ReplyAction="http://tempuri.org/IObjectWcfService/GetObjectViewResponse")]
        string GetObjectView(string className, int id, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/GetObjectView", ReplyAction="http://tempuri.org/IObjectWcfService/GetObjectViewResponse")]
        System.Threading.Tasks.Task<string> GetObjectViewAsync(string className, int id, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/CreateObject", ReplyAction="http://tempuri.org/IObjectWcfService/CreateObjectResponse")]
        string CreateObject(string class_name, string json_object, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/CreateObject", ReplyAction="http://tempuri.org/IObjectWcfService/CreateObjectResponse")]
        System.Threading.Tasks.Task<string> CreateObjectAsync(string class_name, string json_object, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/DropObject", ReplyAction="http://tempuri.org/IObjectWcfService/DropObjectResponse")]
        string DropObject(string class_name, int id, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/DropObject", ReplyAction="http://tempuri.org/IObjectWcfService/DropObjectResponse")]
        System.Threading.Tasks.Task<string> DropObjectAsync(string class_name, int id, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/List", ReplyAction="http://tempuri.org/IObjectWcfService/ListResponse")]
        byte[] List(string className, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/List", ReplyAction="http://tempuri.org/IObjectWcfService/ListResponse")]
        System.Threading.Tasks.Task<byte[]> ListAsync(string className, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/ListWithParams", ReplyAction="http://tempuri.org/IObjectWcfService/ListWithParamsResponse")]
        byte[] ListWithParams(string className, string JsonParams, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/ListWithParams", ReplyAction="http://tempuri.org/IObjectWcfService/ListWithParamsResponse")]
        System.Threading.Tasks.Task<byte[]> ListWithParamsAsync(string className, string JsonParams, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/ListWithParamsPaged", ReplyAction="http://tempuri.org/IObjectWcfService/ListWithParamsPagedResponse")]
        byte[] ListWithParamsPaged(string className, string JsonParams, string token, int pageNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/ListWithParamsPaged", ReplyAction="http://tempuri.org/IObjectWcfService/ListWithParamsPagedResponse")]
        System.Threading.Tasks.Task<byte[]> ListWithParamsPagedAsync(string className, string JsonParams, string token, int pageNumber);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/EmptyList", ReplyAction="http://tempuri.org/IObjectWcfService/EmptyListResponse")]
        byte[] EmptyList(string className, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/EmptyList", ReplyAction="http://tempuri.org/IObjectWcfService/EmptyListResponse")]
        System.Threading.Tasks.Task<byte[]> EmptyListAsync(string className, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/PropertiesList", ReplyAction="http://tempuri.org/IObjectWcfService/PropertiesListResponse")]
        byte[] PropertiesList(string className, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/PropertiesList", ReplyAction="http://tempuri.org/IObjectWcfService/PropertiesListResponse")]
        System.Threading.Tasks.Task<byte[]> PropertiesListAsync(string className, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/SimpleList", ReplyAction="http://tempuri.org/IObjectWcfService/SimpleListResponse")]
        string SimpleList(string className, string Where, string Order, int limit, int offset, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/SimpleList", ReplyAction="http://tempuri.org/IObjectWcfService/SimpleListResponse")]
        System.Threading.Tasks.Task<string> SimpleListAsync(string className, string Where, string Order, int limit, int offset, string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/GenerateWordFile", ReplyAction="http://tempuri.org/IObjectWcfService/GenerateWordFileResponse")]
        byte[] GenerateWordFile(string json_args, string templateName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IObjectWcfService/GenerateWordFile", ReplyAction="http://tempuri.org/IObjectWcfService/GenerateWordFileResponse")]
        System.Threading.Tasks.Task<byte[]> GenerateWordFileAsync(string json_args, string templateName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IObjectWcfServiceChannel : Dersa.Common.WordGeneratorService.IObjectWcfService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ObjectWcfServiceClient : System.ServiceModel.ClientBase<Dersa.Common.WordGeneratorService.IObjectWcfService>, Dersa.Common.WordGeneratorService.IObjectWcfService {
        
        public ObjectWcfServiceClient() {
        }
        
        public ObjectWcfServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ObjectWcfServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ObjectWcfServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ObjectWcfServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string KeyName(string class_name, string token) {
            return base.Channel.KeyName(class_name, token);
        }
        
        public System.Threading.Tasks.Task<string> KeyNameAsync(string class_name, string token) {
            return base.Channel.KeyNameAsync(class_name, token);
        }
        
        public string ExecMethod(string class_name, string method_name, string id, string args, string token) {
            return base.Channel.ExecMethod(class_name, method_name, id, args, token);
        }
        
        public System.Threading.Tasks.Task<string> ExecMethodAsync(string class_name, string method_name, string id, string args, string token) {
            return base.Channel.ExecMethodAsync(class_name, method_name, id, args, token);
        }
        
        public string GetUserToken(string name, string password) {
            return base.Channel.GetUserToken(name, password);
        }
        
        public System.Threading.Tasks.Task<string> GetUserTokenAsync(string name, string password) {
            return base.Channel.GetUserTokenAsync(name, password);
        }
        
        public string SetObjectView(string class_name, string json_object, string token) {
            return base.Channel.SetObjectView(class_name, json_object, token);
        }
        
        public System.Threading.Tasks.Task<string> SetObjectViewAsync(string class_name, string json_object, string token) {
            return base.Channel.SetObjectViewAsync(class_name, json_object, token);
        }
        
        public string GetObjectView(string className, int id, string token) {
            return base.Channel.GetObjectView(className, id, token);
        }
        
        public System.Threading.Tasks.Task<string> GetObjectViewAsync(string className, int id, string token) {
            return base.Channel.GetObjectViewAsync(className, id, token);
        }
        
        public string CreateObject(string class_name, string json_object, string token) {
            return base.Channel.CreateObject(class_name, json_object, token);
        }
        
        public System.Threading.Tasks.Task<string> CreateObjectAsync(string class_name, string json_object, string token) {
            return base.Channel.CreateObjectAsync(class_name, json_object, token);
        }
        
        public string DropObject(string class_name, int id, string token) {
            return base.Channel.DropObject(class_name, id, token);
        }
        
        public System.Threading.Tasks.Task<string> DropObjectAsync(string class_name, int id, string token) {
            return base.Channel.DropObjectAsync(class_name, id, token);
        }
        
        public byte[] List(string className, string token) {
            return base.Channel.List(className, token);
        }
        
        public System.Threading.Tasks.Task<byte[]> ListAsync(string className, string token) {
            return base.Channel.ListAsync(className, token);
        }
        
        public byte[] ListWithParams(string className, string JsonParams, string token) {
            return base.Channel.ListWithParams(className, JsonParams, token);
        }
        
        public System.Threading.Tasks.Task<byte[]> ListWithParamsAsync(string className, string JsonParams, string token) {
            return base.Channel.ListWithParamsAsync(className, JsonParams, token);
        }
        
        public byte[] ListWithParamsPaged(string className, string JsonParams, string token, int pageNumber) {
            return base.Channel.ListWithParamsPaged(className, JsonParams, token, pageNumber);
        }
        
        public System.Threading.Tasks.Task<byte[]> ListWithParamsPagedAsync(string className, string JsonParams, string token, int pageNumber) {
            return base.Channel.ListWithParamsPagedAsync(className, JsonParams, token, pageNumber);
        }
        
        public byte[] EmptyList(string className, string token) {
            return base.Channel.EmptyList(className, token);
        }
        
        public System.Threading.Tasks.Task<byte[]> EmptyListAsync(string className, string token) {
            return base.Channel.EmptyListAsync(className, token);
        }
        
        public byte[] PropertiesList(string className, string token) {
            return base.Channel.PropertiesList(className, token);
        }
        
        public System.Threading.Tasks.Task<byte[]> PropertiesListAsync(string className, string token) {
            return base.Channel.PropertiesListAsync(className, token);
        }
        
        public string SimpleList(string className, string Where, string Order, int limit, int offset, string token) {
            return base.Channel.SimpleList(className, Where, Order, limit, offset, token);
        }
        
        public System.Threading.Tasks.Task<string> SimpleListAsync(string className, string Where, string Order, int limit, int offset, string token) {
            return base.Channel.SimpleListAsync(className, Where, Order, limit, offset, token);
        }
        
        public byte[] GenerateWordFile(string json_args, string templateName) {
            return base.Channel.GenerateWordFile(json_args, templateName);
        }
        
        public System.Threading.Tasks.Task<byte[]> GenerateWordFileAsync(string json_args, string templateName) {
            return base.Channel.GenerateWordFileAsync(json_args, templateName);
        }
    }
}