using UnityEngine;

public class Enemy : ActorCore
{
    private void Update()
    {
        if (DetermineIfDead())
        {
            Debug.Log("Enemy is dead");
        }
    }
}
