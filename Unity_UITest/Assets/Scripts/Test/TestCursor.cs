using UnityEngine;
using DG.Tweening; // 添加 DOTween 命名空间


public class TestCursor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 按下空格键触发动画
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayBallAnimation();
        }
    }

    void PlayBallAnimation()
    {
        // 1. 移动动画
        transform.DOMove(new Vector3(2f, 2f, 0), 1f)
            .SetEase(Ease.OutBounce);  // 设置弹跳效果

        // 2. 等待1秒后执行旋转动画
        transform.DORotate(new Vector3(0, 360, 0), 1f)
            .SetDelay(1f)
            .SetEase(Ease.Linear);

        // 3. 缩放动画
        transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f)
            .SetDelay(2f)
            .SetLoops(2, LoopType.Yoyo);  // 来回执行两次

        // 4. 改变颜色（需要物体上有 Renderer 组件）
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().material.DOColor(Color.red, 1f)
                .SetDelay(3f);
        }
    }

    //onClickMouse
  
}
