using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace SSLCertCheck
{
	public class SitesInfo
	{
		#region Constructors

		public SitesInfo(string url)
		{
            this.Url = url;
        }

		#endregion


		#region Fields

		#endregion


		#region Properties

		public string Url { get; set; }

		public DateTime Expiration { get; set; }

        public string Issuer { get; set; }

        public X509Certificate2 Certificate { get; private set; }

		#endregion


		#region Methods

		public void CheckCert()
		{
			ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

			var request = WebRequest.Create(this.Url);
            request.GetResponse();			
		}

		private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
            var newCert = (X509Certificate2)certificate;

            this.Certificate = newCert;
            this.Expiration = newCert.NotAfter;
            this.Issuer = newCert.Issuer;

            return true;
		}

		#endregion
	}
}

