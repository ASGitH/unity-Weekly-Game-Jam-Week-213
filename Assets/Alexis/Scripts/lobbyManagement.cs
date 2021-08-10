using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  

public class lobbyManagement : MonoBehaviour
{
    private bool hasHostedGame = false, hasJoinedGame = false;

    private int playerCount = 0;

    public Button cancelButton;

    public GameObject ipAddress;
    public GameObject players;

    void Start() { }

    void Update() { }

    private void addPlayer()
    {
        if(playerCount < 4) 
        {
            players.transform.GetChild(playerCount).gameObject.SetActive(true);

            playerCount += 1; 
        }
    }

    public void cancel(GameObject _parent)
    {
        cancelButton.gameObject.SetActive(false);

        if (!hasHostedGame) 
        {
            hasJoinedGame = false;

            // playerCount -= 1; 
        }
        else 
        {
            hasHostedGame = false;
            
            playerCount = 0;

            players.transform.GetChild(0).gameObject.SetActive(false);
            players.transform.GetChild(1).gameObject.SetActive(false);
            players.transform.GetChild(2).gameObject.SetActive(false);
            players.transform.GetChild(3).gameObject.SetActive(false);
        }

        ipAddress.SetActive(false);

        _parent.SetActive(true);
    }

    public void hostGame(GameObject _parent)
    {
        cancelButton.gameObject.SetActive(true);

        hasHostedGame = true;

        ipAddress.SetActive(true);
        ipAddress.transform.GetChild(1).gameObject.SetActive(false);

        _parent.SetActive(false);

        players.SetActive(true);

        addPlayer();

        players.transform.GetChild(1).gameObject.SetActive(false);
        players.transform.GetChild(2).gameObject.SetActive(false);
        players.transform.GetChild(3).gameObject.SetActive(false);
    }

    public void joinGame(GameObject _parent)
    {
        // addPlayer();

        cancelButton.gameObject.SetActive(true);

        hasJoinedGame = true;

        ipAddress.SetActive(true);
        ipAddress.transform.GetChild(1).gameObject.SetActive(true);

        _parent.SetActive(false);
    }

    public void updateName(TextMeshProUGUI _name) { _name.text = GetComponent<TextMeshProUGUI>().text; }
}
