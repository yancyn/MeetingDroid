﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoogleCalendarSync {
	
	
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("ICSharpCode.SettingsEditor.SettingsCodeGeneratorTool", "4.3.0.9390")]
	public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
		
		private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
		
		public static Settings Default {
			get {
				return defaultInstance;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string Api {
			get {
				return ((string)(this["Api"]));
			}
			set {
				this["Api"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string CalendarId {
			get {
				return ((string)(this["CalendarId"]));
			}
			set {
				this["CalendarId"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string ClientID {
			get {
				return ((string)(this["ClientID"]));
			}
			set {
				this["ClientID"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string ClientSecret {
			get {
				return ((string)(this["ClientSecret"]));
			}
			set {
				this["ClientSecret"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string Domain {
			get {
				return ((string)(this["Domain"]));
			}
			set {
				this["Domain"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string ExchangeEmail {
			get {
				return ((string)(this["ExchangeEmail"]));
			}
			set {
				this["ExchangeEmail"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string ExchangePassword {
			get {
				return ((string)(this["ExchangePassword"]));
			}
			set {
				this["ExchangePassword"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public string FeedUrl {
			get {
				return ((string)(this["FeedUrl"]));
			}
			set {
				this["FeedUrl"] = value;
			}
		}
			
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("4")]
		public int Interval {
			get {
				return ((int)(this["Interval"]));
			}
			set {
				this["Interval"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("30")]
		public int PeriodDays {
			get {
				return ((int)(this["PeriodDays"]));
			}
			set {
				this["PeriodDays"] = value;
			}
		}
	}
}
