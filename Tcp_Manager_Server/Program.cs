



using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Tcp_Manager_Server;

var ip = IPAddress.Loopback;
var port = 27001;

var listener = new TcpListener(ip, port);

listener.Start();
Console.WriteLine($"Server started [{ip}:{port}]");
while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var br = new BinaryReader(stream);
    var bw = new BinaryWriter(stream);
    while (true)
    {
        try
        {
            var input = br.ReadString();
            var command = JsonSerializer.Deserialize<Command>(input);

            if (command is null) continue;
            Console.WriteLine(command.Text);
            Console.WriteLine(command.Param);
            switch (command.Text)
            {
                case Command.ProcessList:
                    var processes = Process.GetProcesses();
                    var processesNames = JsonSerializer.Serialize(processes.Select(p => p.ProcessName));
                    bw.Write(processesNames);
                    break;
                case Command.Run:
                    string processName = command.Param!;
                    Process.Start(processName);
                    break;
                case Command.Kill:
                    string KilledProcessName = command.Param!;
                    var Killedprocesses = Process.GetProcessesByName(KilledProcessName).ToList();
                    Killedprocesses[0].Kill();
                    break;
                default:
                    break;
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message, ex.ToString());
            Console.WriteLine("Server stopped");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR : " + ex.Message);
        }
    }
}