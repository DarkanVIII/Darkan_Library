using UnityEngine;

public interface IPoolable<T> where T : MonoBehaviour
{
    public event System.Action<T> OnReturnToPool;
}
