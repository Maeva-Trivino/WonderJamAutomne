using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text popup;

    // Translation speed of the player
    public float speed = 1f;

    private PlayerState state;

    // Liste des objects à portée d'intéraction
    private List<GameObject> inRange;

    // Cible de l'intéraction
    private GameObject selected;

    // Start is called before the first frame update
    void Start()
    {
        inRange = new List<GameObject>();
        popup.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // On déplace le joueur (utilisation du GetAxisRaw pour avoir des entrées non lissées pour plus de réactivité)
        transform.Translate(speed * new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * Time.deltaTime);

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
            Action action = interactive.GetAction(state);

            if (action != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                screenPos.y += 30;
                popup.transform.position = screenPos;
                popup.text = action.name;
            }
        }
        else
        {
            // Effacement du popup
            popup.text = "";
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;

            if (selected != null)
            {
                selected.GetComponent<Interactive>().Deselect();
                selected = null;
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
}
