using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManagerPrefab;

    void Awake()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gameManagerPrefab);
        }
    }
}
