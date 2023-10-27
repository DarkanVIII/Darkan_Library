using UnityEngine;

public interface IPooled<T> where T : MonoBehaviour
{
    public event System.Action<T> OnReturnToPool;
}
