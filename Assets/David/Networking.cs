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
    public static Rigidbody dummyPlayers;
    public static int playerCount = 1;
    public static IEnumerator Client()
    {
        if (!skipLobby)
        {
            socket = new UDPSocket();
            socket.Client(ip, 2700);
        }
        while (!skipLobby)
        {
            socket.Send(new byte[] { 0xE9, 0x26 });
            //do the loby stuff here
            if (UDPSocket.lastPacket != null && UDPSocket.lastPacket.ContainsKey(ip) && UDPSocket.lastPacket[ip].Length >= 2 && UDPSocket.lastPacket[ip][0] == 0xE6 && UDPSocket.lastPacket[ip][1] == 0x21) //if the server sends you the magic number
            {
                lobby.startGame(lobby.gameObject);
            }
            yield return null;
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
            while (!skipLobby)
            {
                if (UDPSocket.clients.Count > playerCount)
                {
                    //get player names and increase player count, except we don't do that right now
                    playerCount = UDPSocket.clients.Count;
                }
                yield return null;
            }
        }
        serverSocket.SendServer(new byte[] { 0xE6, 0x21 });
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
