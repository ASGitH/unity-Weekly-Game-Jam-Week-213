using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mono.Nat; //i don't know why this dosn't work
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;
using UDP;
public class Networking
{
    static INatDevice device;
    static byte[] data;
    public static bool isReady = false;
    static UDPSocket socket;
    static UDPSocket serverSocket;
    public static GridContainer gc;
    public static string ip;
    public static bool skipLobby;
    public static lobbyManagement lobby;
    public static IEnumerator Client()
    {
        if (!skipLobby)
        {
            socket = new UDPSocket();
            socket.Client(ip, 2700);
            //do the loby stuff here
        }

        //do the game stuff (send over player positions and grid state)
        while (true)
        {
            socket.Send(gc.destroyedGrid);
            yield return null;
        }
    }
    public static IEnumerator Server()
    {
        if (!skipLobby)
        {
            serverSocket = new UDPSocket();
            serverSocket.Server("127.0.0.1", 2700);
            //wait for players or start button to be pressed, minimum of 2 players
            UDPSocket.clients
            skipLobby = true;
        }
        //do the game stuff (send over player positions and grid state)
        while (true)
        {
            serverSocket.SendServer(gc.destroyedGrid);

            yield return null;
        }
    }
    private static void DeviceFound(object sender, DeviceEventArgs args)
    {
        device = args.Device;
        device.CreatePortMap(new Mapping(Protocol.Udp, 2700, 2700));
        Debug.Log(device.GetExternalIP().ToString());
        var mapings = device.GetAllMappings();
        foreach(var maping in mapings)
        {
            Debug.Log(maping);
        }
        // on device found code
    }
    private static void DeviceLost(object sender, DeviceEventArgs args)
    {
        Debug.Log("dead");
        //device = args.Device;
        //device.DeletePortMap(new Mapping(Protocol.Udp, 2700, 2700));
        // on device disconnect code
    }
}
