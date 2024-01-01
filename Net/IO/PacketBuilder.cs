using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Chat.Net.IO
{
    class PacketBuilder
    {
        MemoryStream ms;

        public PacketBuilder()
        {
            ms = new MemoryStream();
        }

        public void EscritaOpCode(byte opcode) {
            ms.WriteByte(opcode);
        }

        public void EscritaString(string mensagem)
        {
            // Converta a string para bytes usando UTF-8
            byte[] mensagemBytes = Encoding.UTF8.GetBytes(mensagem);

            // Escreva o tamanho da mensagem como um inteiro de 4 bytes (usando little-endian)
            ms.Write(BitConverter.GetBytes(mensagemBytes.Length), 0, 4);

            // Escreva os bytes da mensagem
            ms.Write(mensagemBytes, 0, mensagemBytes.Length);
        }


        public byte[] GetPackedBytes()
        {
            return ms.ToArray();
        }
    }
}
