using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactsMarker : MonoBehaviour
{
    public static ContactsMarker instance;
    [SerializeField]
    private GameObject originObject;
    [SerializeField]
    private List<GameObject> contacts = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public void Mark(GameObject _go,List<GameObject> _contacts) {
        if (originObject != null) {
            Reset();
        }

        originObject = _go;
        contacts = _contacts;

        originObject.GetComponent<Civilian>().spriteRender.color = Color.red;
        foreach (var i in contacts) {
            i.GetComponent<Civilian>().SetExpand(true);
        }
    }

    public void Reset()
    {
        if (originObject == null) return;

        originObject.GetComponent<Civilian>().spriteRender.color = Color.white;
        foreach (var i in contacts) {
            i.GetComponent<Civilian>().SetExpand(false);
        }

        originObject = null;
        contacts.Clear();
    }
}
