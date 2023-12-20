using UnityEngine;


public class MouseHover : MonoBehaviour
{
    [SerializeField] private PlaceBet _placeBet;
    private void Update()
    {
        GameObject[] selected = _placeBet.GetSelectable();

        foreach (GameObject bet in selected)
        {
            if (bet == null)
                continue;
            
            bet.GetComponent<OnHover>().Hovering();
        }
        
    }


}
