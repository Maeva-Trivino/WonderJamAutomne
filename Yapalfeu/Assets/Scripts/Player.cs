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
    private Text popup = null;
    #endregion

    #region Private
    private PlayerState state;
    // Liste des objects à portée d'intéraction
    private List<GameObject> inRange;
    // Cible de l'intéraction
    private GameObject selected;
    private int seedCount;
    private Bucket bucket;

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
        inRange = new List<GameObject>();
        popup.text = "";
        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // On déplace le joueur (utilisation du GetAxisRaw pour avoir des entrées non lissées pour plus de réactivité)
        Vector3 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        _animator.SetBool("IsWalking", input.magnitude > .1f);
        _animator.speed = input.magnitude > .1f ? 1 : input.magnitude;
        _rigidbody2D.MovePosition(transform.position + speed * input * Time.deltaTime);

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

            if (action != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                screenPos.y += 30;
                popup.transform.position = screenPos;
                popup.text = action.name;

                // TODO : mapper l'action.button au bon bouton
                // TODO : gérer le temps d'appui
                // TODO : gérer les combos
                if (Input.GetKeyDown(KeyCode.E))
                    action.Do();
            }
            else
            {
                popup.text = "";
            }
        }
        else
        {
            // Effacement du popup
            popup.text = "";
            _spriteRenderer.color = Color.white;

            if (selected != null)
            {
                selected.GetComponent<Interactive>().Deselect();
                selected = null;
            }
        }

        // TODO : changer le bouton pour poser le seau
        if (Input.GetKeyDown(KeyCode.F))
            DropBucket();
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
    }

    public bool PickUpBucket(Bucket bucket)
    {
        if (!HasBucket())
        {
            this.bucket = bucket;
            this.bucket.Deselect();
            this.bucket.gameObject.SetActive(false);
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
