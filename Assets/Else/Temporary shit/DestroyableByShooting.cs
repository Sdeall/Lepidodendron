using UnityEngine;

public class DestroyableByShooting : MonoBehaviour, Shootable
{
    [SerializeField] int life;
    public void Shoot(int damage)
    {
        life -= damage;
        if(life <= 0)
            Destroy(gameObject);
    }
}
