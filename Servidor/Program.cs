﻿using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;


namespace servidorsincrono
{
    public class SynchronousSocketListener
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
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.127"), 11000);
                
                //socket de espera a conexiones
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //asignar al socket la ip del servidor y el puerto 
                listener.Bind(localEndPoint);

                //numero maximo de conexiones en espera (si ya existe conexion)
                listener.Listen(10);

                //MAP CON USUARIOS Y CONTRASEÑAS ------------------------------------
                Dictionary<string, string> usuarios = new Dictionary<string, string>();
                usuarios["usuario1"] = "11111";
                usuarios["usuario2"] = "22222";
                usuarios["usuario3"] = "33333";
                usuarios["usuario4"] = "44444";
                //-------------------------------------------------------------------

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
                // -------------------------------- recibir del cliente ---------------------------------------
                        //almacenar datos
                        string data = "";

                        //para recibir datos del cliente
                        byte[] bytes = null;

                        //bucle para solo un mensaje
                        while (true)
                        {
                            bytes = new byte[1024];

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

                        // if (data == "") {
                        //     //dejamos de recibir mensajes
                        //     return;
                        // }

                        Console.Write("{0} envio:\n nombreDeArchivo: {1}", handler.RemoteEndPoint, data);

                        //mensaje a enviar al cliente
                        string mensaje = "";
                        string mensajeSrv = "";

                        if (estaArchivoEnServicio(data)) {
                            //enviamos el mensaje de exito y el archivo

                            //mensaje
                            mensaje += "existe";
                            mensaje += "*****";
                            
                            //archivo


                            //srv
                            mensajeSrv += "-----> EXISTE";

                        } else {
                            //enviamos el mensaje de error y null
                            mensaje += "no existe";
                            mensaje += "*****";

                            //archivo
                            mensaje += "null";

                            //srv
                            mensajeSrv += "-----> NO existe";
                        }

                        Console.WriteLine(mensajeSrv);
                        //enviamos al cliente el mensaje
                        byte[] msgToClient = Encoding.ASCII.GetBytes(mensaje);
                        handler.Send(msgToClient);


                        //recibir mensaje
                        // int br = handler.Receive(bytes);
                        // if (Encoding.ASCII.GetString(bytes, 0, br) == "salir") {
                        //     break;
                        // }
                        
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

        //verifica si un archivo esta en el servicio
        static bool estaArchivoEnServicio(string nombrearchivo) {
            return true;
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
        }
    }
}
// > dotnet run
// Waiting for a connection...
// Text received : This is a test
// Waiting for a connection...
//