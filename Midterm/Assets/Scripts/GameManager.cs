using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    [SerializeField]
    Text text;

    [SerializeField]
    Text playerInfo;

    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject particle;

    private string winString = "You Win";
    private string loseString = "You Lose";

    public bool win = false;
    public bool lose = false;
    private bool alreadyDead = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (win)
        {
            text.text = winString;
        }
        if (lose)
        {
            text.text = loseString;
            if (alreadyDead == false)
            {
                GameObject explosion = Instantiate(particle, player.transform.position, Quaternion.identity);
                alreadyDead = true;
            }
        }

        playerInfo.text = "X Velocity: " + Mathf.Round(player.GetComponent<Particle2D>().velocity.x *1000) /1000 + "\n";
        playerInfo.text += "Y Velocity: " + Mathf.Round(player.GetComponent<Particle2D>().velocity.y * 1000) / 1000 + "\n";
        playerInfo.text += "Remaining Fuel: " + player.GetComponent<Particle2D>().fuel;


    }
}
