using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Message    = PlayerIOClient.Message;
using Connection = PlayerIOClient.Connection;

public class EventHandler
{
    Connection connection;
    
    List<Trail> trails;
    List<Structure> structures;

    public EventHandler(Connection connection, MMConfiguration configuration)
    {
        this.connection = connection;

        trails = configuration.Trails;
        structures = configuration.Structures;
    }

    public void Manage()
    {
        connection.OnMessage += this.OnMessage;
        connection.OnDisconnect += this.OnDisconnect;

        connection.Send("init");
    }

    void OnMessage(object sender, Message message)
    {
        if (message.Type == "init") { connection.Send("init2"); return; }

        if (message.Type == "b")
        {
            int  layer    = message.GetInt(0);
            uint x        = message.GetUInt(1);
            uint y        = message.GetUInt(2);
            uint blockId  = message.GetUInt(3);
            uint playerId = message.GetUInt(4);

            foreach (Trail trail in trails)
            {
                if (blockId == trail.Trigger)
                {
                    Task.Run( () => {
                        foreach (int newBlockId in trail.Body)
                        {
                            Thread.Sleep(trail.ms_Decay);
                            connection.Send("b", layer, x, y, newBlockId);
                        }
                    });
                }
            }

            foreach (Structure structure in structures)
            {
                if (blockId == structure.Trigger)
                {
                    Task.Run( () => {
                        foreach (Segment segment in structure.Segments)
                        {
                            int nx = (int) x + segment.XOffset;
                            int ny = (int) y + segment.YOffset;

                            if (nx < 0 || ny < 0) continue;

                            Task.Run( () => {
                                Thread.Sleep(segment.ms_Build);
                                connection.Send("b", layer, (uint) nx, (uint) ny, segment.Id);

                                if (segment.ms_Decay > 0)
                                {
                                    Thread.Sleep(segment.ms_Decay);
                                    connection.Send("b", layer, (uint) nx, (uint) ny, 0);
                                }
                            });
                        }
                    });
                }
            }
        }
    }

    void OnDisconnect(object sender, string reason)
    {
        Console.WriteLine($"Disconnected, reason: {reason}");
    }
}
