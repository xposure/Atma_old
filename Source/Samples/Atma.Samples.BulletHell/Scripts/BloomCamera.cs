﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Atma;
//using Atma.Managers;
//using Atma.Engine;

//namespace GameName1.BulletHell.Scripts
//{
//    public class BloomCamera : Camera
//    {

//        SpriteBatch spriteBatch;
//        Effect bloomExtractEffect;
//        Effect bloomCombineEffect;
//        Effect gaussianBlurEffect;
//        RenderTarget2D sceneRenderTarget;
//        RenderTarget2D renderTarget1;
//        RenderTarget2D renderTarget2;
//        BloomSettings settings = BloomSettings.PresetSettings[0];
//        IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

//        // Optionally displays one of the intermediate buffers used
//        // by the bloom postprocess, so you can see exactly what is
//        // being drawn into each rendertarget.
//        public enum IntermediateBuffer
//        {
//            Original,
//            PreBloom,
//            BlurredHorizontally,
//            BlurredBothWays,
//            FinalResult,
//        }

//        public IntermediateBuffer ShowBuffer
//        {
//            get { return showBuffer; }
//            set { showBuffer = value; }
//        }


//        // Choose what display settings the bloom should use.
//        public BloomSettings Settings
//        {
//            get { return settings; }
//            set { settings = value; }
//        }


//        private void init()
//        {
//            //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
//            //bloomExtractEffect = resources.loadEffect("content/textures/bullethell/BloomExtract.mgfxo");
//            //bloomCombineEffect = resources.loadEffect("content/textures/bullethell/BloomCombine.mgfxo");
//            //BgaussianBlurEffect = resources.loadEffect("content/textures/bullethell/GaussianBlur.mgfxo");


//            //PresentationParameters pp = graphicsDevice.PresentationParameters;

//            //int width = pp.BackBufferWidth;
//            //int height = pp.BackBufferHeight;

//            //SurfaceFormat format = pp.BackBufferFormat;

//            //// Create a texture for rendering the main scene, prior to applying bloom.
//            //sceneRenderTarget = new RenderTarget2D(graphicsDevice, width, height, false,
//            //                                       format, pp.DepthStencilFormat, pp.MultiSampleCount,
//            //                                       RenderTargetUsage.DiscardContents);

//            //// Create two rendertargets for the bloom processing. These are half the
//            //// size of the backbuffer, in order to minimize fillrate costs. Reducing
//            //// the resolution in this way doesn't hurt quality, because we are going
//            //// to be blurring the bloom images in any case.
//            //width /= 2;
//            //height /= 2;

//            //renderTarget1 = new RenderTarget2D(graphicsDevice, width, height, false, format, DepthFormat.None);
//            //renderTarget2 = new RenderTarget2D(graphicsDevice, width, height, false, format, DepthFormat.None);

//            //spriteBatch = new SpriteBatch(graphicsDevice);
//        }

//        protected override void ondraw()
//        {
//            //graphicsDevice.SetRenderTarget(sceneRenderTarget);
//            //graphicsDevice.Viewport = viewport;
//            //graphicsDevice.Clear(clear);
//            //var vp = new Viewport(
//            //graphics.render(ViewMatrix, sceneRenderTarget, viewport);
//            //graphics.render(ViewMatrix);
//        }

//        //http://gamedev.tutsplus.com/tutorials/implementation/make-a-neon-vector-shooter-in-xna-bloom-and-black-holes/
//        protected override void postprocess()
//        {
//            //graphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;

//            //// Pass 1: draw the scene into rendertarget 1, using a
//            //// shader that extracts only the brightest parts of the image.
//            //bloomExtractEffect.Parameters["BloomThreshold"].SetValue(
//            //    Settings.BloomThreshold);

//            //DrawFullscreenQuad(sceneRenderTarget, renderTarget1,
//            //                   bloomExtractEffect,
//            //                   IntermediateBuffer.PreBloom);

//            //// Pass 2: draw from rendertarget 1 into rendertarget 2,
//            //// using a shader to apply a horizontal gaussian blur filter.
//            //SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0);

//            //DrawFullscreenQuad(renderTarget1, renderTarget2,
//            //                   gaussianBlurEffect,
//            //                   IntermediateBuffer.BlurredHorizontally);

//            //// Pass 3: draw from rendertarget 2 back into rendertarget 1,
//            //// using a shader to apply a vertical gaussian blur filter.
//            //SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height);

//            //DrawFullscreenQuad(renderTarget2, renderTarget1,
//            //                   gaussianBlurEffect,
//            //                   IntermediateBuffer.BlurredBothWays);

//            //// Pass 4: draw both rendertarget 1 and the original scene
//            //// image back into the main backbuffer, using a shader that
//            //// combines them to produce the final bloomed result.
//            //graphicsDevice.SetRenderTarget(((Atma.MonoGame.Graphics.RenderToTexture)target).target);

//            //EffectParameterCollection parameters = bloomCombineEffect.Parameters;

//            //parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
//            //parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
//            //parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
//            //parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);

//            //graphicsDevice.Textures[1] = sceneRenderTarget;

//            //Viewport viewport = graphicsDevice.Viewport;

//            //DrawFullscreenQuad(renderTarget1,
//            //                   viewport.Width, viewport.Height,
//            //                   bloomCombineEffect,
//            //                   IntermediateBuffer.FinalResult);
//        }


//        /// <summary>
//        /// Helper for drawing a texture into a rendertarget, using
//        /// a custom shader to apply postprocessing effects.
//        /// </summary>
//        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
//                                Effect effect, IntermediateBuffer currentBuffer)
//        {
//            graphicsDevice.SetRenderTarget(renderTarget);

//            DrawFullscreenQuad(texture,
//                               renderTarget.Width, renderTarget.Height,
//                               effect, currentBuffer);
//        }


//        /// <summary>
//        /// Helper for drawing a texture into the current rendertarget,
//        /// using a custom shader to apply postprocessing effects.
//        /// </summary>
//        void DrawFullscreenQuad(Texture2D texture, int width, int height,
//                                Effect effect, IntermediateBuffer currentBuffer)
//        {
//            // If the user has selected one of the show intermediate buffer options,
//            // we still draw the quad to make sure the image will end up on the screen,
//            // but might need to skip applying the custom pixel shader.
//            if (showBuffer < currentBuffer)
//            {
//                effect = null;
//            }

//            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
//            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
//            spriteBatch.End();
//        }


//        /// <summary>
//        /// Computes sample weightings and texture coordinate offsets
//        /// for one pass of a separable gaussian blur filter.
//        /// </summary>
//        void SetBlurEffectParameters(float dx, float dy)
//        {
//            // Look up the sample weight and offset effect parameters.
//            EffectParameter weightsParameter, offsetsParameter;

//            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
//            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

//            // Look up how many samples our gaussian blur effect supports.
//            int sampleCount = weightsParameter.Elements.Count;

//            // Create temporary arrays for computing our filter settings.
//            float[] sampleWeights = new float[sampleCount];
//            Vector2[] sampleOffsets = new Vector2[sampleCount];

//            // The first sample always has a zero offset.
//            sampleWeights[0] = ComputeGaussian(0);
//            sampleOffsets[0] = new Vector2(0);

//            // Maintain a sum of all the weighting values.
//            float totalWeights = sampleWeights[0];

//            // Add pairs of additional sample taps, positioned
//            // along a line in both directions from the center.
//            for (int i = 0; i < sampleCount / 2; i++)
//            {
//                // Store weights for the positive and negative taps.
//                float weight = ComputeGaussian(i + 1);

//                sampleWeights[i * 2 + 1] = weight;
//                sampleWeights[i * 2 + 2] = weight;

//                totalWeights += weight * 2;

//                // To get the maximum amount of blurring from a limited number of
//                // pixel shader samples, we take advantage of the bilinear filtering
//                // hardware inside the texture fetch unit. If we position our texture
//                // coordinates exactly halfway between two texels, the filtering unit
//                // will average them for us, giving two samples for the price of one.
//                // This allows us to step in units of two texels per sample, rather
//                // than just one at a time. The 1.5 offset kicks things off by
//                // positioning us nicely in between two texels.
//                float sampleOffset = i * 2 + 1.5f;

//                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

//                // Store texture coordinate offsets for the positive and negative taps.
//                sampleOffsets[i * 2 + 1] = delta;
//                sampleOffsets[i * 2 + 2] = -delta;
//            }

//            // Normalize the list of sample weightings, so they will always sum to one.
//            for (int i = 0; i < sampleWeights.Length; i++)
//            {
//                sampleWeights[i] /= totalWeights;
//            }

//            // Tell the effect about our new filter settings.
//            weightsParameter.SetValue(sampleWeights);
//            offsetsParameter.SetValue(sampleOffsets);
//        }


//        /// <summary>
//        /// Evaluates a single point on the gaussian falloff curve.
//        /// Used for setting up the blur filter weightings.
//        /// </summary>
//        float ComputeGaussian(float n)
//        {
//            float theta = Settings.BlurAmount;

//            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
//                           Math.Exp(-(n * n) / (2 * theta * theta)));
//        }

//    }


//    /// <summary>
//    /// Class holds all the settings used to tweak the bloom effect.
//    /// </summary>
//    public class BloomSettings
//    {
//        #region Fields


//        // Name of a preset bloom setting, for display to the user.
//        public readonly string Name;


//        // Controls how bright a pixel needs to be before it will bloom.
//        // Zero makes everything bloom equally, while higher values select
//        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
//        public readonly float BloomThreshold;


//        // Controls how much blurring is applied to the bloom image.
//        // The typical range is from 1 up to 10 or so.
//        public readonly float BlurAmount;


//        // Controls the amount of the bloom and base images that
//        // will be mixed into the final scene. Range 0 to 1.
//        public readonly float BloomIntensity;
//        public readonly float BaseIntensity;


//        // Independently control the color saturation of the bloom and
//        // base images. Zero is totally desaturated, 1.0 leaves saturation
//        // unchanged, while higher values increase the saturation level.
//        public readonly float BloomSaturation;
//        public readonly float BaseSaturation;


//        #endregion


//        /// <summary>
//        /// Constructs a new bloom settings descriptor.
//        /// </summary>
//        public BloomSettings(string name, float bloomThreshold, float blurAmount,
//                             float bloomIntensity, float baseIntensity,
//                             float bloomSaturation, float baseSaturation)
//        {
//            Name = name;
//            BloomThreshold = bloomThreshold;
//            BlurAmount = blurAmount;
//            BloomIntensity = bloomIntensity;
//            BaseIntensity = baseIntensity;
//            BloomSaturation = bloomSaturation;
//            BaseSaturation = baseSaturation;
//        }


//        /// <summary>
//        /// Table of preset bloom settings, used by the sample program.
//        /// </summary>
//        public static BloomSettings[] PresetSettings =
//        {
//            //                Name           Thresh  Blur Bloom  Base  BloomSat BaseSat
//            new BloomSettings("Default",     0.25f,  4,   2f,    1,    1.5f,    1),
//            new BloomSettings("Default",     0.25f,  4,   1.25f, 1,    1,       1),
//            new BloomSettings("Soft",        0,      3,   1,     1,    1,       1),
//            new BloomSettings("Desaturated", 0.5f,   8,   2,     1,    0,       1),
//            new BloomSettings("Saturated",   0.25f,  4,   2,     1,    2,       0),
//            new BloomSettings("Blurry",      0,      2,   1,     0.1f, 1,       1),
//            new BloomSettings("Subtle",      0.5f,   2,   1,     1,    1,       1),
//        };
//    }
//}
