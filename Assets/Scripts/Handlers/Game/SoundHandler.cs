using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour {

    public AudioClip buildingPlaced;
    public AudioClip notification;

    public AudioSource source;

    float soundDelay = 0;

    public static SoundHandler instance;

    private void Awake()
    {
        instance = this;
    }

    public void RegisterCallbacks()
    {
        source = Camera.main.GetComponent<AudioSource>();
        Building.RegisterOnBuildingPlaced(OnBuildingPlaced);
        BuildingListUI.RegisterOnNotificationDisplayed(OnNotificationDisplayed);
    }

    private void OnDisable()
    {
        Building.UnregisterOnBuildingPlaced(OnBuildingPlaced);
        BuildingListUI.UnregisterOnNotificationDisplayed(OnNotificationDisplayed);
    }

    private void Update()
    {
        if(soundDelay > 0)
        {
            soundDelay -= Time.deltaTime;
        }
    }

    public void PlaySound(AudioClip sound)
    {
        if (soundDelay > 0)
            return;
        soundDelay = 0.5f;
        source.clip = sound;
        source.Play();
    }

    void OnBuildingPlaced(Building building)
    {
        PlaySound(buildingPlaced);
    }

    void OnNotificationDisplayed()
    {
        PlaySound(notification);
    }

}
