using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BossSettings : ScriptableObject
{
    public String bossName;
    public int bossMaxHp;
    public float secondsBetweenAttacks;
    public BulletPattern[] bulletPatterns;

}
