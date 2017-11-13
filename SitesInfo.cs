using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace SSLCertCheck
{
	public class SitesInfo
	{
		public SitesInfo(string url) : this(url,443)
		{
		}

		public SitesInfo(string url, int port)
		{
			this.Url = url;
			this.Port = port;

			CheckCert ();
		}

		#region Properties

		public string Url {
			get;
			set;
		}

		public int Port {
			get;
			set;
		}

		public X509Certificate Certificate {
			get;
			private set;
		}

		#endregion

		#region Methods

		private void CheckCert()
		{
			ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;

			var request = WebRequest.Create(this.Url);
			request.GetResponse();
		}

		private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			this.Certificate = certificate;

			return true;
		}

		#endregion
	}
}

