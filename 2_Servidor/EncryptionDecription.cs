using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace servidorsincrono
{
    public class RsaEncryptionStatic {
        private RSACryptoServiceProvider csp;
        //clave privada para desencriptar
        private RSAParameters _privateKey;
        //clave publica para encriptar
        private RSAParameters _publicKey;


        // a traves del constructor, nos generara claves aleatorias
        public RsaEncryptionStatic() {
            csp = new RSACryptoServiceProvider(2048);
            _privateKey = csp.ExportParameters(true);
            _publicKey = csp.ExportParameters(false);
        }

        //obtener la clave publica
        public string GetPublicKey() {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _publicKey);
            return sw.ToString();
        }


        //obtener clave privada
        public string GetPrivateKey() {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _privateKey);
            return sw.ToString();
        }

        //Crear parametros desde una cadena
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

        //encriptar mediante la clave publica
        public string Encrypt(string plainText, RSAParameters publicKey) {
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(publicKey);
            var data = Encoding.Unicode.GetBytes(plainText);
            var cypher = csp.Encrypt(data, false);
            return Convert.ToBase64String(cypher);
        }

        //Desencriptar con la clave privada
        public string Decrypt(string cypherText, RSAParameters privateKey) {
            var dataBytes = Convert.FromBase64String(cypherText);
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey);
            var plainText = csp.Decrypt(dataBytes, false);
            return Encoding.Unicode.GetString(plainText);;
        }
    }

}