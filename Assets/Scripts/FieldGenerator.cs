using System.Collections.Generic;
using UnityEngine;


public class FieldGenerator : MonoBehaviour
{
    [SerializeField] GameObject spikeObj;
    [SerializeField] GameObject coinObj;
    [SerializeField] GameObject playerObj;

    int heightBorder = 9;
    int widthBorder = 5;  // 16:9

    [SerializeField] int minSpikes = 10;
    [SerializeField] int maxSpikes = 20;

    [SerializeField] int minCoins = 2;
    [SerializeField] int maxCoins = 8;

    List<Vector3> matchedList;

    int maxObjNum;

    public int Initialize()
    {
        matchedList = new List<Vector3>();

        if (Camera.main.aspect >= 0.44) // 20:9
            widthBorder--;

        maxObjNum = widthBorder * heightBorder * 4 + 1;

        Generating(minSpikes, maxSpikes, spikeObj);
        Generating(1, 1, playerObj, false);
        return Generating(minCoins, maxCoins, coinObj);
    }

    int Generating(int min, int max, GameObject obj, bool copy = true)
    {        
        int num = Random.Range(min % maxObjNum, max % maxObjNum);
        for (int i = 0; i < num; i++)
        {
            Vector3 objVector3;
            do
                objVector3 = new Vector3(
                   (float)Random.Range(-widthBorder, widthBorder),
                   (float)Random.Range(-heightBorder, heightBorder),
                   1f);
            while (matchedList.Contains(objVector3));

            matchedList.Add(objVector3);

            if (copy)
                Instantiate(obj, matchedList[matchedList.Count - 1], new Quaternion());
            else obj.transform.position = objVector3;
        }
        maxObjNum -= num;
        return num;
    }

}
