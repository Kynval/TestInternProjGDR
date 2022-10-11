using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameSceneManager;

public class Player : MonoBehaviour
{
    public AudioSource playerSoundSource;

    [SerializeField] AudioClip[] blowSoundList;
    [SerializeField] AudioClip[] coinSoundList;

    Touch input;

    [SerializeField] static Rigidbody2D cachedRigidbody2D;
    [SerializeField] GameObject point;
    [SerializeField] GameObject field;

    LineRenderer cachedLineRenderer;

    ParticleSystem cachedParticleSystem;

    Animator cachedAnimator;

    Coins coins;

    List<Vector2> positionsList = new List<Vector2>();

    bool restart;

    TMPro.TMP_Text coinsCountTMP;
    TMPro.TMP_Text gameFinalTMP;
    Button MenuBtn;
    GameObject WonMenu;

    private void Start()
    {
        restart = false;

        cachedRigidbody2D = GetComponent<Rigidbody2D>();        
        cachedParticleSystem = GetComponent<ParticleSystem>();
        cachedAnimator = GetComponent<Animator>();

        cachedLineRenderer = GetComponent<LineRenderer>();
        cachedLineRenderer.positionCount = 1;
      
        WonMenu = FindObjectsOfType<GameObject>(true).FirstOrDefault(x => x.name == "WonMenu");
        MenuBtn = FindObjectsOfType<Button>(true).FirstOrDefault(x => x.name == "MenuBtn");
        gameFinalTMP = FindObjectsOfType<TMPro.TMP_Text>(true).FirstOrDefault(x => x.name == "InfoText");

        coins = new Coins();

        coinsCountTMP = FindObjectsOfType<TMPro.TMP_Text>(true).FirstOrDefault(x => x.name == "CoinsCountTmp");
        coinsCountTMP.text = coins.GetCoinsCount().ToString() + "/" + coins.GetSceneCoins();
    }  

    void Update()
    {
        if (InputHelper.GetTouches().Count > 0)
        {
            input = InputHelper.GetTouches()[0];

            if (input.phase == TouchPhase.Began && !restart)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(input.position.x, input.position.y));

                if (!EventSystem.current.IsPointerOverGameObject() && !IsPointerOverUIObject())
                {
                    Vector2 touchPosition = new Vector2(ray.origin.x, ray.origin.y);

                    Instantiate(point, touchPosition, new Quaternion());

                    positionsList.Add(touchPosition);

                    cachedLineRenderer.positionCount++;
                    cachedLineRenderer.SetPosition(cachedLineRenderer.positionCount - 1, positionsList.Last());

                    if (cachedRigidbody2D.velocity == Vector2.zero)
                        StartCoroutine(QueueMove(positionsList, cachedRigidbody2D));
                }
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch(collider.tag)
        {
            case "PointCollider":
                {
                    cachedRigidbody2D.velocity = Vector2.zero;

                    UnityEngine.Object.Destroy(collider.gameObject);
                    break;
                }
            case "Spike":
                {
                    EndGamePref();
                    gameFinalTMP.text = "You Lose..";

                    PlaySound(playerSoundSource, blowSoundList);

                    cachedAnimator.SetBool("GameOver", true);
                    cachedAnimator.Play("CircleBlow");
                    cachedParticleSystem.Play();
                  
                    UnityEngine.Object.Destroy(this.gameObject, 0.7f);
                    break;
                }
            case "Coin":
                {
                    collider.GetComponent<CircleCollider2D>().radius = 0;
                    collider.GetComponent<Animator>().SetBool("CoinDrop", true);

                    PlaySound(playerSoundSource, coinSoundList);

                    coins.AddCoin();
                    coinsCountTMP.text = coins.GetCoinsCount().ToString() + "/" + coins.GetSceneCoins();

                    UnityEngine.Object.Destroy(collider.gameObject, 1);
                    if (coins.GetSceneCoins() == coins.GetCoinsCount())
                    {
                        EndGamePref();
                        gameFinalTMP.text = "You Win!!";                                          
                    }
                    break;
                }
            default: Debug.Log("ER: Non of");
                break;
        }
    }

    private void EndGamePref()
    {
        restart = true;
        WonMenu.SetActive(true);
        MenuBtn.gameObject.SetActive(false);
        positionsList.Clear();
        cachedRigidbody2D.velocity = Vector2.zero;
    }

    IEnumerator QueueMove(List<Vector2> targetPositions, Rigidbody2D objRigidbody)
    {
        for (int i = 0; i < targetPositions.Count; i++)
        {
            Vector2 nOffset = targetPositions[i] - objRigidbody.position;

            objRigidbody.velocity = nOffset;

            do {
                cachedLineRenderer.SetPosition(i, objRigidbody.position);               
                yield return null;
            } while (objRigidbody.velocity != Vector2.zero);

            if (targetPositions.Count > 0)
            {
                cachedLineRenderer.SetPositions(targetPositions.Select(x => new Vector3(x.x, x.y, 0)).ToArray());
                cachedLineRenderer.positionCount = targetPositions.Count();
                targetPositions.RemoveAt(i--);         
            }
        }
        cachedLineRenderer.positionCount = 1;
        targetPositions.Clear();
    }

    void PlaySound(AudioSource source, AudioClip[] soundList)
    {
        source.clip = soundList[UnityEngine.Random.Range(0, soundList.Length)];
        source.pitch = UnityEngine.Random.Range(0.7f, 1.3f);
        source.volume = UnityEngine.Random.Range(0.3f, 0.5f);
        source.Play();
    }
}
