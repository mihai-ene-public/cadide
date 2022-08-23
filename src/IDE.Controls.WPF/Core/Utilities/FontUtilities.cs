/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace IDE.Controls.WPF.Core.Utilities
{
    internal class FontUtilities
    {
        internal static IEnumerable<FontFamily> Families
        {
            get
            {
                var fonts = new List<FontFamily>();
                foreach (var font in Fonts.SystemFontFamilies)
                {
                    try
                    {
                        // In WPF 4, this will throw an exception.
                        var throwAcess = font.FamilyNames;
                        fonts.Add(font);
                    }
                    catch
                    {
                    }
                }

                return fonts;
            }
        }

        internal static IEnumerable<FontWeight> Weights
        {
            get
            {
                return new[]
                {
                    FontWeights.Black,
                    FontWeights.Bold,
                    FontWeights.ExtraBlack,
                    FontWeights.ExtraBold,
                    FontWeights.ExtraLight,
                    FontWeights.Light,
                    FontWeights.Medium,
                    FontWeights.Normal,
                    FontWeights.SemiBold,
                    FontWeights.Thin
                };
            }
        }

        internal static IEnumerable<FontStyle> Styles
        {
            get
            {
                return new[]
                {
                    FontStyles.Italic,
                    FontStyles.Normal
                };
            }
        }

        internal static IEnumerable<FontStretch> Stretches
        {
            get
            {
                return new[]
                {
                    FontStretches.Condensed,
                    FontStretches.Expanded,
                    FontStretches.ExtraCondensed,
                    FontStretches.ExtraExpanded,
                    FontStretches.Normal,
                    FontStretches.SemiCondensed,
                    FontStretches.SemiExpanded,
                    FontStretches.UltraCondensed,
                    FontStretches.UltraExpanded
                };

            }
        }
    }
}
