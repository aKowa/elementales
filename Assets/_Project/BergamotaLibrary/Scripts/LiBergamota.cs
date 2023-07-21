using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    public static class LiBergamota
    {
        #region Colisao

        #region Rect

        /// <summary>
        /// Confere a colisao de um ponto com um Rect.
        /// </summary>
        /// <param name="ponto">O ponto</param>
        /// <param name="colisao">Rect com o qual verificar a colisao</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(Vector2 ponto, Rect colisao)
        {
            if (((ponto.x >= colisao.center.x - (colisao.width / 2)) && (ponto.x <= colisao.center.x + (colisao.width / 2))) &&
                ((ponto.y >= colisao.center.y - (colisao.height / 2)) && (ponto.y <= colisao.center.y + (colisao.height / 2))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um Rect com outro Rect.
        /// </summary>
        /// <param name="colisao">Primeiro Rect</param>
        /// <param name="colisao2">Segundo Rect</param>
        /// <returns></returns>
        static public bool HitTest(Rect colisao, Rect colisao2)
        {
            return colisao.Overlaps(colisao2);
        }

        /// <summary>
        /// Confere a colisao de um Rect, tendo a sua posicao alterada por um offSet, com outro Rect.
        /// </summary>
        /// <param name="colisao">Primeiro Rect</param>
        /// <param name="offSet">OffSet para o primeiro Rect</param>
        /// <param name="colisao2">Segundo Rect</param>
        /// <returns></returns>
        static public bool HitTest(Rect colisao, Vector2 offSet, Rect colisao2)
        {
            colisao.center += offSet;

            return colisao.Overlaps(colisao2);
        }

        #endregion

        #region BoxCollider2D

        /// <summary>
        /// Confere a colisao de um ponto com um BoxCollider2D.
        /// </summary>
        /// <param name="ponto">O ponto</param>
        /// <param name="colisao">BoxCollider2D com o qual verificar a colisao</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(Vector2 ponto, BoxCollider2D colisao)
        {
            if (((ponto.x >= colisao.bounds.center.x - colisao.bounds.extents.x) && (ponto.x <= colisao.bounds.center.x + colisao.bounds.extents.x)) &&
                ((ponto.y >= colisao.bounds.center.y - colisao.bounds.extents.y) && (ponto.y <= colisao.bounds.center.y + colisao.bounds.extents.y)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um BoxCollider2D com outro BoxCollider2D.
        /// </summary>
        /// <param name="colisao">Primeiro BoxCollider2D</param>
        /// <param name="colisao2">Segundo BoxCollider2D</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(BoxCollider2D colisao, BoxCollider2D colisao2)
        {
            if (((colisao.bounds.center.x - colisao.bounds.extents.x <= colisao2.bounds.center.x + colisao2.bounds.extents.x) && (colisao.bounds.center.x + colisao.bounds.extents.x >= colisao2.bounds.center.x - colisao2.bounds.extents.x)) &&
                ((colisao.bounds.center.y - colisao.bounds.extents.y <= colisao2.bounds.center.y + colisao2.bounds.extents.y) && (colisao.bounds.center.y + colisao.bounds.extents.y >= colisao2.bounds.center.y - colisao2.bounds.extents.y)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um BoxCollider2D, tendo a sua posicao alterada por um offSet, com outro BoxCollider2D.
        /// </summary>
        /// <param name="colisao">Primeiro BoxCollider2D</param>
        /// <param name="offSet">OffSet para o primeiro BoxCollider2D</param>
        /// <param name="colisao2">Segundo BoxCollider2D</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(BoxCollider2D colisao, Vector2 offSet, BoxCollider2D colisao2)
        {
            if (((colisao.bounds.center.x - colisao.bounds.extents.x + offSet.x <= colisao2.bounds.center.x + colisao2.bounds.extents.x) && (colisao.bounds.center.x + colisao.bounds.extents.x + offSet.x >= colisao2.bounds.center.x - colisao2.bounds.extents.x)) &&
                ((colisao.bounds.center.y - colisao.bounds.extents.y + offSet.y <= colisao2.bounds.center.y + colisao2.bounds.extents.y) && (colisao.bounds.center.y + colisao.bounds.extents.y + offSet.y >= colisao2.bounds.center.y - colisao2.bounds.extents.y)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region CircleCollider2D

        /// <summary>
        /// Confere a colisao de um ponto com um CircleCollider2D.
        /// </summary>
        /// <param name="ponto">O ponto</param>
        /// <param name="colisao">CircleCollider2D com o qual verificar a colisao</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(Vector2 ponto, CircleCollider2D colisao)
        {
            float distanciaX, distanciaY, distancia;

            distanciaX = ponto.x - colisao.bounds.center.x;
            distanciaY = ponto.y - colisao.bounds.center.y;
            distancia = ((distanciaX * distanciaX) + (distanciaY * distanciaY));

            if (distancia <= (colisao.radius * colisao.radius))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um CircleCollider2D com outro CircleCollider2D.
        /// </summary>
        /// <param name="colisao">Primeiro CircleCollider2D</param>
        /// <param name="colisao2">Segundo CircleCollider2D</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(CircleCollider2D colisao, CircleCollider2D colisao2)
        {
            float distanciaX, distanciaY, distancia;

            distanciaX = colisao.bounds.center.x - colisao2.bounds.center.x;
            distanciaY = colisao.bounds.center.y - colisao2.bounds.center.y;
            distancia = ((distanciaX * distanciaX) + (distanciaY * distanciaY));

            if (distancia <= ((colisao.radius + colisao2.radius) * (colisao.radius + colisao2.radius)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um CircleCollider2D, tendo a sua posicao alterada por um offSet, com outro CircleCollider2D.
        /// </summary>
        /// <param name="colisao">Primeiro CircleCollider2D</param>
        /// <param name="offSet">OffSet para o primeiro CircleCollider2D</param>
        /// <param name="colisao2">Segundo CircleCollider2D</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(CircleCollider2D colisao, Vector2 offSet, CircleCollider2D colisao2)
        {
            float distanciaX, distanciaY, distancia;

            distanciaX = colisao.bounds.center.x + offSet.x - colisao2.bounds.center.x;
            distanciaY = colisao.bounds.center.y + offSet.y - colisao2.bounds.center.y;
            distancia = ((distanciaX * distanciaX) + (distanciaY * distanciaY));

            if (distancia <= ((colisao.radius + colisao2.radius) * (colisao.radius + colisao2.radius)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region BoxCollider

        /// <summary>
        /// Confere a colisao de um ponto com um BoxCollider.
        /// </summary>
        /// <param name="ponto">O ponto</param>
        /// <param name="colisao">BoxCollider com o qual verificar a colisao</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(Vector3 ponto, BoxCollider colisao)
        {
            if (((ponto.x >= colisao.bounds.center.x - colisao.bounds.extents.x) && (ponto.x <= colisao.bounds.center.x + colisao.bounds.extents.x)) &&
                ((ponto.y >= colisao.bounds.center.y - colisao.bounds.extents.y) && (ponto.y <= colisao.bounds.center.y + colisao.bounds.extents.y)) &&
                ((ponto.z >= colisao.bounds.center.z - colisao.bounds.extents.z) && (ponto.z <= colisao.bounds.center.z + colisao.bounds.extents.z)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um BoxCollider com outro BoxCollider.
        /// </summary>
        /// <param name="colisao">Primeiro BoxCollider</param>
        /// <param name="colisao2">Segundo BoxCollider</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(BoxCollider colisao, BoxCollider colisao2)
        {
            if (((colisao.bounds.center.x - colisao.bounds.extents.x <= colisao2.bounds.center.x + colisao2.bounds.extents.x) && (colisao.bounds.center.x + colisao.bounds.extents.x >= colisao2.bounds.center.x - colisao2.bounds.extents.x)) &&
                ((colisao.bounds.center.y - colisao.bounds.extents.y <= colisao2.bounds.center.y + colisao2.bounds.extents.y) && (colisao.bounds.center.y + colisao.bounds.extents.y >= colisao2.bounds.center.y - colisao2.bounds.extents.y)) &&
                ((colisao.bounds.center.z - colisao.bounds.extents.z <= colisao2.bounds.center.z + colisao2.bounds.extents.z) && (colisao.bounds.center.z + colisao.bounds.extents.z >= colisao2.bounds.center.z - colisao2.bounds.extents.z)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um BoxCollider, tendo a sua posicao alterada por um offSet, com outro BoxCollider.
        /// </summary>
        /// <param name="colisao">Primeiro BoxCollider</param>
        /// <param name="offSet">OffSet para o primeiro BoxCollider</param>
        /// <param name="colisao2">Segundo BoxCollider</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(BoxCollider colisao, Vector3 offSet, BoxCollider colisao2)
        {
            if (((colisao.bounds.center.x - colisao.bounds.extents.x + offSet.x <= colisao2.bounds.center.x + colisao2.bounds.extents.x) && (colisao.bounds.center.x + colisao.bounds.extents.x + offSet.x >= colisao2.bounds.center.x - colisao2.bounds.extents.x)) &&
                ((colisao.bounds.center.y - colisao.bounds.extents.y + offSet.y <= colisao2.bounds.center.y + colisao2.bounds.extents.y) && (colisao.bounds.center.y + colisao.bounds.extents.y + offSet.y >= colisao2.bounds.center.y - colisao2.bounds.extents.y)) &&
                ((colisao.bounds.center.z - colisao.bounds.extents.z + offSet.z <= colisao2.bounds.center.z + colisao2.bounds.extents.z) && (colisao.bounds.center.z + colisao.bounds.extents.z + offSet.z >= colisao2.bounds.center.z - colisao2.bounds.extents.z)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region SphereCollider

        /// <summary>
        /// Confere a colisao de um ponto com um SphereCollider.
        /// </summary>
        /// <param name="ponto">O ponto</param>
        /// <param name="colisao">SphereCollider com o qual verificar a colisao</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(Vector3 ponto, SphereCollider colisao)
        {
            float distanciaX, distanciaY, distanciaZ, distancia;

            distanciaX = ponto.x - colisao.bounds.center.x;
            distanciaY = ponto.y - colisao.bounds.center.y;
            distanciaZ = ponto.z - colisao.bounds.center.z;
            distancia = ((distanciaX * distanciaX) + (distanciaY * distanciaY) + (distanciaZ * distanciaZ));

            if (distancia <= (colisao.radius * colisao.radius))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um SphereCollider com outro SphereCollider.
        /// </summary>
        /// <param name="colisao">Primeiro SphereCollider</param>
        /// <param name="colisao2">Segundo SphereCollider</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(SphereCollider colisao, SphereCollider colisao2)
        {
            float distanciaX, distanciaY, distanciaZ, distancia;

            distanciaX = colisao.bounds.center.x - colisao2.bounds.center.x;
            distanciaY = colisao.bounds.center.y - colisao2.bounds.center.y;
            distanciaZ = colisao.bounds.center.z - colisao2.bounds.center.z;
            distancia = ((distanciaX * distanciaX) + (distanciaY * distanciaY) + (distanciaZ * distanciaZ));

            if (distancia <= ((colisao.radius + colisao2.radius) * (colisao.radius + colisao2.radius)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Confere a colisao de um SphereCollider, tendo a sua posicao alterada por um offSet, com outro SphereCollider.
        /// </summary>
        /// <param name="colisao">Primeiro SphereCollider</param>
        /// <param name="offSet">OffSet para o primeiro SphereCollider</param>
        /// <param name="colisao2">Segundo SphereCollider</param>
        /// <returns>Uma booleana</returns>
        static public bool HitTest(SphereCollider colisao, Vector3 offSet, SphereCollider colisao2)
        {
            float distanciaX, distanciaY, distanciaZ, distancia;

            distanciaX = colisao.bounds.center.x + offSet.x - colisao2.bounds.center.x;
            distanciaY = colisao.bounds.center.y + offSet.y - colisao2.bounds.center.y;
            distanciaZ = colisao.bounds.center.z + offSet.z - colisao2.bounds.center.z;
            distancia = ((distanciaX * distanciaX) + (distanciaY * distanciaY) + (distanciaZ * distanciaZ));

            if (distancia <= ((colisao.radius + colisao2.radius) * (colisao.radius + colisao2.radius)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Retorna um retangulo nas coordenadas globais a partir de um RectTransform.
        /// </summary>
        /// <param name="rectTransform">RectTransform para servir de base para o retangulo</param>
        /// <returns>Um Rect</returns>
        static public Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] bordas = new Vector3[4];
            rectTransform.GetWorldCorners(bordas);

            return new Rect(bordas[0], bordas[2] - bordas[0]);
        }

        /// <summary>
        /// Retorna se qualquer uma das coordenadas dos vetores tem uma diferenca maior que 0.01f.
        /// </summary>
        /// <param name="vetor1">Primeiro vetor</param>
        /// <param name="vetor2">Segundo vetor</param>
        /// <returns>Uma booleana</returns>
        static public bool VetorDiferente(Vector2 vetor1, Vector2 vetor2)
        {
            return Mathf.Abs(vetor2.x - vetor1.x) > 0.01f || Mathf.Abs(vetor2.y - vetor1.y) > 0.01f;
        }

        /// <summary>
        /// Retorna se qualquer uma das coordenadas dos vetores tem uma diferenca maior que 0.01f.
        /// </summary>
        /// <param name="vetor1">Primeiro vetor</param>
        /// <param name="vetor2">Segundo vetor</param>
        /// <returns>Uma booleana</returns>
        static public bool VetorDiferente(Vector3 vetor1, Vector3 vetor2)
        {
            return Mathf.Abs(vetor2.x - vetor1.x) > 0.01f || Mathf.Abs(vetor2.y - vetor1.y) > 0.01f || Mathf.Abs(vetor2.z - vetor1.z) > 0.01f;
        }

        /// <summary>
        /// Retorna a distancia entre duas posicoes ao quadrado.
        /// </summary>
        /// <param name="posicao1">Posicao 1</param>
        /// <param name="posicao2">Posicao 2</param>
        /// <returns>A distancia ao quadrado</returns>
        static public float Distancia(Vector3 posicao1, Vector3 posicao2)
        {
            return (posicao1 - posicao2).sqrMagnitude;
        }

        /// <summary>
        /// Retorna se um ponto, saindo de uma origem, ja passou de um ponto de destino.
        /// </summary>
        /// <param name="origem">Ponto de origem</param>
        /// <param name="destino">Ponto de destino</param>
        /// <param name="posicaoAtual">Posicao atual</param>
        /// <returns>Uma booleana</returns>
        static public bool PassouDoPonto(Vector3 origem, Vector3 destino, Vector3 posicaoAtual)
        {
            //The vector between the character and the waypoint we are going from
            Vector3 a = posicaoAtual - origem;

            //The vector between the waypoints
            Vector3 b = destino - origem;

            //Vector projection from https://en.wikipedia.org/wiki/Vector_projection
            //To know if we have passed the upcoming waypoint we need to find out how much of b is a1
            //a1 = (a.b / |b|^2) * b
            //a1 = progress * b -> progress = a1 / b -> progress = (a.b / |b|^2)
            float progress = (a.x * b.x + a.y * b.y + a.z * b.z) / (b.x * b.x + b.y * b.y + b.z * b.z);

            //If progress is above or equal 1 we know we have passed or reached the waypoint
            if (progress >= 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retorna a posicao do centro do tile que estiver na posicao passada.
        /// </summary>
        /// <param name="posicao">Posicao</param>
        /// <returns>Um Vector2</returns>
        static public Vector2 PosicaoDoTile(Vector2 posicao)
        {
            Vector2 posicaoDoTile = new Vector2(Mathf.FloorToInt(posicao.x) + 0.5f, Mathf.FloorToInt(posicao.y) + 0.5f);

            return posicaoDoTile;
        }
    }
}
