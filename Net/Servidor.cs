using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Chat.Net.IO;

namespace Chat.Net
{
    class Servidor
    {
        TcpClient cliente;
        public PacketReader PacketReader;

        public event Action conexaoEvento;
        public event Action mensagemEvento;
        public event Action usuariodesconectadoEvento;

        public Servidor()
        {
            cliente = new TcpClient();
        }

        public void ConectaraoServidor(String Usuario)
        {
            if (!cliente.Connected)
            {
                cliente.Connect("127.0.0.1", 8080);
                PacketReader = new PacketReader(cliente.GetStream());
                if (!string.IsNullOrEmpty(Usuario))
                {
                    var packetConexao = new PacketBuilder();
                    packetConexao.EscritaOpCode(0);
                    packetConexao.EscritaString(Usuario);
                    cliente.Client.Send(packetConexao.GetPackedBytes());
                }
                LerPackets();
            }
        }

        private void LerPackets()
        {
            Task.Run(() =>
                {
                    while (true)
                    {
                        var opcode = PacketReader.ReadByte();
                        switch (opcode)
                        {
                            case 1:
                                conexaoEvento?.Invoke();
                                break;

                            case 5:
                                mensagemEvento?.Invoke();
                                break;

                            case 10:
                                usuariodesconectadoEvento?.Invoke();
                                break;

                            default:
                                Console.WriteLine("Nada a ocorrer.");
                                break;
                        }
                    }
                });
        }

        public void MandarMensagemparaServidor(string mensagem)
        {
            var packetMensagem = new PacketBuilder();
            packetMensagem.EscritaOpCode(5);
            packetMensagem.EscritaString(mensagem);
            cliente.Client.Send(packetMensagem.GetPackedBytes());
        }
    }
}
