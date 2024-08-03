using UnityEngine;
using AK.Wwise; // Assurez-vous d'avoir les bons namespaces importés

public class MaterialSoundManager : MonoBehaviour
{
    // Déclarez une classe ou une structure pour mapper les matériaux aux événements sonores
    [System.Serializable]
    public class MaterialSoundMapping
    {
        public Material material;
        public AK.Wwise.Event impactSoundEvent;
    }

    // Liste des mappings matériel-événement
    public MaterialSoundMapping[] materialSoundMappings;

    // Méthode pour jouer le son d'impact en fonction du matériau
    public void PlayImpactSound(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material objMaterial = renderer.material;

            // Recherche du mapping correspondant pour le matériau actuel
            foreach (MaterialSoundMapping mapping in materialSoundMappings)
            {
                if (mapping.material == objMaterial)
                {
                    // Déclenche l'événement sonore correspondant
                    mapping.impactSoundEvent.Post(obj);
                    return;
                }
            }
        }

        // Si aucun mapping correspondant n'est trouvé, affichez un avertissement
        Debug.LogWarning("Aucun événement sonore d'impact trouvé pour ce matériau.");
    }
}
