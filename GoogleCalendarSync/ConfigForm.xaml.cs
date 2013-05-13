/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 5/13/2013
 * Time: 11:04 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GoogleCalendarSync
{
	/// <summary>
	/// Interaction logic for ConfigForm.xaml
	/// </summary>
	public partial class ConfigForm : Window
	{
		public ConfigForm()
		{
			InitializeComponent();
		}
		
		void Image_MouseDown(object sender, MouseButtonEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("open browser for help");
			Process.Start("https://code.google.com/apis/console");
		}
	}
}