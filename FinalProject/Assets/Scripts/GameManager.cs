using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [SerializeField]
    GameObject cannon;

    [SerializeField]
    Text playerInfo;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerInfo.text = "Shots remaining: " + cannon.GetComponent<CannonMove>().getAmmoCount() + "\n";
        playerInfo.text += "Game will be a destroy the castle game, obb objects will serve as triggers for special powerups" +"\n";
        playerInfo.text += "Forces will be gravity,drag, the force from the cannon, and friction";

    }
}
