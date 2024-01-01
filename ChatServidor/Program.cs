using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Chat.Net.IO;

namespace ChatServidor
{
    class Program
    {
        static List<Cliente> usuarios;
        static TcpListener escutador;

        static void Main(string[] args)
        {
            usuarios = new List<Cliente>();
            escutador = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            escutador.Start();
            Console.WriteLine("Cliente lançado.");

            // Inicie o loop para aceitar conexões continuamente
            while (true)
            {
                // Aguarde e aceite conexões de clientes de forma assíncrona
                TcpClient tcpClient = escutador.AcceptTcpClient();

                // Processar o cliente conectado
                var cliente = new Cliente(tcpClient);
                usuarios.Add(cliente);

                TransmitirConexao();
            }
        }

        static void TransmitirConexao()
        {
            foreach (var usuario in usuarios)
            {

                    var packetTransmissao = new PacketBuilder();
                    packetTransmissao.EscritaOpCode(1);
                    packetTransmissao.EscritaString(usuario.Usuario);
                    packetTransmissao.EscritaString(usuario.ID.ToString());
                    usuario.SocketCliente.Client.Send(packetTransmissao.GetPackedBytes());
                    Console.WriteLine("Conexao sendo iniciada");
            }
        }

        public static void TransmitirMensagem(string mensagem)
        {
            foreach (var usador in usuarios)
            {
                var packetMensagem = new PacketBuilder();
                packetMensagem.EscritaOpCode(5);
                packetMensagem.EscritaString(mensagem);
                usador.SocketCliente.Client.Send(packetMensagem.GetPackedBytes());
            }
        }

        public static void TransmitirMensagemDesconexao(string ID)
        {
            var usuarioDesconectado = usuarios.Find(x => x.ID.ToString() == ID);
            usuarios.Remove(usuarioDesconectado);

            foreach (var usador in usuarios)
            {
                var packetTransmissao = new PacketBuilder();
                packetTransmissao.EscritaOpCode(10);
                packetTransmissao.EscritaString(ID);
                usador.SocketCliente.Client.Send(packetTransmissao.GetPackedBytes());
            }

            TransmitirMensagem($"[{usuarioDesconectado.Usuario}] Desconectado!");
        }
    }
}
