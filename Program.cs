using System;
using System.IO;

namespace SSLCertCheck
{
	class MainClass
	{
		public static void Main (string[] args)
		{
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
                    Console.WriteLine("-- Expiration: {0}", siteCheck.Expiration);
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

		public static string[] LoadSites()
		{
			string sitesListFile = "./certlist.txt";
			if(!File.Exists(sitesListFile)){
				Console.WriteLine ("The file {0} with a list of URLs did not exist. Creating a blank one for your convinience.", sitesListFile);
				try {
					System.IO.File.CreateText (sitesListFile);
					Console.WriteLine("The file was successfully created");
				} catch (Exception ex) {
					Console.WriteLine ("Failed to create file: {0}", ex.Message);
				}

			}

			var sitesList = System.IO.File.ReadAllLines(sitesListFile);
			return sitesList;
		}

	}
}
