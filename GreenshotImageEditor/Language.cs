
using System;
using System.Resources;
using System.Globalization;
using System.Threading;
using Greenshot.Configuration;
using System.Diagnostics;


namespace Greenshot
{
	/// <summary>
	/// Description of Language.
	/// </summary>
	public class Language
	{
		private ResourceManager rm;
		private static Language uniqueInstance;
		private Language() {
			rm = new ResourceManager("Greenshot.UI", System.Reflection.Assembly.GetExecutingAssembly());
			SetLanguage(AppConfig.GetInstance().Ui_Language);
		}

		public static Language GetInstance() {
			if(uniqueInstance == null) {
				uniqueInstance = new Language();
			}
			return uniqueInstance;
				
		}
		public void SetLanguage(string cultureInfo) {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);
		}
		public string GetString(string id) {
			string s = rm.GetString(id);
			return (s != null) ? s : "string ###"+id+"### not found";
		}
	}
}
