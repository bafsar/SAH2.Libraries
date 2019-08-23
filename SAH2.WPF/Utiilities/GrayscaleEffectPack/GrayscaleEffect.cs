/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                                             Source                                                  ***
 ***       http://bursjootech.blogspot.com.tr/2008/06/grayscale-effect-pixel-shader-effect-in.html       ***
 ***                                              via                                                    *** 
 ***                           http://stackoverflow.com/a/4708346/2374053                                ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SAH2.WPF.Utiilities.GrayscaleEffectPack
{
    public class GrayscaleEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new PixelShader
        {
            UriSource =
                new Uri(@"pack://application:,,,/SAH2.WPF.Utilities.GrayscaleEffectPack;component/GrayscaleEffect.ps")
        };

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input",
            typeof(GrayscaleEffect), 0);

        public static readonly DependencyProperty DesaturationFactorProperty =
            DependencyProperty.Register("DesaturationFactor", typeof(double), typeof(GrayscaleEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceDesaturationFactor));

        public GrayscaleEffect()
        {
            PixelShader = _pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DesaturationFactorProperty);
        }

        public Brush Input
        {
            get { return (Brush) GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public double DesaturationFactor
        {
            get { return (double) GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
        }

        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            var effect = (GrayscaleEffect) d;
            var newFactor = (double) value;

            if (newFactor < 0.0 || newFactor > 1.0)
            {
                return effect.DesaturationFactor;
            }

            return newFactor;
        }
    }

}