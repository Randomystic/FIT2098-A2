using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class TimeOfDayController : MonoBehaviour
{
	[Range(0, 100)] public float timeOfDay;

	public float transitionTime = 2f;

	public Material skyMaterial;
	public Light directionalLight;

	float lastTime = -1f;
	float environmentUpdateTimer;
	Coroutine timeRoutine;

	struct SkyKey
	{
		public float t;
		public Color top, middle, bottom, lightColor;
		public float lightIntensity, exp;
		public Vector3 lightRotation;

		public SkyKey(
			float t,
			string top,
			string middle,
			string bottom,
			string lightColor,
			float lightIntensity,
			Vector3 lightRotation,
			float exp)
		{
			this.t = t;
			this.top = Hex(top);
			this.middle = Hex(middle);
			this.bottom = Hex(bottom);
			this.lightColor = Hex(lightColor);
			this.lightIntensity = lightIntensity;
			this.lightRotation = lightRotation;
			this.exp = exp;
		}
	}

	static Color Hex(string hex)
	{
		ColorUtility.TryParseHtmlString(hex, out Color c);
		return c;
	}

	SkyKey[] keys = new SkyKey[]
	{
		new SkyKey(0,   "#17215E", "#FF6F61", "#FFD37A", "#FFB073", 0.35f, new Vector3(5,   -110, 0), 1.2f),
		new SkyKey(15,  "#3FA7FF", "#FFE5A3", "#B7F0FF", "#FFF0C2", 0.80f, new Vector3(25,  -75,  0), 1.0f),
		new SkyKey(30,  "#0B67D1", "#91D9FF", "#E8FBFF", "#FFF7DD", 1.15f, new Vector3(75,  0,    0), 0.8f),
		new SkyKey(45,  "#2D8CEB", "#BDEAFF", "#FFF3C2", "#FFE1A3", 0.95f, new Vector3(50,  65,   0), 1.0f),
		new SkyKey(60,  "#4F78D6", "#FFB36B", "#FF765E", "#FF9B50", 0.60f, new Vector3(18,  110,  0), 1.2f),
		new SkyKey(75,  "#1C2A7A", "#FF5B6E", "#FFB85C", "#FF7040", 0.25f, new Vector3(5,   135,  0), 1.4f),
		new SkyKey(88,  "#070D2E", "#263B7C", "#8A5BAA", "#637CFF", 0.04f, new Vector3(-8,  160,  0), 1.6f),
		new SkyKey(100, "#02040F", "#07133A", "#102050", "#A8C7FF", 0.10f, new Vector3(55,  180,  0), 1.8f),
	};

	void Start()
	{
		ApplyTimeOfDay();
		DynamicGI.UpdateEnvironment();
	}

	void Update()
	{
		if (!Mathf.Approximately(timeOfDay, lastTime))
		{
			ApplyTimeOfDay();
			DynamicGI.UpdateEnvironment();
		}
	}
	void OnValidate()
	{
		if (Application.isPlaying)
			return;

		ApplyTimeOfDay();
		DynamicGI.UpdateEnvironment();
	}


	public void AddTime(float amount)
	{
		SetTime(timeOfDay + amount);
	}

	public void SetTime(float targetTime)
	{
		targetTime = Mathf.Clamp(targetTime, 0f, 100f);

		if (!Application.isPlaying)
		{
			timeOfDay = targetTime;
			ApplyTimeOfDay();
			DynamicGI.UpdateEnvironment();
			return;
		}

		if (timeRoutine != null)
			StopCoroutine(timeRoutine);

		timeRoutine = StartCoroutine(TransitionTime(targetTime));
	}

	IEnumerator TransitionTime(float targetTime)
	{
		float startTime = timeOfDay;
		float duration = Mathf.Max(transitionTime, 0.01f);
		float timer = 0f;

		environmentUpdateTimer = 0f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			float blend = Mathf.Clamp01(timer / duration);

			timeOfDay = Mathf.Lerp(startTime, targetTime, blend);

			ApplyTimeOfDay();
			UpdateEnvironmentLighting();

			yield return null;
		}

		timeOfDay = targetTime;
		ApplyTimeOfDay();
		DynamicGI.UpdateEnvironment();

		timeRoutine = null;

		Debug.Log("Time of day moved to: " + timeOfDay);
	}

	void ApplyTimeOfDay()
	{
		if (!skyMaterial || !directionalLight)
			return;

		timeOfDay = Mathf.Clamp(timeOfDay, 0f, 100f);
		lastTime = timeOfDay;

		SkyKey a = keys[0];
		SkyKey b = keys[1];

		for (int i = 0; i < keys.Length - 1; i++)
		{
			if (timeOfDay >= keys[i].t && timeOfDay <= keys[i + 1].t)
			{
				a = keys[i];
				b = keys[i + 1];
				break;
			}
		}

		float blend = Mathf.InverseLerp(a.t, b.t, timeOfDay);
		blend = blend * blend * (3f - 2f * blend);

		skyMaterial.SetColor(
			"_TopColor",
			Color.Lerp(a.top, b.top, blend));

		skyMaterial.SetColor(
			"_MiddleColor",
			Color.Lerp(a.middle, b.middle, blend));

		skyMaterial.SetColor(
			"_BottomColor",
			Color.Lerp(a.bottom, b.bottom, blend));

		skyMaterial.SetFloat(
			"_Exp",
			Mathf.Lerp(a.exp, b.exp, blend));

		directionalLight.color =
			Color.Lerp(a.lightColor, b.lightColor, blend);

		directionalLight.intensity =
			Mathf.Lerp(a.lightIntensity, b.lightIntensity, blend);

		directionalLight.transform.rotation = Quaternion.Slerp(
			Quaternion.Euler(a.lightRotation),
			Quaternion.Euler(b.lightRotation),
			blend
		);

		RenderSettings.skybox = skyMaterial;
	}

	void UpdateEnvironmentLighting()
	{
		environmentUpdateTimer += Time.deltaTime;

		if (environmentUpdateTimer >= 0.5f)
		{
			DynamicGI.UpdateEnvironment();
			environmentUpdateTimer = 0f;
		}

	}

	[ContextMenu("Print Lighting State")]
	void PrintLightingState()
	{
		Debug.Log(
			"Time: " + timeOfDay +
			"\nActive Skybox: " +
			(RenderSettings.skybox ? RenderSettings.skybox.name : "None") +
			"\nAssigned Skybox: " +
			(skyMaterial ? skyMaterial.name : "None") +
			"\nAmbient Mode: " + RenderSettings.ambientMode +
			"\nAmbient Intensity: " + RenderSettings.ambientIntensity +
			"\nDirectional Light: " +
			(directionalLight ? directionalLight.name : "None") +
			"\nLight Intensity: " +
			(directionalLight ? directionalLight.intensity : 0f) +
			"\nLight Color: " +
			(directionalLight ? directionalLight.color.ToString() : "None")
		);
	}
}