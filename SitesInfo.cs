using System;
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

		#endregion


		#region Properties

        /// <summary>
        /// Full url of the website to check
        /// </summary>
		public string Url { get; set; }

        /// <summary>
        /// Expiration date of the certificate
        /// </summary>
		public DateTime Expiration { get { return Certificate.NotAfter; } }

        /// <summary>
        /// Issuer of the certificate
        /// </summary>
        public string Issuer { get { return Certificate.Issuer; } }

        private X509Certificate2 Certificate { get; set; }

        #endregion


        #region Methods

        /// <summary>
        /// Make a web request and capture the SSL handshake. Stored the certificate info in the Certificate property
        /// </summary>
        public void CheckCert()
		{
			ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

            var request = (HttpWebRequest)WebRequest.Create(this.Url);
            request.AllowAutoRedirect = false;
            request.GetResponse();
		}

        /// <summary>
        /// Catches the SSL certificate check when a WebRequest is made
        /// </summary>
		private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
            var newCert = (X509Certificate2)certificate;
            this.Certificate = newCert;

            return true;
		}

		#endregion
	}
}

