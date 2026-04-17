using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public Color[] material;
    public SkinnedMeshRenderer[] partesDelCuerpo;

    public PlayerRenderers[] playerRenderer;
    public Dictionary<string,Skins> skins;

    public void Start()
    {
        // Cuando consiga la informacion del jugador, quiero cambiar la skin de mi player
        PlayfabManager._PlayfabManager.GetPlayerData(new Action[] { RpcApplySkin });
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.All)]
    public void RpcApplySkin()
    {
        for (int i = 0; i < playerRenderer.Length; i++)
        { 
        
            if(playerRenderer[i].skinnedMeshRenderer != null) // Si mi skinnedMeshRenderer tiene algo, lo cambio
            {
                // Tengo mi indice de que skin tengo puesta. Si mi indice es diferente al de playfab, lo cambio
                // Body
                if(playerRenderer[i].skinIndex != int.Parse(PlayfabManager._PlayfabManager.playerData[playerRenderer[i].name].Value))
                {
                    ///   mi skin actual            = De mi diccionario de skins, el skin con indice 
                    playerRenderer[i].skinnedMeshRenderer =
                        skins[playerRenderer[i].name].skinnedMeshRenderer[int.Parse(PlayfabManager._PlayfabManager.playerData[playerRenderer[i].name].Value)];
                      // De mi diccionario voy a buscar las skins de "Body"
                }
            }
            else if(playerRenderer[i].renderer != null) // Si no, reviso si mi renderer tiene algo dentro y lo cambio
            {
                if (playerRenderer[i].skinIndex != int.Parse(PlayfabManager._PlayfabManager.playerData[playerRenderer[i].name].Value))
                {
                    playerRenderer[i].renderer =
                        skins[playerRenderer[i].name].renderer[
                            int.Parse(PlayfabManager._PlayfabManager.playerData[playerRenderer[i].name].Value)
                        ];
                }
            }
        
        }

        for (int i = 0; i < partesDelCuerpo.Length; i++) 
        {
            partesDelCuerpo[i].material.color =
                material[int.Parse(PlayfabManager._PlayfabManager.playerData["Material"].Value)];
        }
    }
}

[Serializable]
public struct PlayerRenderers
{
    public string name;
    public SkinnedMeshRenderer skinnedMeshRenderer; // Este renderer es para lo que tiene rig
    public Renderer renderer; // Este renderer es para lo que no tiene rig, es decir, lo normal
    public int skinIndex;
}


[Serializable]
public struct Skins
{
    public string name;
    public SkinnedMeshRenderer[] skinnedMeshRenderer; // Este renderer es para lo que tiene rig
    public Renderer[] renderer; // Este renderer es para lo que no tiene rig, es decir, lo normal
    public Material[] material;
    public int skinIndex;
}

