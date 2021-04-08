using System;
using COM3D2.NPRShader.Plugin;
using UnityEngine;
using UnityEngine.Rendering;

namespace COM3D2.NPRShader.Managed
{
	public static class NPRShaderManaged
	{
		public static bool ChangeNPRSMaterial(TBody inst, string f_strSlotName, int f_nMatNo, string f_strFileName)
		{
			COM3D2.NPRShader.Plugin.NPRShader.reloadMaidViewRequest = true;
			if (!f_strFileName.Contains("_NPRMAT_"))
			{
				return false;
			}
			if (!TBody.hashSlotName.ContainsKey(f_strSlotName))
			{
				NDebug.Assert("マテリアル変更：スロット名がみつかりません。：" + f_strSlotName, false);
				return false;
			}
			int index = (int)TBody.hashSlotName[f_strSlotName];
			TBodySkin tbodySkin = inst.goSlot[index];
			GameObject obj = tbodySkin.obj;
			if (obj == null)
			{
				return false;
			}
			Debug.Log(string.Concat(new string[]
			{
				"NPRShader :slot:",
				f_strSlotName,
				":MatNo:",
				f_nMatNo.ToString(),
				"のシェーダーを変更します"
			}));
			Debug.Log("menu: " + tbodySkin.m_mp.strFileName);
			Debug.Log("mate: " + f_strFileName);
			foreach (Transform transform in obj.transform.GetComponentsInChildren<Transform>(true))
			{
				SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
				if (component != null && component.material != null && f_nMatNo < component.materials.Length)
				{
					Material[] array = new Material[component.materials.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = new Material(component.materials[j]);
					}
					UnityEngine.Object.DestroyImmediate(array[f_nMatNo]);
					array[f_nMatNo] = COM3D2.NPRShader.Plugin.NPRShader.LoadMaterial(f_strFileName, tbodySkin, null);
					array[f_nMatNo].name = f_strFileName;
					for (int k = 0; k < component.materials.Length; k++)
					{
						UnityEngine.Object.DestroyImmediate(component.materials[k]);
					}
					component.materials = array;
					component.sharedMesh.RecalculateTangents();
					if (f_strFileName.Contains("_Reflection_"))
					{
						component.reflectionProbeUsage = ReflectionProbeUsage.BlendProbesAndSkybox;
					}
					f_strFileName.Contains("_Emissiv_");
					return true;
				}
			}
			return false;
		}

		public static bool CheckNPRShader(TBody inst, string f_strSlotName, int f_nMatNo, string f_strShaderFileName)
		{
			if (!TBody.hashSlotName.ContainsKey(f_strSlotName))
			{
				return false;
			}
			int index = (int)TBody.hashSlotName[f_strSlotName];
			TBodySkin tbodySkin = inst.goSlot[index];
			GameObject obj = tbodySkin.obj;
			if (obj == null)
			{
				return false;
			}
			foreach (Transform transform in obj.transform.GetComponentsInChildren<Transform>(true))
			{
				Renderer component = transform.GetComponent<Renderer>();
				if (component != null && component.material != null && f_nMatNo < component.materials.Length && component.materials[f_nMatNo].shader.name.Contains("com3d2mod") && (f_strSlotName == "head" || f_strSlotName == "body"))
				{
					Debug.Log("NPRShader : NPRシェーダーを使用中の為、" + f_strShaderFileName + "へのシェーダー変更処理を中止します。");
					return true;
				}
			}
			return false;
		}

		public static void ChangeBg(BgMgr inst, string f_strPrefubName)
		{
			COM3D2.NPRShader.Plugin.NPRShader.bUpdateCubeMapRequest = true;
			COM3D2.NPRShader.Plugin.NPRShader.reloadMaterialRequest = true;
		}

		public static bool ThumShotCamMove(Maid inst)
		{
			COM3D2.NPRShader.Plugin.NPRShader.bRestorSkyboxRequest = true;
			return false;
		}

		public static bool PresetSave()
		{
			COM3D2.NPRShader.Plugin.NPRShader.bRestorSkyboxRequest = true;
			return false;
		}
	}
}
