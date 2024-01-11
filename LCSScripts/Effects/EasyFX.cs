using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PFX
{
    Undefined = -1, Dirt, Wood, Stone, Metal
}
public enum SFX
{
    Undefined = -1, DirtHit, WoodChop, StoneHit, MetalHit, WoodSplit, WoodStab, BarkScrape
}

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class EasyFX : MonoBehaviour
{
    // Arrays initialized to be called FX on collision events (Collision Event Subscriber)
    [Header("AutoFX")]
    public PhysicMaterial[] physicMaterials;
    public ParticleSystem[] physicMaterialsPFX;
    public AudioContainer[] physicMaterialsSFX;

    // Arrays initialized to call FX defined by Enum (Deactivatable, Splittable classes)
    [Header("EasyFX")]
    public ParticleSystem[] particleEffects;
    public AudioContainer[] soundEffects;

    private void OnEnable()
    {
        // AutoFX
        LoadPhysicMaterials();
        LoadPhysicMaterialsPFX();
        LoadPhysicMaterialsSFX();
        CollisionEventSubscriber.OnCollisionEvent += HandleCollision;
        // AutoFX

        // EasyFX
        LoadPFX();
        LoadSFX();
        // EasyFX
    }
    // AutoFX
    public void LoadPhysicMaterials()
    {
        object loadedPhysicMaterial0 = Resources.Load("PhysicMaterials/Dirt", typeof(PhysicMaterial));
        object loadedPhysicMaterial1 = Resources.Load("PhysicMaterials/Wood", typeof(PhysicMaterial));
        object loadedPhysicMaterial2 = Resources.Load("PhysicMaterials/Stone", typeof(PhysicMaterial));
        object loadedPhysicMaterial3 = Resources.Load("PhysicMaterials/Metal", typeof(PhysicMaterial));

        physicMaterials = new PhysicMaterial[4];
        physicMaterials[0] = (PhysicMaterial)loadedPhysicMaterial0;
        physicMaterials[1] = (PhysicMaterial)loadedPhysicMaterial1;
        physicMaterials[2] = (PhysicMaterial)loadedPhysicMaterial2;
        physicMaterials[3] = (PhysicMaterial)loadedPhysicMaterial3;
    }
    public void LoadPhysicMaterialsPFX()
    {
        object loadedParticleSystem0 = Resources.Load("PFX/hitDirt", typeof(ParticleSystem));
        object loadedParticleSystem1 = Resources.Load("PFX/hitWood", typeof(ParticleSystem));
        object loadedParticleSystem2 = Resources.Load("PFX/hitStone", typeof(ParticleSystem));
        object loadedParticleSystem3 = Resources.Load("PFX/hitMetal", typeof(ParticleSystem));

        physicMaterialsPFX = new ParticleSystem[4];
        physicMaterialsPFX[0] = (ParticleSystem)loadedParticleSystem0;
        physicMaterialsPFX[1] = (ParticleSystem)loadedParticleSystem1;
        physicMaterialsPFX[2] = (ParticleSystem)loadedParticleSystem2;
        physicMaterialsPFX[3] = (ParticleSystem)loadedParticleSystem3;
    }
    public void LoadPhysicMaterialsSFX()
    {
        GameObject dirtHit = Resources.Load("SFX/DirtWoodHit") as GameObject;
        GameObject woodChop = Resources.Load("SFX/WoodChop") as GameObject;
        GameObject stoneHit = Resources.Load("SFX/StoneHit") as GameObject;
        GameObject metalHit = Resources.Load("SFX/MetalHit") as GameObject;
        GameObject woodSplit = Resources.Load("SFX/WoodSplit") as GameObject;
        GameObject woodStab = Resources.Load("SFX/WoodStab") as GameObject;
        GameObject barkScrape = Resources.Load("SFX/BarkScrape") as GameObject;

        physicMaterialsSFX = new AudioContainer[4];
        physicMaterialsSFX[0] = dirtHit.GetComponent<AudioContainer>();
        physicMaterialsSFX[1] = dirtHit.GetComponent<AudioContainer>();
        physicMaterialsSFX[2] = dirtHit.GetComponent<AudioContainer>();
        physicMaterialsSFX[3] = metalHit.GetComponent<AudioContainer>();
    }
    private void HandleCollision(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 position = collision.contacts[0].point;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        PhysicMaterial material1 = collision.contacts[0].thisCollider.sharedMaterial;
        PhysicMaterial material2 = collision.contacts[0].otherCollider.sharedMaterial;
        float velocity = collision.relativeVelocity.magnitude;

        int FXIndex = SetFXIndex(material1, material2);

        if (FXIndex >= 0)
        {
            float scale = SetScale(velocity);
            CollisionPFX(FXIndex, position, rotation, scale);
            CollisionSFX(FXIndex, position, scale);
        }
    }
    private int SetFXIndex(PhysicMaterial material1, PhysicMaterial material2)
    {
        int m1Index = -1;
        int m2Index = -1;
        int FXIndex = -1;

        for (int i = 0; i < physicMaterials.Length; i++)
        {
            if (material1?.name == physicMaterials?[i].name)
            {
                m1Index = i;
            }

            if (material2?.name == physicMaterials?[i].name)
            {
                m2Index = i;
            }
        }

        // If materials are not in list, return index -1 to play nothing, else return lower index (lower index has higher precedence)
        if (m1Index == -1 || m2Index == -1)
        {
            FXIndex = -1;
            return FXIndex;
        }

        if (m1Index > m2Index)
            FXIndex = m2Index;
        else if (m1Index < m2Index)
            FXIndex = m1Index;
        else if (m1Index == m2Index)
            FXIndex = m1Index;

        Debug.Log(FXIndex);
        return FXIndex;
    }
    public float SetScale(float velocity)
    {
        float scale = 0.2f;
        if (velocity > 2.0f && velocity < 4.0f)
            scale = 0.2f;
        else if (velocity > 4.0f && velocity < 6.0f)
            scale = 0.4f;
        else if (velocity > 6.0f && velocity < 8.0f)
            scale = 0.6f;
        else if (velocity > 8.0f && velocity < 10.0f)
            scale = 0.8f;
        else if (velocity > 10.0f)
            scale = 1.0f;

        return scale;
    }
    public void CollisionPFX(int FXIndex, Vector3 position, Quaternion rotation, float scale)
    {
        if (physicMaterialsPFX?.Length > FXIndex)
        {
            ParticleSystem effectInstance = Instantiate(physicMaterialsPFX[FXIndex], position, rotation);
            effectInstance.transform.localScale = new Vector3(scale, scale, scale);
            effectInstance.Play();
            Destroy(effectInstance, 5);
        }
    }
    public void CollisionSFX(int FXIndex, Vector3 position, float scale)
    {
        if (physicMaterialsSFX.Length > FXIndex)
            physicMaterialsSFX?[FXIndex].GetRandomAudioClip(position, (scale * 0.5f));
    }
    // AutoFX

    // EasyFX
    public void LoadPFX()
    {
        object loadedParticleSystem0 = Resources.Load("PFX/hitDirt", typeof(ParticleSystem));
        object loadedParticleSystem1 = Resources.Load("PFX/hitWood", typeof(ParticleSystem));
        object loadedParticleSystem2 = Resources.Load("PFX/hitStone", typeof(ParticleSystem));
        object loadedParticleSystem3 = Resources.Load("PFX/hitMetal", typeof(ParticleSystem));

        particleEffects = new ParticleSystem[4];
        particleEffects[(int)PFX.Dirt] = (ParticleSystem)loadedParticleSystem0;
        particleEffects[(int)PFX.Wood] = (ParticleSystem)loadedParticleSystem1;
        particleEffects[(int)PFX.Stone] = (ParticleSystem)loadedParticleSystem2;
        particleEffects[(int)PFX.Metal] = (ParticleSystem)loadedParticleSystem3;
    }
    public void LoadSFX()
    {
        GameObject dirtHit = Resources.Load("SFX/DirtWoodHit") as GameObject;
        GameObject woodChop = Resources.Load("SFX/WoodChop") as GameObject;
        GameObject stoneHit = Resources.Load("SFX/StoneHit") as GameObject;
        GameObject metalHit = Resources.Load("SFX/MetalHit") as GameObject;
        GameObject woodSplit = Resources.Load("SFX/WoodSplit") as GameObject;
        GameObject woodStab = Resources.Load("SFX/WoodStab") as GameObject;
        GameObject barkScrape = Resources.Load("SFX/BarkScrape") as GameObject;

        soundEffects = new AudioContainer[7];
        soundEffects[(int)SFX.DirtHit] = dirtHit.GetComponent<AudioContainer>();
        soundEffects[(int)SFX.WoodChop] = woodChop.GetComponent<AudioContainer>();
        soundEffects[(int)SFX.StoneHit] = stoneHit.GetComponent<AudioContainer>();
        soundEffects[(int)SFX.MetalHit] = metalHit.GetComponent<AudioContainer>();
        soundEffects[(int)SFX.WoodSplit] = woodSplit.GetComponent<AudioContainer>();
        soundEffects[(int)SFX.WoodStab] = woodStab.GetComponent<AudioContainer>();
        soundEffects[(int)SFX.BarkScrape] = barkScrape.GetComponent<AudioContainer>();
    }
    public void PlayPFX(int FXIndex, Vector3 position, Quaternion rotation, float scale)
    {
        if (particleEffects?.Length > FXIndex)
        {
            ParticleSystem effectInstance = Instantiate(particleEffects[FXIndex], position, rotation);
            effectInstance.transform.localScale = new Vector3(scale, scale, scale);
            effectInstance.Play();
            Destroy(effectInstance, 5);
        }
    }
    public void PlaySFX(int FXIndex, Vector3 position, float scale)
    {
        if (soundEffects.Length > FXIndex)
            soundEffects?[FXIndex].GetRandomAudioClip(position, (scale * 0.5f));
    }
    // EasyFX
}
