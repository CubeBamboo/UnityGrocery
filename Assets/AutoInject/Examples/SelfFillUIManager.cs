using AutoInject.SelfFill;
using UnityEngine;
using UnityEngine.UI;

namespace AutoInject.Examples
{
    public class SelfFillUIManager : MonoBehaviour
    {
        [SerializeField, SelfFill] private Text titleText;
        [SerializeField, SelfFill] private Text scoreText;
        [SerializeField, SelfFill] private GameObject storyBoard;
        [SerializeField, SelfFill(FillType.Type)] private CircleCollider2D circleCollider;
    }
}
