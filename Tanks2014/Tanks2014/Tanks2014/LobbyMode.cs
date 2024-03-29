using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using System.IO;

namespace Tanks2014
{
    class LobbyMode : Mode
    {
        private Int32 port = 1337;
        private string server = System.IO.File.ReadAllText(@"server.dat").Trim();
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader sReader;
        private string message;

        public LobbyMode(TanksGame host) : base(host)
        {
            initialize();
        }

        private void initialize()
        {
            message = "Initializing network connection";
            try
            {
                client = new TcpClient(server, port);
                stream = client.GetStream();
                sReader = new StreamReader(stream, Encoding.ASCII);
            } catch (Exception e)
            {
                Console.WriteLine(e);
                message = "Initialization failed";
            }
        }

        public override void update(GameTime gameTime)
        {
            try
            {
                if (stream.DataAvailable)
                {
                    string line = sReader.ReadLine();
                    int players = int.Parse(line.Trim().Split(' ')[0]);
                    int playersTotal = int.Parse(line.Trim().Split(' ')[1]);
                    if(players != playersTotal)
                    {
                        message = "Waiting for " + (playersTotal - players) + " more players";
                    } else
                    {
                        string data = sReader.ReadToEnd();
                        stream.Close();
                        client.Close();
                        host.setMode(new ClientMode(host,data));
                    }
                }
            } catch (Exception)
            {
                message = "Could not connect to server. Reconnecting...";
                initialize();
            }
        }

        public override void draw(GameTime gameTime, Drawer drawer)
        {           
            drawer.drawText(0,0,message);
        }
    }
}
