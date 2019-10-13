using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climat : MonoBehaviour
{
    #region Variables
    #region Editor
    [SerializeField]
    private static float
        timeBetweenFire = 17f,
        timeBetweenTornado = 12f,
        timeBetweenTempest = 23f;
    #endregion

    #region Private
    private float timeSinceLastFire;
    private float timeSinceLastTempest;
    private float timeSinceLastTornado;

    private Fire fire;
    private Tornado tornado;
    private Tempest tempest;
    #endregion


    #endregion

    // Start is called before the first frame update
    void Start()
    {

        timeSinceLastFire = 0f;
        timeSinceLastTempest = 0f;
        timeSinceLastTornado = 0f;
        fire = new Fire();
        tornado = new Tornado();
        tempest = new Tempest();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastFire += Time.deltaTime;
        timeSinceLastTempest += Time.deltaTime;
        timeSinceLastTornado += Time.deltaTime;

        if (timeSinceLastFire >= timeBetweenFire)
        {
            timeSinceLastFire = 0f;
            fire.Triggerhazard();
        }
        if (timeSinceLastTornado >= timeBetweenTornado)
        {
            timeSinceLastTornado = 0f;
            tornado.Triggerhazard();
        }
        if (timeSinceLastTempest >= timeBetweenTempest)
        {
            timeSinceLastTempest= 0f;
            tempest.Triggerhazard();
        }
    }
}