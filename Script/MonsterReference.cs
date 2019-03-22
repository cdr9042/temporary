using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReference : MonoBehaviour
{
    public List<GameObject> m_Data;
    public Dictionary<int, GameObject> dict_Data;

    public static MonsterReference instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        FunctionHelper.SetRootThenDontDestroy(this.gameObject);
    }

    private void Start()
    {
        dict_Data = new Dictionary<int, GameObject>();
        for (int i = 0; i < m_Data.Count; i++)
        {
            GameObject t_e = m_Data[i];
            var id = t_e.GetComponent<EnemyScript>().m_Data.id;
#if UNITY_EDITOR
            if (dict_Data.ContainsKey(id))
            {
                Debug.Log("mr already contain " + id);
                Debug.Log(t_e);
            }
            else
#endif
                dict_Data.Add(id, t_e);
        }
        m_Data = null;
    }

    public GameObject GetData(int id)
    {
        GameObject r;
        if (!dict_Data.TryGetValue(id, out r))
        {
            Debug.LogError("no monster with that id " + id);
            return null;
        };
        return r;
    }
}
