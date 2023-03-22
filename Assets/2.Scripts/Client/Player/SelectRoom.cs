using UnityEngine;
using UnityEngine.UI;

public class SelectRoom : MonoBehaviour
{
    public Image[] gi;

    public void Selected_Room(int i)
    {
        for(int j = 0; j < gi.Length; j++)
        {
            if(j.Equals(i))
                gi[j].color = new Color(0, 0, 0, 1);
            else
                gi[j].color = new Color(0, 0, 0, 0);
        }
    }
}
