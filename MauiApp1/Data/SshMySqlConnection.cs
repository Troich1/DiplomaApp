using MySqlConnector;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Data;
public class SshMySqlConnection
{
    private readonly string _sshHost;
    private readonly string _sshUsername;
    private readonly string _sshPassword;
    private readonly string _mysqlHost;
    private readonly string _mysqlUsername;
    private readonly string _mysqlPassword;

    public SshMySqlConnection(string sshHost, string sshUsername, string sshPassword,
                               string mysqlHost, string mysqlUsername, string mysqlPassword)
    {
        _sshHost = sshHost;
        _sshUsername = sshUsername;
        _sshPassword = sshPassword;
        _mysqlHost = mysqlHost;
        _mysqlUsername = mysqlUsername;
        _mysqlPassword = mysqlPassword;
    }

    public MySqlConnection GetConnection()
    {
        using var client = new SshClient(_sshHost, _sshUsername, _sshPassword);
        client.Connect();

        var portForwarded = new ForwardedPortLocal("127.0.0.1", 3306, _mysqlHost, 3306);
        client.AddForwardedPort(portForwarded);
        portForwarded.Start();

        var connectionString = $"Server=127.0.0.1;Port={portForwarded.BoundPort};Database=YourDatabase;Uid={_mysqlUsername};Pwd={_mysqlPassword};";
        return new MySqlConnection(connectionString);
    }
}
