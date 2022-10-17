using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class _usrDontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] GameObject obj;
    public void Awake()
    {
        if (FindObjectsOfType(obj.GetType(), true).Where(x => x.name == obj.name).Count() > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
