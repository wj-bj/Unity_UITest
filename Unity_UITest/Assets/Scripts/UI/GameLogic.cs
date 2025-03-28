using DG.Tweening;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public void Start()
    {
        UIManager.Instance.Show("HomeScreen");
        // DOVirtual.DelayedCall(0.1f, () => {
        //     UIManager.Instance.Show("HomeScreen");});
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            UIManager.Instance.Show("HomeScreen_G");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            UIManager.Instance.Show("HomeScreen");
        }
    }

    
}
