using UnityEngine;

public class Item : MonoBehaviour
{

    [SerializeField] private int _id;
    [SerializeField] private int _prize;
    [SerializeField] private string _nombre;
    [SerializeField] private string _descripcion;
    [SerializeField] private string _descripcionIng;
    [SerializeField] private string _rarity;
    [SerializeField] private string _type;
    [SerializeField] private Sprite _sprite2D_UI;
    [SerializeField] private GameObject _modelo;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }  
    public int Prize
    {
        get { return _prize; }
        set { _prize = value; }
    }

    public string Nombre
    {
        get { return _nombre; }
        set { _nombre = value; }
    }

    public string Descripcion
    {
        get { return _descripcion; }
        set { _descripcion = value; }
    }

    public string DescripcionIng
    {
        get { return _descripcionIng; }
        set { _descripcionIng = value; }
    }

    public string Rarity
    {
        get { return _rarity; }
        set { _rarity = value; }
    }
    
    public string Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public Sprite Sprite2D_UI
    {
        get { return _sprite2D_UI; }
        set { _sprite2D_UI = value; }
    }

    public GameObject Modelo
    {
        get { return _modelo; }
        set { _modelo = value; }
    }

   


}
