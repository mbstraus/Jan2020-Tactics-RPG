using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public enum UnitTeam
    {
        PLAYER, ENEMY
    }
    [SerializeField] public string Name;
    [SerializeField] public int CurrentHealthPoints;
    [SerializeField] public int MaxHealthPoints;
    [SerializeField] public int Level;
    [SerializeField] public int Strength;
    [SerializeField] public int Magic;
    [SerializeField] public int Dexterity;
    [SerializeField] public int Speed;
    [SerializeField] public int Luck;
    [SerializeField] public int Defense;
    [SerializeField] public int Resistance;
    [SerializeField] public int Movement;
    [SerializeField] public UnitTeam Team;
}
