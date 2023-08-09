using System.Collections;
using UnityEngine;
public class PlayerMoveInteractionBox : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    [SerializeField] EntityModel.Direction dir;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void ReceiveDirection(EntityModel.Direction direction)
    {
        if (direction != dir)
        {
            switch (direction)
            {
                case EntityModel.Direction.Down:
                    boxCollider.offset = new Vector2(0, -0.5f);
                    boxCollider.size = new Vector2(0.25f, 1);

                    break;

                case EntityModel.Direction.Left:
                    boxCollider.offset = new Vector2(-0.5f, 0.18f);
                    boxCollider.size = new Vector2(1, 0.25f);

                    break;

                case EntityModel.Direction.Up:
                    boxCollider.offset = new Vector2(0, 0.5f);
                    boxCollider.size = new Vector2(0.25f, 1);

                    break;

                case EntityModel.Direction.Right:
                    boxCollider.offset = new Vector2(0.5f, 0.18f);
                    boxCollider.size = new Vector2(1, 0.25f);

                    break;
            }
            dir = direction;
        }

    }
    
}