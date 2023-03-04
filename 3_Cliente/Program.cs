using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace clientesincrono {
    public class SynchronousSocketClient {

        public static void StartClient() {
            try {
//************************ SI NO TE DA, PUEDES PARSEAR TU IP ***************************
                // IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("TUIP"), 11000);
//**************************************************************************************

                //conexiones
                IPAddress ipAddress = getLocalIpAddress();//MAC OS
                // IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.1.127"), 11000);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.1.58"), 11000);
                
                //socket para comunicarse con el servidor
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try {
                    //conectar con la ip y puerto del servidor
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                    Console.WriteLine("Bienvenido a nuestra aplicacion");

                    while (true) {
                        //mostrar mensaje a cliente
                        Console.WriteLine("\n---------------------------");
                        Console.WriteLine("Debes teclear el nombre del archivo (SIN EXTENSION) que desas solicitar al servidor");
                        string nombreArchivo = Console.ReadLine();

                        //enviar peticion de archivo al servidor
                        byte[] msg = Encoding.ASCII.GetBytes(nombreArchivo + "<EOF>");
                        int bytesSent = sender.Send(msg);

                        //recibir archivo o mensaje de error
                        //formato de mensaje: 
                        //("existe" *****  archivo) o ("no existe" ***** null)
                        byte[] bytes = new byte[1024];
                        int byteRec = sender.Receive(bytes);
                        string respSrv = Encoding.ASCII.GetString(bytes, 0, byteRec);
                        
                        //parsear respuesta servidor
                        int posSep = respSrv.IndexOf("*****");
                        string strExiste = respSrv.Substring(0, posSep);//serializado
                        string strArchivo = respSrv.Substring(posSep + 5);
                        

                        if (strExiste == "NO EXISTE") {
                            Console.WriteLine("Este archivo {0} NO existe en el servidor\n", nombreArchivo);
                        } else {
                            Console.WriteLine("Este archivo {0} EXISTE en el servidor\n", nombreArchivo);
                            //recoger el archivo
                            Console.WriteLine("Contenido del archivo {0}.json: \n{1}\n", nombreArchivo, strArchivo);
                            
                        }

                        //preguntar si quiere seguir
                        Console.WriteLine("¿Deseas continuar con la aplicacion?");
                        Console.WriteLine("Teclea cualquier valor para continuar y 'n' para salir");
                        string seguir = Console.ReadLine();
                        
                        if (seguir.ToLower() == "n") {
                            //enviar mensaje para salir
                            // msg = Encoding.ASCII.GetBytes("salir");
                            // bytesSent = sender.Send(msg);
                            
                            break;
                        } 
                    }

                    //cerrar conexion
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                } catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                } catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                } catch (Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        static IPAddress getLocalIpAddress() {
            IPAddress ipAddress = null;
            try {
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces()) {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet) {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses) {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork) {
                                ipAddress = addrInfo.Address;
                            }
                        }
                    }
                }
                if (ipAddress == null) {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
                    ipAddress = ipHostInfo.AddressList[0];
                }
            } catch (Exception) { }
            return ipAddress;
        }

        public static int Main(String[] args) {
            //En MACOS porque no se puede ordenar el orden de arranque: servidor, cliente
            Thread.Sleep(1000);
            StartClient();
            return 0;
        }
    }
}
// > dotnet run
// Socket connected to 192.168.1.104:11000
// Echoed test = This is a test