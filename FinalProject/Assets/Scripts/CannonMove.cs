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
    void Start()
    {
        canFire = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetKeyDown(KeyCode.S)))
        {
            if (transform.position.y >= 4.8)
            {
                transform.position += Vector3.down;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetKeyDown(KeyCode.W)))
        {
            if (transform.position.y <= 16)
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
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSecondsRealtime(2f);
        canFire = true;
    }

    public int getAmmoCount()
    {
        return ammoCount;
    }

    public void addAmmo()
    {
        ammoCount++;
    }
}
