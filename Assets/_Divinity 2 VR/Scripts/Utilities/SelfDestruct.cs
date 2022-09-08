using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 1f;

    private void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}