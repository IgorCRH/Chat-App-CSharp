using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Chat.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream ns;

        public PacketReader(NetworkStream ns) : base(ns)
        {
            this.ns = ns;
        }

        public string LeituraMensagem()
        {
            int tamanhoMensagem = ReadInt32();  // Substitui LeituraInt32 pela função ReadInt32

            // Certifique-se de que o tamanho da mensagem seja válido
            if (tamanhoMensagem < 0)
            {
                throw new InvalidOperationException("Tamanho de mensagem inválido.");
            }

            byte[] buffer = new byte[tamanhoMensagem];
            int totalBytesRead = 0;

            while (totalBytesRead < tamanhoMensagem)
            {
                int bytesRead = ns.Read(buffer, totalBytesRead, tamanhoMensagem - totalBytesRead);  // Substitui stream por ns

                if (bytesRead == 0)
                {
                    // Se bytesRead for 0, significa que não há mais dados para ler
                    throw new InvalidOperationException("Não há mais dados para ler.");
                }

                totalBytesRead += bytesRead;
            }

            return Encoding.UTF8.GetString(buffer, 0, tamanhoMensagem);
        }
    }
}
