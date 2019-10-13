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
    #endregion

    #region Private
    private PlayerState state;
    // Liste des objects à portée d'intéraction
    private HashSet<GameObject> inRange;
    // Cible de l'intéraction
    private GameObject selected;
    private int seedCount;
    private Bucket bucket;
    private Action currentAction;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
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
        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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
            input = new Vector2(InputManager.GetAxis(Axis.Horizontal), InputManager.GetAxis(Axis.Vertical)).normalized;
        _animator.SetBool("IsWalking", input.magnitude > .1f);
        _animator.speed = input.magnitude > .1f ? 1 : input.magnitude;
        _rigidbody2D.MovePosition(transform.position + speed * input * Time.deltaTime);

        if (currentAction == null)
        {
            if (inRange.Count > 0)
            {
                // On cherche l'object intéractif le plus proche
                float distanceMin = float.PositiveInfinity;
                GameObject nearest = null;

                foreach (GameObject o in inRange)
                {
                    float distance = (o.transform.position - transform.position).magnitude;
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        nearest = o;
                    }
                }

                if (nearest != selected)
                {
                    // On désélectionne l'objet sélectionné auparavant
                    if (selected != null)
                        selected.GetComponent<Interactive>().Deselect();
                    selected = nearest;
                }

                Interactive interactive = selected.GetComponent<Interactive>();
                // On le sélectionne (mise en surbrillance)
                interactive.Select();

                // On affiche l'action correspondante
                Action action = interactive.GetAction(this);

                if (action != null && InputManager.GetButtonDown(action.button))
                {
                    currentAction = action;
                    currentAction.Do();

                    if (currentAction.IsDone())
                        currentAction = null;
                }

                updatePopup(action);
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

    private void updatePopup(Action action)
    {
        if (action != null)
        {
            Text text = popup.GetComponentInChildren<Text>();
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, transform.GetComponentInChildren<Renderer>().bounds.size.y/2));

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
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DropBucket()
    {
        if (HasBucket())
        {
            // TODO : poser à côté du joueur et non sur le joueur
            bucket.SetOnGround(transform.position);
            UIManager.instance.DropBucket();
            bucket = null;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #endregion
}
