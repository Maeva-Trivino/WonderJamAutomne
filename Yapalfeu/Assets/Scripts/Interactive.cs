public interface Interactive
{
    // Sélectionne l'objet (mettre en surbrillance)
    // TODO : mettre une vraie surbrillance pour les objets
    void Select();

    // Désélectionne l'objet
    void Deselect();

    UserAction GetAction(Player player);
}
