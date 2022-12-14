// Generated by ProtoGen, Version=2.4.1.473, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace bgs.protocol.game_utilities.v1 {
  
  
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public static partial class GameUtilitiesTypes {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    internal static pbd::MessageDescriptor internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::bgs.protocol.game_utilities.v1.PlayerVariables, global::bgs.protocol.game_utilities.v1.PlayerVariables.Builder> internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__FieldAccessorTable;
    internal static pbd::MessageDescriptor internal__static_bgs_protocol_game_utilities_v1_ClientInfo__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::bgs.protocol.game_utilities.v1.ClientInfo, global::bgs.protocol.game_utilities.v1.ClientInfo.Builder> internal__static_bgs_protocol_game_utilities_v1_ClientInfo__FieldAccessorTable;
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static GameUtilitiesTypes() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          "CixiZ3MvbG93L3BiL2NsaWVudC9nYW1lX3V0aWxpdGllc190eXBlcy5wcm90" + 
          "bxIeYmdzLnByb3RvY29sLmdhbWVfdXRpbGl0aWVzLnYxGidiZ3MvbG93L3Bi" + 
          "L2NsaWVudC9hdHRyaWJ1dGVfdHlwZXMucHJvdG8aJGJncy9sb3cvcGIvY2xp" + 
          "ZW50L2VudGl0eV90eXBlcy5wcm90byKUAQoPUGxheWVyVmFyaWFibGVzEjIK" + 
          "CGlkZW50aXR5GAEgAigLMhYuYmdzLnByb3RvY29sLklkZW50aXR5UghpZGVu" + 
          "dGl0eRIWCgZyYXRpbmcYAiABKAFSBnJhdGluZxI1CglhdHRyaWJ1dGUYAyAD" + 
          "KAsyFy5iZ3MucHJvdG9jb2wuQXR0cmlidXRlUglhdHRyaWJ1dGUiYgoKQ2xp" + 
          "ZW50SW5mbxIlCg5jbGllbnRfYWRkcmVzcxgBIAEoCVINY2xpZW50QWRkcmVz" + 
          "cxItChJwcml2aWxlZ2VkX25ldHdvcmsYAiABKAhSEXByaXZpbGVnZWROZXR3" + 
          "b3JrQjwKH2JuZXQucHJvdG9jb2wuZ2FtZV91dGlsaXRpZXMudjFCF0dhbWVV" + 
          "dGlsaXRpZXNUeXBlc1Byb3RvSAI=");
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__Descriptor = Descriptor.MessageTypes[0];
        internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::bgs.protocol.game_utilities.v1.PlayerVariables, global::bgs.protocol.game_utilities.v1.PlayerVariables.Builder>(internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__Descriptor,
                new string[] { "Identity", "Rating", "Attribute", });
        internal__static_bgs_protocol_game_utilities_v1_ClientInfo__Descriptor = Descriptor.MessageTypes[1];
        internal__static_bgs_protocol_game_utilities_v1_ClientInfo__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::bgs.protocol.game_utilities.v1.ClientInfo, global::bgs.protocol.game_utilities.v1.ClientInfo.Builder>(internal__static_bgs_protocol_game_utilities_v1_ClientInfo__Descriptor,
                new string[] { "ClientAddress", "PrivilegedNetwork", });
        pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();
        RegisterAllExtensions(registry);
        global::bgs.protocol.AttributeTypes.RegisterAllExtensions(registry);
        global::bgs.protocol.EntityTypes.RegisterAllExtensions(registry);
        return registry;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          global::bgs.protocol.AttributeTypes.Descriptor, 
          global::bgs.protocol.EntityTypes.Descriptor, 
          }, assigner);
    }
    #endregion
    
  }
  #region Messages
  
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public sealed partial class PlayerVariables : pb::GeneratedMessage<PlayerVariables, PlayerVariables.Builder> {
    private PlayerVariables() { }
    private static readonly PlayerVariables defaultInstance = new PlayerVariables().MakeReadOnly();
    public static PlayerVariables DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override PlayerVariables DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override PlayerVariables ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::bgs.protocol.game_utilities.v1.GameUtilitiesTypes.internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<PlayerVariables, PlayerVariables.Builder> InternalFieldAccessors {
      get { return global::bgs.protocol.game_utilities.v1.GameUtilitiesTypes.internal__static_bgs_protocol_game_utilities_v1_PlayerVariables__FieldAccessorTable; }
    }
    
    public const int IdentityFieldNumber = 1;
    private bool hasIdentity;
    private global::bgs.protocol.Identity identity_;
    public bool HasIdentity {
      get { return hasIdentity; }
    }
    public global::bgs.protocol.Identity Identity {
      get { return identity_ ?? global::bgs.protocol.Identity.DefaultInstance; }
    }
    
    public const int RatingFieldNumber = 2;
    private bool hasRating;
    private double rating_;
    public bool HasRating {
      get { return hasRating; }
    }
    public double Rating {
      get { return rating_; }
    }
    
    public const int AttributeFieldNumber = 3;
    private pbc::PopsicleList<global::bgs.protocol.Attribute> attribute_ = new pbc::PopsicleList<global::bgs.protocol.Attribute>();
    public scg::IList<global::bgs.protocol.Attribute> AttributeList {
      get { return attribute_; }
    }
    public int AttributeCount {
      get { return attribute_.Count; }
    }
    public global::bgs.protocol.Attribute GetAttribute(int index) {
      return attribute_[index];
    }
    
    public static PlayerVariables ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PlayerVariables ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PlayerVariables ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static PlayerVariables ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static PlayerVariables ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PlayerVariables ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static PlayerVariables ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static PlayerVariables ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static PlayerVariables ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static PlayerVariables ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private PlayerVariables MakeReadOnly() {
      attribute_.MakeReadOnly();
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(PlayerVariables prototype) {
      return new Builder(prototype);
    }
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
    public sealed partial class Builder : pb::GeneratedBuilder<PlayerVariables, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(PlayerVariables cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private PlayerVariables result;
      
      private PlayerVariables PrepareBuilder() {
        if (resultIsReadOnly) {
          PlayerVariables original = result;
          result = new PlayerVariables();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override PlayerVariables MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::bgs.protocol.game_utilities.v1.PlayerVariables.Descriptor; }
      }
      
      public override PlayerVariables DefaultInstanceForType {
        get { return global::bgs.protocol.game_utilities.v1.PlayerVariables.DefaultInstance; }
      }
      
      public override PlayerVariables BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      
      public bool HasIdentity {
       get { return result.hasIdentity; }
      }
      public global::bgs.protocol.Identity Identity {
        get { return result.Identity; }
        set { SetIdentity(value); }
      }
      public Builder SetIdentity(global::bgs.protocol.Identity value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasIdentity = true;
        result.identity_ = value;
        return this;
      }
      public Builder SetIdentity(global::bgs.protocol.Identity.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.hasIdentity = true;
        result.identity_ = builderForValue.Build();
        return this;
      }
      public Builder MergeIdentity(global::bgs.protocol.Identity value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        if (result.hasIdentity &&
            result.identity_ != global::bgs.protocol.Identity.DefaultInstance) {
            result.identity_ = global::bgs.protocol.Identity.CreateBuilder(result.identity_).MergeFrom(value).BuildPartial();
        } else {
          result.identity_ = value;
        }
        result.hasIdentity = true;
        return this;
      }
      public Builder ClearIdentity() {
        PrepareBuilder();
        result.hasIdentity = false;
        result.identity_ = null;
        return this;
      }
      
      public bool HasRating {
        get { return result.hasRating; }
      }
      public double Rating {
        get { return result.Rating; }
        set { SetRating(value); }
      }
      public Builder SetRating(double value) {
        PrepareBuilder();
        result.hasRating = true;
        result.rating_ = value;
        return this;
      }
      public Builder ClearRating() {
        PrepareBuilder();
        result.hasRating = false;
        result.rating_ = 0D;
        return this;
      }
      
      public pbc::IPopsicleList<global::bgs.protocol.Attribute> AttributeList {
        get { return PrepareBuilder().attribute_; }
      }
      public int AttributeCount {
        get { return result.AttributeCount; }
      }
      public global::bgs.protocol.Attribute GetAttribute(int index) {
        return result.GetAttribute(index);
      }
      public Builder SetAttribute(int index, global::bgs.protocol.Attribute value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.attribute_[index] = value;
        return this;
      }
      public Builder SetAttribute(int index, global::bgs.protocol.Attribute.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.attribute_[index] = builderForValue.Build();
        return this;
      }
      public Builder AddAttribute(global::bgs.protocol.Attribute value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.attribute_.Add(value);
        return this;
      }
      public Builder AddAttribute(global::bgs.protocol.Attribute.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.attribute_.Add(builderForValue.Build());
        return this;
      }
      public Builder AddRangeAttribute(scg::IEnumerable<global::bgs.protocol.Attribute> values) {
        PrepareBuilder();
        result.attribute_.Add(values);
        return this;
      }
      public Builder ClearAttribute() {
        PrepareBuilder();
        result.attribute_.Clear();
        return this;
      }
    }
    static PlayerVariables() {
      object.ReferenceEquals(global::bgs.protocol.game_utilities.v1.GameUtilitiesTypes.Descriptor, null);
    }
  }
  
  
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public sealed partial class ClientInfo : pb::GeneratedMessage<ClientInfo, ClientInfo.Builder> {
    private ClientInfo() { }
    private static readonly ClientInfo defaultInstance = new ClientInfo().MakeReadOnly();
    public static ClientInfo DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override ClientInfo DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override ClientInfo ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::bgs.protocol.game_utilities.v1.GameUtilitiesTypes.internal__static_bgs_protocol_game_utilities_v1_ClientInfo__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<ClientInfo, ClientInfo.Builder> InternalFieldAccessors {
      get { return global::bgs.protocol.game_utilities.v1.GameUtilitiesTypes.internal__static_bgs_protocol_game_utilities_v1_ClientInfo__FieldAccessorTable; }
    }
    
    public const int ClientAddressFieldNumber = 1;
    private bool hasClientAddress;
    private string clientAddress_ = "";
    public bool HasClientAddress {
      get { return hasClientAddress; }
    }
    public string ClientAddress {
      get { return clientAddress_; }
    }
    
    public const int PrivilegedNetworkFieldNumber = 2;
    private bool hasPrivilegedNetwork;
    private bool privilegedNetwork_;
    public bool HasPrivilegedNetwork {
      get { return hasPrivilegedNetwork; }
    }
    public bool PrivilegedNetwork {
      get { return privilegedNetwork_; }
    }
    
    public static ClientInfo ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static ClientInfo ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static ClientInfo ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static ClientInfo ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static ClientInfo ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static ClientInfo ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static ClientInfo ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static ClientInfo ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static ClientInfo ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static ClientInfo ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private ClientInfo MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(ClientInfo prototype) {
      return new Builder(prototype);
    }
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
    public sealed partial class Builder : pb::GeneratedBuilder<ClientInfo, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(ClientInfo cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private ClientInfo result;
      
      private ClientInfo PrepareBuilder() {
        if (resultIsReadOnly) {
          ClientInfo original = result;
          result = new ClientInfo();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override ClientInfo MessageBeingBuilt {
        get { return PrepareBuilder(); }
      }
      
      public override Builder Clear() {
        result = DefaultInstance;
        resultIsReadOnly = true;
        return this;
      }
      
      public override Builder Clone() {
        if (resultIsReadOnly) {
          return new Builder(result);
        } else {
          return new Builder().MergeFrom(result);
        }
      }
      
      public override pbd::MessageDescriptor DescriptorForType {
        get { return global::bgs.protocol.game_utilities.v1.ClientInfo.Descriptor; }
      }
      
      public override ClientInfo DefaultInstanceForType {
        get { return global::bgs.protocol.game_utilities.v1.ClientInfo.DefaultInstance; }
      }
      
      public override ClientInfo BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      
      public bool HasClientAddress {
        get { return result.hasClientAddress; }
      }
      public string ClientAddress {
        get { return result.ClientAddress; }
        set { SetClientAddress(value); }
      }
      public Builder SetClientAddress(string value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasClientAddress = true;
        result.clientAddress_ = value;
        return this;
      }
      public Builder ClearClientAddress() {
        PrepareBuilder();
        result.hasClientAddress = false;
        result.clientAddress_ = "";
        return this;
      }
      
      public bool HasPrivilegedNetwork {
        get { return result.hasPrivilegedNetwork; }
      }
      public bool PrivilegedNetwork {
        get { return result.PrivilegedNetwork; }
        set { SetPrivilegedNetwork(value); }
      }
      public Builder SetPrivilegedNetwork(bool value) {
        PrepareBuilder();
        result.hasPrivilegedNetwork = true;
        result.privilegedNetwork_ = value;
        return this;
      }
      public Builder ClearPrivilegedNetwork() {
        PrepareBuilder();
        result.hasPrivilegedNetwork = false;
        result.privilegedNetwork_ = false;
        return this;
      }
    }
    static ClientInfo() {
      object.ReferenceEquals(global::bgs.protocol.game_utilities.v1.GameUtilitiesTypes.Descriptor, null);
    }
  }
  
  #endregion
  
}

#endregion Designer generated code
