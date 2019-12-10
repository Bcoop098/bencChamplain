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

    [SerializeField]
    Text playerInfo;

    bool listCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!listCreated)
        {
            listCreated = true;
            InitCastle();
        }
        playerInfo.text = "Shots remaining: " + cannon.GetComponent<CannonMove>().getAmmoCount() + "\n";
        playerInfo.text += "Game will be a destroy the castle game, obb objects will serve as triggers for special powerups" +"\n";
        playerInfo.text += "Forces will be gravity,drag, the force from the cannon, and friction";

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
}
