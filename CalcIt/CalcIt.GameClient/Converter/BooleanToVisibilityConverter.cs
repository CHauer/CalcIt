// -----------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>CalcIt.GameClient - BooleanToVisibilityConverter.cs</summary>
// -----------------------------------------------------------------------
namespace CalcIt.GameClient.Converter
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Boolean To Visibility Converter.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">
        /// The source data being passed to the target.
        /// </param>
        /// <param name="targetType">
        /// The type of the target property. This uses a different type depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.
        /// </param>
        /// <param name="parameter">
        /// An optional parameter to be used in the converter logic.
        /// </param>
        /// <param name="culture">
        /// The culture parameter.
        /// </param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }

            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">
        /// The target data being passed to the source.
        /// </param>
        /// <param name="targetType">
        /// The type of the target property, specified by a helper structure that wraps the type name.
        /// </param>
        /// <param name="parameter">
        /// An optional parameter to be used in the converter logic.
        /// </param>
        /// <param name="culture">
        /// The culture parameter.
        /// </param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}