using System;
using System.IO;
using System.Collections.Generic;
using PlayerIOClient;

public class MasterMind
{
    const string GameId = "every-build-exists-d6aoqro023pzodrp9jhw";

    MMConfiguration configuration;

    Client client;
    Connection connection;
    EventHandler eventHandler;

    public MasterMind(string configurationFilePath)
        { LoadConfigurationFile(configurationFilePath);
        }

    public void MainLoop()
    {
        // What a beatiful "loop"
        Console.WriteLine(" [/] Press ENTER to stop.");
        Console.ReadLine();
    }

    public void Connect(string roomId)
    {
        if (client is not null)
            try
            {
                Console.WriteLine(" [?] Connecting...");

                connection = client.Multiplayer.CreateJoinRoom(
                    roomId,
                    "public",
                    true,
                    null,
                    null
                );

                Console.WriteLine(" [*] Connected!");

                eventHandler = new EventHandler(connection, configuration);

                eventHandler.Manage();

                Console.WriteLine(" [*] Events registered, all good!");

                return;
            }
            catch (PlayerIOError)
            {
                Console.WriteLine($" [-] Couldn't connect: Check your world ID / Internet Connection.");

                return;
            }

        Console.WriteLine(" [-] Couldn't connect: No client logged in.");
    }

    public void Login()
    {
        if (configuration is not null)
            try
            {
                Console.WriteLine(" [?] Logging in...");

                client = PlayerIO.Authenticate(
                    GameId, 
                    "public",
                    this.configuration.Auth,
                    null
                );

                Console.WriteLine(" [*] Logged in!");

                return;
            }
            catch (PlayerIOError error)
            {
                Console.WriteLine($" [-] Couldn't login: {error.Message}.");

                return;
            }

        Console.WriteLine(" [-] Couldn't login: No configuration given.");
    }

    public void Logout()
    {
        Console.WriteLine(" [?] Logging out...");

        if (client is not null)
        {
            client.Logout();

            Console.WriteLine(" [*] Logged out!");

            return;
        }

        Console.WriteLine(" [/] No client logged in.");
    }

    public void LoadConfigurationFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            Console.WriteLine(" [?] Loading configuration file...");

            configuration = MMParser.Parse(File.ReadAllText(filePath));

            Console.WriteLine(" [*] Configuration Loaded!");

            return;
        }
        
        Console.WriteLine($" [-] Configuration file '{filePath}' does not exist.");
    }
}
