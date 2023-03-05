using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace servidorsincrono
{
    public class Program
    {

        public static void StartListening()
        {
            try
            {
//************************ SI NO TE DA, PUEDES PARSEAR TU IP ***************************
            // IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("TUIP"), 11000);
//**************************************************************************************
                //conexiones
                IPAddress ipAddress = getLocalIpAddress();//MAC OS
                // IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.127"), 11000);
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.58"), 11000);
                //socket de espera a conexiones
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //asignar al socket la ip del servidor y el puerto 
                listener.Bind(localEndPoint);

                //numero maximo de conexiones en espera (si ya existe conexion)
                listener.Listen(10);

                // Start listening for connections
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.
                    Console.WriteLine("Esperando una conexion ...");
                    
                    //se acepta la conexion
                    Socket handler = listener.Accept();//aceptar conexion del cliente
                    Console.WriteLine("Conexion aceptada ...");


                    //bucle para recibir varios mensajes
                    while (true)
                    {
                        //almacenar datos
                        string data = "";

                        //para recibir datos del cliente
                        byte[] bytes = null;

                        //bucle para solo un mensaje
                        while (true)
                        {
                            bytes = new byte[10000];

                            //recibimos el nombre del archivo enviado por el cliente
                            int byteRec = handler.Receive(bytes);

                            //convertimos el mensaje de bytes a string
                            data += Encoding.ASCII.GetString(bytes, 0, byteRec);

                            //si termina por <EOF> termina de enviarse ese unico msj
                            if (data.IndexOf("<EOF>") > -1) {
                                break;
                            }
                        }
                        //quitamos <EOF> de data
                        data =  data.Replace("<EOF>", "");//nombrearchivo

                        //desencriptamos mensaje recibido
                        string[] valores = data.Split("*****");//{pubkey, privkey, msj}

                        string respClienteDesenc = Desencriptar(valores[0], valores[1], valores[2]);
                        // Console.WriteLine(respClienteDesenc);

                        //item para buscar en servicio
                        string item = respClienteDesenc;

                        Console.Write("{0} envio:\n nombreDeArchivo: {1}", handler.RemoteEndPoint, item);

                        //mensaje a enviar al cliente
                        string mensaje = "";
                        string mensajeSrv = "";

                        if (estaItemEnServicio(item)) {
                            //enviamos el mensaje de exito y el archivo

                            //mensaje
                            mensaje += "EXISTE";
                            mensaje += "*****";
                            
                            //archivo
                            string itemJSON = obtenerItemJSON(item);

                            mensaje += itemJSON;

                            //srv
                            mensajeSrv += "-----> EXISTE";

                        } else {
                            //enviamos el mensaje de error y null
                            mensaje += "NO EXISTE";
                            mensaje += "*****";

                            //archivo
                            mensaje += "null";

                            //srv
                            mensajeSrv += "-----> NO EXISTE";
                        }


                        Console.WriteLine(mensajeSrv);
                        //enviamos al cliente el mensaje
                        //encriptar mensaje a enviar
                        string[] resultados = encriptar(mensaje);
                        string msjEncriptado = "";
                        //pubkey
                        msjEncriptado += resultados[0];
                        msjEncriptado += "*****";
                        //privkey
                        msjEncriptado += resultados[1];
                        msjEncriptado += "*****";
                        //mensaje encriptado
                        msjEncriptado += resultados[2];
                        // msjEncriptado += "*****";

                        // Console.WriteLine(msjEncriptado);

                        //pubkey*****privkey*****msjEncriptado

                        //enviarlo                        
                        byte[] msgToClient = Encoding.ASCII.GetBytes(msjEncriptado);
                        handler.Send(msgToClient);


                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
// ------------------------------------------------------------------------------------
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        static string[] encriptar(string str) {
            //instanciamos
            RsaEncryptionStatic rsa = new RsaEncryptionStatic();
            RsaEncryptionStatic rsa2 = new RsaEncryptionStatic();

            //obtenemos string de claves privadas y publicas
            //seran las que mandamos
            string pubKeyStr = rsa.GetPublicKey();
            string privKeyStr = rsa.GetPrivateKey();

            //convertimos string de claves en RSAParameters
            var pubKey = RsaEncryptionStatic.PublicParametersFromXml(pubKeyStr);
            var privKey = RsaEncryptionStatic.PublicParametersFromXml(privKeyStr);

            //texto a encriptar
            // string txt = "HOLA";
            //encriptamos con una clave publica generada
            string resulEncrit = rsa2.Encrypt(str, pubKey);
            // Console.WriteLine(resulEncrit);

            string[] results = {pubKeyStr, privKeyStr, resulEncrit};

            return results;
        }

        static string Desencriptar(string pubkeyStr, string privkeyStr, string msjEncriptado) {
            //instanciamos
            RsaEncryptionStatic rsa = new RsaEncryptionStatic();

            //convertimos string de claves en RSAParameters
            var pubKey = RsaEncryptionStatic.PublicParametersFromXml(pubkeyStr);
            var privKey = RsaEncryptionStatic.PublicParametersFromXml(privkeyStr);

            //desencriptamos con clave privada
            string resulDecript = rsa.Decrypt(msjEncriptado, privKey);
            // Console.WriteLine(resulDecript);

            return resulDecript;
        }

        //verifica si un objeto esta en el servicio web
        //para despues enviarlo o no
        static bool estaItemEnServicio(string nombre) {
            List<TodoItem> list = APIConsumer.GetItems();
            foreach(TodoItem item in list) {
                if (item.Name.ToLower() == nombre.ToLower()) {
                    return true;
                }
            }
            return false;
        }

        //Retorna el objeto serializado como JSON
        //cuyo nombre se especifica como parámetro.
        static string obtenerItemJSON(string nombre) {
            List<TodoItem> list = APIConsumer.GetItems();
            TodoItem item = null;
            foreach(TodoItem i in list) {
                if (i.Name.ToLower() == nombre.ToLower()) {
                    item = i;
                }
            }
            if (item != null) {
                return item.ToJson();
            }
            return null;
        }

        static IPAddress getLocalIpAddress()
        {
            IPAddress ipAddress = null;
            try
            {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddress = addrInfo.Address;
                            }
                        }
                    }
                }
                if (ipAddress == null)
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                    ipAddress = ipHostInfo.AddressList[0];
                }
            }
            catch (Exception) { }
            return ipAddress;
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
            // //prueba
        }
    }
}