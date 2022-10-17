using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject gameManagerObject;
    GameSceneManager gameManager;

    public AudioSource playerSoundSource;

    [SerializeField] AudioClip[] blowSoundList;
    [SerializeField] AudioClip[] coinSoundList;

    [SerializeField] GameObject point;

    Touch input;

    Rigidbody2D cachedRigidbody2D;   

    LineRenderer cachedLineRenderer;

    ParticleSystem cachedParticleSystem;

    Animator cachedAnimator;

    List<Vector2> positionsList = new List<Vector2>();

    bool restart; 

    void Start()
    {
        restart = false;

        cachedRigidbody2D = GetComponent<Rigidbody2D>();        
        cachedParticleSystem = GetComponent<ParticleSystem>();
        cachedAnimator = GetComponent<Animator>();

        cachedLineRenderer = GetComponent<LineRenderer>();
        cachedLineRenderer.positionCount = 1;

        gameManager = gameManagerObject.GetComponentInChildren<GameSceneManager>();
    }  

    void Update()
    {
        if (InputHelper.GetTouches().Count > 0)
        {
            input = InputHelper.GetTouches()[0];

            if (input.phase == TouchPhase.Began && !restart)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(input.position.x, input.position.y));

                if (!IsPointerOverUIObject())
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

                    Destroy(collider.gameObject);
                    break;
                }
            case "Spike":
                {
                    gameManager.EndGamePref("You Lose..");
                    ClearPosition();

                    gameManager.PlaySound(playerSoundSource, blowSoundList);

                    cachedAnimator.SetBool("GameOver", true);
                    cachedAnimator.Play("CircleBlow");
                    cachedParticleSystem.Play();
                  
                    Destroy(this.gameObject, 0.7f);
                    break;
                }
            case "Coin":
                {
                    collider.GetComponent<CircleCollider2D>().radius = 0;
                    collider.GetComponent<Animator>().SetBool("CoinDrop", true);

                    gameManager.PlaySound(playerSoundSource, coinSoundList);
                   
                    Destroy(collider.gameObject, 1);

                    if (gameManager.AddedCoin())
                    {
                        ClearPosition();
                        gameManager.EndGamePref("You Win!!");
                    }
                    break;
                }
            default: Debug.Log("ER: Non of");
                break;
        }
    }

    private void ClearPosition()
    {
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
}
