using System;
using System.IO;
using System.Configuration;
using System.Net.Mail;

namespace SSLCertCheck
{
	class MainClass
	{
        #region Fields

        static string siteListFile = "./certlist.txt";
        static int expireAlertDays = 0;

        static bool emailEnabled = false;
        static string emailServer = "";
        static int emailPort = 25;
        static string emailFrom = "";
        static string emailTo = "";

        #endregion

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

                    var expiresIn = (siteCheck.Expiration - DateTime.Today).TotalDays;

                    var message = string.Format("-- Expiration: {0} ({1} days)", siteCheck.Expiration, expiresIn);
                    if (expiresIn > expireAlertDays)
                    {
                        Console.WriteLine(message);
                    }
                    else
                    {
                        ColorConsole(message, ConsoleColor.Red);
                        SendNotification(siteCheck);
                    }

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
            if (ConfigurationManager.AppSettings["siteListFile"] != null)
            {
                siteListFile = ConfigurationManager.AppSettings["siteListFile"];
            }

            if (ConfigurationManager.AppSettings["expireAlertDays"] != null)
            {
                expireAlertDays = Convert.ToInt32(ConfigurationManager.AppSettings["expireAlertDays"]);
            }

            if (ConfigurationManager.AppSettings["emailEnabled"] != null)
            {
                emailEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["emailEnabled"]);
            }

            if (ConfigurationManager.AppSettings["emailServer"] != null)
            {
                emailServer = ConfigurationManager.AppSettings["emailServer"];
            }

            if (ConfigurationManager.AppSettings["emailPort"] != null)
            {
                emailPort = Convert.ToInt32(ConfigurationManager.AppSettings["emailPort"]);
            }

            if (ConfigurationManager.AppSettings["emailFrom"] != null)
            {
                emailFrom = ConfigurationManager.AppSettings["emailFrom"];
            }

            if (ConfigurationManager.AppSettings["emailTo"] != null)
            {
                emailTo = ConfigurationManager.AppSettings["emailTo"];
            }
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
        /// Sends an email about an expiring certificate
        /// </summary>
        /// <param name="site">The site to send a message about</param>
        public static void SendNotification(SitesInfo site)
        {
            if (!emailEnabled) { return; }

            var emailSubject = String.Format("Expiring Certificate: {0}", site.Url);
            var emailBody = String.Format("The certificate for {0} will expire soon on {1}", site.Url, site.Expiration);

            var smtpClient = new SmtpClient(emailServer, emailPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.SendMailAsync(emailFrom, emailTo, emailSubject, emailBody);
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
