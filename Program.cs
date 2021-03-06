﻿using System;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Mail;

[assembly: log4net.Config.XmlConfigurator(Watch=true)]

namespace SSLCertCheck
{
	class MainClass
	{
        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string siteListFile;
        static int expireAlertDays;
        static bool emailEnabled;
        static string emailServer;
		static string emailAuthUsername;
		static string emailAuthPassword;
        static int emailPort;
        static string emailFrom;
        static string emailTo;

        #endregion

        public static void Main (string[] args)
		{
			Console.Title = "SSL Certificate Checker";
			LoadSettings();

            string[] sites;

			try {
				sites = LoadSites();
			} catch (Exception ex) {
                log.Error("There was an issue trying to load the sites listing", ex);
				return;
			}

			foreach (var site in sites) {
				log.Info(String.Format("Checking: {0}", site));
				var siteCheck = new SitesInfo(site);
                try
                {
                    siteCheck.CheckCert();

                    var expiresIn = (siteCheck.Expiration - DateTime.Today).TotalDays;

                    var message = string.Format("Expiration: {0} ({1} days)", siteCheck.Expiration, expiresIn);
                    if (expiresIn > expireAlertDays)
                    {
                        log.Info(message);
                    }
                    else
                    {
                        log.Warn(message);
                        SendNotification(siteCheck);
                    }

                }
                catch (Exception ex)
                {
                    log.Error("There was an issue getting the certificate", ex);
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
            log.Debug("Loading Settings - Started");

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

			if (ConfigurationManager.AppSettings["emailAuthUsername"] != null)
			{
				emailAuthUsername = ConfigurationManager.AppSettings["emailAuthUsername"];
			}

			if (ConfigurationManager.AppSettings["emailAuthPassword"] != null)
			{
				emailAuthPassword = ConfigurationManager.AppSettings["emailAuthPassword"];
			}

            if (ConfigurationManager.AppSettings["emailFrom"] != null)
            {
                emailFrom = ConfigurationManager.AppSettings["emailFrom"];
            }

            if (ConfigurationManager.AppSettings["emailTo"] != null)
            {
                emailTo = ConfigurationManager.AppSettings["emailTo"];
            }

            log.Debug("Loading Settings - Finished");
        }

		/// <summary>
		/// Reads a text file and returns each line as an array of strings
		/// </summary>
		/// <returns>An array of URLs to check</returns>
		public static string[] LoadSites()
		{
            log.Debug("Loading Sites - Started");

            if (!File.Exists(siteListFile)){
                log.Warn("The file with a list of URLs did not exist. Creating a blank one for your convinience.");
				try {
					var sw = System.IO.File.CreateText(siteListFile);
                    sw.WriteLine("https://github.com/GLotsapot");
                    sw.Close();
					log.Info("The file was successfully created");
				} catch (Exception ex) {
					log.Fatal("Failed to create file: {0}", ex);
				}

			}
			var siteList = System.IO.File.ReadAllLines(siteListFile);

            log.Debug("Loading Sites - Finished");

            return siteList;
        }

        /// <summary>
        /// Sends an email about an expiring certificate
        /// </summary>
        /// <param name="site">The site to send a message about</param>
        public static void SendNotification(SitesInfo site)
        {
            log.Debug("SendNotification - Started");

            if (!emailEnabled) 
			{ 
				log.Debug("SendNotification - Email option disabled, skipping");
				return; 
			}

			var smtpClient = new SmtpClient(emailServer, emailPort);
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

			if (!String.IsNullOrWhiteSpace(emailAuthUsername) & !String.IsNullOrWhiteSpace(emailAuthPassword)) 
			{
				log.Debug("SendNotification - Found SMTP credentials to use");
				smtpClient.Credentials = new NetworkCredential(emailAuthUsername, emailAuthPassword);
			}

            var emailSubject = String.Format("Expiring Certificate: {0}", site.Url);
            var emailBody = String.Format("The certificate for {0} will expire soon on {1}", site.Url, site.Expiration);

            try
            {
				log.Debug("SendNotification - Sending Email");
                smtpClient.Send(emailFrom, emailTo, emailSubject, emailBody);
				log.Info("SendNotification - Email Sent");
            }
            catch (Exception ex)
            {
				log.Error("SendNotification - Email Sending Error", ex);
            }

            log.Debug("SendNotification - Finished");
        }
	}
}
