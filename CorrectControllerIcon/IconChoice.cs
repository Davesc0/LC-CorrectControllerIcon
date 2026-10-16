using System;
using System.Collections.Generic;
using HarmonyLib;
using LethalCompanyInputUtils;
using LethalCompanyInputUtils.Components;
using LethalCompanyInputUtils.Glyphs;
using LethalCompanyInputUtils.Localization;
using TMPro;
using UnityEngine;

namespace CorrectControllerIcon
{
    // The choice in the keybind menu, and InputUtils' own icons following it.
    internal static class IconChoice
    {
        const string ObjectName = "CorrectControllerIconChoice";
        static TMP_Dropdown icons;

        // InputUtils only looks at the device, and with Steam Input every controller is an Xbox one.
        [HarmonyPatch(typeof(ControllerGlyph), nameof(ControllerGlyph.GetBestMatching))]
        static class GlyphPatch
        {
            static void Postfix(ref ControllerGlyph __result)
            {
                if (__result != null && __result == ControllerGlyph.MouseGlyphs)
                    return;
                ControllerGlyph chosen = Choice.Glyphs;
                if (chosen != null)
                    __result = chosen;
            }
        }

        [HarmonyPatch(typeof(KepRemapPanel), "OnEnable")]
        static class PanelPatch
        {
            static void Postfix()
            {
                RemapContainerController container = LcInputActionApi.ContainerInstance;
                if (container == null)
                    return;
                if (container.transform.Find(ObjectName) == null)
                    Create(container);
                UpdateLabel();
            }
        }

        // A copy of InputUtils' own dropdown, in the top right corner over the controller column.
        static void Create(RemapContainerController container)
        {
            TMP_Dropdown original = container.overridePriorityDropdown;
            if (original == null || original.transform.parent == container.transform)
            {
                Plugin.mlg.LogWarning("InputUtils' dropdown is not where it used to be, the icon choice is only in the config.");
                return;
            }

            GameObject copy = UnityEngine.Object.Instantiate(original.transform.parent.gameObject, container.transform);
            copy.name = ObjectName;
            // These would write InputUtils' own texts back over ours.
            foreach (LocaleResolver resolver in copy.GetComponentsInChildren<LocaleResolver>(true))
                UnityEngine.Object.DestroyImmediate(resolver);
            Transform info = copy.transform.Find("Info Slot");
            if (info != null)
                info.gameObject.SetActive(false);

            RectTransform rect = (RectTransform)copy.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-15f, 45f);

            icons = copy.GetComponentInChildren<TMP_Dropdown>(true);
            icons.onValueChanged = new TMP_Dropdown.DropdownEvent();
            icons.ClearOptions();
            List<string> options = new List<string>();
            foreach (ControllerIcons choice in Enum.GetValues(typeof(ControllerIcons)))
                options.Add("Icons: " + choice);
            icons.AddOptions(options);
            icons.onValueChanged.AddListener(value => Plugin.Icons.Value = (ControllerIcons)value);
        }

        internal static void UpdateLabel()
        {
            if (icons != null)
                icons.SetValueWithoutNotify((int)Plugin.Icons.Value);
        }
    }
}
