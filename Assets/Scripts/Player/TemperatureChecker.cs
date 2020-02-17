using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureChecker : MonoBehaviour
{
    public bool enable = false;

    void Create() {
        enable = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enable.Equals(false)) return;

        if (collision.tag.Equals("Civilian")) {
            collision.GetComponent<Civilian>().DisplayTemperature();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enable.Equals(false)) return;

        if (collision.tag.Equals("Civilian")){
            collision.GetComponent<Civilian>().DisableTemperature();
        }
    }
}
