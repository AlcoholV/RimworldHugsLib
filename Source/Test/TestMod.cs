#if TEST_MOD
using System.Collections.Generic;
using System.Linq;
using HugsLib.Settings;
using HugsLib.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Verse;

namespace HugsLib.Test {
	// This mod is for testing the various facilities of the library
	public class TestMod2 : ModBase {
		public override string ModIdentifier {
			get { return "TestMod2"; }
		}
	}

	public class TestMod : ModBase {
		public static TestMod Instance { get; private set; }

		internal new ModLogger Logger {
			get { return base.Logger; }
		}

		public TestMod() {
			Instance = this;
		}

		public override string ModIdentifier {
			get { return "TestMod"; }
		}

		public override void Initialize() {
			Logger.Message("Initialized");
		}

		public override void Tick(int currentTick) {
			//Logger.Message("Tick:"+currentTick);
		}
		
		public override void Update() {
			//Logger.Message("Update");
		}

		public override void FixedUpdate() {
			//Logger.Message("FixedUpdate");
		}

		public override void OnGUI() {
			//Logger.Message("OnGUI");
		}

		public override void WorldLoaded() {
			Logger.Message("WorldLoaded");
			UtilityWorldObjectManager.GetUtilityWorldObject<TestUWO1>().UpdateAndReport();
			UtilityWorldObjectManager.GetUtilityWorldObject<TestUWO2>().UpdateAndReport();
		}

		public override void MapComponentsInitializing(Map map) {
			Logger.Message("MapComponentsInitializing on map:"+map);
		}

		public override void MapLoaded(Map map) {
			Logger.Message("MapLoaded:"+map);
			// this will produce an exception if the map mesh was not regenerated yet
			map.mapDrawer.MapMeshDirty(new IntVec3(0, 0, 0), MapMeshFlag.Buildings);
			
			//HugsLibController.Instance.CallbackScheduler.ScheduleCallback(() => Logger.Trace("scheduler callback"), 150, true);

		}

		public override void SceneLoaded(Scene scene) {
			Logger.Message("SceneLoaded:"+scene.name);
		}

		public override void SettingsChanged() {
			Logger.Message("SettingsChanged");
		}

		private enum HandleEnum {
			DefaultValue,
			ValueOne,
			ValueTwo
		}

		public override void DefsLoaded() {
			Logger.Message("DefsLoaded");
			var str = Settings.GetHandle("str", "String value", "", "value");
			var spinner = Settings.GetHandle("intSpinner", "Spinner", "desc", 5, Validators.IntRangeValidator(0, 30));
			spinner.SpinnerIncrement = 2;
			var enumHandle = Settings.GetHandle("enumThing", "Enum setting", "", HandleEnum.DefaultValue, null, "test_enumSetting_");
			var toggle = Settings.GetHandle("toggle", "Toggle setting", "Toggle setting", false);
			//TestCustomTypeSetting();
			//TestConditionalVisibilitySettings();	
		}

		private void TestConditionalVisibilitySettings() {
			for (int i = 0; i < 50; i++) {
				var toggle = Settings.GetHandle("toggle"+i, "toggle", null, false);
				var index = i;
				toggle.VisibilityPredicate = () => Input.mousePosition.x/22 < index;
			}
		}

		private void TestCustomTypeSetting() {
			var custom = Settings.GetHandle<CustomHandleType>("customType", null, null);
			custom.NeverVisible = true;
			if (custom.Value == null) custom.Value = new CustomHandleType { nums = new List<int>() };
			if (custom.Value.nums.Count < 10) custom.Value.nums.Add(Rand.Range(1, 100));
			HugsLibController.SettingsManager.SaveChanges();
			Logger.Trace("Custom setting values: " + custom.Value.nums.Join(","));
			
		}

		private class CustomHandleType : SettingHandleConvertible {
			public List<int> nums = new List<int>();

			public override void FromString(string settingValue) {
				nums = settingValue.Length > 0 ? settingValue.Split('|').Select(int.Parse).ToList() : new List<int>();
			}

			public override string ToString() {
				return nums != null ? nums.Join("|") : "";
			}
		}
	 
	}
}
#endif