using AOI;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Text infoText;

    private LogicEntity _entity;
    
    public void Init(LogicEntity entity)
    {
        this._entity = entity;
        infoText.text = this._entity.ToString();
    }

    public void TurnOn()
    {
        this.GetComponent<Image>().color = Color.red;
    }

    public void TurnOff()
    {
        this.GetComponent<Image>().color = Color.grey;
    }
    
    public void TurnSelected()
    {
        this.GetComponent<Image>().color = Color.blue;
    }

}
