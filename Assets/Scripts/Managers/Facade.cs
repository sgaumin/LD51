public static class Facade
{
	public static PlayerController Player => PlayerController.Instance;
	public static SceneBase Level => SceneBase.Instance;
	public static PrefabsData Prefabs => PrefabsData.Instance;
	public static SettingsData Settings => SettingsData.Instance;
	public static CardHUD Card => CardHUD.Instance;
}