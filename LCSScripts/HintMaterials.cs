using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentMaterials
{
    Unselected,
    DisableMesh,
    Original,
    HintNotches,
    HintCorrect,
    HintIncorrect
}

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class HintMaterials : MonoBehaviour
{
    public MeshRenderer m_MeshRenderer;
    public Material[] materialsOriginal;
    public Material[] materialHintNotches;
    public Material[] materialHintCorrect;
    public Material[] materialHintIncorrect;

    [Header("Options")]
    public CurrentMaterials currentMaterials;

    void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        materialsOriginal = GetComponent<Renderer>().sharedMaterials;
        materialHintNotches = new Material[materialsOriginal.Length];
        materialHintCorrect = new Material[materialsOriginal.Length];
        materialHintIncorrect = new Material[materialsOriginal.Length];

        FindHintMaterials(materialHintNotches, "Materials/Hint_Notches");
        FindHintMaterials(materialHintCorrect, "Materials/Hint_Correct");
        FindHintMaterials(materialHintIncorrect, "Materials/Hint_Incorrect");
    }
    public void FindHintMaterials(Material[] list, string directory)
    {
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = Resources.Load(directory, typeof(Material)) as Material;
        }
    }
    
    public void DeactivateMesh()
    {
        if (m_MeshRenderer != null)
            currentMaterials = CurrentMaterials.DisableMesh;
        SetCorrectMaterial();
    }
    public void ActivateMaterialsOriginal()
    {
        if (materialsOriginal != null)
            currentMaterials = CurrentMaterials.Original;
        SetCorrectMaterial();
    }
    public void ActivateMaterialsHintNotches()
    {
        if (materialHintNotches != null)
            currentMaterials = CurrentMaterials.HintNotches;
        SetCorrectMaterial();
    }
    public void ActivateMaterialsHintCorrect()
    {
        if (materialHintCorrect != null)
            currentMaterials = CurrentMaterials.HintCorrect;
        SetCorrectMaterial();
    }
    public void ActivateMaterialsHintIncorrect()
    {
        if (materialHintIncorrect != null)
            currentMaterials = CurrentMaterials.HintIncorrect;
        SetCorrectMaterial();
    }

    public void SetCorrectMaterial()
    {
        m_MeshRenderer.enabled = true;

        if (currentMaterials == CurrentMaterials.DisableMesh)
            m_MeshRenderer.enabled = false;
        else if (currentMaterials == CurrentMaterials.Original)
            GetComponent<Renderer>().materials = materialsOriginal;
        else if (currentMaterials == CurrentMaterials.HintNotches)
            GetComponent<Renderer>().materials = materialHintNotches;
        else if (currentMaterials == CurrentMaterials.HintCorrect)
            GetComponent<Renderer>().materials = materialHintCorrect;
        else if (currentMaterials == CurrentMaterials.HintIncorrect)
            GetComponent<Renderer>().materials = materialHintIncorrect;
    }
}
