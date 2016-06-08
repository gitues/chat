using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

using WhatsAppApi;

namespace WebApplication1
{
    public class ChatHub : Hub
    {
        static WhatsApp wa;


        public void send(String to, string msg)
        {
            if (wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.LOGGEDIN)
            {
                wa.SendMessage(to,msg);
                Clients.All.notifyMessage(string.Format ("{0}: {1}",to,msg));
            }

        }

        public void login(string phoneNumber, string pass)
        {
            Thread thread = new Thread(t =>
                {
                    wa = new WhatsApp(phoneNumber, pass, phoneNumber, true);
                    wa.OnConnectSuccess += () =>
                        {
                            Clients.All.notifyMessage("Conectado ...");

                            wa.OnLoginSuccess += (phone, data) =>
                                {
                                    Clients.All.notifyMessage("Logueo exitoso ...");
                                };

                            wa.OnLoginFailed += (data) =>
                                {
                                    Clients.All.notifyMessage(string.Format("Fallo en login {0}", data));
                                };
                            wa.Login();
                        };

                    wa.OnConnectFailed += (ex) =>
                    {
                        Clients.All.notifyMessage(string.Format("Fallo de conexión {0}", ex.StackTrace));
                    };

                    wa.Connect();
                }) {IsBackground = true  };
            thread.Start(); 
        }   
    }
}