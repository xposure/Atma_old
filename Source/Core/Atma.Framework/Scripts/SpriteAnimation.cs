//using Atma.Engine;
//using System.Collections.Generic;

//namespace Atma
//{
//    public class SpriteAnimation : SpriteFrame
//    {
//        private struct SpriteAnimationData
//        {
//            public string name;
//            public int[] frames;
//        }

//        private List<SpriteAnimationData> _animations = new List<SpriteAnimationData>();
//        private SpriteAnimationData _currentAnimation;
//        private int _currentFrame = -1;
//        private float _timer = 0f;
//        private float _fps;

//        public float fps
//        {
//            get { return 1f / _fps; }
//            set
//            {
//                _timer = 1f / value;
//                _fps = 1f / value;
//                frame = 0;
//            }
//        }

//        public void addAnimation(string anim, params int[] frames)
//        {
//            var sad = new SpriteAnimationData();
//            sad.name = anim;
//            sad.frames = frames;
//            _animations.Add(sad);
//        }

//        public void setAnimation(string anim)
//        {
//            _currentFrame = -1;
//            for (var i = 0; i < _animations.Count; i++)
//            {
//                if (_animations[i].name == anim)
//                {
//                    _currentAnimation = _animations[i];
//                    _currentFrame = 0;
//                    frame = _animations[i].frames[0];
//                    _timer = _fps;
//                    break;
//                }
//            }
//        }

//        private void update()
//        {
//            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
//            if (_currentFrame >= 0)
//            {
//                _timer -= time.delta;
//                if (_timer <= 0f)
//                {
//                    _timer += _fps;
//                    _currentFrame = (_currentFrame + 1) % _currentAnimation.frames.Length;
//                    frame = _currentAnimation.frames[_currentFrame];
//                }
//            }
//        }
//    }
//}
