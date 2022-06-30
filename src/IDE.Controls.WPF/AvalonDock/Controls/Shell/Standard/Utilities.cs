/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

// This file contains general utilities to aid in development.
// Classes here generally shouldn't be exposed publicly since
// they're not particular to any library functionality.
// Because the classes here are internal, it's likely this file
// might be included in multiple assemblies.
namespace Standard
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Runtime.InteropServices;
  using System.Security.Cryptography;
  using System.Text;
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  internal static partial class Utility
  {
    private static readonly Version _osVersion = Environment.OSVersion.Version;
    private static readonly Version _presentationFrameworkVersion = Assembly.GetAssembly( typeof( Window ) ).GetName().Version;

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    private static bool _MemCmp( IntPtr left, IntPtr right, long cb )
    {
      int offset = 0;

      for( ; offset < ( cb - sizeof( Int64 ) ); offset += sizeof( Int64 ) )
      {
        Int64 left64 = Marshal.ReadInt64( left, offset );
        Int64 right64 = Marshal.ReadInt64( right, offset );

        if( left64 != right64 )
        {
          return false;
        }
      }

      for( ; offset < cb; offset += sizeof( byte ) )
      {
        byte left8 = Marshal.ReadByte( left, offset );
        byte right8 = Marshal.ReadByte( right, offset );

        if( left8 != right8 )
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>The native RGB macro.</summary>
    /// <param name="c"></param>
    /// <returns></returns>
    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static int RGB( Color c )
    {
      return c.R | ( c.G << 8 ) | ( c.B << 16 );
    }

    /// <summary>Convert a native integer that represent a color with an alpha channel into a Color struct.</summary>
    /// <param name="color">The integer that represents the color.  Its bits are of the format 0xAARRGGBB.</param>
    /// <returns>A Color representation of the parameter.</returns>
    public static Color ColorFromArgbDword( uint color )
    {
      return Color.FromArgb(
          ( byte )( ( color & 0xFF000000 ) >> 24 ),
          ( byte )( ( color & 0x00FF0000 ) >> 16 ),
          ( byte )( ( color & 0x0000FF00 ) >> 8 ),
          ( byte )( ( color & 0x000000FF ) >> 0 ) );
    }


    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static int GET_X_LPARAM( IntPtr lParam )
    {
      return LOWORD( lParam.ToInt32() );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static int GET_Y_LPARAM( IntPtr lParam )
    {
      return HIWORD( lParam.ToInt32() );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static int HIWORD( int i )
    {
      return ( short )( i >> 16 );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static int LOWORD( int i )
    {
      return ( short )( i & 0xFFFF );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    [SuppressMessage( "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands" )]
    public static bool AreStreamsEqual( Stream left, Stream right )
    {
      if( null == left )
      {
        return right == null;
      }
      if( null == right )
      {
        return false;
      }

      if( !left.CanRead || !right.CanRead )
      {
        throw new NotSupportedException( "The streams can't be read for comparison" );
      }

      if( left.Length != right.Length )
      {
        return false;
      }

      var length = ( int )left.Length;

      // seek to beginning
      left.Position = 0;
      right.Position = 0;

      // total bytes read
      int totalReadLeft = 0;
      int totalReadRight = 0;

      // bytes read on this iteration
      int cbReadLeft = 0;
      int cbReadRight = 0;

      // where to store the read data
      var leftBuffer = new byte[ 512 ];
      var rightBuffer = new byte[ 512 ];

      // pin the left buffer
      GCHandle handleLeft = GCHandle.Alloc( leftBuffer, GCHandleType.Pinned );
      IntPtr ptrLeft = handleLeft.AddrOfPinnedObject();

      // pin the right buffer
      GCHandle handleRight = GCHandle.Alloc( rightBuffer, GCHandleType.Pinned );
      IntPtr ptrRight = handleRight.AddrOfPinnedObject();

      try
      {
        while( totalReadLeft < length )
        {

          cbReadLeft = left.Read( leftBuffer, 0, leftBuffer.Length );
          cbReadRight = right.Read( rightBuffer, 0, rightBuffer.Length );

          // verify the contents are an exact match
          if( cbReadLeft != cbReadRight )
          {
            return false;
          }

          if( !_MemCmp( ptrLeft, ptrRight, cbReadLeft ) )
          {
            return false;
          }

          totalReadLeft += cbReadLeft;
          totalReadRight += cbReadRight;
        }


        return true;
      }
      finally
      {
        handleLeft.Free();
        handleRight.Free();
      }
    }


    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static bool IsFlagSet( int value, int mask )
    {
      return 0 != ( value & mask );
    }





    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static bool IsOSVistaOrNewer
    {
      get
      {
        return _osVersion >= new Version( 6, 0 );
      }
    }

   

   


    public static BitmapFrame GetBestMatch( IList<BitmapFrame> frames, int width, int height )
    {
      return _GetBestMatch( frames, _GetBitDepth(), width, height );
    }

    private static int _MatchImage( BitmapFrame frame, int bitDepth, int width, int height, int bpp )
    {
      int score = 2 * _WeightedAbs( bpp, bitDepth, false ) +
              _WeightedAbs( frame.PixelWidth, width, true ) +
              _WeightedAbs( frame.PixelHeight, height, true );

      return score;
    }

    private static int _WeightedAbs( int valueHave, int valueWant, bool fPunish )
    {
      int diff = ( valueHave - valueWant );

      if( diff < 0 )
      {
        diff = ( fPunish ? -2 : -1 ) * diff;
      }

      return diff;
    }

    /// From a list of BitmapFrames find the one that best matches the requested dimensions.
    /// The methods used here are copied from Win32 sources.  We want to be consistent with
    /// system behaviors.
    private static BitmapFrame _GetBestMatch( IList<BitmapFrame> frames, int bitDepth, int width, int height )
    {
      int bestScore = int.MaxValue;
      int bestBpp = 0;
      int bestIndex = 0;

      bool isBitmapIconDecoder = frames[ 0 ].Decoder is IconBitmapDecoder;

      for( int i = 0; i < frames.Count && bestScore != 0; ++i )
      {
        int currentIconBitDepth = isBitmapIconDecoder ? frames[ i ].Thumbnail.Format.BitsPerPixel : frames[ i ].Format.BitsPerPixel;

        if( currentIconBitDepth == 0 )
        {
          currentIconBitDepth = 8;
        }

        int score = _MatchImage( frames[ i ], bitDepth, width, height, currentIconBitDepth );
        if( score < bestScore )
        {
          bestIndex = i;
          bestBpp = currentIconBitDepth;
          bestScore = score;
        }
        else if( score == bestScore )
        {
          // Tie breaker: choose the higher color depth.  If that fails, choose first one.
          if( bestBpp < currentIconBitDepth )
          {
            bestIndex = i;
            bestBpp = currentIconBitDepth;
          }
        }
      }

      return frames[ bestIndex ];
    }

    // This can be cached.  It's not going to change under reasonable circumstances.
    private static int s_bitDepth; // = 0;
    private static int _GetBitDepth()
    {
      if( s_bitDepth == 0 )
      {
        using( SafeDC dc = SafeDC.GetDesktop() )
        {
          s_bitDepth = NativeMethods.GetDeviceCaps( dc, DeviceCap.BITSPIXEL ) * NativeMethods.GetDeviceCaps( dc, DeviceCap.PLANES );
        }
      }
      return s_bitDepth;
    }

   

    /// <summary>GDI's DeleteObject</summary>
    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode" )]
    public static void SafeDeleteObject( ref IntPtr gdiObject )
    {
      IntPtr p = gdiObject;
      gdiObject = IntPtr.Zero;
      if( IntPtr.Zero != p )
      {
        NativeMethods.DeleteObject( p );
      }
    }

    public static void AddDependencyPropertyChangeListener( object component, DependencyProperty property, EventHandler listener )
    {
      if( component == null )
      {
        return;
      }

      DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty( property, component.GetType() );
      dpd.AddValueChanged( component, listener );
    }

    public static void RemoveDependencyPropertyChangeListener( object component, DependencyProperty property, EventHandler listener )
    {
      if( component == null )
      {
        return;
      }

      DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty( property, component.GetType() );
      dpd.RemoveValueChanged( component, listener );
    }

    #region Extension Methods

    public static bool IsThicknessNonNegative( Thickness thickness )
    {
      if( !IsDoubleFiniteAndNonNegative( thickness.Top ) )
      {
        return false;
      }

      if( !IsDoubleFiniteAndNonNegative( thickness.Left ) )
      {
        return false;
      }

      if( !IsDoubleFiniteAndNonNegative( thickness.Bottom ) )
      {
        return false;
      }

      if( !IsDoubleFiniteAndNonNegative( thickness.Right ) )
      {
        return false;
      }

      return true;
    }

    public static bool IsCornerRadiusValid( CornerRadius cornerRadius )
    {
      if( !IsDoubleFiniteAndNonNegative( cornerRadius.TopLeft ) )
      {
        return false;
      }

      if( !IsDoubleFiniteAndNonNegative( cornerRadius.TopRight ) )
      {
        return false;
      }

      if( !IsDoubleFiniteAndNonNegative( cornerRadius.BottomLeft ) )
      {
        return false;
      }

      if( !IsDoubleFiniteAndNonNegative( cornerRadius.BottomRight ) )
      {
        return false;
      }

      return true;
    }

    public static bool IsDoubleFiniteAndNonNegative( double d )
    {
      if( double.IsNaN( d ) || double.IsInfinity( d ) || d < 0 )
      {
        return false;
      }

      return true;
    }

    #endregion
  }
}
