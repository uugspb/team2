﻿using System;
using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private float shootingAnimRange = 2;
    public Animator avatarAnimator;
    public event Action OnDead;
    public float Health;
    public ResourceBar HealthBar;
    [Header("Откуда вылетает пуля")]
    public Transform bulletStartPlace;

    public PlayerModel playerBase;
    public GunModel gun;
    UserSettings settings;

    private bool isRunning = false;

    public static Tower Instance
    {
        get; private set;
    }

    private Tower()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        HealthBar.maxAmount = playerBase.MaxHealth/50;
        HealthBar.amount = playerBase.Health/50;

        playerBase.Health += playerBase.RegenPerSecond;
        if (playerBase.Health > playerBase.MaxHealth)
            playerBase.Health = playerBase.MaxHealth;
    }

    public void Initialize()
    {
        playerBase = new PlayerModel();
        gun = new GunModel();
        settings = FindObjectOfType<UserSettings>();
        settings.Init(playerBase, gun);
    }

    public void StartGame()
    {
        isRunning = true;
        Flush();
        StartCoroutine(DamageCoroutine());
    }

    public void StopGame()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    public void Flush()
    {
        playerBase.Health = playerBase.MaxHealth;
    }

    public void LevelUp(int delta)
    {
        playerBase.skillPoints += delta;
    }

    public void Damage(float count)
    {
        Debug.Log("Tower damaged : old = " + playerBase.Health + " :: new = " + (playerBase.Health - count).ToString());
        playerBase.Health -= count;
        if (playerBase.Health <= 0)
            Dead();
    }

    private IEnumerator DamageCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / gun.fireRate);
            ShowShotAnim();
            GameController.Instance.Damage(gun);
        }
    }

    private void ShowShotAnim()
    {
        avatarAnimator.SetBool("shot", true);
        LeanTween.delayedCall(shootingAnimRange, HideShotAnim);
    }

    private void HideShotAnim()
    {
        avatarAnimator.SetBool("shot", false);
    }

    private void Dead()
    {
        if (OnDead != null)
            OnDead();
    }
}
