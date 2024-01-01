using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Chat.Net.IO;

namespace ChatServidor
{
    class Cliente
    {
        public string Usuario { get; set; }
        public Guid ID { get; set; }
        public TcpClient SocketCliente { get; set; }

        PacketReader packetLeitura;

        public Cliente(TcpClient cliente)
        {
            this.SocketCliente = cliente;
            this.ID = Guid.NewGuid();
            packetLeitura = new PacketReader(SocketCliente.GetStream());

            var opcode = packetLeitura.ReadByte();
            Usuario = packetLeitura.LeituraMensagem();

            Console.WriteLine($"[{DateTime.Now}]: Cliente conectou como usuario {Usuario}");

            Task.Run(() => Processar());
        }

        void Processar()
        {
            try
            {
                var opcode = packetLeitura.ReadByte();
                switch (opcode)
                {
                    case 5:
                        var mensagem = packetLeitura.LeituraMensagem();
                        Console.WriteLine($"[{DateTime.Now}]: Mensagem recebida {Usuario} : {mensagem}");
                        Program.TransmitirMensagem($"[{DateTime.Now}] - {Usuario} : {mensagem}");
                        break;
                    default:
                        Console.WriteLine("Nada a ocorrer.");
                        break;
                }
            }
            catch (Exception)
            {
                Program.TransmitirMensagemDesconexao(ID.ToString());
                SocketCliente.Close();
                throw;
            }
        }

    }
}
