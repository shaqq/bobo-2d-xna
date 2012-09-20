using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
namespace BTBD.ScreenManager
{
    public enum ScreenState
    {
        TransitionToOn,
        Active,
        TransitionToOff,
        Hidden
    }

    public abstract class GameScreen
    {
        public virtual bool isPaused
        {
            get { return paused; }
            set { paused = value; }
        }
        private bool paused;

        public Model LoadModel(string assetName)
        {

            Model newModel = this.ScreenManager.Game.Content.Load<Model>(assetName); foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = this.myEffect.Clone();
            return newModel;
        }

        public Model LoadModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = this.ScreenManager.Game.Content.Load<Model>(assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (Effect currentEffect in mesh.Effects)
                {
                    BasicEffect b = currentEffect as BasicEffect;
                    if(b != null)
                        textures[i++] = b.Texture;
                }
                    

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = this.myEffect.Clone();

            return newModel;
        }
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionToOn;

        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        public Song bgSong;
        bool isExiting = false;

        public bool IsActive
        {
            get { return !hasFocus && (screenState == ScreenState.TransitionToOn || screenState == ScreenState.Active); }
        }

        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        ScreenManager screenManager;

        //location of transition
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;

        //time to transition off
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        //time to transition on 
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;

        TimeSpan transitionOffTime = TimeSpan.Zero;

        public virtual void Update(GameTime gameTime, bool hasFocus, bool isCovered)
        {

            this.hasFocus = hasFocus;
            if (isExiting)
            {
                screenState = ScreenState.TransitionToOff;
                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            if (isCovered)
            {
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionToOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionToOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Active;
                }
            }
        }


        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {

            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        public Effect myEffect;
        public bool hasFocus;
        //load all graphics for screen
        public virtual void LoadContent() { }

        //unload graphics for the screen
        public virtual void UnloadContent() { }

        public virtual void HandleInput(InputState input) { }

        public virtual void Draw(GameTime gameTime) { }



        public void Exit()
        {
            ScreenManager.RemoveScreen(this);
        }
    }
}
