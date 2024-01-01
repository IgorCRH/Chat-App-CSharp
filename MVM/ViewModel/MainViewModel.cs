using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Chat.MVM.Core;
using Chat.Net;
using Chat.MVM.Model;

namespace Chat.MVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UsuarioModel> usuarios { get; set; }
        public ObservableCollection<string> mensagens { get; set; }
        public Comando comandoConectaraoServidor { get; set; }
        public Comando comandoMandarMensagemparaServidor { get; set; }
        public String Usuario { get; set; }
        public String Mensagem { get; set; }
        private Servidor servidor;
        public MainViewModel()
        {
            usuarios = new ObservableCollection<UsuarioModel>();
            mensagens = new ObservableCollection<string>();
            servidor = new Servidor();
            servidor.conexaoEvento += UsuarioConectado;
            servidor.mensagemEvento += MensagemRecebida;
            servidor.usuariodesconectadoEvento += UsuarioDesconectado;
            comandoConectaraoServidor = new Comando(o => servidor.ConectaraoServidor(Usuario), o => !string.IsNullOrEmpty(Usuario));
            comandoMandarMensagemparaServidor = new Comando(o => servidor.MandarMensagemparaServidor(Mensagem), o => !string.IsNullOrEmpty(Mensagem));
        }

        private void UsuarioConectado()
        {
            var usuario = new UsuarioModel
            {
                Usuario = servidor.PacketReader.LeituraMensagem(),
                ID = servidor.PacketReader.LeituraMensagem(),
        };

         if(!usuarios.Any(x => x.ID == usuario.ID))
            {
                Application.Current.Dispatcher.Invoke(() => usuarios.Add(usuario));
            }
        }

        private void MensagemRecebida()
        {
            var mensagem = servidor.PacketReader.LeituraMensagem();
            Application.Current.Dispatcher.Invoke(() => mensagens.Add(mensagem));
        }

        private void UsuarioDesconectado()
        {
            var id = servidor.PacketReader.LeituraMensagem();
            var usuario = usuarios.Where(x => x.ID == id).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => usuarios.Remove(usuario));
        }
    }
}
