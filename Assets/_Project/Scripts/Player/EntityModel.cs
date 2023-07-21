using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModel : MonoBehaviour
{
    //Enums
    public enum Direction { Down, Left, Up, Right };

    //Variables
    private Direction direction;

    //Getters
    public Direction GetDirection => direction;

    //Setters

    /// <summary>
    /// Atualiza a direcao da entidade.
    /// </summary>
    /// <param name="novaDirecao">Nova direcao</param>
    public virtual void SetDirection(Direction novaDirecao)
    {
        direction = novaDirecao;
    }

    /// <summary>
    /// Retorna o enum Direcao que corresponde ao vetor passado.
    /// </summary>
    /// <param name="direcao">O vetor de direcao</param>
    /// <returns>Um enum do tipo Direcao</returns>
    public static Direction GetVectorDirection(Vector2 direcao)
    {
        float vetorX,
              vetorY;

        vetorX = direcao.x;
        vetorY = direcao.y;

        if (Mathf.Abs(vetorY) >= Mathf.Abs(vetorX))
        {
            if (vetorY < 0)
            {
                return EntityModel.Direction.Down;
            }
            else if (vetorY > 0)
            {
                return EntityModel.Direction.Up;
            }
        }

        else if (vetorX > 0)
        {
            return EntityModel.Direction.Right;
        }
        else if (vetorX < 0)
        {
            return EntityModel.Direction.Left;
        }

        return Direction.Down;
    }

    /// <summary>
    /// Retorna um vetor que corresponde ao enum Direcao passado.
    /// </summary>
    /// <param name="direcao">A direcao</param>
    /// <returns>Um Vector2</returns>
    public static Vector2 GetDirectionVector(Direction direcao)
    {
        switch (direcao)
        {
            case Direction.Down:
                return new Vector2(0, -1);

            case Direction.Left:
                return new Vector2(-1, 0);

            case Direction.Up:
                return new Vector2(0, 1);

            case Direction.Right:
                return new Vector2(1, 0);

            default:
                return new Vector2(0, -1);
        }
    }

    /// <summary>
    /// Retorna uma direcao aleatoria.
    /// </summary>
    /// <returns>Um enum do tipo Direcao</returns>
    public static Direction GetRandomDirection()
    {
        Direction direcao = (Direction)Random.Range(0, System.Enum.GetValues(typeof(Direction)).Length);

        return direcao;
    }

    /// <summary>
    /// Retorna uma direcao aleatoria, exceto a direcao que for passada.
    /// </summary>
    /// <param name="direcaoIgnorada">Direcao para ser ignorada</param>
    /// <returns>Um enum do tipo Direcao</returns>
    public static Direction GetRandomDirectionExcept(Direction direcaoIgnorada)
    {
        List<Direction> direcoes = new List<Direction>();

        foreach(int valorDirecao in System.Enum.GetValues(typeof(Direction)))
        {
            if((Direction)valorDirecao != direcaoIgnorada)
            {
                direcoes.Add((Direction)valorDirecao);
            }
        }

        if (direcoes.Count <= 0)
        {
            Debug.LogWarning("Nao sobrou nenhuma direcao para retornar!");
            return Direction.Down;
        }

        Direction direcao = direcoes[Random.Range(0, direcoes.Count)];

        return direcao;
    }

    /// <summary>
    /// Retorna uma direcao aleatoria, exceto as direcoes que forem passadas.
    /// </summary>
    /// <param name="direcoesIgnoradas">Direcoes para serem ignoradas</param>
    /// <returns>Um enum do tipo Direcao</returns>
    public static Direction GetRandomDirectionExcept(Direction[] direcoesIgnoradas)
    {
        List<Direction> direcoes = new List<Direction>();
        bool direcaoIgual;

        foreach (int valorDirecao in System.Enum.GetValues(typeof(Direction)))
        {
            direcaoIgual = false;

            for(int i = 0; i < direcoesIgnoradas.Length; i++)
            {
                if((Direction)valorDirecao == direcoesIgnoradas[i])
                {
                    direcaoIgual = true;
                    break;
                }
            }

            if(direcaoIgual == false)
            {
                direcoes.Add((Direction)valorDirecao);
            }
        }

        if (direcoes.Count <= 0)
        {
            Debug.LogWarning("Nao sobrou nenhuma direcao para retornar!");
            return Direction.Down;
        }

        Direction direcao = direcoes[Random.Range(0, direcoes.Count)];

        return direcao;
    }

    /// <summary>
    /// Vira na direcao do vetor passado.
    /// </summary>
    /// <param name="posicao">Posicao para virar na direcao</param>
    public void VirarNaDirecao(Vector3 posicao)
    {
        Vector2 direcaoParaVirar = posicao - transform.position;

        SetDirection(EntityModel.GetVectorDirection(direcaoParaVirar));
    }

    /// <summary>
    /// Rotaciona na direcao do vetor passado.
    /// </summary>
    /// <param name="direcao">Vetor para rotacionar na direcao</param>
    public void Rotacionar(Vector2 direcao)
    {
        Quaternion paraRotacionar = Quaternion.LookRotation(Vector3.forward, direcao);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, paraRotacionar, 360);
    }
}