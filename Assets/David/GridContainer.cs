using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridContainer : MonoBehaviour
{
    [SerializeField]
    public Transform[] grid;
    [SerializeField]
    GameObject tilePrefab;
    [SerializeField]
    int size = 10;
    public byte[] destroyedGrid;
    bool gracePeriod = true;
    // Start is called before the first frame update
    IEnumerator grace()
    {
        yield return new WaitForSeconds(5);
        gracePeriod = false;
    }
    void Start()
    {
        StartCoroutine(grace());
        destroyedGrid = new byte[size * size];
        if (!Application.isPlaying)
        {
            if (grid.Length < size * size)
            {
                for (int i = 0; i < grid.Length; i++)
                {
                    GameObject.DestroyImmediate(grid[i]);
                }
                grid = new Transform[size * size];
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        var go = GameObject.Instantiate(tilePrefab);
                        go.transform.position = new Vector3(x, 0, y);
                        grid[x + y * size] = go.transform;
                    }
                }
            }
        } else
        {
            Networking.gc = this;
            
        }
    }
    void Update()
    {
        if (Application.isPlaying)
        {
            for(int i = 0; i < size*size; i++)
            {
                if(destroyedGrid[i]==0 && UDP.UDPSocket.lastPacket != null && (UDP.UDPSocket.lastPacket[i]!=0))
                {
                    destroyTile(i);
                }
            }
        }

    }
    public void destroyTile(int x, int y)
    {
        if (gracePeriod) return;
        //find the tile in the grid
        //start co routine to animated it's demise
        if (x < size && y < size && x >= 0 && y >= 0)
        {
            destroyedGrid[x + y * size] = 1;
            StartCoroutine(TileDeath(grid[x + y * size]));
        }
    }
    public void destroyTile(int i)
    {
        //find the tile in the grid
        //start co routine to animated it's demise
        if (i < size*size)
        {
            destroyedGrid[i] = 1;
            StartCoroutine(TileDeath(grid[i]));
        }
    }
    IEnumerator TileDeath(Transform t)
    {

        float velocity = 0;
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 130; i++)
        {
            velocity -= 9.83f*Time.deltaTime;//gravity
            //Debug.Log(velocity);
            t.Translate(new Vector3(0, velocity*Time.deltaTime, 0));
            yield return null;
        }
        t.gameObject.SetActive(false);
    }
}
