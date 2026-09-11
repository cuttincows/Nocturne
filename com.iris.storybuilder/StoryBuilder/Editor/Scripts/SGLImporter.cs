using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

[UnityEditor.AssetImporters.ScriptedImporter(1, "sgl")]
public class SGLImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx) {
        TextAsset subAsset = new TextAsset( File.ReadAllText( ctx.assetPath ) );
        ctx.AddObjectToAsset( "text", subAsset );
        ctx.SetMainObject( subAsset );
    }
}
