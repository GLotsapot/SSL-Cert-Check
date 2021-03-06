﻿using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace SSLCertCheck
{
	public class SitesInfo
	{
		#region Constructors

        /// <summary>
        /// Create a new site to check
        /// </summary>
        /// <param name="url">Full URL of the site to check</param>
        /// <example>http://www.github.com</example>
		public SitesInfo(string url)
		{
            this.Url = url;
        }

        #endregion


        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion


        #region Properties

        /// <summary>
        /// Full url of the website to check
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Expiration date of the certificate
        /// </summary>
		public DateTime Expiration { get; private set; }

        /// <summary>
        /// Issuer of the certificate
        /// </summary>
        public string Issuer { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        /// Make a web request and capture the SSL handshake. Stored the certificate info in the Certificate property
        /// </summary>
        public void CheckCert()
		{
            log.Debug("CheckCert: Requesting website");

            // Add several different Security Protocols as options just incase server doesn't allow defaults
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12
                | SecurityProtocolType.Ssl3;

            var request = (HttpWebRequest)WebRequest.Create(this.Url);
            request.AllowAutoRedirect = false;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
                request.GetResponse();
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;
            }
        }

        /// <summary>
        /// Catches the SSL certificate check when a WebRequest is made
        /// </summary>
		private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
            log.Debug(String.Format("ServerCertificateValidationCallback: Got a cert for {0}", certificate.Subject));
            var newCert = (X509Certificate2)certificate;
            this.Expiration = newCert.NotAfter;
            this.Issuer = newCert.Issuer;

            return true;
		}

		#endregion
	}
}

