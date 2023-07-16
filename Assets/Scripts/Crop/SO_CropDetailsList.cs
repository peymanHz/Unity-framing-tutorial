using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptble Objects/Crop/Crop Details List")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField]
    public List<CropDetails> cropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return cropDetails.Find(X => X.seedItemCode == seedItemCode);
    }
}
