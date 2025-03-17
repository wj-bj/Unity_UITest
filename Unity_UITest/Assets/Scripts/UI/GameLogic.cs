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

    
}
