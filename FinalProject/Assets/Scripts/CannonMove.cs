using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonMove : MonoBehaviour
{
    bool canFire;

    [SerializeField]
    int ammoCount;

    [SerializeField]
    GameObject cannonBall;

    [SerializeField]
    GameObject particle;
    // Start is called before the first frame update

    bool isGameOver;
    void Start()
    {
        canFire = true;
        isGameOver = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetKeyDown(KeyCode.S)))
        {
            if (transform.position.y >= 4) //limit downward movement
            {
                transform.position += Vector3.down;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown(KeyCode.W)))
        {
            if (transform.position.y <= 23) //limit upward movement
            {
                transform.position += Vector3.up;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canFire)
            {
                if(ammoCount > 0)
                {
                    Vector3 pos = new Vector3(transform.position.x +1f, transform.position.y+1f, transform.position.z);
                    Instantiate(cannonBall, pos, Quaternion.identity);
                    Instantiate(particle, new Vector3(transform.position.x + 1f, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                    canFire = false;
                    ammoCount--;
                    StartCoroutine("FireRate");
                }
                    
            }
        }

        if (ammoCount == 0)
        {
            StartCoroutine("GameOver"); //starts a game over check
        }
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSecondsRealtime(2f);
        canFire = true;
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSecondsRealtime(10f);
        isGameOver = true;
        
    }

    public int getAmmoCount()
    {
        return ammoCount;
    }

    public void addAmmo()
    {
        ammoCount++;
        StopCoroutine("GameOver"); //stops the game over check in case you had 0 shots and gained one
    }

    public bool getGameStatus()
    {
        return isGameOver;
    }
}
