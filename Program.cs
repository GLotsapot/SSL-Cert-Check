using System;
using System.IO;
using System.Configuration; 

namespace SSLCertCheck
{
	class MainClass
	{
		static string siteListFile;

		public static void Main (string[] args)
		{
			LoadSettings();

            string[] sites;

			try {
				sites = LoadSites();
			} catch (Exception ex) {
				Console.WriteLine ("There was an issue trying to load the sites listing - {0}", ex.Message);
				return;
			}

			foreach (var site in sites) {
				Console.WriteLine ("Checking: {0}", site);
				var siteCheck = new SitesInfo(site);
                try
                {
                    siteCheck.CheckCert();
					//TODO: Change the line color of output if certificate will expire soon
                    Console.WriteLine("-- Expiration: {0}", siteCheck.Expiration);
					//TODO: Send email if certificate will expire soon
                }
                catch (Exception ex)
                {
                    Console.WriteLine("X- There was an issue getting the certificate: {0}", ex.Message);
                    // throw;
                }
                

				
			}

#if DEBUG
            Console.WriteLine("-- Press a Key to Continue --");
            Console.ReadKey(true);
#endif

        }

		/// <summary>
		/// Loads settings from app.config file
		/// </summary>
		public static void LoadSettings()
		{
			siteListFile = ConfigurationManager.AppSettings["siteListFile"];
			if (siteListFile == null) {
				siteListFile = "./certlist.txt";
			}

			//TODO: Change emailPort and enableEmail to the correct variable types
			string emailEnabled = ConfigurationManager.AppSettings["emailEnabled"];
			string emailServer = ConfigurationManager.AppSettings ["emailServer"];
			string emailPort = ConfigurationManager.AppSettings ["emailPort"];
			string emailFrom = ConfigurationManager.AppSettings ["emailFrom"];
			string emailTo = ConfigurationManager.AppSettings ["emailTo"];
		}

		/// <summary>
		/// Reads a text file and returns each line as an array of strings
		/// </summary>
		/// <returns>An array of URLs to check</returns>
		public static string[] LoadSites()
		{
			if(!File.Exists(siteListFile)){
				Console.WriteLine ("The file {0} with a list of URLs did not exist. Creating a blank one for your convinience.", siteListFile);
				try {
					System.IO.File.CreateText (siteListFile);
					Console.WriteLine("The file was successfully created");
				} catch (Exception ex) {
					Console.WriteLine ("Failed to create file: {0}", ex.Message);
				}

			}

			var siteList = System.IO.File.ReadAllLines(siteListFile);
			return siteList;
		}

		/// <summary>
		/// Colors the console.
		/// </summary>
		/// <param name="message">Message line to display</param>
		/// <param name="bg">Background color to use</param>
		/// <param name="fg">Foreground color to use</param>
		public static void ColorConsole(string message, ConsoleColor bg = ConsoleColor.Blue, ConsoleColor fg = ConsoleColor.Yellow)
		{
			Console.BackgroundColor = bg;
			Console.ForegroundColor = fg;
			Console.WriteLine (message);
			Console.ResetColor ();
		}
	}
}
