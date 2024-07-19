using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static AppController Active;
    [SerializeField] private MailSender mailSender;
    public MailSender MailSender => mailSender;
    [SerializeField] private UserDataHandler userDataHandler;
    public UserDataHandler UserDataHandler => userDataHandler;
    [SerializeField] private CleanerDataHandler cleanerDataHandler;
    public CleanerDataHandler CleanerDataHandler => cleanerDataHandler;
    [SerializeField] private DocumentCreator documentCreator;
    public DocumentCreator DocumentCreator => documentCreator;
    [SerializeField] private DetailsReportsHandler detailsReportsHandler;
    public DetailsReportsHandler DetailsReportsHandler => detailsReportsHandler;
    [SerializeField] private ServerCommunicator serverCommunicator;
    public ServerCommunicator ServerCommunicator => serverCommunicator;

    private void Awake()
    {
        if (Active != null)
        {
            GameObject.Destroy(Active);
        }

        Active = this;
    }
}
