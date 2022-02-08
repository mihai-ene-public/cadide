using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace IDE.Presentation.Extensions
{
    public static class MaterialHelper
    {
      

        /// <summary>
        /// Creates a material for the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The material.</returns>
        public static Material CreateMaterial(Color color)
        {
            return CreateMaterial(new SolidColorBrush(color));
        }

        /// <summary>
        /// Creates a material for the specified color and opacity.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="opacity">The opacity.</param>
        /// <returns>The material.</returns>
        public static Material CreateMaterial(Color color, double opacity)
        {
            return CreateMaterial(Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B));
        }

        /// <summary>
        /// Creates a material with the specified brush as diffuse material.
        /// This method will also add a white specular material.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="specularPower">The specular power.</param>
        /// <param name="ambient">The ambient component.</param>
        /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
        /// <returns>
        /// The material.
        /// </returns>
        public static Material CreateMaterial(
            Brush brush,
            double specularPower = 100,
            byte ambient = 255,
            bool freeze = true)
        {
            return CreateMaterial(brush, 1d, specularPower, ambient, freeze);
        }

        /// <summary>
        /// Creates a material with the specified brush as diffuse material.
        /// This method will also add a white specular material.
        /// </summary>
        /// <param name="brush">The brush of the diffuse material.</param>
        /// <param name="specularBrightness">The brightness of the specular material.</param>
        /// <param name="specularPower">The specular power.</param>
        /// <param name="ambient">The ambient component.</param>
        /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
        /// <returns>
        /// The material.
        /// </returns>
        public static Material CreateMaterial(Brush brush, double specularBrightness, double specularPower = 100, byte ambient = 255, bool freeze = true)
        {
            var mg = new MaterialGroup();
            mg.Children.Add(new DiffuseMaterial(brush) { AmbientColor = Color.FromRgb(ambient, ambient, ambient) });
            if (specularPower > 0)
            {
                var b = (byte)(255 * specularBrightness);
                mg.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromRgb(b, b, b)), specularPower));
            }

            if (freeze)
            {
                mg.Freeze();
            }

            return mg;
        }

        /// <summary>
        /// Creates a material with the specified diffuse, emissive and specular brushes.
        /// </summary>
        /// <param name="diffuse">The diffuse color.</param>
        /// <param name="emissive">The emissive color.</param>
        /// <param name="specular">The specular color.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="specularPower">The specular power.</param>
        /// <param name="freeze">Freeze the material if set to <c>true</c>.</param>
        /// <returns>The material.</returns>
        public static Material CreateMaterial(Brush diffuse, Brush emissive, Brush specular = null, double opacity = 1, double specularPower = 85, bool freeze = true)
        {
            var mg = new MaterialGroup();
            if (diffuse != null)
            {
                diffuse = diffuse.Clone();
                diffuse.Opacity = opacity;
                mg.Children.Add(new DiffuseMaterial(diffuse));
            }

            if (emissive != null)
            {
                emissive = emissive.Clone();
                emissive.Opacity = opacity;
                mg.Children.Add(new EmissiveMaterial(emissive));
            }

            if (specular != null)
            {
                specular = specular.Clone();
                specular.Opacity = opacity;
                mg.Children.Add(new SpecularMaterial(specular, specularPower));
            }

            if (freeze)
            {
                mg.Freeze();
            }

            return mg;
        }

        /// <summary>
        /// Gets the first material of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of material</typeparam>
        /// <param name="material">The source material.</param>
        /// <returns>The first material of the specified type.</returns>
        public static T GetFirst<T>(Material material) where T : Material
        {
            if (material.GetType() == typeof(T))
            {
                return (T)material;
            }

            var mg = material as MaterialGroup;
            if (mg != null)
            {
                return mg.Children.Select(GetFirst<T>).FirstOrDefault(m => m != null);
            }

            return null;
        }

        
    }

}
