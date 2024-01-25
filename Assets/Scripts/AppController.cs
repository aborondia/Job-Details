using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static AppController Active;
    [SerializeField] private MailSender mailSender;
    public MailSender MailSender => mailSender;
    [SerializeField] private DocumentCreator documentCreator;
    public DocumentCreator DocumentCreator => documentCreator;

    private void Awake()
    {
        if (Active != null)
        {
            GameObject.Destroy(Active);
        }

        Active = this;
    }
}
