using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace clientesincrono
{
    public class RsaEncryptionStatic {
        private RSACryptoServiceProvider csp;
        private RSAParameters _privateKey;
        private RSAParameters _publicKey;

        public RsaEncryptionStatic() {
            csp = new RSACryptoServiceProvider(2048);
            _privateKey = csp.ExportParameters(true);
            _publicKey = csp.ExportParameters(false);
        }

        public string GetPublicKey() {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _publicKey);
            return sw.ToString();
        }

        public string GetPrivateKey() {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _privateKey);
            return sw.ToString();
        }

        // public void setPublicKey() {
        // }

        // public static string ExportPublicParametersToXml(RSACryptoServiceProvider rsa)
        // {
        //     RSAParameters publicParameters = new RSAParameters();
        //     publicParameters.Exponent = rsa.ExportParameters(false).Exponent;
        //     publicParameters.Modulus = rsa.ExportParameters(false).Modulus;
        //     using (StringWriter writer = new Utf8StringWriter())
        //     {
        //         XmlSerializer xml = new XmlSerializer(typeof(RSAParameters));
        //         xml.Serialize(writer, publicParameters);
        //         return writer.ToString();
        //     }
        // }

        public static RSAParameters PublicParametersFromXml(string data)
        {
            XmlSerializer xml = new XmlSerializer(typeof(RSAParameters));
            object result;
            using (TextReader reader = new StringReader(data))
            {
                result = xml.Deserialize(reader);
            }
            return (RSAParameters)result;
        }

        public string Encrypt(string plainText, RSAParameters publicKey) {
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(publicKey);
            var data = Encoding.Unicode.GetBytes(plainText);
            var cypher = csp.Encrypt(data, false);
            return Convert.ToBase64String(cypher);
        }

        public string Decrypt(string cypherText, RSAParameters privateKey) {
            var dataBytes = Convert.FromBase64String(cypherText);
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey);
            var plainText = csp.Decrypt(dataBytes, false);
            return Encoding.Unicode.GetString(plainText);;
        }
    }

    // public class Test2{
    //     public static void Main() {
    //         //instanciamos
    //         RsaEncryptionStatic rsa = new RsaEncryptionStatic();
    //         RsaEncryptionStatic rsa2 = new RsaEncryptionStatic();

    //         //obtenemos string de claves privadas y publicas
    //         //seran las que mandamos
    //         string pubKeyStr = rsa.GetPublicKey();
    //         string privKeyStr = rsa.GetPrivateKey();

    //         //convertimos string de claves en RSAParameters
    //         var pubKey = RsaEncryptionStatic.PublicParametersFromXml(pubKeyStr);
    //         var privKey = RsaEncryptionStatic.PublicParametersFromXml(privKeyStr);

    //         //texto a encriptar
    //         string txt = "HOLA";
    //         //encriptamos con una clave publica generada
    //         string resulEncrit = rsa2.Encrypt(txt, pubKey);
    //         Console.WriteLine(resulEncrit);

    //         //desencriptamos con una clave publica generada
    //         string resulDecript = rsa2.Decrypt(resulEncrit, privKey);
    //         Console.WriteLine(resulDecript);


    //         // RsaEncryptionStatic2 rsa = new RsaEncryptionStatic2();
    //         // string cypher = string.Empty;

    //         // Console.WriteLine("Public key: {0}\n", rsa.GetPublicKey());
    //         // Console.WriteLine("-----------------");
    //         // Console.WriteLine("Private key: {0}\n", rsa.GetPrivateKey());

    //         // Console.WriteLine("Enter yout text to encrypt: ");
    //         // var text = Console.ReadLine();
    //         // if (!string.IsNullOrEmpty(text)) {
    //         //     cypher = rsa.Encrypt(text);
    //         //     Console.WriteLine(cypher);
    //         // }

    //         // Console.WriteLine("Press to decrypt: ");
    //         // Console.ReadLine();
    //         // var plainText = rsa.Decrypt(cypher);
    //         // Console.WriteLine("Decrypted message: {0}",plainText);
    //         // Console.ReadLine();
    //     }
    // }
}