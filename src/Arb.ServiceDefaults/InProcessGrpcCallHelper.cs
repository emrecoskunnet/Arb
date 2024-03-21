using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Paros.ServiceDefaults;

public static class InProcessGrpcCallHelper
{
    public static bool RemoteCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
    {
        string s = $@"RemoteCertificateValidationCallback certificate:{certificate} chain:{chain} errors:{errors}";
        Console.WriteLine(s);
        if (errors == System.Net.Security.SslPolicyErrors.None)
        {
            return true;
        }
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if ((errors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) == 0)
        {
            // In all other cases, return false.
            return false;
        }

        if (chain?.ChainStatus == null)
        {
            return true;
        }

        return chain.ChainStatus.Select(status => status.Status)
            .Where(statusFlag => (certificate?.Subject != certificate?.Issuer) 
                                 || (statusFlag != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
            .All(statusFlag => statusFlag == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError);

        // When processing reaches this line, the only errors in the certificate chain are 
        // untrusted root errors for self-signed certificates. These certificates are valid
    }
}
