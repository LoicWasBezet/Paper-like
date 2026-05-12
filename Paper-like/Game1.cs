using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;

namespace Paper_like
{
    public struct InstanceData : IVertexType
    {
        public Vector4 color;
        public Vector2 position;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.Position, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public InstanceData(Vector4 color, Vector2 position)
        {
            this.color = color;
            this.position = position;
        }
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        RenderTarget2D screen;
        Effect mainShader;

        DynamicVertexBuffer instanceBuffer;
        VertexBuffer geometryBuffer;
        InstanceData[] instances;
        int maxPointCount = 1000;
        float dotSize = 0.04f;

        Random rnd = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        protected override void Initialize()
        {
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();
            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.ToggleFullScreen();
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainShader = Content.Load<Effect>("PencilShader");
            //mainShader.Parameters["aspectRatio"].SetValue(graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight);
            // TODO: use this.Content to load your game content here
            screen = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.None);//RenderTargetUsage.PreserveContents


            short[] indices = new short[] { 0, 1, 2, 2, 1, 3 };


            IndexBuffer indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            GraphicsDevice.Indices = indexBuffer;

            instances = new InstanceData[maxPointCount];

            instanceBuffer = new DynamicVertexBuffer(GraphicsDevice, InstanceData.VertexDeclaration, maxPointCount, BufferUsage.WriteOnly);


            float XpixelsPerYpixel = ((float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) / ((float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            VertexPositionTexture[] vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-1, XpixelsPerYpixel, 0)*dotSize/2, new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, XpixelsPerYpixel, 0)*dotSize/2, new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-1, -XpixelsPerYpixel, 0)*dotSize/2, new Vector2(0, 1)),

                new VertexPositionTexture(new Vector3(1, -XpixelsPerYpixel, 0)*dotSize/2, new Vector2(1, 1))
            };
            geometryBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
            geometryBuffer.SetData(vertices);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < maxPointCount; i++)
            {
                instances[i].position = new Vector2(rnd.NextSingle()*1000,rnd.NextSingle()*1000);
                instances[i].color = new Vector4(rnd.NextSingle()*255,rnd.NextSingle()*255,rnd.NextSingle()*255,1);
            }
            instanceBuffer.SetData(instances);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(1.0f / 255, 3.0f / 255, 20.0f / 255, 1));
            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            GraphicsDevice.SetVertexBuffers(
                new VertexBufferBinding(geometryBuffer, 0, 0),
                new VertexBufferBinding(instanceBuffer, 0, 1)
            );



            foreach (EffectPass pass in mainShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawInstancedPrimitives(
                    PrimitiveType.TriangleList, 0, 0, 2, 0
                );
            }
            if (rnd.Next(0, 100) == 0)
            {
                Debug.WriteLine(1 / gameTime.ElapsedGameTime.TotalSeconds);
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            mainShader.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(
                texture: screen,
                position: Vector2.Zero,
                sourceRectangle: null,
                color: Color.White,//Color.RosyBrown
                rotation: 0,
                origin: Vector2.Zero,
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: 0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
