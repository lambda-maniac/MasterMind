using System;
using System.IO;
using System.Collections.Generic;
using PlayerIOClient;

string Input(string prompt)
    { Console.Write(prompt); return Console.ReadLine();
    }

MasterMind mm = new MasterMind(Input("Configuration file path: "));

mm.Login();
mm.Connect(Input("Room Id: "));
mm.MainLoop();
mm.Logout();
