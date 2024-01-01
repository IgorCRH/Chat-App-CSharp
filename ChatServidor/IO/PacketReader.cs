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
            // Leia o tamanho da mensagem como um inteiro de 4 bytes (usando little-endian)
            int tamanhoMensagem = BitConverter.ToInt32(ReadBytes(4), 0);

            // Certifique-se de que o tamanho da mensagem seja válido
            if (tamanhoMensagem < 0)
            {
                throw new InvalidOperationException("Tamanho de mensagem inválido.");
            }

            byte[] buffer = new byte[tamanhoMensagem];
            int totalBytesRead = 0;

            while (totalBytesRead < tamanhoMensagem)
            {
                int bytesRead = Read(buffer, totalBytesRead, tamanhoMensagem - totalBytesRead);

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
