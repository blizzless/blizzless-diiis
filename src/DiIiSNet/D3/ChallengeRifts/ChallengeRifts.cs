// Generated by ProtoGen, Version=2.4.1.473, Culture=neutral, PublicKeyToken=55f7125234beb589.  DO NOT EDIT!
#pragma warning disable 1591, 0612
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace D3.ChallengeRifts {
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public static partial class ChallengeRifts {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    internal static pbd::MessageDescriptor internal__static_D3_ChallengeRifts_ChallengeData__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::D3.ChallengeRifts.ChallengeData, global::D3.ChallengeRifts.ChallengeData.Builder> internal__static_D3_ChallengeRifts_ChallengeData__FieldAccessorTable;
    internal static pbd::MessageDescriptor internal__static_D3_ChallengeRifts_AccountData__Descriptor;
    internal static pb::FieldAccess.FieldAccessorTable<global::D3.ChallengeRifts.AccountData, global::D3.ChallengeRifts.AccountData.Builder> internal__static_D3_ChallengeRifts_AccountData__FieldAccessorTable;
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static ChallengeRifts() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          "ChRDaGFsbGVuZ2VSaWZ0cy5wcm90bxIRRDMuQ2hhbGxlbmdlUmlmdHMaEEhl" + 
          "cm9Db21tb24ucHJvdG8i/AIKDUNoYWxsZW5nZURhdGESKQoQY2hhbGxlbmdl" + 
          "X251bWJlchgBIAEoDVIPY2hhbGxlbmdlTnVtYmVyEjkKGWNoYWxsZW5nZV9z" + 
          "dGFydF91bml4X3RpbWUYAiABKANSFmNoYWxsZW5nZVN0YXJ0VW5peFRpbWUS" + 
          "SgoiY2hhbGxlbmdlX2xhc3RfYnJvYWRjYXN0X3VuaXhfdGltZRgDIAEoA1Ie" + 
          "Y2hhbGxlbmdlTGFzdEJyb2FkY2FzdFVuaXhUaW1lEkQKH2NoYWxsZW5nZV9l" + 
          "bmRfdW5peF90aW1lX2NvbnNvbGUYBCABKANSG2NoYWxsZW5nZUVuZFVuaXhU" + 
          "aW1lQ29uc29sZRIlCg5jaGFsbGVuZ2VfaGFzaBgFIAEoBFINY2hhbGxlbmdl" + 
          "SGFzaBJMCiFjaGFsbGVuZ2VfbmVwaGFsZW1fb3JiX211bHRpcGxpZXIYBiAB" + 
          "KAI6ATFSHmNoYWxsZW5nZU5lcGhhbGVtT3JiTXVsdGlwbGllciLOAQoLQWNj" + 
          "b3VudERhdGESPwocbGFzdF9jaGFsbGVuZ2VfcmV3YXJkX2Vhcm5lZBgBIAEo" + 
          "DVIZbGFzdENoYWxsZW5nZVJld2FyZEVhcm5lZBIwChRsYXN0X2NoYWxsZW5n" + 
          "ZV90cmllZBgCIAEoDVISbGFzdENoYWxsZW5nZVRyaWVkEkwKE3NhdmVkX2Nv" + 
          "bnZlcnNhdGlvbnMYAyABKAsyGy5EMy5IZXJvLlNhdmVkQ29udmVyc2F0aW9u" + 
          "c1ISc2F2ZWRDb252ZXJzYXRpb25z");
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        internal__static_D3_ChallengeRifts_ChallengeData__Descriptor = Descriptor.MessageTypes[0];
        internal__static_D3_ChallengeRifts_ChallengeData__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::D3.ChallengeRifts.ChallengeData, global::D3.ChallengeRifts.ChallengeData.Builder>(internal__static_D3_ChallengeRifts_ChallengeData__Descriptor,
                new string[] { "ChallengeNumber", "ChallengeStartUnixTime", "ChallengeLastBroadcastUnixTime", "ChallengeEndUnixTimeConsole", "ChallengeHash", "ChallengeNephalemOrbMultiplier", });
        internal__static_D3_ChallengeRifts_AccountData__Descriptor = Descriptor.MessageTypes[1];
        internal__static_D3_ChallengeRifts_AccountData__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::D3.ChallengeRifts.AccountData, global::D3.ChallengeRifts.AccountData.Builder>(internal__static_D3_ChallengeRifts_AccountData__Descriptor,
                new string[] { "LastChallengeRewardEarned", "LastChallengeTried", "SavedConversations", });
        pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();
        RegisterAllExtensions(registry);
        global::D3.Hero.HeroCommon.RegisterAllExtensions(registry);
        return registry;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          global::D3.Hero.HeroCommon.Descriptor, 
          }, assigner);
    }
    #endregion
    
  }
  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public sealed partial class ChallengeData : pb::GeneratedMessage<ChallengeData, ChallengeData.Builder> {
    private ChallengeData() { }
    private static readonly ChallengeData defaultInstance = new ChallengeData().MakeReadOnly();
    private static readonly string[] _challengeDataFieldNames = new string[] { "challenge_end_unix_time_console", "challenge_hash", "challenge_last_broadcast_unix_time", "challenge_nephalem_orb_multiplier", "challenge_number", "challenge_start_unix_time" };
    private static readonly uint[] _challengeDataFieldTags = new uint[] { 32, 40, 24, 53, 8, 16 };
    public static ChallengeData DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override ChallengeData DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override ChallengeData ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::D3.ChallengeRifts.ChallengeRifts.internal__static_D3_ChallengeRifts_ChallengeData__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<ChallengeData, ChallengeData.Builder> InternalFieldAccessors {
      get { return global::D3.ChallengeRifts.ChallengeRifts.internal__static_D3_ChallengeRifts_ChallengeData__FieldAccessorTable; }
    }
    
    public const int ChallengeNumberFieldNumber = 1;
    private bool hasChallengeNumber;
    private uint challengeNumber_;
    public bool HasChallengeNumber {
      get { return hasChallengeNumber; }
    }
    public uint ChallengeNumber {
      get { return challengeNumber_; }
    }
    
    public const int ChallengeStartUnixTimeFieldNumber = 2;
    private bool hasChallengeStartUnixTime;
    private long challengeStartUnixTime_;
    public bool HasChallengeStartUnixTime {
      get { return hasChallengeStartUnixTime; }
    }
    public long ChallengeStartUnixTime {
      get { return challengeStartUnixTime_; }
    }
    
    public const int ChallengeLastBroadcastUnixTimeFieldNumber = 3;
    private bool hasChallengeLastBroadcastUnixTime;
    private long challengeLastBroadcastUnixTime_;
    public bool HasChallengeLastBroadcastUnixTime {
      get { return hasChallengeLastBroadcastUnixTime; }
    }
    public long ChallengeLastBroadcastUnixTime {
      get { return challengeLastBroadcastUnixTime_; }
    }
    
    public const int ChallengeEndUnixTimeConsoleFieldNumber = 4;
    private bool hasChallengeEndUnixTimeConsole;
    private long challengeEndUnixTimeConsole_;
    public bool HasChallengeEndUnixTimeConsole {
      get { return hasChallengeEndUnixTimeConsole; }
    }
    public long ChallengeEndUnixTimeConsole {
      get { return challengeEndUnixTimeConsole_; }
    }
    
    public const int ChallengeHashFieldNumber = 5;
    private bool hasChallengeHash;
    private ulong challengeHash_;
    public bool HasChallengeHash {
      get { return hasChallengeHash; }
    }
    public ulong ChallengeHash {
      get { return challengeHash_; }
    }
    
    public const int ChallengeNephalemOrbMultiplierFieldNumber = 6;
    private bool hasChallengeNephalemOrbMultiplier;
    private float challengeNephalemOrbMultiplier_ = 1F;
    public bool HasChallengeNephalemOrbMultiplier {
      get { return hasChallengeNephalemOrbMultiplier; }
    }
    public float ChallengeNephalemOrbMultiplier {
      get { return challengeNephalemOrbMultiplier_; }
    }
    
    public override bool IsInitialized {
      get {
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _challengeDataFieldNames;
      if (hasChallengeNumber) {
        output.WriteUInt32(1, field_names[4], ChallengeNumber);
      }
      if (hasChallengeStartUnixTime) {
        output.WriteInt64(2, field_names[5], ChallengeStartUnixTime);
      }
      if (hasChallengeLastBroadcastUnixTime) {
        output.WriteInt64(3, field_names[2], ChallengeLastBroadcastUnixTime);
      }
      if (hasChallengeEndUnixTimeConsole) {
        output.WriteInt64(4, field_names[0], ChallengeEndUnixTimeConsole);
      }
      if (hasChallengeHash) {
        output.WriteUInt64(5, field_names[1], ChallengeHash);
      }
      if (hasChallengeNephalemOrbMultiplier) {
        output.WriteFloat(6, field_names[3], ChallengeNephalemOrbMultiplier);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasChallengeNumber) {
          size += pb::CodedOutputStream.ComputeUInt32Size(1, ChallengeNumber);
        }
        if (hasChallengeStartUnixTime) {
          size += pb::CodedOutputStream.ComputeInt64Size(2, ChallengeStartUnixTime);
        }
        if (hasChallengeLastBroadcastUnixTime) {
          size += pb::CodedOutputStream.ComputeInt64Size(3, ChallengeLastBroadcastUnixTime);
        }
        if (hasChallengeEndUnixTimeConsole) {
          size += pb::CodedOutputStream.ComputeInt64Size(4, ChallengeEndUnixTimeConsole);
        }
        if (hasChallengeHash) {
          size += pb::CodedOutputStream.ComputeUInt64Size(5, ChallengeHash);
        }
        if (hasChallengeNephalemOrbMultiplier) {
          size += pb::CodedOutputStream.ComputeFloatSize(6, ChallengeNephalemOrbMultiplier);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static ChallengeData ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static ChallengeData ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static ChallengeData ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static ChallengeData ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static ChallengeData ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static ChallengeData ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static ChallengeData ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static ChallengeData ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static ChallengeData ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static ChallengeData ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private ChallengeData MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(ChallengeData prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
    public sealed partial class Builder : pb::GeneratedBuilder<ChallengeData, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(ChallengeData cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private ChallengeData result;
      
      private ChallengeData PrepareBuilder() {
        if (resultIsReadOnly) {
          ChallengeData original = result;
          result = new ChallengeData();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override ChallengeData MessageBeingBuilt {
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
        get { return global::D3.ChallengeRifts.ChallengeData.Descriptor; }
      }
      
      public override ChallengeData DefaultInstanceForType {
        get { return global::D3.ChallengeRifts.ChallengeData.DefaultInstance; }
      }
      
      public override ChallengeData BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is ChallengeData) {
          return MergeFrom((ChallengeData) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(ChallengeData other) {
        if (other == global::D3.ChallengeRifts.ChallengeData.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasChallengeNumber) {
          ChallengeNumber = other.ChallengeNumber;
        }
        if (other.HasChallengeStartUnixTime) {
          ChallengeStartUnixTime = other.ChallengeStartUnixTime;
        }
        if (other.HasChallengeLastBroadcastUnixTime) {
          ChallengeLastBroadcastUnixTime = other.ChallengeLastBroadcastUnixTime;
        }
        if (other.HasChallengeEndUnixTimeConsole) {
          ChallengeEndUnixTimeConsole = other.ChallengeEndUnixTimeConsole;
        }
        if (other.HasChallengeHash) {
          ChallengeHash = other.ChallengeHash;
        }
        if (other.HasChallengeNephalemOrbMultiplier) {
          ChallengeNephalemOrbMultiplier = other.ChallengeNephalemOrbMultiplier;
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_challengeDataFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _challengeDataFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 8: {
              result.hasChallengeNumber = input.ReadUInt32(ref result.challengeNumber_);
              break;
            }
            case 16: {
              result.hasChallengeStartUnixTime = input.ReadInt64(ref result.challengeStartUnixTime_);
              break;
            }
            case 24: {
              result.hasChallengeLastBroadcastUnixTime = input.ReadInt64(ref result.challengeLastBroadcastUnixTime_);
              break;
            }
            case 32: {
              result.hasChallengeEndUnixTimeConsole = input.ReadInt64(ref result.challengeEndUnixTimeConsole_);
              break;
            }
            case 40: {
              result.hasChallengeHash = input.ReadUInt64(ref result.challengeHash_);
              break;
            }
            case 53: {
              result.hasChallengeNephalemOrbMultiplier = input.ReadFloat(ref result.challengeNephalemOrbMultiplier_);
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public bool HasChallengeNumber {
        get { return result.hasChallengeNumber; }
      }
      public uint ChallengeNumber {
        get { return result.ChallengeNumber; }
        set { SetChallengeNumber(value); }
      }
      public Builder SetChallengeNumber(uint value) {
        PrepareBuilder();
        result.hasChallengeNumber = true;
        result.challengeNumber_ = value;
        return this;
      }
      public Builder ClearChallengeNumber() {
        PrepareBuilder();
        result.hasChallengeNumber = false;
        result.challengeNumber_ = 0;
        return this;
      }
      
      public bool HasChallengeStartUnixTime {
        get { return result.hasChallengeStartUnixTime; }
      }
      public long ChallengeStartUnixTime {
        get { return result.ChallengeStartUnixTime; }
        set { SetChallengeStartUnixTime(value); }
      }
      public Builder SetChallengeStartUnixTime(long value) {
        PrepareBuilder();
        result.hasChallengeStartUnixTime = true;
        result.challengeStartUnixTime_ = value;
        return this;
      }
      public Builder ClearChallengeStartUnixTime() {
        PrepareBuilder();
        result.hasChallengeStartUnixTime = false;
        result.challengeStartUnixTime_ = 0L;
        return this;
      }
      
      public bool HasChallengeLastBroadcastUnixTime {
        get { return result.hasChallengeLastBroadcastUnixTime; }
      }
      public long ChallengeLastBroadcastUnixTime {
        get { return result.ChallengeLastBroadcastUnixTime; }
        set { SetChallengeLastBroadcastUnixTime(value); }
      }
      public Builder SetChallengeLastBroadcastUnixTime(long value) {
        PrepareBuilder();
        result.hasChallengeLastBroadcastUnixTime = true;
        result.challengeLastBroadcastUnixTime_ = value;
        return this;
      }
      public Builder ClearChallengeLastBroadcastUnixTime() {
        PrepareBuilder();
        result.hasChallengeLastBroadcastUnixTime = false;
        result.challengeLastBroadcastUnixTime_ = 0L;
        return this;
      }
      
      public bool HasChallengeEndUnixTimeConsole {
        get { return result.hasChallengeEndUnixTimeConsole; }
      }
      public long ChallengeEndUnixTimeConsole {
        get { return result.ChallengeEndUnixTimeConsole; }
        set { SetChallengeEndUnixTimeConsole(value); }
      }
      public Builder SetChallengeEndUnixTimeConsole(long value) {
        PrepareBuilder();
        result.hasChallengeEndUnixTimeConsole = true;
        result.challengeEndUnixTimeConsole_ = value;
        return this;
      }
      public Builder ClearChallengeEndUnixTimeConsole() {
        PrepareBuilder();
        result.hasChallengeEndUnixTimeConsole = false;
        result.challengeEndUnixTimeConsole_ = 0L;
        return this;
      }
      
      public bool HasChallengeHash {
        get { return result.hasChallengeHash; }
      }
      public ulong ChallengeHash {
        get { return result.ChallengeHash; }
        set { SetChallengeHash(value); }
      }
      public Builder SetChallengeHash(ulong value) {
        PrepareBuilder();
        result.hasChallengeHash = true;
        result.challengeHash_ = value;
        return this;
      }
      public Builder ClearChallengeHash() {
        PrepareBuilder();
        result.hasChallengeHash = false;
        result.challengeHash_ = 0UL;
        return this;
      }
      
      public bool HasChallengeNephalemOrbMultiplier {
        get { return result.hasChallengeNephalemOrbMultiplier; }
      }
      public float ChallengeNephalemOrbMultiplier {
        get { return result.ChallengeNephalemOrbMultiplier; }
        set { SetChallengeNephalemOrbMultiplier(value); }
      }
      public Builder SetChallengeNephalemOrbMultiplier(float value) {
        PrepareBuilder();
        result.hasChallengeNephalemOrbMultiplier = true;
        result.challengeNephalemOrbMultiplier_ = value;
        return this;
      }
      public Builder ClearChallengeNephalemOrbMultiplier() {
        PrepareBuilder();
        result.hasChallengeNephalemOrbMultiplier = false;
        result.challengeNephalemOrbMultiplier_ = 1F;
        return this;
      }
    }
    static ChallengeData() {
      object.ReferenceEquals(global::D3.ChallengeRifts.ChallengeRifts.Descriptor, null);
    }
  }
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
  public sealed partial class AccountData : pb::GeneratedMessage<AccountData, AccountData.Builder> {
    private AccountData() { }
    private static readonly AccountData defaultInstance = new AccountData().MakeReadOnly();
    private static readonly string[] _accountDataFieldNames = new string[] { "last_challenge_reward_earned", "last_challenge_tried", "saved_conversations" };
    private static readonly uint[] _accountDataFieldTags = new uint[] { 8, 16, 26 };
    public static AccountData DefaultInstance {
      get { return defaultInstance; }
    }
    
    public override AccountData DefaultInstanceForType {
      get { return DefaultInstance; }
    }
    
    protected override AccountData ThisMessage {
      get { return this; }
    }
    
    public static pbd::MessageDescriptor Descriptor {
      get { return global::D3.ChallengeRifts.ChallengeRifts.internal__static_D3_ChallengeRifts_AccountData__Descriptor; }
    }
    
    protected override pb::FieldAccess.FieldAccessorTable<AccountData, AccountData.Builder> InternalFieldAccessors {
      get { return global::D3.ChallengeRifts.ChallengeRifts.internal__static_D3_ChallengeRifts_AccountData__FieldAccessorTable; }
    }
    
    public const int LastChallengeRewardEarnedFieldNumber = 1;
    private bool hasLastChallengeRewardEarned;
    private uint lastChallengeRewardEarned_;
    public bool HasLastChallengeRewardEarned {
      get { return hasLastChallengeRewardEarned; }
    }
    public uint LastChallengeRewardEarned {
      get { return lastChallengeRewardEarned_; }
    }
    
    public const int LastChallengeTriedFieldNumber = 2;
    private bool hasLastChallengeTried;
    private uint lastChallengeTried_;
    public bool HasLastChallengeTried {
      get { return hasLastChallengeTried; }
    }
    public uint LastChallengeTried {
      get { return lastChallengeTried_; }
    }
    
    public const int SavedConversationsFieldNumber = 3;
    private bool hasSavedConversations;
    private global::D3.Hero.SavedConversations savedConversations_;
    public bool HasSavedConversations {
      get { return hasSavedConversations; }
    }
    public global::D3.Hero.SavedConversations SavedConversations {
      get { return savedConversations_ ?? global::D3.Hero.SavedConversations.DefaultInstance; }
    }
    
    public override bool IsInitialized {
      get {
        if (HasSavedConversations) {
          if (!SavedConversations.IsInitialized) return false;
        }
        return true;
      }
    }
    
    public override void WriteTo(pb::ICodedOutputStream output) {
      int size = SerializedSize;
      string[] field_names = _accountDataFieldNames;
      if (hasLastChallengeRewardEarned) {
        output.WriteUInt32(1, field_names[0], LastChallengeRewardEarned);
      }
      if (hasLastChallengeTried) {
        output.WriteUInt32(2, field_names[1], LastChallengeTried);
      }
      if (hasSavedConversations) {
        output.WriteMessage(3, field_names[2], SavedConversations);
      }
      UnknownFields.WriteTo(output);
    }
    
    private int memoizedSerializedSize = -1;
    public override int SerializedSize {
      get {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasLastChallengeRewardEarned) {
          size += pb::CodedOutputStream.ComputeUInt32Size(1, LastChallengeRewardEarned);
        }
        if (hasLastChallengeTried) {
          size += pb::CodedOutputStream.ComputeUInt32Size(2, LastChallengeTried);
        }
        if (hasSavedConversations) {
          size += pb::CodedOutputStream.ComputeMessageSize(3, SavedConversations);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
    }
    
    public static AccountData ParseFrom(pb::ByteString data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static AccountData ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static AccountData ParseFrom(byte[] data) {
      return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
    }
    public static AccountData ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
    }
    public static AccountData ParseFrom(global::System.IO.Stream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static AccountData ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    public static AccountData ParseDelimitedFrom(global::System.IO.Stream input) {
      return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
    }
    public static AccountData ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
      return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
    }
    public static AccountData ParseFrom(pb::ICodedInputStream input) {
      return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
    }
    public static AccountData ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
      return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
    }
    private AccountData MakeReadOnly() {
      return this;
    }
    
    public static Builder CreateBuilder() { return new Builder(); }
    public override Builder ToBuilder() { return CreateBuilder(this); }
    public override Builder CreateBuilderForType() { return new Builder(); }
    public static Builder CreateBuilder(AccountData prototype) {
      return new Builder(prototype);
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ProtoGen", "2.4.1.473")]
    public sealed partial class Builder : pb::GeneratedBuilder<AccountData, Builder> {
      protected override Builder ThisBuilder {
        get { return this; }
      }
      public Builder() {
        result = DefaultInstance;
        resultIsReadOnly = true;
      }
      internal Builder(AccountData cloneFrom) {
        result = cloneFrom;
        resultIsReadOnly = true;
      }
      
      private bool resultIsReadOnly;
      private AccountData result;
      
      private AccountData PrepareBuilder() {
        if (resultIsReadOnly) {
          AccountData original = result;
          result = new AccountData();
          resultIsReadOnly = false;
          MergeFrom(original);
        }
        return result;
      }
      
      public override bool IsInitialized {
        get { return result.IsInitialized; }
      }
      
      protected override AccountData MessageBeingBuilt {
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
        get { return global::D3.ChallengeRifts.AccountData.Descriptor; }
      }
      
      public override AccountData DefaultInstanceForType {
        get { return global::D3.ChallengeRifts.AccountData.DefaultInstance; }
      }
      
      public override AccountData BuildPartial() {
        if (resultIsReadOnly) {
          return result;
        }
        resultIsReadOnly = true;
        return result.MakeReadOnly();
      }
      
      public override Builder MergeFrom(pb::IMessage other) {
        if (other is AccountData) {
          return MergeFrom((AccountData) other);
        } else {
          base.MergeFrom(other);
          return this;
        }
      }
      
      public override Builder MergeFrom(AccountData other) {
        if (other == global::D3.ChallengeRifts.AccountData.DefaultInstance) return this;
        PrepareBuilder();
        if (other.HasLastChallengeRewardEarned) {
          LastChallengeRewardEarned = other.LastChallengeRewardEarned;
        }
        if (other.HasLastChallengeTried) {
          LastChallengeTried = other.LastChallengeTried;
        }
        if (other.HasSavedConversations) {
          MergeSavedConversations(other.SavedConversations);
        }
        this.MergeUnknownFields(other.UnknownFields);
        return this;
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input) {
        return MergeFrom(input, pb::ExtensionRegistry.Empty);
      }
      
      public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        PrepareBuilder();
        pb::UnknownFieldSet.Builder unknownFields = null;
        uint tag;
        string field_name;
        while (input.ReadTag(out tag, out field_name)) {
          if(tag == 0 && field_name != null) {
            int field_ordinal = global::System.Array.BinarySearch(_accountDataFieldNames, field_name, global::System.StringComparer.Ordinal);
            if(field_ordinal >= 0)
              tag = _accountDataFieldTags[field_ordinal];
            else {
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              continue;
            }
          }
          switch (tag) {
            case 0: {
              throw pb::InvalidProtocolBufferException.InvalidTag();
            }
            default: {
              if (pb::WireFormat.IsEndGroupTag(tag)) {
                if (unknownFields != null) {
                  this.UnknownFields = unknownFields.Build();
                }
                return this;
              }
              if (unknownFields == null) {
                unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
              }
              ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
              break;
            }
            case 8: {
              result.hasLastChallengeRewardEarned = input.ReadUInt32(ref result.lastChallengeRewardEarned_);
              break;
            }
            case 16: {
              result.hasLastChallengeTried = input.ReadUInt32(ref result.lastChallengeTried_);
              break;
            }
            case 26: {
              global::D3.Hero.SavedConversations.Builder subBuilder = global::D3.Hero.SavedConversations.CreateBuilder();
              if (result.hasSavedConversations) {
                subBuilder.MergeFrom(SavedConversations);
              }
              input.ReadMessage(subBuilder, extensionRegistry);
              SavedConversations = subBuilder.BuildPartial();
              break;
            }
          }
        }
        
        if (unknownFields != null) {
          this.UnknownFields = unknownFields.Build();
        }
        return this;
      }
      
      
      public bool HasLastChallengeRewardEarned {
        get { return result.hasLastChallengeRewardEarned; }
      }
      public uint LastChallengeRewardEarned {
        get { return result.LastChallengeRewardEarned; }
        set { SetLastChallengeRewardEarned(value); }
      }
      public Builder SetLastChallengeRewardEarned(uint value) {
        PrepareBuilder();
        result.hasLastChallengeRewardEarned = true;
        result.lastChallengeRewardEarned_ = value;
        return this;
      }
      public Builder ClearLastChallengeRewardEarned() {
        PrepareBuilder();
        result.hasLastChallengeRewardEarned = false;
        result.lastChallengeRewardEarned_ = 0;
        return this;
      }
      
      public bool HasLastChallengeTried {
        get { return result.hasLastChallengeTried; }
      }
      public uint LastChallengeTried {
        get { return result.LastChallengeTried; }
        set { SetLastChallengeTried(value); }
      }
      public Builder SetLastChallengeTried(uint value) {
        PrepareBuilder();
        result.hasLastChallengeTried = true;
        result.lastChallengeTried_ = value;
        return this;
      }
      public Builder ClearLastChallengeTried() {
        PrepareBuilder();
        result.hasLastChallengeTried = false;
        result.lastChallengeTried_ = 0;
        return this;
      }
      
      public bool HasSavedConversations {
       get { return result.hasSavedConversations; }
      }
      public global::D3.Hero.SavedConversations SavedConversations {
        get { return result.SavedConversations; }
        set { SetSavedConversations(value); }
      }
      public Builder SetSavedConversations(global::D3.Hero.SavedConversations value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        result.hasSavedConversations = true;
        result.savedConversations_ = value;
        return this;
      }
      public Builder SetSavedConversations(global::D3.Hero.SavedConversations.Builder builderForValue) {
        pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
        PrepareBuilder();
        result.hasSavedConversations = true;
        result.savedConversations_ = builderForValue.Build();
        return this;
      }
      public Builder MergeSavedConversations(global::D3.Hero.SavedConversations value) {
        pb::ThrowHelper.ThrowIfNull(value, "value");
        PrepareBuilder();
        if (result.hasSavedConversations &&
            result.savedConversations_ != global::D3.Hero.SavedConversations.DefaultInstance) {
            result.savedConversations_ = global::D3.Hero.SavedConversations.CreateBuilder(result.savedConversations_).MergeFrom(value).BuildPartial();
        } else {
          result.savedConversations_ = value;
        }
        result.hasSavedConversations = true;
        return this;
      }
      public Builder ClearSavedConversations() {
        PrepareBuilder();
        result.hasSavedConversations = false;
        result.savedConversations_ = null;
        return this;
      }
    }
    static AccountData() {
      object.ReferenceEquals(global::D3.ChallengeRifts.ChallengeRifts.Descriptor, null);
    }
  }
  
  #endregion
  
}

#endregion Designer generated code
