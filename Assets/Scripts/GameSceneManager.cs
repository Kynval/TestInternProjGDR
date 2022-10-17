using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FieldGenerator))]
public class GameSceneManager : MonoBehaviour
{
    TMPro.TMP_Text coinsCountTMP;
    TMPro.TMP_Text gameFinalTMP;
    Button MenuBtn;
    GameObject WonMenu;

    Coins coins;

    void Awake()
    {
        Application.targetFrameRate = 30;

        coins = new Coins(GetComponent<FieldGenerator>());
    }

    void Start()
    {
        WonMenu = FindObjectsOfType<GameObject>(true).FirstOrDefault(x => x.name == "WonMenu");
        MenuBtn = FindObjectsOfType<Button>(true).FirstOrDefault(x => x.name == "MenuBtn");
        gameFinalTMP = FindObjectsOfType<TMPro.TMP_Text>(true).FirstOrDefault(x => x.name == "InfoText");

        coinsCountTMP = FindObjectsOfType<TMPro.TMP_Text>(true).FirstOrDefault(x => x.name == "CoinsCountTmp");
        coinsCountTMP.text = $"0/{coins.GeneratedCoins}";
    }
    public bool EndGamePref(string endGameText)
    {
        WonMenu.SetActive(true);
        MenuBtn.gameObject.SetActive(false);
        gameFinalTMP.text = endGameText;

        return true;
    }

    public void PlaySound(AudioSource source, AudioClip[] soundList)
    {
        source.clip = soundList[UnityEngine.Random.Range(0, soundList.Length)];
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.volume = UnityEngine.Random.Range(0.3f, 0.5f);
        source.Play();
    }

    public bool AddedCoin()
    {
        coins.AddCoin();
        coinsCountTMP.text = coins.GetCoinsCount().ToString() + "/" + coins.GeneratedCoins;

        return coins.GetCoinsCount() == coins.GeneratedCoins;
    }

    public class Coins
    {
        int CoinsCount;
        public int GeneratedCoins { get; }
        public Coins(FieldGenerator generator)
        {
            CoinsCount = 0;
            GeneratedCoins = generator.Initialize();
        }
        public void AddCoin()
        {
            CoinsCount++;
        }
        public int GetCoinsCount()
        {
            return CoinsCount;
        }
    }
}
