using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [SerializeField]
    GameObject cannon;

    public GameObject[] castleObjects;
    public List<GameObject> castleRemaining;

    public GameObject[] triggerHulls;

    [SerializeField]
    Text playerInfo;

    [SerializeField]
    Text gameState;

    [SerializeField]
    int winAmount;

    [SerializeField]
    GameObject fireworks1;

    [SerializeField]
    GameObject fireworks2;

    [SerializeField]
    GameObject fireworks3;

    [SerializeField]
    GameObject fireworks4;

    bool listCreated = false;

    bool alreadyEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
        for (int i = 0; i < triggerHulls.Length; i++)
        {
            triggerHulls[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!listCreated)
        {
            listCreated = true;
            InitCastle();//tracks how many pieces of the castle are left
        }
        playerInfo.text = "Shots remaining: " + cannon.GetComponent<CannonMove>().getAmmoCount() + "\n";
        playerInfo.text += "OBB-OBB and AABB-OBB is shown if you win (Collisions make fireworks)" + "\n";
        playerInfo.text += "Use W/UpArrow to move the cannon up, S/Down to move the cannon down, Space to Shoot" + "\n";
        playerInfo.text += "Leave 5 or less parts of the castle standing to win" + "\n";
        playerInfo.text += "Sphere-Sphere response occurs when 2 cannonballs collide" + "\n";

        if (cannon.GetComponent<CannonMove>().getAmmoCount() == 0 && cannon.GetComponent<CannonMove>().getGameStatus())
        {
            if (castleRemaining.Count <= winAmount)
            {
                if (!alreadyEnabled) //only sets the win objects to be active once, otherwise they never disable
                {
                    alreadyEnabled = true;
                    for (int i = 0; i < triggerHulls.Length; i++)
                    {
                        triggerHulls[i].SetActive(true);
                    }
                }
                
                gameState.text = "You Win";
            }
            else
                gameState.text = "You Lose";
        }

    }

    void InitCastle()
    {
        castleRemaining = new List<GameObject>();
        for (int i = 0; i < castleObjects.Length; i++)
        {
            castleRemaining.Add(castleObjects[i]);
        }
    }

    public void removeCastlePart(GameObject castle)
    {
        for (int i = 0; i < castleRemaining.Count; i++)
        {
            if (castleRemaining[i] == castle)
            {
                castleRemaining.RemoveAt(i);
                break;
            }
        }
    }

    public void Fireworks1()
    {
        Instantiate(fireworks1, new Vector3(-6f, 20.18f, 5f), Quaternion.identity);
    }

    public void Fireworks2()
    {
        Instantiate(fireworks2, new Vector3(-6f, 23.37f, 5f), Quaternion.identity);
    }

    public void Fireworks3()
    {
        Instantiate(fireworks3, new Vector3(-6f, 25.96f, 5f), Quaternion.identity);
    }

    public void Fireworks4()
    {
        Instantiate(fireworks4, new Vector3(-6f, 17.51f, 5f), Quaternion.identity);
    }
}
