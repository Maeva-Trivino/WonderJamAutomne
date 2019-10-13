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
    #endregion

    #region Private
    private PlayerState state;
    // Liste des objects à portée d'intéraction
    private HashSet<GameObject> inRange;
    // Cible de l'intéraction
    private GameObject selected;
    private int seedCount;
    private Bucket bucket;
    private UserAction currentAction;
    private Vector2 direction;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider, _trigger;
    #endregion
    #endregion

    #region Methods
    #region Editor
    // Start is called before the first frame update
    void Start()
    {
        seedCount = 1;
        inRange = new HashSet<GameObject>();
        updatePopup(null);
        UIManager.instance.UpdateSeeds(seedCount);

        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bucket_on_head.GetComponent<SpriteRenderer>().sprite = null;
        List<CircleCollider2D> cs = new List<CircleCollider2D>(GetComponents<CircleCollider2D>());
        _trigger = cs.Find(c => c.isTrigger);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        }

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
        Vector3 input = Vector3.zero;
        if (currentAction == null)
        {
            input = new Vector2(InputManager.GetAxis(Axis.Horizontal), InputManager.GetAxis(Axis.Vertical));
            if(input.magnitude > 1)
                input.Normalize();
        }
        if (input != Vector3.zero)
            direction = input;
        _animator.SetBool("IsWalking", input.magnitude > .1f);
        _animator.SetBool("Walk_back", InputManager.GetAxis(Axis.Vertical) > .2f);
        _animator.SetBool("Walk_front", InputManager.GetAxis(Axis.Vertical) < -.2f);
        _animator.SetBool("Walk_right", InputManager.GetAxis(Axis.Horizontal) > .2f);
        _animator.SetBool("Walk_left", InputManager.GetAxis(Axis.Horizontal) < -.2f);
        _animator.speed = input.magnitude > .1f ? 1 : input.magnitude;
        _rigidbody2D.MovePosition(transform.position + speed * input * Time.deltaTime);

        if (currentAction == null)
        {
            if (inRange.Count > 0)
            {
                // On cherche l'object intéractif le plus proche
                float distanceMin = float.PositiveInfinity;
                GameObject nearest = null;

                float distanceMinWithAction = float.PositiveInfinity;
                GameObject nearestWithAction = null;
                UserAction nearestAction = null;

                foreach (GameObject o in inRange)
                {
                    float distance = (o.transform.position - transform.position).magnitude;
                    UserAction action = o.GetComponent<Interactive>().GetAction(this);

                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        nearest = o;
                    }

                    if (action != null && distance < distanceMinWithAction)
                    {
                        distanceMinWithAction = distance;
                        nearestWithAction = o;
                        nearestAction = action;
                    }
                }

                if (nearestWithAction != null)
                    nearest = nearestWithAction;

                // On désélectionne l'objet sélectionné auparavant
                if (selected != null)
                    selected.GetComponent<Interactive>().Deselect();
                selected = nearest;

                Interactive interactive = selected.GetComponent<Interactive>();
                // On le sélectionne (mise en surbrillance)
                interactive.Select();

                if (nearestAction != null && InputManager.GetButtonDown(nearestAction.button))
                {
                    currentAction = nearestAction;
                    currentAction.Do();

                    if (currentAction.IsDone())
                        currentAction = null;
                }

                updatePopup(nearestAction);
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

    private void updatePopup(UserAction action)
    {
        if (action != null)
        {
            Text text = popup.GetComponentInChildren<Text>();
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.GetComponentInChildren<Renderer>().bounds.size.y));

            if (action == currentAction)
            {
                if (action.combos == null)
                {
                    text.text = "";
                    text.fontSize = 12;
                }
                else
                {
                    text.text = "";
                    foreach (Button b in action.combos)
                    {
                        text.text += InputManager.GetButtonName(b) + " ";
                    }

                    text.text = text.text.Remove(text.text.Length - 1);
                    text.fontSize = 16;
                }

                Slider slider = popup.GetComponentInChildren<Slider>();
                slider.transform.localScale = new Vector3(1, 1, 1);
                slider.value = action.progression;
                slider.gameObject.SetActive(true);
            }
            else
            {
                text.text = "(" + InputManager.GetButtonName(action.button) + ") " + action.name;
                text.fontSize = 12;
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
            UIManager.instance.EmptyBucket();
            bucket.Empty();
            bucket_on_head.GetComponent<SpriteRenderer>().sprite = this.bucket.GetComponent<SpriteRenderer>().sprite;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HarvestSeed()
    {
        seedCount++;
        UIManager.instance.UpdateSeeds(seedCount);
    }

    public bool PickUpBucket(Bucket bucket)
    {
        if (!HasBucket())
        {
            this.bucket = bucket;
            this.bucket.Deselect();
            this.bucket.gameObject.SetActive(false);
            UIManager.instance.PickUpBucket(this.bucket.isFilled());
            bucket_on_head.GetComponent<SpriteRenderer>().sprite = this.bucket.GetComponent<SpriteRenderer>().sprite;
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
            UIManager.instance.FilledBucket();
            bucket.Fill();
            bucket_on_head.GetComponent<SpriteRenderer>().sprite = this.bucket.GetComponent<SpriteRenderer>().sprite;
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
    #endregion

    #region Private
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
