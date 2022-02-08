using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IDE.Core.Presentation.Licensing
{
    /// <summary>
    /// Usage Guide:
    /// Command for creating the certificate
    /// >> makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=<YourCertName>" -sky Signature
    /// Then export the cert with private key from key store with a password
    /// Also export another cert with only public key
    /// example: makecert -pe -ss My -sr CurrentUser -$ commercial -n "CN=modernpcbstudio.com" -sk ModernPCB
    /// </summary>
    public class LicenseHandler
    {


        public static LicenseEntity ParseLicenseFromBASE64String(Type licenseObjType, string licenseString, string publicKeyXmlString, out LicenseStatus licStatus, out string validationMsg)
        {
            validationMsg = string.Empty;
            licStatus = LicenseStatus.UNDEFINED;

            if (string.IsNullOrWhiteSpace(licenseString))
            {
                licStatus = LicenseStatus.CRACKED;
                return null;
            }

            string _licXML = string.Empty;
            LicenseEntity license = null;

            try
            {

                using (var rsaKey = RSA.Create())
                {
                    rsaKey.FromXmlString(publicKeyXmlString);

                    var xmlDoc = new XmlDocument();

                    // Load an XML file into the XmlDocument object.
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(licenseString)));

                    // Verify the signature of the signed XML.            
                    if (VerifyXml(xmlDoc, rsaKey))
                    {
                        XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
                        xmlDoc.DocumentElement.RemoveChild(nodeList[0]);

                        _licXML = xmlDoc.OuterXml;

                        //Deserialize license
                        XmlSerializer _serializer = new XmlSerializer(typeof(LicenseEntity), new Type[] { licenseObjType });
                        using (StringReader _reader = new StringReader(_licXML))
                        {
                            license = (LicenseEntity)_serializer.Deserialize(_reader);
                        }

                        var licenseValidator = new LicenseValidator();

                        licStatus = licenseValidator.Validate(license, out validationMsg);
                    }
                    else
                    {
                        licStatus = LicenseStatus.INVALID;
                    }
                }
            }
            catch
            {
                licStatus = LicenseStatus.CRACKED;
            }

            return license;
        }


        // Verify the signature of an XML file against an asymmetric 
        // algorithm and return the result.
        private static bool VerifyXml(XmlDocument Doc, RSA Key)
        {
            // Check arguments.
            if (Doc == null)
                throw new ArgumentException("Doc");
            if (Key == null)
                throw new ArgumentException("Key");

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(Doc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = Doc.GetElementsByTagName("Signature");

            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            // This example only supports one signature for
            // the entire XML document.  Throw an exception 
            // if more than one signature was found.
            if (nodeList.Count >= 2)
            {
                throw new CryptographicException("Verification failed: More that one signature was found for the document.");
            }

            // Load the first <signature> node.  
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.
            return signedXml.CheckSignature(Key);
        }

       
    }
}
