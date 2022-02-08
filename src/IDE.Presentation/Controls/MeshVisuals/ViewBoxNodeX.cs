using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;

namespace IDE.Controls
{
    public class ViewBoxNodeX : ScreenSpacedNode
    {
        private readonly Color textColor = Color.White;
        private readonly Color faceColor = new Color(80, 80, 80);
        private readonly Color cornerColor = new Color(140, 140, 140);
        private readonly Color edgeColor = new Color(110, 110, 110);

        #region Properties
        private Stream viewboxTexture;
        /// <summary>
        /// Gets or sets the view box texture.
        /// </summary>
        /// <value>
        /// The view box texture.
        /// </value>
        public Stream ViewBoxTexture
        {
            set
            {
                if (Set(ref viewboxTexture, value))
                {
                    UpdateTexture(value);
                }
            }
            get { return viewboxTexture; }
        }


        private Vector3 upDirection = new Vector3(0, 1, 0);
        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public Vector3 UpDirection
        {
            set
            {
                if (Set(ref upDirection, value))
                {
                    UpdateModel(value);
                }
            }
            get
            {
                return upDirection;
            }
        }
        #endregion

        #region Fields
        private const float size = 5;
        private const float cornerSize = size / 5 - 0.01f;
        private const float halfSize = size / 2;
        private const float edgeSize = size;//- cornerSize;//halfSize * 1.5f;

        private static readonly Vector3[] xAligned = { new Vector3(0, -1, -1), new Vector3(0, 1, -1), new Vector3(0, -1, 1), new Vector3(0, 1, 1) }; //x
        private static readonly Vector3[] yAligned = { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1) };//y
        private static readonly Vector3[] zAligned = { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(1, 1, 0) };//z

        private static readonly Vector3[] cornerPoints =   {
                    new Vector3(-1,-1,-1 ), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1),
                    new Vector3(-1,-1,1 ),new Vector3(1,-1,1 ),new Vector3(1,1,1 ),new Vector3(-1,1,1 )};

        private static readonly Matrix[] cornerInstances;
        private static readonly Geometry3D cornerGeometry;

        private static readonly Geometry3D edgeGeometry;
        private static readonly Matrix[] edgeInstances;

        private readonly MeshNode ViewBoxMeshModel;
        private readonly InstancingMeshNode EdgeModel;
        private readonly InstancingMeshNode CornerModel;

        #endregion


        static ViewBoxNodeX()
        {
            //corner geometry
            var builder = new MeshBuilder(true, false);
            builder.AddBox(Vector3.Zero, cornerSize, cornerSize, cornerSize);
            cornerGeometry = builder.ToMesh();

            //edge geometry
            builder = new MeshBuilder(true, false);
            builder.AddBox(Vector3.Zero, cornerSize, edgeSize, cornerSize);
            edgeGeometry = builder.ToMesh();

            const float factor = 1.00f;

            cornerInstances = new Matrix[cornerPoints.Length];
            for (int i = 0; i < cornerPoints.Length; ++i)
            {
                cornerInstances[i] = Matrix.Translation(cornerPoints[i] * (halfSize + 0.5f * cornerSize));
            }
            int count = xAligned.Length;
            edgeInstances = new Matrix[count * 3];

            for (int i = 0; i < count; ++i)
            {
                edgeInstances[i] = Matrix.RotationZ((float)Math.PI / 2) * Matrix.Translation(xAligned[i] * (halfSize + 0.5f * cornerSize) * factor);
            }
            for (int i = count; i < count * 2; ++i)
            {
                edgeInstances[i] = Matrix.Translation(yAligned[i % count] * (halfSize + 0.5f * cornerSize) * factor);
            }
            for (int i = count * 2; i < count * 3; ++i)
            {
                edgeInstances[i] = Matrix.RotationX((float)Math.PI / 2) * Matrix.Translation(zAligned[i % count] * (halfSize + 0.5f * cornerSize) * factor);
            }
        }

        public ViewBoxNodeX()
        {
            CameraType = ScreenSpacedCameraType.Perspective;
            RelativeScreenLocationX = 0.8f;
            ViewBoxMeshModel = new MeshNode() { EnableViewFrustumCheck = false, CullMode = CullMode.Back };
            var sampler = DefaultSamplers.LinearSamplerWrapAni1;
            sampler.BorderColor = Color.Gray;
            sampler.AddressU = sampler.AddressV = sampler.AddressW = TextureAddressMode.Border;
            this.AddChildNode(ViewBoxMeshModel);
            ViewBoxMeshModel.Material = new DiffuseMaterialCore()
            {
                DiffuseColor = Color.White,
                DiffuseMapSampler = sampler
            };

            CornerModel = new InstancingMeshNode()
            {
                EnableViewFrustumCheck = false,
                Material = new DiffuseMaterialCore() { DiffuseColor = cornerColor },
                Geometry = cornerGeometry,
                Instances = cornerInstances,
                //Visible = false
            };
            this.AddChildNode(CornerModel);

            EdgeModel = new InstancingMeshNode()
            {
                EnableViewFrustumCheck = false,
                Material = new DiffuseMaterialCore() { DiffuseColor = edgeColor},
                Geometry = edgeGeometry,
                Instances = edgeInstances,
            };
            this.AddChildNode(EdgeModel);
            UpdateModel(UpDirection);
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                var material = (ViewBoxMeshModel.Material as DiffuseMaterialCore);
                if (material.DiffuseMap == null)
                {
                   
                    material.DiffuseMap = ViewBoxTexture ?? BitmapExtensions.CreateViewBoxTexture(host.EffectsManager,
                        "FRONT", "BACK",
                        "LEFT", "RIGHT",
                        "UP", "DOWN",
                       //Color.Red, Color.Red, Color.Blue, Color.Blue, Color.Green, Color.Green,
                       //Color.White, Color.White, Color.White, Color.White, Color.White, Color.White
                       faceColor, faceColor, faceColor, faceColor, faceColor, faceColor,
                        textColor, textColor, textColor, textColor, textColor, textColor,
                        fontSize: 25,
                        faceSize: 100
                        );
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateTexture(Stream texture)
        {
            if (ViewBoxMeshModel.Material is DiffuseMaterialCore material)
                material.DiffuseMap = texture;
        }

        protected void UpdateModel(Vector3 up)
        {
            var left = new Vector3(up.Y, up.Z, up.X);
            var front = Vector3.Cross(left, up);
            var cubeSize = size + cornerSize;
            var builder = new MeshBuilder(true, true, false);
            builder.AddCubeFace(new Vector3(0, 0, 0), front, up, cubeSize, cubeSize, cubeSize);
            builder.AddCubeFace(new Vector3(0, 0, 0), -front, up, cubeSize, cubeSize, cubeSize);
            builder.AddCubeFace(new Vector3(0, 0, 0), left, up, cubeSize, cubeSize, cubeSize);
            builder.AddCubeFace(new Vector3(0, 0, 0), -left, up, cubeSize, cubeSize, cubeSize);
            builder.AddCubeFace(new Vector3(0, 0, 0), up, left, cubeSize, cubeSize, cubeSize);
            builder.AddCubeFace(new Vector3(0, 0, 0), -up, -left, cubeSize, cubeSize, cubeSize);

            var mesh = builder.ToMesh();
            CreateTextureCoordinates(mesh);

            ViewBoxMeshModel.Geometry = mesh;
        }

        private static void CreateTextureCoordinates(MeshGeometry3D mesh)
        {
            int faces = 6;
            int segment = 4;
            float inc = 1f / faces;

            for (int i = 0; i < mesh.TextureCoordinates.Count; ++i)
            {
                mesh.TextureCoordinates[i] = new Vector2(mesh.TextureCoordinates[i].X * inc + inc * (int)(i / segment), mesh.TextureCoordinates[i].Y);
            }
        }

        protected override bool CanHitTest(HitTestContext context)
        {
            return context != null;
        }

        protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
        {
            if (base.OnHitTest(context, totalModelMatrix, ref hits))
            {
                Debug.WriteLine("View box hit.");
                var hit = hits[0];
                Vector3 normal = Vector3.Zero;

                if (hit.ModelHit == CornerModel)
                {
                    if (hit.Tag is int index && index < cornerInstances.Length)
                    {
                        Matrix transform = cornerInstances[index];
                        normal = -transform.TranslationVector;
                    }
                }
                else if (hit.ModelHit == EdgeModel)
                {
                    if (hit.Tag is int index && index < edgeInstances.Length)
                    {
                        Matrix transform = edgeInstances[index];
                        normal = -transform.TranslationVector;
                    }
                }
                else if (hit.ModelHit == ViewBoxMeshModel)
                {
                    normal = -hit.NormalAtHit;
                    //Fix the normal if returned normal is reversed
                    if (Vector3.Dot(normal, context.RenderMatrices.CameraParams.LookAtDir) < 0)
                    {
                        normal *= -1;
                    }
                }
                else
                {
                    return false;
                }
                
                normal.Normalize();
                hit.NormalAtHit = normal;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
