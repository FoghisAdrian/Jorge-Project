using UnityEngine;

public class MainMenuScrollingBackground : MonoBehaviour
{
    [Header("Layer Parents")]
    [SerializeField] private Transform layer1Sky;
    [SerializeField] private Transform layer2MidClouds;
    [SerializeField] private Transform layer3CloseClouds;

    [Header("Movement Speeds")]
    [SerializeField] private float speedLayer1 = 0.2f;
    [SerializeField] private float speedLayer2 = 0.8f;
    [SerializeField] private float speedLayer3 = 1.5f;

    [Header("Background Dimensions")]
    [SerializeField] private float backgroundWidth = 19.2f;

    void Update()
    {
        MoveLayer(layer1Sky, speedLayer1);
        MoveLayer(layer2MidClouds, speedLayer2);
        MoveLayer(layer3CloseClouds, speedLayer3);
    }

    void MoveLayer(Transform layerTransform, float speed)
    {
        if (layerTransform == null) return;

        layerTransform.Translate(Vector3.left * speed * Time.deltaTime);

        if (layerTransform.position.x <= -backgroundWidth)
        {
            Vector3 newPos = layerTransform.position;
            newPos.x += backgroundWidth;
            layerTransform.position = newPos;
        }
    }
}
