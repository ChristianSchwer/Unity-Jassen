using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NextCard : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public GameObject DropZone;

    string player;
    string firstLetter;
    string trumpf;
    int move;

    public GameObject PlayerArea1;
    public GameObject PlayerArea2;
    public GameObject PlayerArea3;
    public GameObject PlayerArea4;

    List<GameObject> player1 = new List<GameObject>();
    List<GameObject> player2 = new List<GameObject>();
    List<GameObject> player3 = new List<GameObject>();
    List<GameObject> player4 = new List<GameObject>();

    //TurnState state;          Enum over all scripts, dosn't work

    public GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        move = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;
            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            List<RaycastResult> memory = new List<RaycastResult>();

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                memory.Add(result);
            }
            foreach (RaycastResult result in memory)
            {
                if (player == "player1")
                {
                    if (result.gameObject.layer == 9)
                    {
                        foreach (RaycastResult i in memory)
                        {
                            if (move == 1)
                            {
                                if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                {
                                    i.gameObject.transform.SetParent(DropZone.transform, false);
                                    i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                    firstLetter = i.gameObject.GetComponent<PlayCard>().unitName.Substring(0, 1);
                                    player = "None";
                                    move = 2;
                                    player1.Remove(i.gameObject);
                                    gameManager.Player2Turn();
                                }
                            }
                            else
                            {
                                bool check = CheckList(player1, firstLetter);
                                if (check == false)
                                {
                                    if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player1.Remove(i.gameObject);
                                        gameManager.Player2Turn();
                                    }
                                }
                                else
                                {
                                    if (i.gameObject.name.Contains(firstLetter) || i.gameObject.name.Contains(trumpf))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player1.Remove(i.gameObject);
                                        gameManager.Player2Turn();
                                    }
                                }
                            }
                        }
                    }
                }
                if (player == "player2")
                {
                    if (result.gameObject.layer == 10)
                    {
                        foreach (RaycastResult i in memory)
                        {
                            if (move == 1)
                            {
                                if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                {
                                    i.gameObject.transform.SetParent(DropZone.transform, false);
                                    i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                    firstLetter = i.gameObject.GetComponent<PlayCard>().unitName.Substring(0, 1);
                                    player = "None";
                                    move = 2;
                                    player2.Remove(i.gameObject);
                                    gameManager.Player3Turn();
                                }
                            }
                            else
                            {
                                bool check = CheckList(player2, firstLetter);
                                if (check == false)
                                {
                                    if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player2.Remove(i.gameObject);
                                        gameManager.Player3Turn();
                                    }
                                }
                                else
                                {
                                    if (i.gameObject.name.Contains(firstLetter) || i.gameObject.name.Contains(trumpf))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player2.Remove(i.gameObject);
                                        gameManager.Player3Turn();
                                    }
                                }
                            }
                        }
                    }
                }
                if (player == "player3")
                {
                    if (result.gameObject.layer == 11)
                    {
                        foreach (RaycastResult i in memory)
                        {
                            if (move == 1)
                            {
                                if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                {
                                    i.gameObject.transform.SetParent(DropZone.transform, false);
                                    i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                    firstLetter = i.gameObject.GetComponent<PlayCard>().unitName.Substring(0, 1);
                                    player = "None";
                                    move = 2;
                                    player3.Remove(i.gameObject);
                                    gameManager.Player4Turn();
                                }
                            }
                            else
                            {
                                bool check = CheckList(player3, firstLetter);
                                if (check == false)
                                {
                                    if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player3.Remove(i.gameObject);
                                        gameManager.Player4Turn();
                                    }
                                }
                                else
                                {
                                    if (i.gameObject.name.Contains(firstLetter) || i.gameObject.name.Contains(trumpf))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player3.Remove(i.gameObject);
                                        gameManager.Player4Turn();
                                    }
                                }
                            }
                        }
                    }
                }
                if (player == "player4")
                {
                    if (result.gameObject.layer == 12)
                    {
                        foreach (RaycastResult i in memory)
                        {
                            if (move == 1)
                            {
                                if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                {
                                    i.gameObject.transform.SetParent(DropZone.transform, false);
                                    i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                    firstLetter = i.gameObject.GetComponent<PlayCard>().unitName.Substring(0, 1);
                                    player = "None";
                                    move = 2;
                                    player4.Remove(i.gameObject);
                                    gameManager.Player1Turn();
                                }
                            }
                            else
                            {
                                bool check = CheckList(player4, firstLetter);
                                if (check == false)
                                {
                                    if (i.gameObject.name.Contains("E") || i.gameObject.name.Contains("B") || i.gameObject.name.Contains("H") || i.gameObject.name.Contains("S"))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player4.Remove(i.gameObject);
                                        gameManager.Player1Turn();
                                    }
                                }
                                else
                                {
                                    if (i.gameObject.name.Contains(firstLetter) || i.gameObject.name.Contains(trumpf))
                                    {
                                        i.gameObject.transform.SetParent(DropZone.transform, false);
                                        i.gameObject.GetComponent<PlayCard>().SetPlayer(player);
                                        player = "None";
                                        player4.Remove(i.gameObject);
                                        gameManager.Player1Turn();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void nextCard(string turn)
    {
        if (turn == "player1")
        {
            player = "player1";
        }
        if (turn == "player2")
        {
            player = "player2";
        }
        if (turn == "player3")
        {
            player = "player3";
        }
        if (turn == "player4")
        {
            player = "player4";
        }
    }

    public void FirstPlayer(int first)
    {
        if (first == 1)
        {
            move = 1;
        }
    }

    public void Trumpf(string newtrumpf)
    {
        trumpf = newtrumpf;
    }

    public void GetCards()
    {
        for (int i = 0; i < PlayerArea1.transform.childCount; i++)
        {
            GameObject child = PlayerArea1.transform.GetChild(i).gameObject;
            player1.Add(child);
        }
        for (int i = 0; i < PlayerArea2.transform.childCount; i++)
        {
            GameObject child = PlayerArea2.transform.GetChild(i).gameObject;
            player2.Add(child);
        }
        for (int i = 0; i < PlayerArea3.transform.childCount; i++)
        {
            GameObject child = PlayerArea3.transform.GetChild(i).gameObject;
            player3.Add(child);
        }
        for (int i = 0; i < PlayerArea4.transform.childCount; i++)
        {
            GameObject child = PlayerArea4.transform.GetChild(i).gameObject;
            player4.Add(child);
        }
        gameManager.Player1Turn();
    }

    bool CheckList(List<GameObject> cards, string Letter)
    {
        //Check if firstLetter in List
        List<GameObject> letterCards = new List<GameObject>();
        foreach (GameObject card in cards)
        {
            if (card.GetComponent<PlayCard>().unitName.Substring(0, 1) == Letter)
            {
                letterCards.Add(card);
            }
        }
        if (letterCards.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
