using UnityEngine;
using System.Collections.Generic;

public class GameObjectActivator : MonoBehaviour
{

    public List<GameObject> gameObjects;

    public void Toggle(bool active)
    {
        foreach (var g in gameObjects)
        {
            g.SetActive(!active);
        }
    }
}
