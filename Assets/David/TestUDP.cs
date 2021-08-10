using System.Collections;
using System.Collections.Generic;
using UDP;
using UnityEngine;

public class TestUDP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UDPSocket s = new UDPSocket();
        s.Server("127.0.0.1", 27000);

        UDPSocket c = new UDPSocket();
        c.Client("127.0.0.1", 27000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
