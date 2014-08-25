using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Core;

namespace Atma._2_0
{
    public class DestroyableAspect : Aspect
    {
        internal static Logger _logger = Logger.getLogger(typeof(DestroyableAspect));

        internal static HashSet<uint> _delayDestroy = new HashSet<uint>();

        public void destroy()
        {
            _delayDestroy.Add(this.id);
        }

        public void destroyNow()
        {
            destroyNow(this);
        }

        internal static void processDestroyed()
        {
            while (_delayDestroy.Count > 0)
            {
                //lets do this in case somehow destroy creates new destroys
                var list = _delayDestroy.ToArray();
                _delayDestroy.Clear();

                foreach (var id in list)
                {
                    Aspect aspect;
                    if (!_aspects.TryGetValue(id, out aspect))
                        _logger.warn("aspect {0} was marked for destroy but was missing", id);

                    destroyNow(aspect);
                }
            }
        }

        internal static void destroyNow(Aspect aspect)
        {
            if (aspect)
                _logger.warn("tried to destroy a null aspect");
            else
            {
                _aspects.Remove(aspect.id);

                //needed in case destroy was called and then destroyNow - were going to let the warnings catch it
                //_delayDestroy.Remove(aspect.id);
            }
        }


    }
}
