/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 05/13/2013
 * Time: 14:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Input;

namespace GoogleCalendarSync
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	public sealed partial class Settings
	{
		private SaveSetting saveSetting;
		public SaveSetting SaveSetting {get {return this.saveSetting;}}
		public Settings()
		{
			this.saveSetting = new SaveSetting(this);
		}
	}
	public class SaveSetting: ICommand
	{
		private Settings manager;
		public SaveSetting(Settings sender)
		{
			this.manager = sender;
		}
		public bool CanExecute(object parameter)
		{
			return true;
		}
		public event EventHandler CanExecuteChanged;
		public void Execute(object parameter)
		{
			System.Diagnostics.Debug.WriteLine("saving config...");
			this.manager.Save();
			this.manager.Reload();
		}
	}
}
