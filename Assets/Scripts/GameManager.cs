using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text textSumMove;

    int SumMoves;

    int[] EmptySlot = new int[2];
    int[] oldEmptySlot = new int[2];
    private GameObject[,] GOGameElement = new GameObject[4, 4];
    public GameObject GamePanel;
    private int[,] originalSlot = new int[4, 4];//В этом массиве по сути цифровой вариант того, что творится на поле. От 0_0(первый элемент) до 3_3(последний элемент). Понадобится для восстановления оригинала и обратно

    public float durationMove;

    public bool original = false;

    void Start () {
        ReWrite();
        StartCoroutine(Mixing());
        textSumMove.text = "" + SumMoves;
    }

    private void ReWrite()//При старте и после "Показать оригинал" + "Перемешать" идет полная перезапись геймобджектов
    {
        EmptySlot[0] = 3;
        EmptySlot[1] = 3;
        oldEmptySlot[0] = 3;
        oldEmptySlot[1] = 3;
        for (int i = 0; i <= 3; i++)
        {
            for (int j = 0; j <= 3; j++)
            {
                if (i != 3 || j != 3)
                {
                    GOGameElement[i, j] = GamePanel.transform.FindChild(i + "_" + j).FindChild("Image").gameObject;
                }
                originalSlot[i, j] = i * 10 + j;

            }
        }
        
    }
	
    public void ElementButton(int index)
    {
        if (GOGameElement[index / 10, index % 10]
            //проверка на то, щелчек это по изображению или по свободной клетке
            && ((index / 10 == EmptySlot[0] && (index % 10 + 1 == EmptySlot[1] || index % 10 - 1 == EmptySlot[1])) || (index % 10 == EmptySlot[1] && (index / 10 + 1 == EmptySlot[0] || index / 10 - 1 == EmptySlot[0]))))
            //првоерка на то, есть ли рядом пустая клетка, куда можно было бы переместить текущую ячейку
        {
            MoveElement(index);
            SumMoves += 1;
            textSumMove.text = "" + SumMoves;
        }
    }

    private void MoveElement(int index)
    {
        originalSlot[EmptySlot[0], EmptySlot[1]] += originalSlot[index / 10, index % 10];
        originalSlot[index / 10, index % 10] = originalSlot[EmptySlot[0], EmptySlot[1]] - originalSlot[index / 10, index % 10];
        originalSlot[EmptySlot[0], EmptySlot[1]] -= originalSlot[index / 10, index % 10];

        GOGameElement[index / 10, index % 10].transform.SetParent(GamePanel.transform.FindChild(EmptySlot[0] + "_" + EmptySlot[1]));
        GOGameElement[index / 10, index % 10].transform.DOLocalMove(new Vector2(0, 0), durationMove);
        GOGameElement[EmptySlot[0], EmptySlot[1]] = GOGameElement[index / 10, index % 10];
        GOGameElement[index / 10, index % 10] = null;

        EmptySlot[0] = index / 10;
        EmptySlot[1] = index % 10;
    }

    public void RemixingButton()
    {
        if(original)
        {
            ReWrite();
            original = false;
        }
        StartCoroutine(Mixing());
        SumMoves = 0;
        textSumMove.text = "" + SumMoves;
    }

    public IEnumerator Mixing()//перемешивание сделал самое банальное. Если делать что-то серьезное - его конечно нужно дорабатывать, но решил не тратить на это время
    {
        for (int k = 1; k <= 50; k++)
        {            
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (Random.Range(0f, 1f) > 0.5
                        //чтобы избежать одинакового перемешивания - добавил рандом
                        && GOGameElement[i, j]
                        && ((i == EmptySlot[0] && (j + 1 == EmptySlot[1] || j - 1 == EmptySlot[1])) || (j == EmptySlot[1] && (i + 1 == EmptySlot[0] || i - 1 == EmptySlot[0])))
                        && (i != oldEmptySlot[0] || j != oldEmptySlot[1]))
                        //при перемешивании исключил 2 взаимоисключающих движения. То есть не будет такого, что ячейка передвинулась и на следующий ход она же возвращается обратно
                    {
                        oldEmptySlot[0] = EmptySlot[0];
                        oldEmptySlot[1] = EmptySlot[1];
                        MoveElement(i * 10 + j);
                        yield return new WaitForSeconds(0.07f);
                    }
                }
            }
        }
    }

    public void OriginalButton()
    {
        if(!original)
        {
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (originalSlot[i, j] != 33)//исключение пробела
                    {
                        GOGameElement[i, j].transform.SetParent(GamePanel.transform.FindChild(originalSlot[i,j]/10 + "_" + originalSlot[i, j] % 10));
                        GOGameElement[i, j].transform.DOLocalMove(new Vector2(0, 0), durationMove);
                        //гейм обжект временно нереходит на свое родное место(включая родство)
                        //если нажать опять "показать оригинал", то у меня есть originalSlot, по которому я восстанавливаю прошлые позиции
                        //если перемешивание, то идет перезапись, которая была в начале и по сути как новая игра
                    }
                }
            }
            original = true;
        }
        else
        {
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (originalSlot[i, j] != 33)
                    {
                        GOGameElement[i, j].transform.SetParent(GamePanel.transform.FindChild(i + "_" + j));
                        GOGameElement[i, j].transform.DOLocalMove(new Vector2(0, 0), durationMove);
                    }
                }
            }
            original = false;
        }
    }
}
