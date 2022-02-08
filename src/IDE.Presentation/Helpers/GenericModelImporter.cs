using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using IDE.Core.Interfaces;
using IDE.Core.Designers;
using IDE.Presentation.Extensions;
using System.Windows.Media;
//using HelixToolkit.Wpf.SharpDX;

namespace IDE.Documents.Views
{
    public class GenericModelImporter : IModelImporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelImporter"/> class.
        /// </summary>
        public GenericModelImporter()
        {
            DefaultMaterial = MaterialHelper.CreateMaterial(Colors.Gray);

        }

        static GenericModelImporter()
        {
            RegisterDefaultReaders();
        }

        /// <summary>
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        public Material DefaultMaterial { get; set; }

        protected static Dictionary<string, Type> readerDictionary = new Dictionary<string, Type>();
        static void RegisterDefaultReaders()
        {
            //readerDictionary.Add(".3ds", typeof(StudioReader));
            // readerDictionary.Add(".lwo", typeof(LwoReader));
            readerDictionary.Add(".obj", typeof(ObjReader));
            //readerDictionary.Add(".objz", typeof(ObjReader));
            readerDictionary.Add(".stl", typeof(StLReader));
            //readerDictionary.Add(".off", typeof(OffReader));
            //are not imported properly
            //#if DEBUG
            //            readerDictionary.Add(".step", typeof(StepFileReader));
            //            readerDictionary.Add(".stp", typeof(StepFileReader));
            //#endif
        }

        public string[] GetSupportedFileFormats()
        {
            return readerDictionary.Keys.Select(f => $"*{f}").ToArray();
        }

        public void RegisterReader(string extension, Type readerType)
        {
            if (readerDictionary.ContainsKey(extension))
                return;

            readerDictionary.Add(extension, readerType);
        }

        /// <summary>
        /// Loads a model from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="dispatcher">The dispatcher used to create the model.</param>
        /// <param name="freeze">Freeze the model if set to <c>true</c>.</param>
        private BaseMeshItem Load(string path, Dispatcher dispatcher = null, bool freeze = false)
        {
            if (path == null)
                return null;

            if (dispatcher == null)
                dispatcher = Dispatcher.CurrentDispatcher;

            Model3DGroup model = null;
            var ext = Path.GetExtension(path);
            if (ext != null)
                ext = ext.ToLower();

            if (readerDictionary.ContainsKey(ext))
            {
                var readerType = readerDictionary[ext];
                var modelReader = Activator.CreateInstance(readerType, dispatcher) as ModelReader;
                if (modelReader != null)
                {
                    modelReader.DefaultMaterial = DefaultMaterial;
                    modelReader.Freeze = freeze;
                    model = modelReader.Read(path);
                }
            }

            var mg = model.ToMeshItem();
            return mg;
        }

        public void Import(string filePath, IDrawingViewModel canvasModel)
        {
            var modelGroup = Load(filePath, null, true);

            canvasModel.AddItem(modelGroup);
        }
    }

}
