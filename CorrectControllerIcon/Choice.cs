using System;
using LethalCompanyInputUtils.Glyphs;

namespace CorrectControllerIcon
{
    internal static class Choice
    {
        internal static bool PlayStation => Plugin.Icons.Value == ControllerIcons.PlayStation;

        // InputUtils' icon set for the chosen controller. Null on Auto, or before InputUtils has loaded its icons.
        internal static ControllerGlyph Glyphs
        {
            get
            {
                if (Plugin.Icons.Value == ControllerIcons.Auto)
                    return null;
                foreach (ControllerGlyph glyph in ControllerGlyph.Instances)
                {
                    if (glyph == null || glyph == ControllerGlyph.MouseGlyphs)
                        continue;
                    if (IsPlayStation(glyph) == PlayStation)
                        return glyph;
                }
                return null;
            }
        }

        static bool IsPlayStation(ControllerGlyph glyph)
        {
            if (glyph.name.IndexOf("dual", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            foreach (string type in glyph.validGamepadTypes)
                if (type.IndexOf("Dual", StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            return false;
        }
    }
}
