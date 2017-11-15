# SSL-Cert-Check
Mono / .NET 4.5 Console Project for Checking SSL Expire Dates

A simple app that will take a text list of https urls, and check to see when they are going to expire.
You can additionally have it highlight certificates that will expire within a certain number of days, and even email off warning on those specific sites.

The original purpose of this was, as a 3rd party IT provider, to check to see if our customers certificates were going to expire. This gave us the chance to renew the certificate before it caused any outages.

You can change the default site list, expiration date check, and email settings by placing a file in the same directory as SSL-Cert-Check.exe called SSL-Cert-Check.exe.config

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<add key="siteListFile" value="./certlist.txt"/>
		<add key="expireAlertDays" value="30"/>
		<add key="emailEnabled" value="true"/>
		<add key="emailServer" value="smtp.yourdomain.com"/>
		<add key="emailPort" value="25"/>
		<add key="emailFrom" value="sslCertCheck@yourdomain.com"/>
		<add key="emailTo" value="alerts@yourdomain.com"/>
	</appSettings>
</configuration>
```
