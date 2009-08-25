﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScanUploader.cmsws {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="cmsws.WebServiceSoap")]
    public interface WebServiceSoap {
        
        // CODEGEN: Generating message contract since message UploadImageRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UploadImage", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ScanUploader.cmsws.UploadImageResponse UploadImage(ScanUploader.cmsws.UploadImageRequest request);
        
        // CODEGEN: Generating message contract since message SearchPersonRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SearchPerson", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ScanUploader.cmsws.SearchPersonResponse SearchPerson(ScanUploader.cmsws.SearchPersonRequest request);
        
        // CODEGEN: Generating message contract since message RecentBundleListRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/RecentBundleList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ScanUploader.cmsws.RecentBundleListResponse RecentBundleList(ScanUploader.cmsws.RecentBundleListRequest request);
        
        // CODEGEN: Generating message contract since message BundleDetailsRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/BundleDetails", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ScanUploader.cmsws.BundleDetailsResponse BundleDetails(ScanUploader.cmsws.BundleDetailsRequest request);
        
        // CODEGEN: Generating message contract since message UploadBundleRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UploadBundle", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        ScanUploader.cmsws.UploadBundleResponse UploadBundle(ScanUploader.cmsws.UploadBundleRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceAuthHeader : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string usernameField;
        
        private string passwordField;
        
        private int useridField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Username {
            get {
                return this.usernameField;
            }
            set {
                this.usernameField = value;
                this.RaisePropertyChanged("Username");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("Password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public int userid {
            get {
                return this.useridField;
            }
            set {
                this.useridField = value;
                this.RaisePropertyChanged("userid");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
                this.RaisePropertyChanged("AnyAttr");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class BundleDetail : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int peopleIdField;
        
        private decimal amountField;
        
        private System.DateTime dateField;
        
        private int fundField;
        
        private bool pledgeField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public int PeopleId {
            get {
                return this.peopleIdField;
            }
            set {
                this.peopleIdField = value;
                this.RaisePropertyChanged("PeopleId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public decimal Amount {
            get {
                return this.amountField;
            }
            set {
                this.amountField = value;
                this.RaisePropertyChanged("Amount");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public System.DateTime Date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
                this.RaisePropertyChanged("Date");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public int Fund {
            get {
                return this.fundField;
            }
            set {
                this.fundField = value;
                this.RaisePropertyChanged("Fund");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public bool Pledge {
            get {
                return this.pledgeField;
            }
            set {
                this.pledgeField = value;
                this.RaisePropertyChanged("Pledge");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
                this.RaisePropertyChanged("Name");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class BundleResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int bundleIdField;
        
        private System.DateTime dateField;
        
        private System.Nullable<decimal> totalField;
        
        private System.Nullable<int> fundField;
        
        private int countField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public int BundleId {
            get {
                return this.bundleIdField;
            }
            set {
                this.bundleIdField = value;
                this.RaisePropertyChanged("BundleId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public System.DateTime Date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
                this.RaisePropertyChanged("Date");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=2)]
        public System.Nullable<decimal> Total {
            get {
                return this.totalField;
            }
            set {
                this.totalField = value;
                this.RaisePropertyChanged("Total");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true, Order=3)]
        public System.Nullable<int> Fund {
            get {
                return this.fundField;
            }
            set {
                this.fundField = value;
                this.RaisePropertyChanged("Fund");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public int Count {
            get {
                return this.countField;
            }
            set {
                this.countField = value;
                this.RaisePropertyChanged("Count");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4016")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class PersonResult : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string nameField;
        
        private int peopleIdField;
        
        private string addressField;
        
        private string cSZField;
        
        private string phoneField;
        
        private string birthdayField;
        
        private string ageField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
                this.RaisePropertyChanged("Name");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int PeopleId {
            get {
                return this.peopleIdField;
            }
            set {
                this.peopleIdField = value;
                this.RaisePropertyChanged("PeopleId");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Address {
            get {
                return this.addressField;
            }
            set {
                this.addressField = value;
                this.RaisePropertyChanged("Address");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string CSZ {
            get {
                return this.cSZField;
            }
            set {
                this.cSZField = value;
                this.RaisePropertyChanged("CSZ");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string Phone {
            get {
                return this.phoneField;
            }
            set {
                this.phoneField = value;
                this.RaisePropertyChanged("Phone");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Birthday {
            get {
                return this.birthdayField;
            }
            set {
                this.birthdayField = value;
                this.RaisePropertyChanged("Birthday");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string Age {
            get {
                return this.ageField;
            }
            set {
                this.ageField = value;
                this.RaisePropertyChanged("Age");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadImage", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class UploadImageRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> PeopleId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string UserInfo;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public int TypeId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=3)]
        public string mimetype;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=4)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] bits;
        
        public UploadImageRequest() {
        }
        
        public UploadImageRequest(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, System.Nullable<int> PeopleId, string UserInfo, int TypeId, string mimetype, byte[] bits) {
            this.ServiceAuthHeader = ServiceAuthHeader;
            this.PeopleId = PeopleId;
            this.UserInfo = UserInfo;
            this.TypeId = TypeId;
            this.mimetype = mimetype;
            this.bits = bits;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadImageResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class UploadImageResponse {
        
        public UploadImageResponse() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SearchPerson", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class SearchPersonRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string name;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string comm;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string addr;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=3)]
        public string birthday;
        
        public SearchPersonRequest() {
        }
        
        public SearchPersonRequest(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, string name, string comm, string addr, string birthday) {
            this.ServiceAuthHeader = ServiceAuthHeader;
            this.name = name;
            this.comm = comm;
            this.addr = addr;
            this.birthday = birthday;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SearchPersonResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class SearchPersonResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public PersonResult[] SearchPersonResult;
        
        public SearchPersonResponse() {
        }
        
        public SearchPersonResponse(PersonResult[] SearchPersonResult) {
            this.SearchPersonResult = SearchPersonResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RecentBundleList", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class RecentBundleListRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader;
        
        public RecentBundleListRequest() {
        }
        
        public RecentBundleListRequest(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader) {
            this.ServiceAuthHeader = ServiceAuthHeader;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RecentBundleListResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class RecentBundleListResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public BundleResult[] RecentBundleListResult;
        
        public RecentBundleListResponse() {
        }
        
        public RecentBundleListResponse(BundleResult[] RecentBundleListResult) {
            this.RecentBundleListResult = RecentBundleListResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="BundleDetails", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class BundleDetailsRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int BundleId;
        
        public BundleDetailsRequest() {
        }
        
        public BundleDetailsRequest(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, int BundleId) {
            this.ServiceAuthHeader = ServiceAuthHeader;
            this.BundleId = BundleId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="BundleDetailsResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class BundleDetailsResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public BundleDetail[] BundleDetailsResult;
        
        public BundleDetailsResponse() {
        }
        
        public BundleDetailsResponse(BundleDetail[] BundleDetailsResult) {
            this.BundleDetailsResult = BundleDetailsResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadBundle", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class UploadBundleRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public int BundleId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public BundleDetail[] a;
        
        public UploadBundleRequest() {
        }
        
        public UploadBundleRequest(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, int BundleId, BundleDetail[] a) {
            this.ServiceAuthHeader = ServiceAuthHeader;
            this.BundleId = BundleId;
            this.a = a;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadBundleResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class UploadBundleResponse {
        
        public UploadBundleResponse() {
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface WebServiceSoapChannel : ScanUploader.cmsws.WebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class WebServiceSoapClient : System.ServiceModel.ClientBase<ScanUploader.cmsws.WebServiceSoap>, ScanUploader.cmsws.WebServiceSoap {
        
        public WebServiceSoapClient() {
        }
        
        public WebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ScanUploader.cmsws.UploadImageResponse ScanUploader.cmsws.WebServiceSoap.UploadImage(ScanUploader.cmsws.UploadImageRequest request) {
            return base.Channel.UploadImage(request);
        }
        
        public void UploadImage(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, System.Nullable<int> PeopleId, string UserInfo, int TypeId, string mimetype, byte[] bits) {
            ScanUploader.cmsws.UploadImageRequest inValue = new ScanUploader.cmsws.UploadImageRequest();
            inValue.ServiceAuthHeader = ServiceAuthHeader;
            inValue.PeopleId = PeopleId;
            inValue.UserInfo = UserInfo;
            inValue.TypeId = TypeId;
            inValue.mimetype = mimetype;
            inValue.bits = bits;
            ScanUploader.cmsws.UploadImageResponse retVal = ((ScanUploader.cmsws.WebServiceSoap)(this)).UploadImage(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ScanUploader.cmsws.SearchPersonResponse ScanUploader.cmsws.WebServiceSoap.SearchPerson(ScanUploader.cmsws.SearchPersonRequest request) {
            return base.Channel.SearchPerson(request);
        }
        
        public PersonResult[] SearchPerson(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, string name, string comm, string addr, string birthday) {
            ScanUploader.cmsws.SearchPersonRequest inValue = new ScanUploader.cmsws.SearchPersonRequest();
            inValue.ServiceAuthHeader = ServiceAuthHeader;
            inValue.name = name;
            inValue.comm = comm;
            inValue.addr = addr;
            inValue.birthday = birthday;
            ScanUploader.cmsws.SearchPersonResponse retVal = ((ScanUploader.cmsws.WebServiceSoap)(this)).SearchPerson(inValue);
            return retVal.SearchPersonResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ScanUploader.cmsws.RecentBundleListResponse ScanUploader.cmsws.WebServiceSoap.RecentBundleList(ScanUploader.cmsws.RecentBundleListRequest request) {
            return base.Channel.RecentBundleList(request);
        }
        
        public BundleResult[] RecentBundleList(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader) {
            ScanUploader.cmsws.RecentBundleListRequest inValue = new ScanUploader.cmsws.RecentBundleListRequest();
            inValue.ServiceAuthHeader = ServiceAuthHeader;
            ScanUploader.cmsws.RecentBundleListResponse retVal = ((ScanUploader.cmsws.WebServiceSoap)(this)).RecentBundleList(inValue);
            return retVal.RecentBundleListResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ScanUploader.cmsws.BundleDetailsResponse ScanUploader.cmsws.WebServiceSoap.BundleDetails(ScanUploader.cmsws.BundleDetailsRequest request) {
            return base.Channel.BundleDetails(request);
        }
        
        public BundleDetail[] BundleDetails(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, int BundleId) {
            ScanUploader.cmsws.BundleDetailsRequest inValue = new ScanUploader.cmsws.BundleDetailsRequest();
            inValue.ServiceAuthHeader = ServiceAuthHeader;
            inValue.BundleId = BundleId;
            ScanUploader.cmsws.BundleDetailsResponse retVal = ((ScanUploader.cmsws.WebServiceSoap)(this)).BundleDetails(inValue);
            return retVal.BundleDetailsResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ScanUploader.cmsws.UploadBundleResponse ScanUploader.cmsws.WebServiceSoap.UploadBundle(ScanUploader.cmsws.UploadBundleRequest request) {
            return base.Channel.UploadBundle(request);
        }
        
        public void UploadBundle(ScanUploader.cmsws.ServiceAuthHeader ServiceAuthHeader, int BundleId, BundleDetail[] a) {
            ScanUploader.cmsws.UploadBundleRequest inValue = new ScanUploader.cmsws.UploadBundleRequest();
            inValue.ServiceAuthHeader = ServiceAuthHeader;
            inValue.BundleId = BundleId;
            inValue.a = a;
            ScanUploader.cmsws.UploadBundleResponse retVal = ((ScanUploader.cmsws.WebServiceSoap)(this)).UploadBundle(inValue);
        }
    }
}