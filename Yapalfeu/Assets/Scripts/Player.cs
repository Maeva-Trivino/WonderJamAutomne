using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Variables
    #region Editor
    // Translation speed of the player
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private GameObject popup = null;
    [SerializeField]
    private GameObject bucket_on_head = null;

    #region SoundEffects
    // Sound of getting or putting on the ground the bucket
    [SerializeField]
    private AudioSource getBucketSound = null;
    // Sound of getting a seed
    [SerializeField]
    private AudioSource getSeedSound = null;
    // Sound of water plant
    [SerializeField]
    private AudioSource waterPlantSound = null;
    // Sound of extinguish tree
    [SerializeField]
    private AudioSource exstinguishTreeSound = null;
    //Sound of planting a seed
    [SerializeField]
    private AudioSource plantSeedSound = null;
    //Sound of filling the Bucket
    [SerializeField]
    private AudioSource fillBucketSound = null;
    #endregion
    #endregion

    #region Private
    // Liste des objects à portée d'intéraction
    private HashSet<GameObject> inRange;
    // Cible de l'intéraction
    private GameObject selected;
    private int seedCount;
    private Bucket bucket;
    private UserAction currentAction;
    private Vector2 direction;
    private float dashTimeRemaining;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider, _trigger;

    private bool onExchangeBucket;
    private Vector2 input;
    #endregion
    #endregion

    #region Methods
    #region Editor
    // Start is called before the first frame update
    void Start()
    {
        seedCount = 7;
        inRange = new HashSet<GameObject>();
        updatePopup(null);
        UIManager.instance.UpdateSeeds(seedCount);

        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bucket_on_head.GetComponent<SpriteRenderer>().sprite = null;
        List<CircleCollider2D> cs = new List<CircleCollider2D>(GetComponents<CircleCollider2D>());
        _trigger = cs.Find(c => c.isTrigger);

        dashTimeRemaining = 0;
        onExchangeBucket = false;
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        renderers[0].sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        renderers[1].sortingOrder = renderers[0].sortingOrder + 1;

        if (currentAction != null)
        {
            if (InputManager.GetButton(currentAction.button))
            {
                currentAction.Do();
                updatePopup(currentAction);
            }
            else
            {
                currentAction = null;
            }
        }

        // On déplace le joueur (utilisation du GetAxisRaw pour avoir des entrées non lissées pour plus de réactivité)
        input = Vector2.zero;
        if (currentAction == null && !IsDashing())
        {
            input = new Vector2(InputManager.GetAxis(Axis.Horizontal), InputManager.GetAxis(Axis.Vertical));
            if(input.magnitude > 1)
                input.Normalize();

            if (input != Vector2.zero && InputManager.GetButtonDown(Button.Y))
                dashTimeRemaining = 0.10f;
        }
        if (input != Vector2.zero)
            direction = input;

        if (IsDashing())
        {
            input = direction*2.5f;
            dashTimeRemaining -= Time.deltaTime;
        }

        _animator.SetBool("IsWalking", input.magnitude > .1f);
        _animator.SetBool("Walk_back", InputManager.GetAxis(Axis.Vertical) > .2f);
        _animator.SetBool("Walk_front", InputManager.GetAxis(Axis.Vertical) < -.2f);
        _animator.SetBool("Walk_right", InputManager.GetAxis(Axis.Horizontal) > .2f);
        _animator.SetBool("Walk_left", InputManager.GetAxis(Axis.Horizontal) < -.2f);
        _animator.speed = input.magnitude > .1f ? 1 : input.magnitude;

        if (currentAction == null)
        {
            if (inRange.Count > 0)
            {
                // On cherche l'objet intéractif le plus proche avec la meilleure action
                UserAction bestAction = null;
                float distanceMin = float.PositiveInfinity;
                GameObject nearest = null;

                foreach (GameObject o in inRange)
                {
                    UserAction action = o.GetComponent<Interactive>().GetAction(this);
                    float distance = (o.transform.position - transform.position).magnitude;

                    if (action != null)
                    {
                        if (bestAction == null ||
                            bestAction.priority < action.priority ||
                            (bestAction.priority == action.priority && distance < distanceMin))
                        {
                            bestAction = action;
                            distanceMin = distance;
                            nearest = o;
                        }
                    }
                    else if (bestAction == null && distance < distanceMin)
                    {
                        distanceMin = distance;
                        nearest = o;
                    }
                }

                // On désélectionne l'objet sélectionné auparavant
                if (selected != null)
                    selected.GetComponent<Interactive>().Deselect();
                selected = nearest;

                Interactive interactive = selected.GetComponent<Interactive>();
                // On le sélectionne (mise en surbrillance)
                interactive.Select();

                if (bestAction != null && !IsDashing() && InputManager.GetButtonDown(bestAction.button))
                {
                    currentAction = bestAction;
                    currentAction.Do();

                    if (currentAction.IsDone())
                        currentAction = null;
                }

                updatePopup(bestAction);
            }
            else
            {
                updatePopup(null);
                _spriteRenderer.color = Color.white;

                if (selected != null)
                {
                    selected.GetComponent<Interactive>().Deselect();
                    selected = null;
                }
            }
        }

        // TODO : changer le bouton pour poser le seau
        if (InputManager.GetButtonDown(Button.B))
            DropBucket();
    }

    void FixedUpdate()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + speed * input * Time.deltaTime);
    }

    private void updatePopup(UserAction action)
    {
        if (action != null)
        {
            Text text = popup.GetComponentsInChildren<Text>()[0];
            Text button = popup.GetComponentsInChildren<Text>()[1];
            Text combos = popup.GetComponentsInChildren<Text>()[2];
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.GetComponentInChildren<Renderer>().bounds.size.y));

            if (action == currentAction)
            {
                if (action.combos == null)
                {
                    text.text = "";
                    button.text = "";
                    combos.text = "";
                }
                else
                {
                    text.text = "";
                    combos.text = "";
                    foreach (Button b in action.combos)
                    {
                        combos.text += InputManager.GetButtonName(b) + " ";
                    }

                    combos.text = combos.text.Remove(combos.text.Length - 1);
                    button.text = "";
                }

                Slider slider = popup.GetComponentInChildren<Slider>();
                slider.transform.localScale = new Vector3(1, 1, 1);
                slider.value = action.progression;
                slider.gameObject.SetActive(true);
            }
            else
            {
                text.text = action.name;
                text.fontSize = 12;
                button.text = InputManager.GetButtonName(action.button);
                combos.text = "";
                popup.GetComponentInChildren<Slider>().gameObject.transform.localScale = new Vector3(0, 0, 0);
            }

            popup.transform.position = screenPos;
            popup.gameObject.SetActive(true);
        }
        else
        {
            if (popup.activeSelf)
            {
                popup.GetComponentInChildren<Slider>().gameObject.transform.localScale = new Vector3(0, 0, 0);
                popup.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // On ajoute l'objet à portée dans la liste des objets potentiellement sélectionnable
        Interactive interactive = collision.gameObject.GetComponent<Interactive>();
        if (interactive != null)
        {
            inRange.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // On enlève l'objet qui était à portée
        Interactive interactive = collision.gameObject.GetComponent<Interactive>();
        if (interactive != null)
        {
            inRange.Remove(collision.gameObject);
        }
    }
    #endregion

    #region Public
    public bool HasSeed()
    {
        return seedCount > 0;
    }

    public bool PlantSeed()
    {
        if (HasSeed())
        {
            plantSeedSound.Play();
            seedCount--;
            UIManager.instance.UpdateSeeds(seedCount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasBucket()
    {
        return bucket != null;
    }

    public bool HasEmptyBucket()
    {
        return HasBucket() && !bucket.isFilled();
    }

    public bool HasFilledBucket()
    {
        return HasBucket() && bucket.isFilled();
    }

    public bool WaterPlant()
    {
        if (HasFilledBucket())
        {
            waterPlantSound.Play();
            EmptyBucket();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ExtinguishFire()
    {
        if (HasFilledBucket())
        {
            exstinguishTreeSound.Play();
            EmptyBucket();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ExchangeBucket(bool isGetBucketFill)
    {
        getBucketSound.Play();
        if (isGetBucketFill)
        {
            if (HasFilledBucket())
            {
                EmptyBucket();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            onExchangeBucket = true;
            return FillBucket();
        }
    }

    private void EmptyBucket()
    {
        UIManager.instance.EmptyBucket();
        bucket.Empty();
        bucket_on_head.GetComponent<SpriteRenderer>().sprite = this.bucket.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    public void HarvestSeed()
    {
        getSeedSound.Play();
        seedCount++;
        UIManager.instance.UpdateSeeds(seedCount);
    }

    public bool PickUpBucket(Bucket bucket)
    {
        if (!HasBucket())
        {
            getBucketSound.Play();
            this.bucket = bucket;
            this.bucket.Deselect();
            this.bucket.gameObject.SetActive(false);
            UIManager.instance.PickUpBucket(this.bucket.isFilled());
            bucket_on_head.GetComponent<SpriteRenderer>().sprite = this.bucket.GetComponentInChildren<SpriteRenderer>().sprite;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool FillBucket()
    {
        if (HasEmptyBucket())
        {
            if (!onExchangeBucket)
            {
                fillBucketSound.Play();
            }
            UIManager.instance.FilledBucket();
            bucket.Fill();
            bucket_on_head.GetComponent<SpriteRenderer>().sprite = this.bucket.GetComponentInChildren<SpriteRenderer>().sprite;
            onExchangeBucket = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DropBucket()
    {
        if (HasBucket() && IsDropAllowed())
        {
            getBucketSound.Play();
            // TODO : poser à côté du joueur et non sur le joueur
            bucket.SetOnGround(transform.position + (Vector3)direction);
            UIManager.instance.DropBucket();
            bucket_on_head.GetComponent<SpriteRenderer>().sprite = null;
            bucket = null;
            return true;
        }
        else
        {
            return false;
        }
    }
    public Bucket GetBucket()
    {
        return bucket;
    }

    #endregion

    #region Private
    private bool IsDashing()
    {
        return dashTimeRemaining > 0;
    }

    private bool IsDropAllowed()
    {
        RaycastHit2D[] hits = new RaycastHit2D[2];
        Physics2D.Raycast(transform.position, direction, new ContactFilter2D(), hits);

        // If it hits something...
        if (hits[1].collider != null)
        {
            // Calculate the distance from the surface and the "error" relative
            // to the floating height.
            float distance = ((Vector3) hits[1].point - transform.position).magnitude;
            return distance > _trigger.radius;
        }
        return true;
    }
    #endregion
    #endregion
}
