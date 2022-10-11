using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
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

    public static int sceneCoins;

    private void Awake()
    {
        Application.targetFrameRate = 30;

        matchedList = new List<Vector3>();

        Debug.Log(Camera.main.aspect);

        if (Camera.main.aspect >= 0.44) // 20:9
            widthBorder--;

        Generating(minSpikes, maxSpikes, spikeObj);

        sceneCoins = Generating(minCoins, maxCoins, coinObj);

        Generating(1, 1, playerObj, false);
    }
    int Generating(int min, int max, GameObject obj, bool copy = true)
    {
        int maxObj = widthBorder * heightBorder;
        int num = Random.Range(min, max);
        for (int i = 0; i < num && matchedList.Count < maxObj; i++)
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
        return num;
    }
    public class Coins
    {
        int CoinsCount { get; set; }
        public Coins()
        {
            CoinsCount = 0;
        }
        public void AddCoin()
        {
            CoinsCount++;
        }
        public int GetCoinsCount()
        {
            return CoinsCount;
        }
        public int GetSceneCoins()
        {
            return sceneCoins;
        }
    }
}
